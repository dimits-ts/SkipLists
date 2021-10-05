# SkipLists
A skip list is a probabilistic data structure that behaves like a binary tree both on its properties and time complexity of its operations. Unlike binary trees however, skip lists are much faster when it comes to concurrent execution, which is why languages like Java use them for concurrent sorted dictionaries. C# doesn't provide such a class, which this project aims to cover.

This project implements a skip list from scratch, with the goal of optimizing performance and memory usage. A concurrent version is internally used to guarantee the best possible concurrent performance. All this is made available to the user in the form of public wrappers implementing the IDictionary and ISet interfaces already defined in the C# standard library. 

The project also contains a standard test for the skip list, as well as a working Demo that highlights its use.
