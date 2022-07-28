using System;

namespace API_Project.Models.Log
{
    public class ServerStateModel
    {
        public int CountRequests { get; set; }
        public DateTime DateStart { get; set; }
        public int CountSentEmails { get; set; }
        public int CountTodayLogs { get; set; }
        public int CountTokens { get; set; }
    }
}