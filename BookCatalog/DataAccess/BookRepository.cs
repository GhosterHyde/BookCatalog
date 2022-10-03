using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using BookCatalog.Model;
using TravelAgency;

namespace BookCatalog.DataAccess
{
    internal class BookRepository
    {
        public List<Book> Books { get; }
        public DataContext DataContext { get; set; }

        public BookRepository(DataContext dataContext)
        {
            DataContext = dataContext;
            Books = LoadBooks();
        }
        private List<Book> LoadBooks()
        {
            DataContext.Book.Load();
            return DataContext.Book.OrderBy(book => book.Name).Include(book => book.Authors).ToList();
        }

        public void AddBook(Book book)
        {
            if (Books.Contains(book ?? throw new ArgumentNullException(nameof(book))))
            {
                return;
            }

            try
            {
                DataContext.Book.Add(book);
                DataContext.SaveChanges();
                Books.Add(book);
            }
            catch (Exception)
            {
                Books.Add(book);
            }
        }
        public void UpdateBook(Book book)
        {
            DataContext.Book.AddOrUpdate(book);
            DataContext.SaveChanges();
        }

        public void DeleteBook(Book book)
        {
            if (!Books.Contains(book ?? throw new ArgumentNullException(nameof(book))))
            {
                return;
            }

            try
            {
                DataContext.SaveChanges();
                DataContext.Book.Remove(book);
                DataContext.SaveChangesAsync();
                Books.Remove(book);
            }
            catch (Exception)
            {
                Books.Remove(book);
            }
        }

        public bool ContainsBook(Book book)
        {
            return Books.Contains(book);
        }
    }
}
