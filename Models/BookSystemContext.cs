
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace sp_project_guide_api.Models
{
    public class BookSystemContext : DbContext
    {

        public BookSystemContext(DbContextOptions<BookSystemContext> options): base(options) {
            

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Define that this entity doesn't have a primary key
        //    modelBuilder.Entity<Link>().HasNoKey();
        //}

        public DbSet<Book> Books { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<BookV2> BooksV2 { get; set; }

    }
}
