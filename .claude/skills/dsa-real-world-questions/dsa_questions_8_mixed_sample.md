# DSA Real-World Practice Questions

*8 questions · Mixed difficulty · Spanning multiple DSA concepts*

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

**Concept:** Hash Set

**Why:** Membership check across an unordered collection of 2M items is the textbook case for a hash set — average `O(1)` lookup versus `O(log n)` for a balanced tree or `O(n)` for a plain list.

### 2. [Easy] A print queue in an office needs to process documents in exactly the order they were submitted, and multiple employees can add jobs to the queue at any time.

**Concept:** Queue (FIFO)

**Why:** "Process in the order submitted" is the definition of first-in-first-out — a stack would reverse the order, so a queue (or circular buffer for a fixed-capacity version) is the natural fit.

### 3. [Medium] A music streaming app keeps a "recently played" cache of the last 500 songs per user. Once full, playing a new song should evict whichever song hasn't been touched the longest, and re-playing an existing song should refresh its position instead of duplicating it.

**Concept:** LRU Cache (HashMap + Doubly Linked List)

**Why:** Needs O(1) lookup by song *and* O(1) "move to most-recent" / eviction of the least-recent item — a hashmap alone gives fast lookup but no cheap way to track/update recency order, so it's paired with a doubly linked list.

### 4. [Medium] A company's build system has hundreds of internal packages, where some packages depend on others being built first. Before running a build, the system needs to determine a valid build order — or detect that no valid order exists.

**Concept:** Topological Sort (+ cycle detection)

**Why:** Dependencies are directed edges in a DAG; a valid build order is exactly a topological ordering, and "no valid order exists" corresponds to detecting a cycle during the sort.

### 5. [Medium] A logistics company has a map of cities connected by roads, each with a travel time. For a given truck route, they need the fastest way to get from the depot to a delivery city, where all travel times are positive.

**Concept:** Dijkstra's Algorithm

**Why:** Weighted shortest path with non-negative weights is exactly Dijkstra's use case — plain BFS only finds shortest paths by edge *count*, not by weighted travel time.

### 6. [Hard] A telecom provider is laying fiber cable to connect 50 new towers to the network at minimum total cable cost, given the cost to lay cable between every possible pair of towers.

**Concept:** Minimum Spanning Tree (Kruskal's or Prim's algorithm)

**Why:** "Connect everything at minimum total edge cost" is precisely the MST problem. Kruskal's (sort edges + Union-Find) suits this dense-but-small graph well; Prim's would be preferable if the graph were much sparser and represented via adjacency lists.

### 7. [Hard] An analytics dashboard ingests millions of price ticks for a stock and must answer, for any arbitrary time range a user selects, the sum of trading volume in that range — while new ticks keep streaming in and old ranges can still be queried.

**Concept:** Fenwick Tree / Binary Indexed Tree (or Segment Tree)

**Why:** Needs both range-sum queries *and* point updates (new ticks) at scale — recomputing sums naively is `O(n)` per query; a Fenwick/segment tree gives `O(log n)` for both operations.

### 8. [Hard] A warehouse robot can move through a grid of shelves, but some cells are blocked and the "cost" of entering a cell changes over time (congestion). The robot needs the minimum-cost path from its dock to a target shelf, re-planning as costs update, and a plain shortest-unweighted-path approach is too slow at the warehouse's scale (10,000+ cells, updates every few seconds).

**Concept:** Dijkstra's Algorithm with a min-heap (weighted grid shortest path)

**Why:** Costs vary per cell, so plain BFS (which assumes uniform edge weight) gives the wrong answer; treating the grid as a weighted graph and running Dijkstra with a priority queue gives correct, efficiently re-computable shortest paths as weights change.
