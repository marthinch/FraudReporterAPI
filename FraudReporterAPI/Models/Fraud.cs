using System;

namespace FraudReporterAPI.Models
{
    public class Fraud : BaseModel
    {
        public int Category { get; set; }
        public string Phone { get; set; }
        public string Provider { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public DateTime ReceivedDateTime { get; set; }
    }
}
