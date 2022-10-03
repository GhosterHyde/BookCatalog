using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BookCatalog.Core;
using BookCatalog.DataAccess;
using BookCatalog.Model;
using Microsoft.Win32;
using TravelAgency;

namespace BookCatalog.ViewModel
{
    internal class BookViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly ARoot _root;
        private readonly BookRepository _bookRepository;
        private readonly AuthorRepository _authorRepository;
        private RelayCommand _saveCommand;
        private RelayCommand _uploadCoverCommand;
        private RelayCommand _deleteCoverCommand;
        private RelayCommand _addAuthorCommand;
        private RelayCommand _deleteAuthorCommand;

        public Book Book { get; set; }

        public List<Author> AllAuthors => _authorRepository.Authors;

        public string Name
        {
            get => Book.Name;
            set => Book.Name = value;
        }

        public ObservableCollection<Author> Authors
        {
            get => Book.Authors;
            set => Book.Authors = value;
        }
        public Author FirstAuthor { get; set; }

        public string Year
        {
            get => Book.Year;
            set => Book.Year = value;
        }

        public string Isbn
        {
            get => Book.Isbn;
            set => Book.Isbn = value;
        }

        public BitmapImage Cover
        {
            get
            {
                if (Book.Cover == null)
                {
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets", @"noCover.jpg")));
                }
                return GetBitMapImage(Book.Cover);
            }
            set
            {
                Book.Cover = GetByteArray(value);

                OnPropertyChanged("Cover");
            }
        }

        private byte[] GetByteArray(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                return null;
            }

            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        private BitmapImage GetBitMapImage(byte[] bookCover)
        {
            using (var ms = new System.IO.MemoryStream(bookCover))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }


        public string Description
        {
            get => Book.Description;
            set => Book.Description = value;
        }

        public Author AddableAuthor { get; set; }
        public Author DeletableAuthor { get; set; }


        public BookViewModel(Book book, BookRepository bookRepository, AuthorRepository authorRepository, ARoot root)
        {
            Book = book ?? throw new ArgumentNullException(nameof(book));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _root = root ?? throw new ArgumentNullException(nameof(root));
            if (!IsNewBook(book))
            {
                Name = book.Name;
                Authors = book.Authors ?? new ObservableCollection<Author>();
                FirstAuthor = Authors.Count > 0 ? Authors[0] : null;
                Year = book.Year;
                Isbn = book.Isbn;
                Cover = GetBitMapImage(book.Cover);
                Description = book.Description;
            }
        }

        private bool IsNewBook(Book book)
        {
            return !_bookRepository.ContainsBook(book);
        }

        public RelayCommand AddAuthor
        {
            get
            {
                if (_addAuthorCommand == null)
                {
                    _addAuthorCommand = new RelayCommand(
                        param => AddSelectedAuthor(),
                        param => CanAdd
                    );
                }
                return _addAuthorCommand;
            }
        }

        public RelayCommand DeleteAuthor
        {
            get
            {
                if (_deleteAuthorCommand == null)
                {
                    _deleteAuthorCommand = new RelayCommand(
                        param => DeleteSelectedAuthor(),
                        param => CanDelete
                    );
                }
                return _deleteAuthorCommand;
            }
        }

        public RelayCommand SaveBook
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

        public RelayCommand UploadCover
        {
            get
            {
                if (_uploadCoverCommand == null)
                {
                    _uploadCoverCommand = new RelayCommand(
                        param => this.LoadCover()
                    );
                }
                return _uploadCoverCommand;
            }
        }

        public RelayCommand DeleteCover
        {
            get
            {
                if (_deleteCoverCommand == null)
                {
                    _uploadCoverCommand = new RelayCommand(
                        param => this.DropCover(),
                        param => this.CanDeleteCover
                    );
                }
                return _uploadCoverCommand;
            }
        }

        private void Save()
        {
            try
            {
                AddDataForBook();
                if (IsNewBook(Book))
                {
                    _bookRepository.AddBook(Book);
                }
                else
                {
                    _bookRepository.UpdateBook(Book);
                }
                _root.CurrentView = new BookSuccessfullAddingVM();
            }
            catch (Exception ex)
            {
                _root.CurrentView = new ErrorReportingViewModel(ex.Message);
            }
        }

        private void AddDataForBook()
        {
            Book.Name = Name;
            Book.Authors = Authors;
            Book.Year = Year;
            Book.Isbn = Isbn;
            Book.Cover = GetByteArray(Cover);
            Book.Description = Description;
        }

        private void LoadCover()
        {
            var od = new OpenFileDialog
            {
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png"
            };
            if (od.ShowDialog() == true)
            {
                Cover = new BitmapImage(new Uri(od.FileName));
            }
        }

        private void DropCover()
        {
            Cover = null;
        }

        private void AddSelectedAuthor()
        {
            Authors.Add(AddableAuthor);
        }

        private void DeleteSelectedAuthor()
        {
            Authors.Remove(DeletableAuthor);
        }

        private bool CanSave => Book.IsValid();
        private bool CanAdd => AddableAuthor != null && !Authors.Contains(AddableAuthor);
        private bool CanDelete => DeletableAuthor != null;
        private bool CanDeleteCover => Book.Cover != null;

        string IDataErrorInfo.Error { get; }

        string IDataErrorInfo.this[string propertyName] => Book.GetValidationError(propertyName);
    }
}
