using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using static System.String;

namespace BookCatalog.Model
{
    public class Book
    {
        private const int MaxYearLength = 4;
        private const int IsbnLength = 13;
        private const int IsbnLengthWithDashes = 17;

        public int ID { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Author> Authors { get; set; }
        public string Year { get; set; }
        public string Isbn { get; set; }
        public byte[] Cover { get; set; }
        public string Description { get; set; }

        public Book()
        {
            Authors = new ObservableCollection<Author>();
        }

        public bool IsValid()
        {
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }

        static readonly string[] ValidatedProperties =
        {
            "Name",
            "Year",
            "Isbn"
        };

        public string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
            {
                return null;
            }

            string error = null;

            switch (propertyName)
            {
                case "Name":
                    error = this.ValidateName();
                    break;

                case "Year":
                    error = this.ValidateYear();
                    break;

                case "Isbn":
                    error = this.ValidateIsbn();
                    break;

                default:
                    Debug.Fail("Unexpected property being validated on Book: " + propertyName);
                    break;
            }
            return error;
        }

        private string ValidateName()
        {
            if (IsStringMissing(Name))
            {
                return "Название книги не может быть пустым";
            }
            return null;
        }

        private string ValidateYear()
        {
            if (IsStringMissing(Year))
            {
                return "Год издания не может быть пустым";
            }
            if (!IsValidYear(Year))
            {
                return "Год издания книги некорректен";
            }

            return null;
        }

        private static bool IsValidYear(string year)
        {
            return year.Length <= MaxYearLength && year.All(symbol => symbol is >= '0' and <= '9');
        }

        private string ValidateIsbn()
        {
            if (IsStringMissing(Isbn))
            {
                return "ISBN не может быть пустым";
            }
            if (!IsValidIsbn(Isbn))
            {
                return "ISBN книги некорректен";
            }

            return null;
        }

        private static bool IsValidIsbn(string isbn)
        {
            return (isbn.Length == IsbnLength && isbn.All(symbol => symbol is >= '0' and <= '9')) || 
                   isbn.Length == IsbnLengthWithDashes && isbn.All(symbol => symbol is >= '0' and <= '9' or '-');
        }

        private static bool IsStringMissing(string value)
        {
            return IsNullOrEmpty(value) || value.Trim() == Empty;
        }

        public override string ToString()
        {
            return Authors.Count > 0 ? Format(Name + " - " + Authors[0]) : Name;
        }
    }
}