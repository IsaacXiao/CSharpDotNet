using System.Diagnostics;
using System.Collections;
using System.Collections.Concurrent;

public record Customer(long id, decimal score,long rank);

public record struct Rank : IComparable
    {
    public decimal score_ = -1;
    public readonly long customerId_ { get; init; } = -1;

    public Rank(decimal score, long customer_id)
        {
        score_ = score;
        customerId_ = customer_id;
        }

    public int CompareTo(object obj)
        {
        Debug.Assert(null != obj);
        Rank other_record = (Rank)obj;

        if (this.score_ == other_record.score_)
            return this.customerId_.CompareTo(other_record.customerId_);
        else
            return other_record.score_.CompareTo(this.score_);
        }
    }

public static class CustomerModel
    {
    private const string sample_file_ = "../samples.txt";
    public static SortedList leaderboard_ { get; set; } = SortedList.Synchronized(new SortedList());
    public static ConcurrentDictionary<long, int> indexed_rank_byid_ { get; set; } = new ConcurrentDictionary<long, int>();
    public static void InitSamples()
        {
        Console.WriteLine("Init some data to do test, see samples.txt\n初始化一点点数据便于测试，见samples.txt");
        Debug.Assert(System.IO.File.Exists(sample_file_));
        using (StreamReader customer = new StreamReader(sample_file_))
            {
            while (!customer.EndOfStream)
                {
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
    }

