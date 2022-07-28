namespace API_Project.Models.Log
{
    public class LogModel
    {
        public LogModel() { }

        /// <summary>
        /// Actual log`s filename in storage
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Log content
        /// </summary>
        public string Content { get; set; }
    }
}