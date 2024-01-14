using Kinashy.ArrestSearchWebAPI.Data.RequiredProperties;

namespace Kinashy.ArrestSearchWebAPI.Data.Library
{
    public class LibraryDB : DbContext
    {
        public LibraryDB(DbContextOptions<LibraryDB> options) : base(options) { }
        public DbSet<RequiredProperty> RequiredProperties => Set<RequiredProperty>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<BatchProperty> BatchProperties => Set<BatchProperty>();
        public DbSet<Complect> Complects => Set<Complect>();
        public DbSet<ComplectProperty> ComplectsProperty => Set<ComplectProperty>();

        [DbFunction(Name = "to_date", IsBuiltIn = true, IsNullable = false)]
        public static DateTime ToDate(string input, string format = "dd.mm.yyyy")
        {
            throw new NotImplementedException();
        }
    }
}