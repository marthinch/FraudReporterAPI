using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FraudReporterAPI.Models;

namespace FraudReporterAPI.Data
{
    public class FraudReporterAPIContext : DbContext
    {
        public FraudReporterAPIContext (DbContextOptions<FraudReporterAPIContext> options)
            : base(options)
        {
        }

        public DbSet<FraudReporterAPI.Models.Fraud> Fraud { get; set; }
    }
}
