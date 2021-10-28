using System;

namespace FraudReporterAPI.DTOs
{
    public class FraudListDTO
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDateTime { get; set; }
    }
}
