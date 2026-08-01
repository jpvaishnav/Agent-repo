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
(with a believable scale or constraint), followed by the concept and a one-line "why."
Avoid restating textbook problem names (e.g. don't just say "implement LRU cache" — describe
the situation that *needs* one).

> **A gaming platform has 1 million users. During registration, the system must check
> whether a chosen username is already taken, instantly.**
> **Concept:** Hash Table / Hash Set
> **Why:** Average O(1) lookup regardless of how many usernames exist — a sorted structure
> would cost O(log n) per check, unnecessary here since order doesn't matter.

> **You're building a cache that holds at most 1,000 recently-used product thumbnails; when
> it's full, the least recently accessed thumbnail should be evicted first.**
> **Concept:** LRU Cache (HashMap + Doubly Linked List)
> **Why:** Need O(1) get/put *and* O(1) "move to most-recently-used" / eviction — a hashmap
> alone gives fast lookup but not fast recency tracking; pairing it with a doubly linked list
> gives O(1) reordering too.

> **A browser needs to support back and forward navigation across visited pages.**
> **Concept:** Stack (two stacks, or one stack + pointer into a list)
> **Why:** Navigation is inherently LIFO within each direction — the last page visited is the
> first one you go "back" to.

> **Given distances between Delhi and several other cities (some connected directly, some
> only via intermediate cities), find the shortest path from Delhi to Jaipur.**
> **Concept:** Dijkstra's algorithm (weighted shortest path)
> **Why:** Edge weights (distances) are non-negative and we need the shortest cumulative path
> in a weighted graph — BFS alone only works for unweighted shortest paths.

> **An e-commerce checkout needs to validate that a shopping cart's promo-code stacking rules
> don't create a circular dependency (code A requires B, B requires C, C requires A) before
> applying discounts.**
> **Concept:** Cycle detection in a directed graph
> **Why:** Dependencies are directed edges; a cycle means the rule set is unsatisfiable, so
> the system needs graph traversal (DFS with recursion-stack tracking, or Union-Find variants
> for undirected cases) rather than any linear scan.

> **A ride-sharing app needs to instantly report the single closest available driver to a
> rider's location, where drivers' locations update every few seconds.**
> **Concept:** Heap / Priority Queue (or spatial index like a k-d tree for the geo variant)
> **Why:** Repeatedly needing the current minimum/closest from a changing set is exactly what
> a min-heap is for — O(log n) update and O(1) peek at the minimum, versus O(n) rescans of an
> unsorted list.

> **A text editor's autocomplete needs to suggest all words in the dictionary that start with
> what the user has typed so far, as they type.**
> **Concept:** Trie (prefix tree)
> **Why:** Prefix lookups are the trie's core strength — O(length of prefix) traversal to find
> the subtree of all matching words, versus scanning the whole dictionary per keystroke.

> **A stock-price monitoring service needs to answer, for any given time window, the maximum
> price seen — over millions of price ticks, with new prices streaming in constantly.**
> **Concept:** Monotonic deque (sliding window maximum)
> **Why:** Recomputing the max over each window naively is O(n·k); a monotonic deque keeps
> only candidates that could still be the max, giving O(n) total across all windows.

> **A ticket-booking system needs to answer, for any range of seats, how many are already
> booked — and support marking new seats booked — across millions of queries on a
> venue with 100,000 seats.**
> **Concept:** Segment Tree / Fenwick Tree (range query + update)
> **Why:** Naive range-sum recomputation is O(n) per query; a segment/Fenwick tree gives
> O(log n) per query and update, which is what makes it viable at that query volume.

Use these as a calibration for tone and structure, not a fixed bank to copy verbatim — write
fresh scenarios (varying the product domain: fintech, gaming, e-commerce, logistics,
healthcare, social media, IoT, etc.) so a run with a large `n` doesn't feel repetitive.
