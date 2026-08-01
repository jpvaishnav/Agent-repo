# DSA Concept Taxonomy & Style Guide

## Full concept list

Use this as the pool to draw from when no specific `concept` is requested. Walk it
top-to-bottom, left-to-right for round-robin coverage. It's organized by category so you can
also filter to one category if the person names a broad area (e.g. "graph algorithms").

### 1. Linear data structures
- Arrays / dynamic arrays
- Strings
- Linked list — singly linked
- Linked list — doubly linked
- Linked list — circular
- Stack
- Queue (simple)
- Deque (double-ended queue)
- Circular queue / ring buffer

### 2. Hashing
- Hash table / hash map
- Hash set
- Collision handling (chaining, open addressing)

### 3. Trees
- Binary tree
- Binary search tree (BST)
- Self-balancing BST (AVL / Red-Black)
- Heap / priority queue (min-heap, max-heap)
- Trie (prefix tree)
- N-ary tree
- Segment tree (range queries/updates)
- Fenwick tree / Binary Indexed Tree (prefix sums, range updates)

### 4. Graphs
- Graph representation (adjacency list/matrix)
- Breadth-first search (BFS)
- Depth-first search (DFS)
- Shortest path — Dijkstra (weighted, non-negative)
- Shortest path — Bellman-Ford (handles negative weights)
- Shortest path — Floyd-Warshall (all-pairs)
- Minimum spanning tree — Kruskal
- Minimum spanning tree — Prim
- Topological sort
- Cycle detection (directed/undirected)
- Union-Find / Disjoint Set Union (DSU)
- Bipartite check
- Strongly connected components (Kosaraju / Tarjan)
- Articulation points & bridges

### 5. Sorting
- Comparison sorts (merge sort, quick sort, heap sort)
- Simple sorts (bubble, selection, insertion) — mainly for small-n or nearly-sorted cases
- Non-comparison sorts (counting sort, radix sort, bucket sort)

### 6. Searching
- Linear search
- Binary search (and variants: first/last occurrence, search in rotated sorted array, search
  in answer space / "binary search on the answer")

### 7. Two pointers / sliding window
- Fixed-size sliding window
- Variable-size sliding window
- Two-pointer (opposite ends / fast-slow)

### 8. Recursion & divide and conquer
- Plain recursion / recursion tree reasoning
- Divide and conquer (merge sort style, closest pair of points, etc.)

### 9. Backtracking
- Permutations / combinations / subsets generation
- Constraint satisfaction (N-Queens, Sudoku)
- Maze / path exploration with pruning

### 10. Greedy algorithms
- Interval scheduling / activity selection
- Huffman coding
- Job sequencing / deadline scheduling

### 11. Dynamic programming
- 0/1 knapsack family
- Longest common subsequence (LCS) / edit distance
- Longest increasing subsequence (LIS)
- Matrix chain multiplication style (interval DP)
- Coin change / unbounded knapsack family
- DP on trees
- DP on grids/paths

