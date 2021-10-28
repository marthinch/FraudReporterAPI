using System;
using System.ComponentModel.DataAnnotations;

namespace FraudReporterAPI.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
