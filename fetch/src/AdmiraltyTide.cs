
using System;
using System.Text.Json.Serialization;

namespace DailyTide
{    
    public class AdmiraltyTide
    {
        public string EventType { get; set; }
        
        public DateTime DateTime { get; set; }

        public double Height { get; set; }
    }
}