
using System.Diagnostics;
using System.Collections;
using System.Collections.Concurrent;

using SortedRank = System.Collections.SortedList;
using IndexedRankById = System.Collections.Concurrent.ConcurrentDictionary<long,int>;

//为了在Main.cs中调用InitTestSamples()所以把这个类做成static
//运行成熟没问题的话可以去掉初始化测试样本
//否则考虑让Controller持有Model，但是没必要做单例
public static class CustomerModel
    {
    private static SortedRank leaderboard_ = SortedList.Synchronized(new SortedList());
    private static IndexedRankById indexed_rank_byid_ = new ConcurrentDictionary<long, int>();
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
    public static async Task UpdateScore(long customer_id, decimal score)
        {
        await Task.Run
            ( () =>
                {
                Rank new_rank = new Rank(score, customer_id);
                if (!CustomerIdExsits(customer_id))
                    {
                    leaderboard_.Add(new_rank, null);
                    indexed_rank_byid_.TryAdd(new_rank.customerId_, leaderboard_.IndexOfKey(new_rank) + 1);
                    }
                else
                    {
                    int old_index = indexed_rank_byid_[customer_id] - 1;
                    leaderboard_.RemoveAt(old_index);
                    leaderboard_.Add(new_rank, null);
                    indexed_rank_byid_[customer_id] = leaderboard_.IndexOfKey(new_rank) + 1;
                    }
                }
            );
        }
    public static Task<List<Customer>> GetCustomerByRank(int start,int end)
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
