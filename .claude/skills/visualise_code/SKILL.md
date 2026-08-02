---
name: visualise_code
description: Use whenever the user asks to "visualize", "show visually", "create a picture/image/GIF/video/animation of" a piece of code, an algorithm, or a solution's logic/flow (e.g. DP recurrences, iteration steps, pointer movement, graph/tree traversal, sequence/data flow). Produces a single explanatory diagram (image) for a static overview, or a frame-by-frame GIF/video for step-by-step / sequential execution. Not for generic charts of numeric data (use the data-viz / chart tooling for that) — this skill is specifically for illustrating how CODE executes, step by step.
---

# Visualise Code

## When to use this skill
Trigger this skill when the user wants to *see* how a piece of code or an
algorithm works, rather than just read an explanation. Typical phrasings:
- "create a visual gif of this"
- "show this DP as animated frames"
- "make a picture explaining this algorithm"
- "visualize the recursion / pointer movement / iteration"

## Core design principle: sequence-flow
Every visual must follow a **sequence flow design pattern** — i.e. it should
read like a story with a clear beginning, middle, and end, mirroring the
actual order of execution of the code (iteration by iteration, recursive
call by recursive call, pointer move by pointer move). Do not just dump a
final-state diagram; walk the viewer through *how the state got there*.

Concretely, for iterative/DP code this means: one frame per loop iteration
(or per meaningfully distinct state change), always showing:
1. What changed this step (the current index/node/pointer, highlighted).
2. Where the new value/state came from (arrows back to the earlier
   state(s) it depends on — e.g. `dp[i-1]`, `dp[i-2]`, `dp[x-coin]`, a
   parent node, a visited neighbor, etc).
3. The actual computation/comparison with real numbers plugged in (not just
   the symbolic formula) — this is what makes it "explain like iterations".
4. The result of that step (updated array/state + running answer so far).

## Deciding: single image vs GIF vs video
- **Single static image**: use when the user wants one picture that explains
  the whole approach/concept at a glance (e.g. "explain this algorithm with
  a diagram", architecture/flow overview, or just a summary infographic with
  no real step-by-step needed). One `show_widget`-quality PNG (or SVG via the
  Visualizer if it's a quick inline diagram) is enough.
- **GIF**: default choice for step-by-step / sequence / data-flow requests
  — DP tables filling in, pointers moving, recursion unwinding, array/tree
  traversal, BFS/DFS frontier expansion, etc. This is the most common case
  for "visualize this code" requests.
- **Video (mp4)**: use only if the user explicitly asks for a video, or the
  number of frames is large enough that a GIF file size would be unreasonable
  (rule of thumb: more than ~25-30 frames, or the user wants
  audio/narration). Build it from the same PNG frames via ffmpeg (see below)
  instead of Pillow's GIF writer.

## Frame timing
**Default frame interval is 5 seconds per frame**, unless the user specifies
a different interval. Always confirm/apply whatever interval the user asks
for; 5s is only the fallback default when nothing is specified.

## Libraries & tools to use
- **Pillow (`PIL`)** — the primary tool for drawing each frame (boxes,
  arrows, text, code panels) and for assembling frames into a GIF via
  `Image.save(..., save_all=True, append_images=..., duration=..., loop=0)`.
  Already installed in this environment; no pip install needed.
- **DejaVu fonts** (already on the system) — use these truetype fonts for
  crisp text instead of Pillow's default bitmap font:
  - `/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf` (titles, box values)
  - `/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf` (subtitles/body)
  - `/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf` (code/computation lines — always use a mono font for anything that looks like code so columns line up)
- **ffmpeg** (already installed, on PATH) — use this instead of Pillow when
  a video output is needed. Typical pattern: save each frame as a numbered
  PNG (`frame_00.png`, `frame_01.png`, ...), then:
  ```bash
  ffmpeg -y -framerate 1/5 -i frame_%02d.png -vf "fps=25,format=yuv420p" -c:v libx264 output.mp4
  ```
  (`-framerate 1/5` = 5 seconds shown per input frame; raise/lower to match
  the requested interval.)
- Do **not** reach for `matplotlib`/`plotly` for this kind of visual — those
  are for numeric charts, not for hand-drawn step diagrams with boxes,
  arrows, and code panels. Pillow gives full manual control over layout,
  which is what a sequence-flow diagram needs.

## Visual consistency rule (critical — apply to every visual)
**The same category/kind of element must always use the same color and
style, everywhere in the visual.** This is the single biggest factor in
whether a viewer can "grasp it" quickly — a new color per branch/coin/case
forces the viewer to relearn the legend at every frame, which defeats the
purpose. Concretely:
- If several things are all "a dependency pointer read from an earlier
  state" (e.g. `dp[i-1]` and `dp[i-2]`, or `dp[x-coin]` for several
  different coins), they are the SAME category of element and get the SAME
  color — not a different hue per coin/branch/case. Do not invent a rainbow
  just because there happen to be 3 branches instead of 2.
