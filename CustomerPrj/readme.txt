

TestSamples��Ŀ����������һ���ʼ�������ݵģ�Ϊ����2������3ѡ�õ����ݽṹ���Ժ͵��ԣ�
д��samples.txt��
û�����ݺϷ��ԵĴ���
���ֶ�ɾ������ID�����ظ���


WebService��Ŀ�µ�wwwroot/auto_update.html������ģ������1����POST�����
����˴�https://localhost:7292/auto_update.html
�����js�����������ҵģ������˺þ����ÿ���CORSȨ�޲Ÿ㶨
firefox�������չ��RESTClient��HttpRequester��Postman�Ƿ����ʹ��ûȥ��֤��
����˵Postman���ã�


�������ݽṹ��ѡ��
����2Ҫ���Ǹ�Rank��������������3�������������ΪId��������Index
���Դ˳�����ά��2�����ϣ�
1. SortedList�洢��score,id��ʾ��Rank���������������������
2. Dictionary�洢<id,Rank>�������SortedList��������
������õ���SortedList����SortedDictionary����Ȼ��������
����ǰ����Ƚ��ں����������洢�ģ�CPU������ƶ����ܵ�Ӱ���ر�󣡣�
����������Բμ���������ԭ�����Scott Meyers�Ľ���
https://youtu.be/WDIkqP4JbkE���跭ǽ���ʣ�


���ڲ�������
�첽�����ڶ��̣߳���
CLR���ƻ��ϲ���ϵͳ�ܡ����ܡ���Ϊ����ѡ���Ƿ���Ҫ�����µ��߳�
û������ΪAsync�ĺ����п��ܱ����̲߳���ִ��
��Async�����˵ĺ����п����������߳�ͬ��ִ��
����1��д�����������Ƿ�����Async�����봮��ִ��
����2������3�Ƕ���������û���߳�д��ʱ����Բ�����ͬʱ��
�������������Ϊ�����Ԥ������ReaderWriterLock��CLR������


����URL��λ����
����û����ȫ��pdf����Ϊ�˷�����Ժ͵��ԣ����г���û�����ٸ�������
����VS�Դ���swaggerֱ���������ˢ�⼸��URL
https://localhost:7292/Customer/GetAll 			����Rank��SortedList��ʾ����������������2������3��ִ�н���ȶԣ�
https://localhost:7292/Customer/IndexedRank	����Index��Dictionary��ʾ����������������2������3��ִ�н���ȶԣ�
����2
https://localhost:7292/Customer/GetCustomerByRank?start=1&end=5
����3
https://localhost:7292/Customer/GetCustomerById?customer_id=2&high=1&low=2

