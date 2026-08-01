---
name: dsa-real-world-questions
description: 'Generate Data Structures & Algorithms (DSA) interview-prep questions framed as real-life engineering scenarios (e.g. a gaming platform with 1M users needs O(1) username lookup, so the answer is a hash table), instead of abstract textbook problems. Use this skill whenever the user asks to create, generate, frame, or practice DSA/coding-interview questions tied to real-world problems, wants a quiz or question bank covering data structures and algorithms concepts, or mentions practicing for SDE/software-engineer interviews with scenario-based questions. Supports parameters for number of questions (n), difficulty (Easy/Medium/Hard), and a specific concept filter (e.g. linked-list, graph, dynamic-programming). Outputs a self-contained HTML file with two sections -- questions only, then questions with answers (which DSA/algorithm to use and why).'
---

# DSA Real-World Question Generator

Generates a set of DSA practice questions where each question is a realistic engineering
scenario (product feature, system behavior, business constraint) that maps cleanly onto a
specific data structure or algorithm — the same style as: *"A gaming platform has 1M users;
at registration, check if a username is taken in O(1)" → Hash Table*.

The point is to help the person recognize **which DSA concept applies when the problem is
described the way it'd actually show up at work or in a system-design-flavored interview**,
not to restate LeetCode problems verbatim.

## Step 1 — Read the concept reference

Before generating anything, read `references/concepts.md`. It has:
- The full taxonomy of DSA concepts (data structures, algorithm families, techniques) to draw from.
- A style guide with worked examples of good real-world framings per category, in the exact
  register to match (concise scenario, then the concept + one-line justification).

Do not skip this — it's what keeps output grounded and consistent instead of freeform.

## Step 2 — Resolve parameters

| Parameter | Values | Default if not given |
|---|---|---|
| `n` | any positive integer | 10 |
| `difficulty` | `Easy`, `Medium`, `Hard`, or `Mixed` | `Mixed` (spread roughly evenly across the n questions) |
| `concept` | a specific concept/category from the taxonomy (e.g. `linked-list`, `graph`, `dynamic-programming`, `heap`) | none — pull from the full taxonomy |

If the person's request is ambiguous about any of these, pick the sensible default above,
state the assumption briefly in your reply, and proceed — don't block on asking unless they
gave truly conflicting instructions.

## Step 3 — Select which concepts each question targets

**If `concept` is specified:** every question comes from that concept, but vary the *sub-topic
and scenario* so they don't feel repetitive (e.g. for `graph`: one on shortest path, one on
cycle detection, one on connected components, one on topological ordering — not the same
shortest-path question four times).

**If no `concept` is specified (default — "use all DSA concepts"):**
- Walk the taxonomy in `references/concepts.md` and assign **one distinct concept per
  question**, covering as much breadth across categories (linear DS, trees/graphs, sorting/
  searching, greedy, DP, backtracking, string algorithms, bit manipulation, advanced/design
  structures) as possible before repeating anything.
- If `n` is less than or equal to the number of concepts in the taxonomy, don't repeat any
  concept.
- If `n` exceeds the number of concepts, cycle back through the taxonomy in a second pass —
  each repeat should use a **different scenario and sub-topic** than its first appearance
  (e.g. round 1 "graph" = shortest path / Dijkstra, round 2 "graph" = detecting a cycle in a
  dependency graph / topological sort).
- Favor spreading across *categories* (don't give 5 questions that are all secretly sorting
  variants) unless the person's `concept` filter says otherwise.

## Step 4 — Calibrate scenario complexity to difficulty

