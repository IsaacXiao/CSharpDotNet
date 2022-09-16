
using System.Diagnostics;



public record Customer(long id, decimal score,long rank);


public record Rank : IComparable
    {
    //C++从C++11开始可以这样定义别名
    //但C#编译报错，暂时没发现好办法提高可读性的
    //using ScoreType = decimal;
    //using IdType = long;
    //TODO: 把0做成可配的
    public decimal score_ { get; set; } = -1;
    public long customer_id_ { get; init; } = 0;

    public Rank(decimal score_, long customer_id_)
        {
        this.score_ = score_;
        this.customer_id_ = customer_id_;
        }

    public int CompareTo(object obj)
        {
        Debug.Assert(null != obj);
        Rank other_record = obj as Rank;
        Debug.Assert(null != other_record);

        if (this.score_ == other_record.score_)
            return this.customer_id_.CompareTo(other_record.customer_id_);
        else
            return other_record.score_.CompareTo(this.score_);
        }
    }


