using System;
using System.Collections.ObjectModel;
using System.Linq;
using BookCatalog.DataAccess;
using BookCatalog.Model;
using BookCatalog.Core;

namespace BookCatalog.ViewModel
{
    internal class AllBooksViewModel
    {
        private readonly BookRepository _bookRepository;
        private readonly AuthorRepository _authorRepository;
        private readonly ARoot _root;

        public ObservableCollection<BookViewModel> AllBooks
        {
            get; 
            set;
        }
        
        public RelayCommand UpdateBook { get; set; }
        public RelayCommand DeleteBook { get; set; }

        public AllBooksViewModel(BookRepository bookRepository, AuthorRepository authorRepository, ARoot root)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _root = root ?? throw new ArgumentNullException(nameof(root));

            AllBooks = new ObservableCollection<BookViewModel>();
            CreateAllBooks();

            UpdateBook = new RelayCommand(o =>
            {
                _root.CurrentView = o;
            });
            DeleteBook = new RelayCommand(o =>
            {
                var bookVm = (BookViewModel)o;
                var book = bookVm.Book;
                _bookRepository.DeleteBook(book);
                AllBooks?.Remove(bookVm);
            });
        }

        private void CreateAllBooks()
        {
            foreach (var bookVm in _bookRepository.Books.Select(book => new BookViewModel(book, _bookRepository, _authorRepository, _root)))
            {
                AllBooks.Add(bookVm);
            }
        }
    }
}
