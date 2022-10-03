using System.Threading.Tasks;
using BookCatalog.Core;
using BookCatalog.DataAccess;
using BookCatalog.Model;
using TravelAgency;

namespace BookCatalog.ViewModel
{
    internal class MainWindowViewModel : ARoot
    {
        private BookRepository _bookRepository;
        private AuthorRepository _authorRepository;
        private object _nextView;

        private DataContext dataContext;

        public RelayCommand ShowAllBooks { get; set; }
        public RelayCommand AddNewBook { get; set; }
        public RelayCommand ShowAllAuthors { get; set; }
        public RelayCommand AddNewAuthor { get; set; }

        public MainWindowViewModel()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            CurrentView = new LoadingViewModel();
            Task.Run(() =>
            {
                dataContext = new DataContext();
                _bookRepository = new BookRepository(dataContext);
                _authorRepository = new AuthorRepository(dataContext);
                CurrentView = new AllBooksViewModel(_bookRepository, _authorRepository, this);
            });

            AddNewBook = new RelayCommand(o =>
            {
                if (CurrentView is not LoadingViewModel)
                {
                    CurrentView = new BookViewModel(new Book(), _bookRepository, _authorRepository, this);
                }
            });

            AddNewAuthor = new RelayCommand(o =>
            {
                if (CurrentView is not LoadingViewModel)
                {
                    CurrentView = new AuthorViewModel(new Author(), _authorRepository, this);
                }
            });

            ShowAllBooks = new RelayCommand(o =>
            {
                if (CurrentView is not LoadingViewModel)
                {
                    CurrentView = new AllBooksViewModel(_bookRepository, _authorRepository, this);
                }
            });

            ShowAllAuthors = new RelayCommand(o =>
            {
                if (CurrentView is not LoadingViewModel)
                {
                    CurrentView = new AllAuthorsViewModel(_authorRepository, this);
                }
            });
        }
    }
}
