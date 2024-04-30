
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace sp_project_guide_api.Models
{
    public class BookSystemContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "BookSystem");
            base.OnConfiguring(optionsBuilder);
        }
        public BookSystemContext(DbContextOptions<BookSystemContext> options): base(options) {

        }

        public BookSystemContext() { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define that this entity doesn't have a primary key
            modelBuilder.Entity<Link>().HasNoKey();
        }

        //for the xUnit test framework, i have to set these dbSets as Virtual so they can be interacted with.
        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Member> Members { get; set; }

        public virtual DbSet<BookV2> BooksV2 { get; set; }
    }
}
