using System;
using System.Collections.ObjectModel;
using System.Linq;
using BookCatalog.DataAccess;
using BookCatalog.Model;
using BookCatalog.Core;

namespace BookCatalog.ViewModel
{
    internal class AllAuthorsViewModel
    {
        private readonly AuthorRepository _authorRepository;
        private readonly ARoot _root;

        public ObservableCollection<AuthorViewModel> AllAuthors { get; set; }
        
        public RelayCommand UpdateAuthor { get; set; }
        public RelayCommand DeleteAuthor { get; set; }

        public AllAuthorsViewModel(AuthorRepository authorRepository, ARoot root)
        {
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _root = root ?? throw new ArgumentNullException(nameof(root));

            AllAuthors = new ObservableCollection<AuthorViewModel>();
            CreateAllAuthors();
            
            UpdateAuthor = new RelayCommand(o =>
            {
                _root.CurrentView = o;
            });
            DeleteAuthor = new RelayCommand(o =>
            {
                var authorVm = (AuthorViewModel)o;
                var author = authorVm.Author;
                _authorRepository.DeleteAuthor(author);
                AllAuthors?.Remove(authorVm);
            });
        }

        private void CreateAllAuthors()
        {
            foreach (var authorVm in _authorRepository.Authors.Select(author => new AuthorViewModel(author, _authorRepository, _root)))
            {
                AllAuthors.Add(authorVm);
            }
        }
    }
}
