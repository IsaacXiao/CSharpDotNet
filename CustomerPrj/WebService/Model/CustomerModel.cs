

using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

//因为C#没有泛型的SortedList<T>，只能写成SortedList<Rank,无意义的模板参数>
//尝试过非泛型版本的SortedList，发现C#有装箱和拆箱的开销（根本不同于C/C++里的强制类型转换）
//如果嫌这个Placeholder影响代码的美观
//也可通过继承List<T>或包含List<T>来做成Sorted，stackoverflow有此讨论和实现
//参见https://stackoverflow.com/questions/3663613/why-is-there-no-sortedlistt-in-net
using SortedRank = System.Collections.Generic.SortedList<Rank,Placeholder?>;
using IndexedRankById = System.Collections.Generic.Dictionary<long,int>;

//C#没法像C++一样写全局函数
//为了在Main.cs中调用InitTestSamples()所以把这个类做成static
//否则可以让Controller持有Model，有没必要做单例视业务逻辑来
public static class CustomerModel
    {
    private static SortedRank leaderboard_ = new (Config.LEADERBOARD_SIZE);
    private static IndexedRankById indexed_rank_byid_ = new();
    private static ReaderWriterLock table_lock_ = new ReaderWriterLock();
    
    public static RankBoundary RankRange() { return new RankBoundary(1,leaderboard_.Count); }
    public static bool CustomerIdExsits(long id) { return indexed_rank_byid_.ContainsKey(id); }
    public static int RangeIndex(long id) 
        { 
        //Debug.Assert( true == CustomerIdExsits(id) );
        return indexed_rank_byid_[id]; 
        }

    public static void InitTestSamples()
        {
        Console.WriteLine("Init some data to do test, see samples.txt\n初始化一点点数据便于测试，见samples.txt");
        Console.WriteLine("the initial size of leaderboard 初始化的排名表大小为: {0}", leaderboard_.Capacity);
        using (StreamReader customer = new StreamReader(Config.TEST_FILE))
            {
            while (!customer.EndOfStream)
                {
                //只要测试样本的txt文件别删掉、别乱改它
                //编译器的警告就装作没看见，被断言断到就当是中奖了
                Debug.Assert(System.IO.File.Exists(Config.TEST_FILE));
                string[] item = customer.ReadLine().Split('\t');
                long customer_id = long.Parse(item[0]);
                var score = decimal.Parse(item[1]);
                leaderboard_.Add(new Rank(score, customer_id),null);
                }
            foreach(var item in leaderboard_)
                {
                Rank rank = item.Key;
                Debug.Assert( true == indexed_rank_byid_.TryAdd(rank.customerId_,leaderboard_.IndexOfKey(rank)+1) );
                }
            }
        }

    public static IEnumerable<IndexedRank> GetIndexedRank()
        {
        foreach(var item in indexed_rank_byid_)
            yield return new IndexedRank(item.Key,item.Value);
        }
    public static void AddRecord(Rank rank)
        {
        table_lock_.AcquireWriterLock(Config.LOCK_TIMEOUT);
            
            //Debug.Assert(false==leaderboard_.ContainsKey(rank));
            leaderboard_.Add(rank, null);
            Debug.Assert(true == indexed_rank_byid_.TryAdd(rank.customerId_, leaderboard_.IndexOfKey(rank) + 1));

            //Add之后的 [ first, last ) 元素排名因为移动了位置，所以要在相应的索引表里更新
            int first = leaderboard_.IndexOfKey(rank) + 1;
            int last = leaderboard_.Count;
            while (first < last)
                {
                Rank migrated = leaderboard_.Keys[first++];
                ++indexed_rank_byid_[migrated.customerId_];
                }

        table_lock_.ReleaseWriterLock();
        }
    public static decimal RemoveRecord(long customer_id)
        {
        table_lock_.AcquireWriterLock(Config.LOCK_TIMEOUT);

            int where = indexed_rank_byid_[customer_id] - 1;
            Rank rank = leaderboard_.Keys[where];
            Debug.Assert(rank.customerId_==customer_id);
            leaderboard_.RemoveAt(where);
            Debug.Assert(true == indexed_rank_byid_.Remove(customer_id));

            //Remove之后的 [ first,last ) 元素排名因为移动了位置，所以要在相应的索引表里更新
            int first = where;
            int last = leaderboard_.Count;
            while (first < last)
                {
                Rank migrated = leaderboard_.Keys[first++];
                --indexed_rank_byid_[migrated.customerId_];
                }

        table_lock_.ReleaseWriterLock();
        return rank.score_;
        }
        
    public static async Task<List<Customer>> GetCustomerByRange(int start,int end)
        {
        List<Customer> outcome = new List<Customer>();
        await Task.Run
            ( () =>
                {
                table_lock_.AcquireReaderLock(Config.LOCK_TIMEOUT);

                    for(int rank = start; rank <= end; rank++)
                        {
                        Rank item = leaderboard_.Keys[rank-1];
                        outcome.Add(new Customer(item.customerId_, item.score_, rank));
                        }

                table_lock_.ReleaseReaderLock();
                }
            );
        return outcome;
        }
    }

