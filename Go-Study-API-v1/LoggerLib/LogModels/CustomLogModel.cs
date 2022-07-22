namespace LoggerLib.LogModels
{
    public class CustomLogModel : Base.BaseLogModel
    {
        public int State { get; set; }

        public object Entity { get; set; }
    }
}
