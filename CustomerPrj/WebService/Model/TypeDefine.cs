

using System.Collections;
using System.Diagnostics;


//literal const variable should not appear in code
//this is merely doing a homework
//so I don't bother using ConfigurationBuilder and json file
public static class Config
    {
    public const decimal DEFAULT_SCORE = 0;
    public const long INVALID_CUSTOMERID = 0;
    public const decimal SCORE_DECREASE = -1000;
    public const decimal SCORE_INCREASE = 1000;
    //avoid runtime expansion and memory re-allocation for SortedList
    public const int LEADERBOARD_SIZE = 2000;
    public const string TEST_FILE = "../samples.txt";
    public const int LOCK_TIMEOUT = 250;
    }


public record Customer(long id, decimal score,long rank);
//promote readability for validity check
public record RankBoundary(int lower_bound, int upper_bound);
public record IndexedRank(long id,int rank);


public struct Rank : IComparable<Rank>
    {
    public readonly decimal score_ { get; init; } = Config.DEFAULT_SCORE;
    public readonly long customerId_ { get; init; } = Config.INVALID_CUSTOMERID;

    public Rank(decimal score, long customer_id)
        {
        score_ = score;
        customerId_ = customer_id;
        }

    public int CompareTo(Rank other_record)
        {
        if (this.score_ == other_record.score_)
            return this.customerId_.CompareTo(other_record.customerId_);
        else
            return other_record.score_.CompareTo(this.score_);
        }
    }

public struct Placeholder { }

