# DSA Real-World Practice Questions

*8 questions · Mixed difficulty · Naive baseline & optimal approach(es) with trade-offs*

## Questions

1. **[Easy]** A food-delivery app has 2 million registered phone numbers. When someone tries to sign up, the system must instantly confirm whether that phone number is already registered.

2. **[Easy]** A print queue in an office needs to process documents in exactly the order they were submitted, and multiple employees can add jobs to the queue at any time.

3. **[Medium]** A music streaming app keeps a "recently played" cache of the last 500 songs per user. Once full, playing a new song should evict whichever song hasn't been touched the longest, and re-playing an existing song should refresh its position instead of duplicating it.

4. **[Medium]** A company's build system has hundreds of internal packages, where some packages depend on others being built first. Before running a build, the system needs to determine a valid build order — or detect that no valid order exists.

5. **[Medium]** A logistics company has a map of cities connected by roads, each with a travel time. For a given truck route, they need the fastest way to get from the depot to a delivery city, where all travel times are positive.

6. **[Hard]** A telecom provider is laying fiber cable to connect 50 new towers to the network at minimum total cable cost, given the cost to lay cable between every possible pair of towers.

7. **[Hard]** An analytics dashboard ingests millions of price ticks for a stock and must answer, for any arbitrary time range a user selects, the sum of trading volume in that range — while new ticks keep streaming in and old ranges can still be queried.

8. **[Hard]** A warehouse robot can move through a grid of shelves, but some cells are blocked and the "cost" of entering a cell changes over time (congestion). The robot needs the minimum-cost path from its dock to a target shelf, re-planning as costs update, and a plain shortest-unweighted-path approach is too slow at the warehouse's scale (10,000+ cells, updates every few seconds).

## Answers & Approach

### 1. [Easy] A food-delivery app has 2 million registered phone numbers. When someone tries to sign up, the system must instantly confirm whether that phone number is already registered.

**Naive — Linear scan:** Check the new number against every stored number. `O(n)` per check — at 2M+ numbers and every signup attempt, this doesn't hold up.

**Optimal — Hash Set:** Compute a hash of the number — `O(L)`, L = number length, paid in every case (best/avg/worst), since the whole string must be hashed — then do an average `O(1)` lookup. Simplest, lowest memory overhead, the default choice for exact membership.

**Optimal (alternative) — Trie (digit tree):** `O(L)` worst case, but many "not registered" checks diverge from every stored number within the first few digits, so average-case negative lookups are often faster than a hash set's fixed O(L) hashing cost. Also enables prefix-based features later (e.g. grouping by area/country code) for free. Trade-off: more memory per node (child pointers) and more implementation complexity than a flat hash set.

**Trade-off:** hash set is simpler and sufficient for pure membership checks; trie is worth it if negative lookups dominate or prefix-based features are a genuine future need — not just hypothetical.

### 2. [Easy] A print queue in an office needs to process documents in exactly the order they were submitted, and multiple employees can add jobs to the queue at any time.

**Naive — Array with front-removal:** Store jobs in an array; removing the front job means shifting every remaining element down. `O(n)` per dequeue.

**Optimal — Queue (linked-list based or circular buffer):** "Process in the order submitted" is the definition of FIFO. A linked-list queue or circular buffer gives `O(1)` enqueue/dequeue — a stack would reverse the order entirely, so it's ruled out.

**Trade-off:** linked-list queue is unbounded but has per-node pointer overhead; a circular buffer (fixed-size array) is more cache-friendly and memory-efficient but needs resizing logic once capacity is hit.

### 3. [Medium] A music streaming app keeps a "recently played" cache of the last 500 songs per user. Once full, playing a new song should evict whichever song hasn't been touched the longest, and re-playing an existing song should refresh its position instead of duplicating it.

**Naive — Plain list, linear scan on every access:** Find the song by scanning, then move it to the front. `O(n)` per access.

**Optimal — LRU Cache (HashMap + Doubly Linked List):** `O(1)` lookup by song and `O(1)` "move to most-recent" / eviction. The hashmap alone gives fast lookup but not fast recency reordering — pairing it with a doubly linked list makes reordering O(1) too.

