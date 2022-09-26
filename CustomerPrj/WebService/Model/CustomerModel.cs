
using System.Diagnostics;
using System.Collections;
using System.Collections.Concurrent;

考虑不用线程安全容器
using SortedRank = System.Collections.SortedList;
using IndexedRankById = System.Collections.Concurrent.ConcurrentDictionary<long,int>;

//为了在Main.cs中调用InitTestSamples()所以把这个类做成static
//运行成熟没问题的话可以去掉初始化测试样本
//否则考虑让Controller持有Model，但是没必要做单例
public static class CustomerModel
    {
    private static SortedRank leaderboard_ = SortedRank.Synchronized(new SortedRank());
    private static IndexedRankById indexed_rank_byid_ = new();
    private static readonly object table_lock_ = new object();
    public static Tuple<int, int> RankRange() { return Tuple.Create<int, int>(1,leaderboard_.Count); }
    public static bool CustomerIdExsits(long id) { return indexed_rank_byid_.ContainsKey(id); }
    public static int RangeIndex(long id) { return indexed_rank_byid_[id]; }
    public static void InitTestSamples()
        {
        Console.WriteLine("Init some data to do test, see samples.txt\n初始化一点点数据便于测试，见samples.txt");
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
    public static async Task AddRecord(Rank rank)
        {
        await Task.Run
            ( ( ) =>
                {
                lock(table_lock_)
                    {
                    leaderboard_.Add(rank, null);
                    Debug.Assert(true == indexed_rank_byid_.TryAdd(rank.customerId_, leaderboard_.IndexOfKey(rank) + 1));
                    //插入之后的[first,last)元素排名因为移动了位置，所以要在相应的索引表里更新
                    int first = leaderboard_.IndexOfKey(rank) + 1;
                    int last = leaderboard_.Count;
                    while (first < last)
                        {
                        Rank migrated = (Rank)(leaderboard_.GetKey(first++));
                        ++indexed_rank_byid_[migrated.customerId_];
                        }
                    }
                }
            );
        }
    public static async Task RemoveRecord(long customer_id)
        {
        await Task.Run
            ( ( ) =>
                {
                lock(table_lock_)
                    {
                    int where = indexed_rank_byid_[customer_id] - 1;
                    Rank rank = (Rank)(leaderboard_.GetKey(where));
                    Debug.Assert(rank.customerId_==customer_id);
                    leaderboard_.RemoveAt(where);
                    //删除之后的[first,last)元素排名因为移动了位置，所以要在相应的索引表里更新
                    int first = where;
                    int last = leaderboard_.Count;
                    while (first < last)
                        {
                        Rank migrated = (Rank)(leaderboard_.GetKey(first++));
                        --indexed_rank_byid_[migrated.customerId_];
                        }
                    }
                int which;
                Debug.Assert(true == indexed_rank_byid_.TryRemove(customer_id,out which));
                }
            );
        }
    public static Task<List<Customer>> GetCustomerByRange(int start,int end)
        {
        List<Customer> outcome = new List<Customer>();
        for(int rank = start; rank <= end; rank++)
            {
            Rank item = (Rank)(leaderboard_.GetKey(rank-1));
            outcome.Add(new Customer(item.customerId_, item.score_, rank));
            }
        return Task.FromResult(outcome);
        }
    }
