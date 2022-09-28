
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


//TODO: 有空把SortedList改为泛型版本的SortedList以消除装箱和拆箱的开销
using SortedRank = System.Collections.SortedList;
//using SortedRank = System.Collections.Generic.SortedList<Rank,string>;
using IndexedRankById = System.Collections.Generic.Dictionary<long,int>;

//C#没法像C++一样写全局函数
//为了在Main.cs中调用InitTestSamples()所以把这个类做成static
//否则可以让Controller持有Model，但是没必要做单例
public static class CustomerModel
    {
    private static SortedRank leaderboard_ = new (Config.LEADERBOARD_SIZE);
    private static IndexedRankById indexed_rank_byid_ = new();
    private static ReaderWriterLock table_lock_ = new ReaderWriterLock();

    public static RankBoundary RankRange() { return new RankBoundary(1,leaderboard_.Count); }
    public static bool CustomerIdExsits(long id) { return indexed_rank_byid_.ContainsKey(id); }
    public static int RangeIndex(long id) { return indexed_rank_byid_[id]; }

    //为了便于测试和调试，人为写入少许测试样本
    //在程序刚启动运行时从文件中读进来
    public static void InitTestSamples()
        {
        Console.WriteLine("Init some data to do test, see samples.txt\n初始化一点点数据便于测试，见samples.txt");
        Console.WriteLine("the initial size of leaderboard 初始化的排名表大小为: {0}", leaderboard_.Capacity);
        using (StreamReader customer = new StreamReader(Config.TEST_FILE))
            {
            while (!customer.EndOfStream)
                {
                //只要测试样本的txt文件没删掉，编译器的警告就装作没看见
                Debug.Assert(System.IO.File.Exists(Config.TEST_FILE));
                string[] item = customer.ReadLine().Split('\t');
                long customer_id = long.Parse(item[0]);
                var score = decimal.Parse(item[1]);
                leaderboard_.Add(new Rank(score, customer_id),null);
                }
            foreach(DictionaryEntry item in leaderboard_)
                {
                Rank rank = (Rank)(item.Key);
                indexed_rank_byid_.TryAdd(rank.customerId_,leaderboard_.IndexOfKey(rank)+1);
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

            leaderboard_.Add(rank, null);
            Debug.Assert(true == indexed_rank_byid_.TryAdd(rank.customerId_, leaderboard_.IndexOfKey(rank) + 1));

            //Add之后的[first,last)元素排名因为移动了位置，所以要在相应的索引表里更新
            int first = leaderboard_.IndexOfKey(rank) + 1;
            int last = leaderboard_.Count;
            while (first < last)
                {
                Rank migrated = (Rank)(leaderboard_.GetKey(first++));
                ++indexed_rank_byid_[migrated.customerId_];
                }

        table_lock_.ReleaseWriterLock();
        }
    public static decimal RemoveRecord(long customer_id)
        {
        table_lock_.AcquireWriterLock(Config.LOCK_TIMEOUT);

            int where = indexed_rank_byid_[customer_id] - 1;
            Rank rank = (Rank)(leaderboard_.GetKey(where));
            Debug.Assert(rank.customerId_==customer_id);
            leaderboard_.RemoveAt(where);

            //Remove之后的[first,last)元素排名因为移动了位置，所以要在相应的索引表里更新
            int first = where;
            int last = leaderboard_.Count;
            while (first < last)
                {
                Rank migrated = (Rank)(leaderboard_.GetKey(first++));
                --indexed_rank_byid_[migrated.customerId_];
                }
            Debug.Assert(true == indexed_rank_byid_.Remove(customer_id));

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
                        Rank item = (Rank)(leaderboard_.GetKey(rank-1));
                        outcome.Add(new Customer(item.customerId_, item.score_, rank));
                        }

                table_lock_.ReleaseReaderLock();
                }
            );
        return outcome;
        }
    }
