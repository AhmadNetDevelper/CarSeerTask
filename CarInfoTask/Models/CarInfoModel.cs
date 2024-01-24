namespace CarInfoTask.Models
{
    public class CarInfoModel
    {
        public long Count { get; set; }
        public string Message { get; set; }
        public List<CarInfo> Results { get; set; } = null!;

    }

    public class CarInfo
    {
        public long Make_ID { get; set; }
        public string Make_Name { get; set; }
        public long Model_ID { get; set; }
        public string Model_Name { get; set; }
    }
}
