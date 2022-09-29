
using System.Collections;
using System.Diagnostics;

//纯粹为避免在代码里出现字面值
//仅做题就没必要做到文件里去配了
public static class Config
    {
    public const decimal DEFAULT_SCORE = 0;
    public const long INVALID_CUSTOMERID = 0;
    public const decimal SCORE_DECREASE = -1000;
    public const decimal SCORE_INCREASE = 1000;
    //为了提高效率避免SortedList动态扩容，可以分配给leaderboard一个较大的初始容量
    public const int LEADERBOARD_SIZE = 2000;
    public const string TEST_FILE = "../samples.txt";
    public const int LOCK_TIMEOUT = 250;
    }

public record Customer(long id, decimal score,long rank);
//用于提高程序的可读性
public record RankBoundary(int lower_bound, int upper_bound);
public record IndexedRank(long id,int rank);


//C#的struct实例是分配在栈上class实例是分配在堆空间中的
//因为操作系统要为分配的每个堆内存记录信息（俗称“收税”）
//用collection存储大量class实例可能会导致内存性能效率低下或是内存碎片问题
//所以Rank在此声明为struct而非class!!!!
//关于这个可以参见操作系统原理
//就像C++的容器在语义上是按值存储一样
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
