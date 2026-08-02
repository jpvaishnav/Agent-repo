from PIL import Image, ImageDraw, ImageFont

W, H = 1100, 700
BG = (18, 20, 28)
FG = (235, 238, 245)
ACCENT = (99, 179, 237)      # blue - current index
GREEN = (72, 200, 130)       # included / chosen
RED = (235, 90, 90)          # excluded
GRAY = (70, 76, 92)
BOX_BORDER = (110, 118, 140)
YELLOW = (240, 200, 90)

F_TITLE = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 34)
F_SUB   = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 22)
F_BOX   = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 26)
F_LABEL = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 18)
F_CODE  = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 22)
F_SMALL = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 18)
F_BIG   = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 40)

nums = [5, 1, 2, 10, 6, 2, 7, 9, 3, 1]
n = len(nums)

BOX = 78
GAP = 12
ARR_TOP = 150
DP_TOP = 300
START_X = (W - (n * BOX + (n - 1) * GAP)) // 2

def box_center(i, top):
    x = START_X + i * (BOX + GAP)
    return x, top

def text_center(draw, cx, cy, text, font, fill):
    bbox = draw.textbbox((0, 0), text, font=font)
    w = bbox[2] - bbox[0]
    h = bbox[3] - bbox[1]
    draw.text((cx - w / 2, cy - h / 2 - bbox[1]), text, font=font, fill=fill)

def rounded_box(draw, x, y, w, h, fill, outline, width=3, radius=12):
    draw.rounded_rectangle([x, y, x + w, y + h], radius=radius, fill=fill, outline=outline, width=width)

def draw_array(draw, highlight_i=None, dep1=None, dep2=None):
    text_center(draw, W / 2, ARR_TOP - 35, "nums[] — houses with cash", F_SUB, GRAY)
    for i in range(n):
        x, y = box_center(i, ARR_TOP)
        fill = (30, 34, 46)
        outline = BOX_BORDER
        if i == highlight_i:
            fill = (40, 55, 75)
            outline = ACCENT
        elif i == dep1:
            outline = GREEN
        elif i == dep2:
            outline = YELLOW
        rounded_box(draw, x, y, BOX, BOX, fill, outline, width=4)
        text_center(draw, x + BOX / 2, y + BOX / 2, str(nums[i]), F_BOX, FG)
        text_center(draw, x + BOX / 2, y + BOX + 18, f"i={i}", F_LABEL, GRAY)

def draw_dp(draw, dp_vals, highlight_i=None, dep1=None, dep2=None):
    text_center(draw, W / 2, DP_TOP - 35, "dp[] — max amount robbed up to house i", F_SUB, GRAY)
    for i in range(n):
        x, y = box_center(i, DP_TOP)
        val = dp_vals[i]
        fill = (30, 34, 46)
        outline = BOX_BORDER
        if val is None:
            fill = (24, 26, 34)
            outline = (50, 54, 66)
        if i == highlight_i:
            outline = ACCENT
        elif i == dep1:
            outline = GREEN
        elif i == dep2:
            outline = YELLOW
        rounded_box(draw, x, y, BOX, BOX, fill, outline, width=4)
        if val is not None:
            text_center(draw, x + BOX / 2, y + BOX / 2, str(val), F_BOX, FG)
        else:
            text_center(draw, x + BOX / 2, y + BOX / 2, "?", F_BOX, GRAY)

def draw_arrow_link(draw, i_from, i_to, top, color, label):
    x1, y = box_center(i_from, top)
    x2, _ = box_center(i_to, top)
    x1c = x1 + BOX / 2
    x2c = x2 + BOX / 2
    y_arrow = top + BOX + 45
    draw.line([(x1c, y_arrow), (x2c, y_arrow)], fill=color, width=3)
    ah = 7
    direction = 1 if x2c > x1c else -1
    draw.polygon([(x2c, y_arrow), (x2c - direction * ah, y_arrow - ah), (x2c - direction * ah, y_arrow + ah)], fill=color)
    text_center(draw, (x1c + x2c) / 2, y_arrow - 16, label, F_LABEL, color)

def frame(title, subtitle, dp_vals, highlight_i=None, dep1=None, dep2=None,
          code_lines=None, answer=None, note=None):
    img = Image.new("RGB", (W, H), BG)
    draw = ImageDraw.Draw(img)

    text_center(draw, W / 2, 45, title, F_TITLE, FG)
    if subtitle:
        text_center(draw, W / 2, 85, subtitle, F_SUB, (170, 178, 196))

    draw_array(draw, highlight_i, dep1, dep2)
    draw_dp(draw, dp_vals, highlight_i, dep1, dep2)

    if dep1 is not None and highlight_i is not None:
        draw_arrow_link(draw, highlight_i, dep2, DP_TOP, YELLOW, "dp[i-1]")
        draw_arrow_link(draw, highlight_i, dep1, DP_TOP, GREEN, "dp[i-2]")

    # code / computation panel
    panel_top = 430
    rounded_box(draw, 70, panel_top, W - 140, 170, (24, 27, 36), (55, 60, 74), width=2, radius=14)
    if code_lines:
        ly = panel_top + 22
        for line, color in code_lines:
            draw.text((100, ly), line, font=F_CODE, fill=color)
            ly += 34

    if note:
        text_center(draw, W / 2, panel_top + 150, note, F_LABEL, (170, 178, 196))

    if answer is not None:
        rounded_box(draw, W / 2 - 220, 620, 440, 60, (30, 45, 38), GREEN, width=3, radius=14)
        text_center(draw, W / 2, 650, f"Answer so far (max robbed): {answer}", F_BIG if False else F_SUB, GREEN)

    return img

