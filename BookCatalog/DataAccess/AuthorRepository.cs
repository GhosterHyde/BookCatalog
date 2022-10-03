using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using BookCatalog.Model;
using TravelAgency;

namespace BookCatalog.DataAccess
{
    internal class AuthorRepository
    {
        public List<Author> Authors { get; }
        public DataContext DataContext { get; set; }

        public AuthorRepository(DataContext dataContext)
        {
            DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            Authors = LoadAuthors();
        }

        private List<Author> LoadAuthors()
        {
            DataContext.Author.Load();
            return DataContext.Author.OrderBy(author => author.Surname).ThenBy(author => author.Name).ThenBy(author => author.SecondName).ToList();
        }

        public void AddAuthor(Author author)
        {
            if (Authors.Contains(author ?? throw new ArgumentNullException(nameof(author))))
            {
                return;
            }

            try
            {
                DataContext.Author.Add(author);
                DataContext.SaveChangesAsync();
                Authors.Add(author);
            }
            catch (Exception)
            {
                Authors.Add(author);
            }
        }

        public void UpdateAuthor(Author author)
        {
            DataContext.Author.AddOrUpdate(author);
            DataContext.SaveChangesAsync();
        }

        public void DeleteAuthor(Author author)
        {
            if (!Authors.Contains(author ?? throw new ArgumentNullException(nameof(author))))
            {
                return;
            }

            try
            {
                DataContext.SaveChanges();
                DataContext.Author.Remove(author);
                DataContext.SaveChangesAsync();
                Authors.Remove(author);
            }
            catch (Exception)
            {
                Authors.Remove(author);
            }
        }

        public bool ContainsAuthor(Author author)
        {
            return Authors.Contains(author);
        }
    }
}
