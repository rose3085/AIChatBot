using AIChatBot.Model;
using Microsoft.EntityFrameworkCore;

namespace AIChatBot.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }
        public DbSet<ResponseModel> Responses { get; set; }
    }
}