**Optimal (alternative) — Approximate LRU / Clock algorithm:** A circular buffer with a per-entry "recently used" bit, swept like a clock hand to pick eviction candidates. Cheaper per-entry memory than pointer-heavy DLL nodes and simpler to implement, at the cost of only approximating true recency — the same trade real systems (OS page caches, Redis's approximated-LRU mode) make deliberately.

**Trade-off:** exact LRU when correctness of "least recently used" matters precisely; Clock/approximate LRU when optimizing for memory and throughput, where near-LRU behavior is good enough.

### 4. [Medium] A company's build system has hundreds of internal packages, where some packages depend on others being built first. Before running a build, the system needs to determine a valid build order — or detect that no valid order exists.

**Naive — Trial-and-error ordering:** Repeatedly try to build whatever seems ready and retry failures until stable, or check all permutations of build order for validity. Wasteful re-checking, and permutation checking is factorial — infeasible at hundreds of packages.

**Optimal — Topological Sort (DFS-based):** Dependencies are directed edges in a DAG; a valid build order is exactly a topological ordering. DFS with post-order reversal gives the order in `O(V + E)`, and a node revisited on the current recursion stack signals a cycle (no valid order).

**Optimal (alternative) — Topological Sort (Kahn's Algorithm / BFS with in-degree counting):** Also `O(V + E)`. Explicitly surfaces a cycle as "leftover nodes with nonzero in-degree" rather than via recursion-stack tracking, avoids recursion depth limits on very large dependency graphs, and naturally groups nodes by "build level" — useful if builds at the same level can run in parallel.

**Trade-off:** DFS-based is slightly simpler to code; Kahn's is safer for very deep graphs (no recursion limit) and gives parallel-build grouping for free.

### 5. [Medium] A logistics company has a map of cities connected by roads, each with a travel time. For a given truck route, they need the fastest way to get from the depot to a delivery city, where all travel times are positive.

**Naive — Brute-force route enumeration:** Enumerate all possible routes and sum travel times. Exponential in the number of cities — infeasible beyond a handful of nodes.

**Optimal — Dijkstra's Algorithm (array-based):** `O(V²)` — fine for a small/dense city graph. Weighted shortest path with non-negative weights is exactly Dijkstra's use case; plain BFS only finds shortest paths by edge count, not weighted travel time.

**Optimal (alternative) — Dijkstra's Algorithm (min-heap / priority queue):** `O((V + E) log V)` — scales far better once the road network is large and sparse (most real road networks are).

**Trade-off:** array-based is simpler to write and fine for a small fixed city list; heap-based is the right call once the network is large and sparse.

### 6. [Hard] A telecom provider is laying fiber cable to connect 50 new towers to the network at minimum total cable cost, given the cost to lay cable between every possible pair of towers.

**Naive — Enumerate all spanning trees:** Try every possible spanning tree and pick the cheapest. By Cayley's formula there are n^(n-2) spanning trees on n nodes — combinatorially explosive well before 50 towers.

**Optimal — Kruskal's Algorithm (sort edges + Union-Find):** `O(E log E)`. "Connect everything at minimum total edge cost" is precisely the MST problem. Kruskal's suits this scenario directly since all pairwise costs are already given as a flat edge list.

**Optimal (alternative) — Prim's Algorithm (with min-heap):** `O(E log V)`. Better suited if the graph were represented via adjacency lists and much sparser (e.g. towers only have quoted costs to nearby towers, not every pair).

**Trade-off:** Kruskal's is the natural fit here since costs are given for every pair (dense, edge-list-shaped input); Prim's is preferable when the graph is sparse and adjacency-list-shaped.

### 7. [Hard] An analytics dashboard ingests millions of price ticks for a stock and must answer, for any arbitrary time range a user selects, the sum of trading volume in that range — while new ticks keep streaming in and old ranges can still be queried.

**Naive — Direct scan or plain prefix-sum array:** Scan the relevant range on every query (`O(n)` per query), or maintain a prefix-sum array that must be rebuilt on every new tick (`O(n)` per update). Either way, one of the two operations degrades badly at millions of ticks.

**Optimal — Fenwick Tree / Binary Indexed Tree:** `O(log n)` per point update (new tick) and per prefix/range-sum query. Lean, low memory overhead — the right default when sums are all that's needed.

**Optimal (alternative) — Segment Tree:** Also `O(log n)` per update/query, with more implementation and memory overhead than a Fenwick tree, but generalizes beyond sums — range min/max/gcd, and range *updates* via lazy propagation, if requirements are likely to expand.

**Trade-off:** Fenwick tree when the need is (and will likely stay) range-sum-shaped; segment tree when more query types are needed now or foreseeably.

### 8. [Hard] A warehouse robot can move through a grid of shelves, but some cells are blocked and the "cost" of entering a cell changes over time (congestion). The robot needs the minimum-cost path from its dock to a target shelf, re-planning as costs update, and a plain shortest-unweighted-path approach is too slow at the warehouse's scale (10,000+ cells, updates every few seconds).

**Naive — Unweighted BFS, or full re-plan from scratch:** Plain BFS assumes uniform edge cost and ignores congestion, giving a wrong/suboptimal path; alternatively, recomputing a full weighted search from scratch on every cost update is too slow at 10,000+ cells updating every few seconds.

**Optimal — Dijkstra's Algorithm with a min-heap (weighted grid):** `O((V + E) log V)` per full replan. Treating the grid as a weighted graph and using a priority queue gives correct shortest paths under varying per-cell cost, unlike unweighted BFS.

**Optimal (alternative) — A\* Search (heuristic-guided):** Same worst-case complexity as Dijkstra, but an admissible heuristic (e.g. Manhattan/Euclidean distance to the target) directs the search toward the goal, often visiting far fewer nodes in practice. Trade-off: needs a good heuristic to pay off.

**Optimal (alternative) — D\* Lite / incremental replanning:** Reuses prior search results and only recomputes the portion of the path affected by a cost change, rather than restarting from scratch every few seconds — the best fit for genuinely repeated replanning, at the cost of significantly higher implementation complexity.

**Trade-off:** plain Dijkstra is simplest and fine if replans are infrequent; A* speeds up each individual replan with a decent heuristic; D* Lite is the real answer once replanning happens continuously and re-solving from scratch every time is itself the bottleneck.