- Distinguish *which one was chosen* (e.g. the winner of a `min()`/`max()`)
  by **weight/emphasis within that same color**, not by switching to a new
  color: full-brightness + thicker line + a glow ring/star for the winner,
  the same hue but dimmed/thinner for the ones considered and rejected.
  Color answers "what kind of thing is this"; brightness/weight answers
  "which one won."
- Reserve a genuinely different color only for a genuinely different
  category of element — e.g. the "current index/node being computed" (an
  accent color, distinct from the dependency-pointer color) is a different
  kind of thing from "a value being read", so it legitimately gets its own
  color. Two colors total (current-index accent + dependency-pointer color,
  with brightness for winner/loser) is usually enough for most DP/traversal
  visuals — resist adding more.
- Keep this mapping identical across every frame of the same GIF/video, and
  include a small legend the first time a color is introduced if it isn't
  self-evident.

## Reusable visual language (apply consistently)
Keep a small, consistent visual grammar across frames so the sequence reads
clearly without extra explanation:
- **Dark background** (`(18, 20, 28)` works well) with light text — reads
  cleanly as a GIF and doesn't blow up file size on plain backgrounds.
- **Accent color for "current index/node being computed"**, and a single,
  separate color for "a dependency/value being read from an earlier state"
  (see the Visual consistency rule above — do not multiply colors per
  branch/coin/case).
- **A code/computation panel** on every step-frame showing the actual
  formula with real numbers substituted in (not just symbols) — this is
  what makes the GIF double as a worked dry-run, matching how the user
  writes dry-run comments in their own code.
- **Rounded boxes** for array/dp cells, **thin arrows with arrowheads**
  for dependencies, **monospace font** for anything code-like.
- First frame = intro (problem + recurrence/approach in plain text).
  Last frame = final state + answer + complexity notes.
- **Layout margins**: when stacking multiple arrow/label rows below an
  array (one row per dependency), compute the panel/box that goes below
  them dynamically from how many rows there are — a hardcoded y-offset that
  worked for 2 dependencies will silently get overlapped/hidden by a panel
  when a 3rd dependency row is added. Always leave a clear margin
  (~30-40px) between the lowest drawn element and the next panel's top.

## Reference examples
Two complete, working end-to-end scripts are included in
`reference_scripts/` in this skill folder — read one before starting a new
visualization, then adapt its helper functions (`text_center`,
`rounded_box`, `draw_arrows`, frame-composition helpers, GIF assembly) to
the new problem rather than rewriting from scratch:

- `reference_scripts/example_house_robber_dp_gif.py` — House Robber DP
  (`dp[i] = max(nums[i]+dp[i-2], dp[i-1])`). Good reference for a **two-way**
  dependency (include vs exclude) with green/yellow source coloring.
- `reference_scripts/example_coin_change_dp_gif.py` — Coin Change DP
  (`dp[x] = min(dp[x-coin]+1)` over multiple coins). Good reference for an
  **N-way** dependency fan-out where every dependency pointer shares ONE
  consistent color, and the min()-winner is distinguished purely by
  brightness/weight/glow — not a different hue — per the visual consistency
  rule above. Also demonstrates computing panel/legend layout dynamically
  from the number of dependency rows instead of hardcoding offsets.

Both scripts follow the exact workflow this skill describes: define frame
helpers → build one frame per step with real numbers substituted in →
`save_all=True` GIF assembly with `duration` in milliseconds per frame.

## Workflow checklist
1. Understand the code's step-by-step execution order (loop iterations,
   recursive calls, pointer moves) — this determines your frame count.
2. Pick image vs GIF vs video per the section above.
3. Skim one reference script in `reference_scripts/` for the helper-function
   pattern to reuse.
4. Build frames with real substituted numbers, consistent color-per-source,
   and an intro + final-result frame.
5. Assemble with Pillow (GIF) or ffmpeg (video) using the requested frame
   interval (default 5s).
6. Save the output to `/mnt/user-data/outputs/` and use `present_files` to
   share it — never just describe the visual in text.
