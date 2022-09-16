
using System.Collections.Immutable;
using System.Diagnostics;

public static class COMMON
    {
    private const string init_file = "../samples.txt";
    public static SortedSet<Rank> LeaderBoard =new SortedSet<Rank>();
    //public static ImmutableSortedSet<Rank> LeaderBoard { get; set; } = ImmutableSortedSet.Create<Rank>();
    
    public static void InitSamples()
        {
        Console.WriteLine("Init some data to do test, see samples.txt\n初始化一点点数据便于测试，见samples.txt");
        using (StreamReader customer = new StreamReader(init_file))
            {
            while (!customer.EndOfStream)
                {
                string[] item = customer.ReadLine().Split('\t');
                long customer_id = long.Parse(item[0]);
                var score = decimal.Parse(item[1]);
                LeaderBoard.Add(new Rank(score, customer_id));
                }
            }
        }
}