frames = []
durations = []

def add(img, seconds=5):
    frames.append(img)
    durations.append(int(seconds * 1000))

# --- Frame 0: Intro ---
img = Image.new("RGB", (W, H), BG)
draw = ImageDraw.Draw(img)
text_center(draw, W / 2, 90, "House Robber — Dynamic Programming", F_TITLE, FG)
text_center(draw, W / 2, 140, "dp[i] = max(nums[i] + dp[i-2], dp[i-1])", F_SUB, ACCENT)
draw_array(draw, None)
lines = [
    "You can't rob two adjacent houses.",
    "At each house i, you choose:",
    "  • INCLUDE house i  →  nums[i] + dp[i-2]",
    "  • EXCLUDE house i  →  dp[i-1]  (best without robbing it)",
    "dp[i] keeps the BETTER of the two choices.",
]
ly = 330
for l in lines:
    draw.text((100, ly), l, font=F_CODE, fill=FG)
    ly += 34
add(img, 5)

# --- Frame 1: base case dp[0] ---
dp = [None] * n
dp[0] = nums[0]
img = frame(
    "Step 1 — Base case dp[0]", "Only one house so far → rob it",
    dp, highlight_i=0,
    code_lines=[("dp[0] = nums[0]", GREEN), (f"dp[0] = {nums[0]}", FG)],
    answer=dp[0],
)
add(img, 5)

# --- Frame 2: base case dp[1] ---
dp[1] = max(nums[0], nums[1])
img = frame(
    "Step 2 — Base case dp[1]", "Two houses: rob the bigger one",
    dp, highlight_i=1,
    code_lines=[
        ("dp[1] = max(nums[0], nums[1])", GREEN),
        (f"dp[1] = max({nums[0]}, {nums[1]}) = {dp[1]}", FG),
    ],
    answer=dp[1],
)
add(img, 5)

# --- Frames for i = 2..n-1 ---
for i in range(2, n):
    include = nums[i] + dp[i - 2]
    exclude = dp[i - 1]
    choice = max(include, exclude)
    dp[i] = choice
    chosen_label = "INCLUDE" if include >= exclude else "EXCLUDE"
    chosen_color = GREEN if include >= exclude else RED
    code_lines = [
        (f"dp[{i}] = max(nums[{i}] + dp[{i-2}],  dp[{i-1}])", FG),
        (f"        = max({nums[i]} + {dp[i-2]},  {dp[i-1]})", FG),
        (f"        = max({include},  {exclude})  =  {choice}", chosen_color),
    ]
    note = f"House {i} is {chosen_label}D  →  dp[{i}] = {choice}"
    img = frame(
        f"Step {i+1} — Compute dp[{i}]",
        f"Rob house {i} (nums[{i}]={nums[i]}) or skip it?",
        dp, highlight_i=i, dep1=i - 2, dep2=i - 1,
        code_lines=code_lines, answer=dp[i], note=note,
    )
    add(img, 5)

# --- Final frame ---
final_answer = dp[n - 1]
img = Image.new("RGB", (W, H), BG)
draw = ImageDraw.Draw(img)
text_center(draw, W / 2, 70, "Final Result", F_TITLE, FG)
draw_array(draw, None)
draw_dp(draw, dp)
text_center(draw, W / 2, 430, f"Maximum amount that can be robbed = {final_answer}",
            F_TITLE, GREEN)
text_center(draw, W / 2, 480, "Space-optimized: only prevPrev, prev, curr are kept (O(1) space)",
            F_SUB, (170, 178, 196))
lines = [
    "int prevPrev = nums[0];",
    "int prev = max(nums[1], prevPrev);",
    "for (i = 2; i < n; i++) {",
    "    curr = max(nums[i] + prevPrev, prev);",
    "    prevPrev = prev; prev = curr;",
    "}",
]
ly = 540
for l in lines:
    text_center(draw, W / 2, ly, l, F_SMALL, FG)
    ly += 26
add(img, 5)

out_path = "/home/claude/dp_gif/house_robber_dp.gif"
frames[0].save(
    out_path,
    save_all=True,
    append_images=frames[1:],
    duration=durations,
    loop=0,
)
print("saved", out_path, "frames:", len(frames))
