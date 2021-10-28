using Microsoft.EntityFrameworkCore;
using FraudReporterAPI.Models;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using System.Text;

namespace FraudReporterAPI.Data
{
    public class FraudReporterAPIContext : DbContext
    {
        private readonly IEncryptionProvider encryptionProvider;
        private readonly byte[] encryptionKey;
        private readonly byte[] encryptionIV;

        public FraudReporterAPIContext(DbContextOptions<FraudReporterAPIContext> options) : base(options)
        {
            encryptionKey = Encoding.UTF8.GetBytes("mysmallkey123456");
            encryptionIV = Encoding.UTF8.GetBytes("mysmallkey123456");

            this.encryptionProvider = new AesProvider(this.encryptionKey, this.encryptionIV);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(encryptionProvider);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Fraud> Fraud { get; set; }
    }
}
