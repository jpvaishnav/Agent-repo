from PIL import Image, ImageDraw, ImageFont

W, H = 1100, 700
BG = (18, 20, 28)
FG = (235, 238, 245)
ACCENT = (99, 179, 237)      # blue - current index x
GRAY = (70, 76, 92)
BOX_BORDER = (110, 118, 140)
DIM = (55, 60, 74)
WINNER_GLOW = (255, 255, 255)  # white glow ring around the box that WON the min()
INVALID = (60, 63, 74)         # gray - not reachable

# SAME visual element (a "dependency pointer" — box being read from, and the
# arrow that reads it) always gets the SAME color, regardless of which coin
# it belongs to. Consistency > variety: the eye should learn "this color =
# a value read from an earlier subproblem" once, and reuse that everywhere.
# The winning source is distinguished by WEIGHT/GLOW, never by a new hue.
POINTER_COLOR = (110, 190, 220)      # teal-blue: every dependency pointer/box
POINTER_COLOR_DIM = (70, 110, 125)   # same hue, dimmed, for non-winning sources

F_TITLE = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 34)
F_SUB   = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 22)
F_BOX   = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 26)
F_LABEL = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 18)
F_CODE  = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 22)
F_SMALL = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 18)

coins = [1, 3, 4]
amount = 6
INF = "INF"

BOX = 84
GAP = 16
DP_TOP = 170   # raised up so there's guaranteed clear room below for arrow rows
START_X = (W - ((amount + 1) * BOX + amount * GAP)) // 2

# Arrows are drawn as stacked rows below the dp boxes, one row per coin.
# Compute exactly how far down the LAST arrow row + its label reaches, so the
# code panel can start safely below it with margin (this is what was
# previously overlapping / hiding the last pointer, e.g. "x-4").
ARROWS_TOP = DP_TOP + BOX + 45
ARROWS_BOTTOM = ARROWS_TOP + (len(coins) - 1) * 26 + 18  # + label height
PANEL_TOP = ARROWS_BOTTOM + 35

def box_x(x):
    return START_X + x * (BOX + GAP)

def text_center(draw, cx, cy, text, font, fill):
    bbox = draw.textbbox((0, 0), text, font=font)
    w = bbox[2] - bbox[0]
    h = bbox[3] - bbox[1]
    draw.text((cx - w / 2, cy - h / 2 - bbox[1]), text, font=font, fill=fill)

def rounded_box(draw, x, y, w, h, fill, outline, width=3, radius=12):
    draw.rounded_rectangle([x, y, x + w, y + h], radius=radius, fill=fill, outline=outline, width=width)

def draw_dp_row(draw, dp_vals, highlight_x=None, target_xs=None, winner_x=None):
    """target_xs: set of x indices being read from this step — ALL drawn in the
    same POINTER_COLOR (dimmed if not the winner) so the element type reads
    consistently no matter which coin produced it."""
    target_xs = target_xs or set()
    text_center(draw, W / 2, DP_TOP - 35, "dp[amount] — min coins needed to make that amount", F_SUB, GRAY)
    for x in range(amount + 1):
        bx = box_x(x)
        val = dp_vals[x]
        fill = (30, 34, 46)
        outline = BOX_BORDER
        width = 4
        if val is None:
            fill = (24, 26, 34)
            outline = INVALID
        if x in target_xs:
            outline = POINTER_COLOR if x == winner_x else POINTER_COLOR_DIM
            width = 5
        if x == winner_x:
            rounded_box(draw, bx - 5, DP_TOP - 5, BOX + 10, BOX + 10, None, WINNER_GLOW, width=2, radius=16)
            width = 6
        if x == highlight_x:
            outline = ACCENT
            width = 5
        rounded_box(draw, bx, DP_TOP, BOX, BOX, fill, outline, width=width)
        label = str(val) if val is not None else "?"
        text_center(draw, bx + BOX / 2, DP_TOP + BOX / 2, label, F_BOX, FG)
        text_center(draw, bx + BOX / 2, DP_TOP + BOX + 18, f"x={x}", F_LABEL, GRAY)

def draw_arrows(draw, highlight_x, coin_targets, winner_target):
    """Every arrow is the SAME color (POINTER_COLOR) since they're all the same
    kind of element — a backward read into an earlier subproblem. Only the
    winning arrow is emphasized (full brightness + thicker + star label);
    the rest are the same hue, just dimmed. Color never encodes "which coin"."""
    y_base = ARROWS_TOP
    for idx, (coin, target, valid) in enumerate(coin_targets):
        if target < 0 or not valid:
            continue
        is_winner = (target == winner_target)
        color = POINTER_COLOR if is_winner else POINTER_COLOR_DIM
        x1c = box_x(highlight_x) + BOX / 2
        x2c = box_x(target) + BOX / 2
        y = y_base + idx * 26
        width = 5 if is_winner else 2
        draw.line([(x1c, y), (x2c, y)], fill=color, width=width)
        ah = 7
        draw.polygon([(x2c, y), (x2c - ah, y - ah), (x2c - ah, y + ah)], fill=color)
        crown = "  \u2605 chosen" if is_winner else ""
        text_center(draw, (x1c + x2c) / 2, y - 14, f"x-{coin}{crown}", F_LABEL, color)

