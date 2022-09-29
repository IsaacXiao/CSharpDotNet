

TestSamples项目是用于生成一点初始样本数据的，便于测试和调试
写在samples.txt里，没做数据合法性的处理（需手动删掉负数ID或者重复的）
若不需要的话，在WebService项目里把InitTestSamples注释掉即可


WebService项目才是用于答题的
目录下wwwroot/auto_update.html是用来模拟需求1发送POST请求的
需如此打开https://localhost:7292/auto_update.html


关于数据结构的选用见jpg图
注意这里对Rank采用的是SortedList而非SortedDictionary
虽然他们在数据访问的时间复杂度上各有所长
但是前者相比较于后者是连续存储的，CPU缓存机制对性能的影响特别大!!!!
SortedList更适合满足题目要求的frequent operations 
关于这个可以参见计算机组成原理或者Scott Meyers的讲座
https://youtu.be/WDIkqP4JbkE（需翻墙访问）


关于并发请求
异步不等于多线程！！
CLR机制会结合操作系统很“智能”地为我们选择是否需要开辟新的线程
没有声明为Async的函数有可能被多线程并发执行
用Async声明了的函数有可能是用主线程同步执行
需求1是写操作，不论是否做成Async都必须串行执行
需求2和需求3是读操作，在没有线程写的时候可以并发地同时读
所以这里是用读写锁来区分对待


关于URL定位请求
命名没有完全照pdf的是为了方便测试和调试，运行成熟没问题再改名即可

不用VS自带的swagger直接在浏览器如此更方便：
https://localhost:7292/Customer/GetAll 			（把Rank表SortedList显示出来，浏览器开标签页显示比断点调试更直观）
https://localhost:7292/Customer/IndexedRank	（把Index表Dictionary显示出来，浏览器开标签页显示比断点调试更直观）
需求2
https://localhost:7292/Customer/GetCustomerByRank?start=1&end=5
需求3
https://localhost:7292/Customer/GetCustomerById?customer_id=2&high=1&low=2


我的代码风格与规范主要源自3本技术书籍
《代码大全2》——降低程序设计的复杂度
《重构——改善既有代码设计》——消除“坏味道”追求极致的可读性
《编程珠玑》——多多善用assert消除bug隐患
除此3条之外若还有什么没考虑周全之处望多多指教！！
（作为C++程序员转型C#第一次写.net core噢）

