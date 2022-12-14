using Microsoft.Extensions.Configuration;
public static class TEST_SAMPLES
    {
    private static IConfigurationRoot config_;
    private static long id_max_ = -1;
    private static long sample_count_ = -1;
    private static long score_max_ = 0;

    private static void ReadConfigInfo()
        {
        ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("config.json", optional: false, reloadOnChange: false);
        config_ = configBuilder.Build();
        id_max_ = long.Parse(config_["id_max"]);
        sample_count_ = long.Parse(config_["sample_count"]);
        score_max_ = long.Parse(config_["score_max"]);
        }

    public static void GenerateSamples()
        {
        ReadConfigInfo();
        using (StreamWriter generator = new StreamWriter(config_["file_path"]))
            {
            for (long i = 0; i < sample_count_; i++)
                {
                Random factor = new Random();
                generator.WriteLine("{0}\t{1}", factor.NextInt64(1, id_max_), 
                                                (decimal)(factor.NextInt64(1,1000)));
                }
            }
        }

    public static void ShowSamples()
        {
        Console.WriteLine("Init some data to do test\n");
        using (StreamReader customer = new StreamReader(config_["file_path"]))
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

