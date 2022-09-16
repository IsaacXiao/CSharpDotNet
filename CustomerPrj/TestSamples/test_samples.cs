using Microsoft.Extensions.Configuration;


public static class TEST_SAMPLES
    {
    private static IConfigurationRoot config_;
    private static long sample_count_ = -1;
    private static long id_max_ = -1;
    private static long full_score_ = -1;

    private static void ReadConfigInfo()
        {
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("config.json", optional: false, reloadOnChange: false);
        config_ = configBuilder.Build();
        id_max_ = long.Parse(config_["id_max"]);
        sample_count_ = long.Parse(config_["sample_count"]);
        full_score_ = long.Parse(config_["full_score"]);
        }

    public static void GenerateSamples()
        {
        ReadConfigInfo();
        using (StreamWriter generator = new StreamWriter(config_["init_file"]))
            {
                for (long i = 0; i < sample_count_; i++)
                    {
                    Random customer_id = new Random();
                    Random score = new Random();
                    generator.WriteLine("{0}\t{1}", customer_id.NextInt64(sample_count_, id_max_), score.NextInt64(sample_count_, full_score_));
                    }
            }
        }
    public static void ShowSamples()
        {
        Console.WriteLine("Init some data to do test\n初始化一点点数据便于测试");
        using (StreamReader customer = new StreamReader(config_["init_file"]))
            {
            while (!customer.EndOfStream)
                {
                string[] item = customer.ReadLine().Split('\t');
                long customer_id = long.Parse(item[0]);
                var score = decimal.Parse(item[1]);
                Console.WriteLine("{0}\t{1}", customer_id, score);
                }
            }
        }
    }

