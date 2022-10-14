

TestSamples project is meant to generate a few test samples for test and debugging.
Test samples in samples.txt are not checked for validity ( I do it manually ), if no need for it, you can comment ¡°InitTestSamples()¡± out in WebService project which is my solution to this homework.
c# doesn't have global function mechanism like c++
in order to do InitTestSamples() in Main.cs, class CustomerModel is defined as static


./wwwroot/auto_update.html is to simulate http post ( requirements 1 ) automatically by opening the url
https://localhost:7292/auto_update.html
Javascript in auto_update.html is not written by me but comes from the internet.


As for data structure.jpg 
Note I choose SortedList rather than SortedDictionary.
Although SortedList supports efficient indexed retrieval of keys and in the mean time SortedDictionary is faster on insertion and removal. 
But SortedList prevails SortedDictionary by utilizing cpu cache well ( in additon to its less memory usage ).
Elements in SortedList are contiguous to take this significant advantage !!
Cpu not only caches data but also instructions to do frequent operations ( Nonfunctional Requirements of this problem ).
( See also https://youtu.be/WDIkqP4JbkE )
Accessing and manipulating cpu cached data is extremely faster than SortedDictionary which is built on binary search tree.
As for the disadvantage of SortedList, we can proactively initialize a very huge capacity to avoid runtime expansion.
c# doesn't have generic version of SortedList<T>, so I have to write SortedList<Rank,meaningless template parameter>.
Without Placeholder, an alternative way is non-generic version of SortedList storing a bunch of "object".
Unlike type cast or convert in c++, boxing and and unboxing objects in c# are costly.    
Another choice SortedList<id_, score> is not feasible either,
because the field "score" is not able to participate ( id is comparable key !! ) comparison as our business logic defined in PDF:
Two customers with the same score, their ranks are determined by their CustomerID, lower is first.
The perfect solution is to implement SortedList<T> by inheritance from List<T>, as the stackoverflow link below describes,
https://stackoverflow.com/questions/3663613/why-is-there-no-sortedlistt-in-net
we can manually make it sorted, but it will increase complexity.


As for Rank, I declared it as struct instead of class,
struct in C# is allocated on stack and class is built up from heap space,
in order to keep track of heap memory given to apps,
OS maintains a double linked list like stuff to record informations,
I would call this extra overhead like a taxes collector,
storing too many class objects in collection could cause memory inefficiency and memory fragments.


As for simultaneous requests
Async is not equal to multiple threads, it depends on CLR.
Requirement 1 is writing operation, requirement 2 and 3 are reading operations, 
ConcurrentDictionary and synchronized SortedList can only guarantee single element access (baby step) is thread safe,
but updating 2 tables should be packaged as an intact transaction ( you can see it from my code ), 
since they are coupled with one another, I use ReadWriteLock to do this concurrent work.


I would prefer to access data from browser new tab instead of visual studio swagger: 
requirement 2
https://localhost:7292/Customer/GetCustomerByRank?start=1&end=5
requirement 3
https://localhost:7292/Customer/GetCustomerById?customer_id=2&high=1&low=2


My coding style and specification are mostly from 3 technical books
1. <Code Complete 2>: how to downgrade complexity in programming
2. <Refactoring: Improving the Design of Existing Code>: how to dispel code smell and make it neat
3. <Programming Pearls>: making good use of assertion to write robust code
In addtion to these 3 points above,
if there is any other thing out of my consideration,
please let me know, I would appreciate your comments and advices.
This is the 1st time I write c# or .net core as a C++ programmer :)

