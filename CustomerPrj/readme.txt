

TestSamples项目是用于生成一点初始样本数据的（为需求2和需求3选用的数据结构测试和调试）
写在samples.txt里
没做数据合法性的处理
需手动删掉负数ID或者重复的


WebService项目下的wwwroot/auto_update.html是用来模拟需求1发送POST请求的
需如此打开https://localhost:7292/auto_update.html
里面的js代码是网上找的，折腾了好久设置跨域CORS权限才搞定
firefox浏览器扩展如RESTClient、HttpRequester、Postman是否更好使就没去验证了
（据说Postman好用）


关于数据结构的选用
需求2要的是个Rank排序结果，而需求3是在这个基础上为Id建立索引Index
所以此程序需维护2个集合：
1. SortedList存储由score,id表示的Rank，排序规则照题意来定义
2. Dictionary存储<id,Rank>，与这个SortedList关联起来
这里采用的是SortedList而非SortedDictionary，虽然各有所长
但是前者相比较于后者是连续存储的，CPU缓存机制对性能的影响特别大！！
关于这个可以参见计算机组成原理或者Scott Meyers的讲座
https://youtu.be/WDIkqP4JbkE（需翻墙访问）


关于并发请求
异步不等于多线程！！
CLR机制会结合操作系统很“智能”地为我们选择是否需要开辟新的线程
没有声明为Async的函数有可能被多线程并发执行
用Async声明了的函数有可能是用主线程同步执行
需求1是写操作，不论是否做成Async都必须串行执行
需求2和需求3是读操作，在没有线程写的时候可以并发地同时读
所以无需过多人为编码干预，交给ReaderWriterLock和CLR打理即可


关于URL定位请求
命名没有完全照pdf的是为了方便测试和调试，运行成熟没问题再改名即可
不用VS自带的swagger直接在浏览器刷这几个URL
https://localhost:7292/Customer/GetAll 			（把Rank表SortedList显示出来，便于与需求2和需求3的执行结果比对）
https://localhost:7292/Customer/IndexedRank	（把Index表Dictionary显示出来，便于与需求2和需求3的执行结果比对）
需求2
https://localhost:7292/Customer/GetCustomerByRank?start=1&end=5
需求3
https://localhost:7292/Customer/GetCustomerById?customer_id=2&high=1&low=2

