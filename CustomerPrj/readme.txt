

注意生成的测试样本ID是随机的
ID要是生成了重复的则重新运行程序即可
靠代码处理麻烦：
https://blog.csdn.net/cjolj/article/details/59486865



C#没有像C++一样的作用域限定符::
所以自定义命名空间或者全局函数/变量通通大写

C++的容器是通过基于placement new的内存分配器来管理、装东西的
即便如此，在语义上也最好是按值存储而不是存放指针
C++17加入了std::pmr::synchronized_pool_resource

C#中的collection内存管理机制未知，存储class而非struct会否有内存碎片的问题不得而知
大量存储struct会否要操心stackoverflow而去调整编译器的设置就不得而知了


CLR机制和操作系统会很智能地为我们选择是否需要开辟新的线程来做异步操作



