# Cell Algorithms Documentation


## Table of Contents
1. [Introduction](#intro)
    1. [Performance](#performance)
2. [Base Functions](#base)


## 1.1 Performance <a name="performance"></a>
When generating maps, the usual process goes from generating a base map, and adding additional passes (IE iterating over the whole map each pass) in order to refine the map. There are also some passes that only process a subset of the map with concepts such as "regions" or "rooms" however most of the time to create a "good" map, the designer must pick and choose what and how many and in which order of passes to generate the final map. This approach will make time complexity grow as additional passes for map generation grows.
(In this case, complexity is mainly in amortized time, where p is amount of passes)
$$ O(n) = {n^2*p} $$
However this can be circumvented through different strategies such as using realistic matrix sizes, and utilizing "chunking" of maps rather than generating one large map at once.
