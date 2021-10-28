using System;
using System.ComponentModel.DataAnnotations;

namespace FraudReporterAPI.Models
{
    public class Fraud : BaseModel
    {
        public int Category { get; set; }

        [Encrypted]
        public string Phone { get; set; }

        [Encrypted]
        public string Provider { get; set; }

        [Encrypted]
        public string Message { get; set; }

        public int Status { get; set; }
        public DateTime ReceivedDateTime { get; set; }
    }
}