- **Easy**: single concept, direct/obvious mapping, one core operation (e.g. "check for
  duplicate usernames" → Hash Set).
- **Medium**: concept requires a small combination or a twist (e.g. two data structures
  together like HashMap + Doubly Linked List for LRU cache, or a graph problem needing
  weighted shortest path instead of plain BFS).
- **Hard**: multi-concept or requires real optimization reasoning (e.g. combining DP with
  graphs, needing a segment tree / Fenwick tree for range queries at scale, or requiring the
  person to justify *why* a naive approach is too slow at the given scale before naming the
  right structure). Hard questions are also where multiple optimal approaches with genuine
  trade-offs are most likely to show up (see Step 5) — that's a feature, not a complication to
  avoid.

Always ground the scenario in a believable scale or constraint (number of users, requests/sec,
memory limits, real-time requirement) — that's what makes it feel like an engineering problem
instead of a puzzle.

## Step 5 — Write the output in both HTML and Markdown

Produce **two files from the same generated question set** (write the content once, then
render it into both formats — don't regenerate scenarios independently for each, they must
match exactly). Both follow the same two-section structure, in this order:

1. **"Questions"** — numbered list of just the scenarios, each tagged with its difficulty
   (Easy/Medium/Hard badge). No hints about which DSA concept to use.
2. **"Answers & Approach"** — the same questions repeated (same numbering), each followed by a
   **naive baseline, then the optimal approach(es)**:

   - **Naive approach:** the straightforward-but-doesn't-scale solution (e.g. linear scan,
     brute-force enumeration, re-scanning on every query). State its complexity and, briefly,
     why it breaks down at the scenario's stated scale — this is what makes the optimal
     approach's payoff legible instead of asserted.
   - **Optimal approach(es):** one or more named concepts that actually solve it at scale. For
     each: **Concept**, **Why** (ties the scenario's specific constraints to the choice), and
     **Complexity** (Big-O for the relevant operations).
   - **When there is more than one reasonable optimal approach, give both** (don't collapse to
     a single "correct" answer just because one is more common) **and add a short Trade-off
     line** comparing them — e.g. hash table (O(L) hash computation, then O(1) average lookup,
     simplest/least memory) vs. trie (O(L) worst case but can terminate early on a mismatching
     prefix so average-case negative lookups are often faster, plus enables prefix-based
     features later, at the cost of per-node pointer overhead and more complex code). Real
     engineering problems usually do have more than one defensible answer — showing that is
     part of the point of this skill, not a hedge to avoid.
   - Don't manufacture a second optimal option where none meaningfully exists — a plain queue
     for FIFO processing doesn't need a contrived alternative. Only include multiple optimal
     options where the trade-off is real and worth knowing (this will naturally happen often:
     array-based vs. heap-based Dijkstra, Kruskal vs. Prim for MST, Fenwick tree vs. segment
     tree for range queries, exact LRU vs. approximate/Clock-based eviction, DFS-based vs.
     Kahn's-algorithm topological sort, Dijkstra vs. A*/D* Lite for repeated replanning, etc.)

**HTML version** (`.html`): self-contained (inline `<style>`, no external dependencies). Keep
styling clean and readable (generous line-height, clear section headers, a distinct badge
color per difficulty level, monospace for any Big-O notation) — this doesn't need to be
elaborate, just legible as a study document someone will actually read end to end.

**Markdown version** (`.md`): plain, portable formatting — `##` for the two section headers,
a numbered list for "Questions", and for "Answers & Approach" repeat each numbered question
as a `###` (or bold) heading followed by **Concept:** / **Why:** lines. Use a bracketed tag
like `[Easy]` / `[Medium]` / `[Hard]` next to each question number in place of the HTML badge.
No raw HTML inside the markdown file — it should render cleanly anywhere markdown is viewed
(GitHub, Notion, plain text editors, etc.).

Save both files to `/mnt/user-data/outputs/` and present them together with `present_files`.
Use matching filenames aside from the extension, e.g.
`dsa_questions_<n>_<difficulty>_<concept-or-mixed>.html` and `....md`.

## Notes on scope

This skill is about **conceptual pattern recognition** (recognizing "this situation calls for
a heap" or "this is a Union-Find problem"), not about generating full solutions/code — keep
the "Why" explanations to reasoning about the choice of data structure/algorithm, not a
line-by-line implementation. If the person separately asks for code for a specific question
afterward, that's a normal follow-up request, not part of this skill's output.

- created by Claude Sonnet 5 Medium
