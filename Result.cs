using System;

namespace MemoryGame
{
    public class Result
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long GameID { get; set; }
        public int ResultScore { get; set; }
        public string Data { get; set; }
        public DateTime ResultDateTime { get; set; }
    }
}
