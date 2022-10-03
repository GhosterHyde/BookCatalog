using System.Data.Entity;
using BookCatalog.Model;

namespace TravelAgency
{
    public class DataContext : DbContext
    {
        public DbSet<Book> Book { get; set; }
        public DbSet<Author> Author { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany<Author>(s => s.Authors)
                .WithMany(c => c.Books)
                .Map(cs =>
                {
                    cs.MapLeftKey("AuthorId");
                    cs.MapRightKey("BookId");
                    cs.ToTable("BookAuthor");
                });
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<Author>().ToTable("Author");
            base.OnModelCreating(modelBuilder);
        }

        public DataContext() : base(@"Data Source=DESKTOP-4K63I0T\SQLEXPRESS;Database=BookCatalog;Trusted_Connection=True;")
        {
        }
    }
}
