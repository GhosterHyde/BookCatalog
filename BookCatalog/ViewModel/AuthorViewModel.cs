using System;
using System.ComponentModel;
using BookCatalog.Core;
using BookCatalog.DataAccess;
using BookCatalog.Model;

namespace BookCatalog.ViewModel
{
    internal class AuthorViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly ARoot _root;
        private readonly AuthorRepository _authorRepository;
        private RelayCommand _saveCommand;

        public Author Author { get; set; }

        public string Surname
        {
            get => Author.Surname;
            set => Author.Surname = value;
        }
        public string Name
        {
            get => Author.Name;
            set => Author.Name = value;
        }

        public string SecondName
        {
            get => Author.SecondName;
            set => Author.SecondName = value;
        }

        public AuthorViewModel(Author author, AuthorRepository authorRepository, ARoot root)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _root = root ?? throw new ArgumentNullException(nameof(root));
            if (!IsNewAuthor(author))
            {
                Surname = author.Surname;
                Name = author.Name;
                SecondName = author.SecondName;
            }
        }

        public RelayCommand SaveAuthor
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => this.Save(),
                        param => this.CanSave
                    );
                }
                return _saveCommand;
            }
        }

        private void Save()
        {
            try
            {
                AddDataForAuthor();
                if (IsNewAuthor(Author))
                {
                    _authorRepository.AddAuthor(Author);
                }
                else
                {
                    _authorRepository.UpdateAuthor(Author);
                }
                _root.CurrentView = new AuthorSuccessfullAddingVM();
            }
            catch (Exception ex)
            {
                _root.CurrentView = new ErrorReportingViewModel(ex.Message);
            }
        }

        private bool IsNewAuthor(Author author)
        {
            return !_authorRepository.ContainsAuthor(author);
        }

        private void AddDataForAuthor()
        {
            Author.Surname = Surname;
            Author.Name = Name;
            Author.SecondName = SecondName;
        }

        private bool CanSave => Author.IsValid();
        string IDataErrorInfo.Error { get; }
        string IDataErrorInfo.this[string propertyName] => Author.GetValidationError(propertyName);
    }
}