### 12. String algorithms
- Pattern matching — KMP
- Pattern matching — Rabin-Karp (rolling hash)
- Z-algorithm
- Palindrome-specific (Manacher's algorithm)
- Trie-based string problems (autocomplete, prefix search)

### 13. Bit manipulation
- Bitmasking for subsets/state compression
- Basic bit tricks (count set bits, power of two check, XOR properties)

### 14. Math & number theory
- GCD/LCM (Euclidean algorithm)
- Sieve of Eratosthenes (prime generation)
- Modular exponentiation / modular arithmetic
- Combinatorics basics (nCr, factorial-based counting)

### 15. Advanced / design data structures
- LRU cache (HashMap + Doubly Linked List)
- LFU cache
- Monotonic stack / monotonic queue (next greater element, sliding window max)
- Skip list
- Bloom filter

---

## Style guide: worked examples

Match this register: a short, concrete scenario grounded in a real product/system context
(with a believable scale or constraint), followed by a **naive baseline** and then the
**optimal approach(es)**. Avoid restating textbook problem names (e.g. don't just say
"implement LRU cache" — describe the situation that *needs* one).

> **A gaming platform has 1 million users. During registration, the system must check
> whether a chosen username is already taken, instantly.**
>
> **Naive:** Scan the full list of existing usernames on every check. O(n) per lookup — at
> 1M+ users and every registration attempt, this doesn't hold up.
>
> **Optimal — Hash Set:** compute a hash of the input (O(L), L = username length — this cost
> is paid in every case, best/average/worst, since the whole string must be hashed) then do
> an O(1) average lookup into the table. Simple, low memory overhead, the default choice when
> all you need is exact membership.
>
> **Optimal — Trie:** O(L) worst case, but many "not taken" checks diverge from every stored
> username within the first few characters, so the average case for negative lookups is often
> faster than a hash table's fixed O(L) hashing cost. Also gives prefix-based features for
> free later (e.g. "suggest similar available usernames"). Trade-off: more memory per node
> (child pointers) and more implementation complexity than a flat hash set.
>
> **Trade-off:** hash set is simpler and usually sufficient; trie wins if negative lookups
> dominate or prefix functionality is a real future need — not just a hypothetical one.

> **You're building a cache that holds at most 1,000 recently-used product thumbnails; when
> it's full, the least recently accessed thumbnail should be evicted first.**
>
> **Naive:** Keep a plain list; on every access, linearly scan to find the item and move it to
> the front. O(n) per access.
>
> **Optimal — HashMap + Doubly Linked List (exact LRU):** O(1) get/put and O(1) "move to
> most-recently-used" / eviction. The hashmap alone gives fast lookup but not fast recency
> reordering; the doubly linked list is what makes reordering O(1) too.
>
> **Optimal — Approximate LRU / Clock algorithm:** a circular buffer with a per-entry "recently
> used" bit, sweeping like a clock hand to find eviction candidates. Cheaper per-entry memory
> (no pointer-heavy DLL nodes) and simpler than exact LRU, at the cost of only approximating
> true recency order — the same trade real systems like OS page caches and Redis's
> approximated-LRU mode make deliberately.
>
> **Trade-off:** exact LRU when correctness of "least recently used" matters precisely;
> approximate/Clock when you're optimizing for memory/throughput and near-LRU behavior is
> good enough.

> **A browser needs to support back and forward navigation across visited pages.**
>
> **Naive:** Store visited pages in a single array and track an index; "going back" after
> visiting a new page from a back-state means shifting/truncating array contents. Works, but
> gets fiddly and copy-heavy compared to the structure that matches the access pattern.
>
> **Optimal — Two Stacks:** one stack for "back" history, one for "forward" — going back pops
> from one and pushes onto the other. O(1) per navigation action, and it matches the problem's
> inherent LIFO structure directly (the last page visited is the first one "back" returns to).

> **Given distances between Delhi and several other cities (some connected directly, some
> only via intermediate cities), find the shortest path from Delhi to Jaipur.**
>
> **Naive:** Enumerate all possible routes between the two cities and sum distances. Exponential
> in the number of cities — infeasible beyond a handful of nodes.
>
> **Optimal — Dijkstra (array-based):** O(V²) — fine for small/dense city graphs.
>
> **Optimal — Dijkstra (min-heap/priority queue):** O((V + E) log V) — scales much better as
> the number of cities/roads grows and the graph stays sparse.
>
> **Trade-off:** array-based is simpler to write and fine for a small fixed city list;
> heap-based is the right call once the road network is large and sparse (most real maps are).

> **An e-commerce checkout needs to validate that a shopping cart's promo-code stacking rules
> don't create a circular dependency (code A requires B, B requires C, C requires A) before
> applying discounts.**
>
> **Naive:** Try applying rules repeatedly until nothing changes or a fixed iteration cap is
> hit, and assume a cycle if the cap is reached. Imprecise and wastes work re-checking the same
> dependencies.
>
> **Optimal — Cycle detection in a directed graph (DFS with recursion-stack tracking, or
> Kahn's algorithm / topological sort leaving leftover nodes):** treats rules as directed
> edges and detects a cycle in O(V + E), definitively rather than heuristically.

> **A ride-sharing app needs to instantly report the single closest available driver to a
> rider's location, where drivers' locations update every few seconds.**
>
> **Naive:** Rescan every available driver's location and compute distance on each request.
> O(n) per request — expensive at city scale with constant location churn.
>
> **Optimal — Heap / Priority Queue (by distance to a fixed query point):** O(log n) update,
> O(1) peek at the current minimum.
>
> **Optimal — Spatial index (e.g. k-d tree / geohash grid) for arbitrary query points:**
> O(log n)-ish nearest-neighbor queries for *any* rider location, not just one fixed point —
> the right structure once riders (not just drivers) are also moving/varied.
>
> **Trade-off:** a heap is enough if you're always querying distance from one reference point;
> a spatial index is needed once you must answer "nearest driver" for many different, changing
> rider locations efficiently.

> **A text editor's autocomplete needs to suggest all words in the dictionary that start with
> what the user has typed so far, as they type.**
>
> **Naive:** Filter the entire dictionary by string-prefix match on every keystroke. O(n·L)
> per keystroke.
>
> **Optimal — Trie (prefix tree):** O(length of prefix) to reach the matching subtree, then
> enumerate matches from there — dramatically cheaper than rescanning the whole dictionary
> per keystroke.

> **A stock-price monitoring service needs to answer, for any given time window, the maximum
> price seen — over millions of price ticks, with new prices streaming in constantly.**
>
> **Naive:** Recompute the max by scanning each window directly. O(n·k) across all windows.
>
> **Optimal — Monotonic deque (sliding window maximum):** keeps only candidates that could
> still become the max as the window slides, giving O(n) total across all windows instead of
> O(n·k).

> **A ticket-booking system needs to answer, for any range of seats, how many are already
> booked — and support marking new seats booked — across millions of queries on a
> venue with 100,000 seats.**
>
> **Naive:** Recompute the range sum by scanning the seat array on every query, and update a
> single cell directly. O(n) per range query even though updates are cheap.
>
> **Optimal — Fenwick Tree / Binary Indexed Tree:** O(log n) per point update and O(log n) per
> prefix/range-sum query. Lean, low memory overhead, easiest to implement when sums (or other
> invertible operations) are all you need.
>
> **Optimal — Segment Tree:** also O(log n) per update/query, with more implementation/memory
> overhead than a Fenwick tree, but generalizes beyond sums — range min/max/gcd, and range
> *updates* via lazy propagation, if requirements are likely to grow beyond simple range sums.
>
> **Trade-off:** Fenwick tree when the need is (and will likely stay) range-sum-shaped;
> segment tree when you need more query types or foresee needing them.

Use these as a calibration for tone and structure, not a fixed bank to copy verbatim — write
fresh scenarios (varying the product domain: fintech, gaming, e-commerce, logistics,
healthcare, social media, IoT, etc.) so a run with a large `n` doesn't feel repetitive. Not
every question needs two optimal options — only include a second when the trade-off is real
(see Step 5 of SKILL.md); several of the examples above intentionally have just one.
