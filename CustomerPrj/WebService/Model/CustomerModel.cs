

using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;


//c# doesn't have generic version of SortedList<T>, so I have to write SortedList<Rank,meaningless template parameter>
//without Placeholder, an alternative way is non-generic version of SortedList storing a bunch of "object"
//unlike type cast or convert in c++, boxing and and unboxing objects in c# are costly    
//SortedList<id_,score> is not feasible
//because the field "score" is not able to participate ( key is id !! ) comparison as our business logic defined in PDF:
//Two customers with the same score, their ranks are determined by their CustomerID, lower is first.
//the perfect solution is to implement SortedList<T> by inheritance from List<T>, as the link below describes,
//https://stackoverflow.com/questions/3663613/why-is-there-no-sortedlistt-in-net
//we can manually make it sorted, but it will increase complexity
using SortedRank = System.Collections.Generic.SortedList<Rank,Placeholder?>;
using IndexedRankById = System.Collections.Generic.Dictionary<long,int>;


//c# doesn't have global function mechanism like c++
//in order to do InitTestSamples() in Main.cs, this class is defined as static
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
        Console.WriteLine("Init some data to do test, see samples.txt\n，samples.txt");
        Console.WriteLine("the initial size of leaderboard: {0}", leaderboard_.Capacity);
        using (StreamReader customer = new StreamReader(Config.TEST_FILE))
            {
            while (!customer.EndOfStream)
                {
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

            //after Add, elements ranks in range [ first, last ) are increased subsequently
            //so they should be updated in time in Index table
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
            leaderboard_.RemoveAt(where);
            Debug.Assert(rank.customerId_==customer_id);
            Debug.Assert(true == indexed_rank_byid_.Remove(customer_id));

            //after Remove, elements ranks in range [ first, last ) are decreased subsequently
            //so they should be updated in time in Index table
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

