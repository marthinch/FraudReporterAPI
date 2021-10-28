using System;

namespace FraudReporterAPI.DTOs
{
    public class FraudDetailDTO
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public string Phone { get; set; }
        public string Provider { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDateTime { get; set; }
    }
}
