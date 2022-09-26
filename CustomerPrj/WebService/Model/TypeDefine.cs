
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;


public record Customer(long id, decimal score,long rank);

//这些没必要做到文件里去配了
public static class Config
    {
    public const decimal DEFAULT_SCORE = 0;
    public const long INVALID_CUSTOMERID = -1;
    public const string TEST_FILE = "../samples.txt";
    }

public record struct Rank : IComparable
    {
    public decimal score_ = Config.DEFAULT_SCORE;
    public readonly long customerId_ { get; init; } = Config.INVALID_CUSTOMERID;

    public Rank(decimal score, long customer_id)
        {
        score_ = score;
        customerId_ = customer_id;
        }

    public int CompareTo(object obj)
        {
        //编译器的警告就装作没看见，被断言断到就当是中奖了
        Debug.Assert(null != obj);
        Rank other_record = (Rank)obj;

        if (this.score_ == other_record.score_)
            return this.customerId_.CompareTo(other_record.customerId_);
        else
            return other_record.score_.CompareTo(this.score_);
        }
    }