def draw_legend(draw):
    x0 = 100
    y0 = PANEL_TOP + 190 + 25
    draw.rectangle([x0, y0, x0 + 22, y0 + 22], fill=POINTER_COLOR_DIM)
    draw.text((x0 + 32, y0), "value read from an earlier dp[x]", font=F_LABEL, fill=FG)
    x1 = x0 + 340
    draw.rectangle([x1, y0, x1 + 22, y0 + 22], outline=WINNER_GLOW, width=2)
    draw.rectangle([x1 + 4, y0 + 4, x1 + 18, y0 + 18], fill=POINTER_COLOR)
    draw.text((x1 + 32, y0), "the one min() actually chose", font=F_LABEL, fill=FG)

def code_panel(draw, top, lines):
    rounded_box(draw, 70, top, W - 140, 190, (24, 27, 36), (55, 60, 74), width=2, radius=14)
    ly = top + 20
    for line, color in lines:
        draw.text((100, ly), line, font=F_CODE, fill=color)
        ly += 32

def base_frame(title, subtitle):
    img = Image.new("RGB", (W, H), BG)
    draw = ImageDraw.Draw(img)
    text_center(draw, W / 2, 45, title, F_TITLE, FG)
    if subtitle:
        text_center(draw, W / 2, 85, subtitle, F_SUB, (170, 178, 196))
    return img, draw

frames, durations = [], []

def add(img, seconds=5):
    frames.append(img)
    durations.append(int(seconds * 1000))

# ---------- Frame 0: Intro ----------
img, draw = base_frame("Coin Change — Dynamic Programming",
                        f"coins = {coins}   amount = {amount}")
text_center(draw, W / 2, 130, "dp[x] = min( 1 + dp[x-coin] )  over every coin",
            F_SUB, ACCENT)
lines = [
    "dp[x] = minimum number of coins to make amount x",
    "To fill dp[x], look BACKWARDS at x-coin for every coin:",
    "  dp[x] = min( dp[x-c1]+1, dp[x-c2]+1, dp[x-c3]+1, ... )",
    "Each pointer reuses an already-solved smaller subproblem.",
    "dp[0] = 0   (zero coins needed for amount 0)",
    "A coin itself needs just 1 coin: dp[coin] = 1",
]
ly = 300
for l in lines:
    draw.text((100, ly), l, font=F_CODE, fill=FG)
    ly += 34
add(img, 5)

# ---------- Frame 1: initialization ----------
dp = [None] * (amount + 1)
dp[0] = 0
for c in coins:
    if c <= amount:
        dp[c] = 1

img, draw = base_frame("Step 1 — Initialization",
                        "dp[0] = 0.  Every single coin value needs exactly 1 coin.")
draw_dp_row(draw, dp)
code_panel(draw, PANEL_TOP, [
    ("dp[0] = 0;", ACCENT),
    ("for each coin c <= amount: dp[c] = 1;", FG),
    (f"coins = {coins}  ->  dp[1]=1, dp[3]=1, dp[4]=1", (170, 178, 196)),
])
add(img, 5)

# ---------- Frames for x = 1..amount ----------
for x in range(1, amount + 1):
    coin_targets = []
    best = None
    best_target = None
    for coin in coins:
        target = x - coin
        valid = target >= 0 and dp[target] is not None
        coin_targets.append((coin, target, valid))
        if valid:
            candidate = dp[target] + 1
            if best is None or candidate < best:
                best = candidate
                best_target = target

    prev_val = dp[x]  # value before this step (may already be set from init)
    if best is not None:
        if prev_val is None or best < prev_val:
            dp[x] = best
    # (dp[x] now finalized for this step)

    target_xs = {t for (_, t, v) in coin_targets if v}

    img, draw = base_frame(f"Step {x+1} — Compute dp[{x}]",
                            f"Look back at x-coin for each coin in {coins}")
    draw_dp_row(draw, dp, highlight_x=x, target_xs=target_xs, winner_x=best_target)
    draw_arrows(draw, x, coin_targets, best_target)

    calc_lines = []
    for coin, target, valid in coin_targets:
        is_winner = valid and target == best_target
        if valid:
            val = f"dp[{target}]+1 = {dp[target]}+1 = {dp[target]+1}"
            color = POINTER_COLOR if is_winner else POINTER_COLOR_DIM
        elif target < 0:
            val = "invalid (negative index)"
            color = GRAY
        else:
            val = "unreachable (dp[target] = INF)"
            color = GRAY
        marker = " \u2605" if is_winner else ""
        calc_lines.append((f"coin={coin}: target=x-{coin}={target}  ->  {val}{marker}", color))
    calc_lines.append((f"dp[{x}] = min(...) = {dp[x]}", ACCENT))
    code_panel(draw, PANEL_TOP, calc_lines)
    draw_legend(draw)

    add(img, 5)

# ---------- Final frame ----------
img, draw = base_frame("Final Result", f"Minimum coins to make amount {amount}")
draw_dp_row(draw, dp)
answer = dp[amount] if dp[amount] is not None else -1
text_center(draw, W / 2, 420, f"dp[{amount}] = {answer}  →  fewest coins needed = {answer}",
            F_TITLE, (72, 200, 130))
lines = [
    "Every dp[x] was built purely from earlier, already-solved",
    "subproblems dp[x-coin] — no recomputation, each solved once.",
    f"Time: O(amount * len(coins))     Space: O(amount)",
]
ly = 480
for l in lines:
    text_center(draw, W / 2, ly, l, F_SUB, (170, 178, 196))
    ly += 32
add(img, 5)

out_path = "/home/claude/dp_gif/coin_change_dp.gif"
frames[0].save(
    out_path,
    save_all=True,
    append_images=frames[1:],
    duration=durations,
    loop=0,
)
print("saved", out_path, "frames:", len(frames))
