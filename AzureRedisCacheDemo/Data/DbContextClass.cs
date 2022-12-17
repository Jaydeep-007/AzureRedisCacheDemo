using AzureRedisCacheDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace AzureRedisCacheDemo.Data
{
    public class DbContextClass : DbContext
    {

        public DbContextClass(DbContextOptions<DbContextClass>
options) : base(options)
        {

        }

        public DbSet<ProductDetails> Products { get; set; }
    }
}
