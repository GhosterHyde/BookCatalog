using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using static System.String;

namespace BookCatalog.Model
{
    public class Author
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public List<Book> Books { get; set; }

        public Author()
        {
        }

        public bool IsValid()
        {
            foreach (string property in ValidatedProperties)
            {
                if (GetValidationError(property) != null)
                {
                    return false;
                }
            }
            return true;
        }

        static readonly string[] ValidatedProperties =
        {
            "Surname",
            "Name",
            "SecondName"
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
                case "Surname":
                    error = ValidateName(Surname, "Фамилия");
                    break;

                case "Name":
                    error = ValidateName(Name, "Имя");
                    break;

                case "SecondName":
                    error = this.ValidateSecondName();
                    break;

                default:
                    Debug.Fail("Unexpected property being validated on Author: " + propertyName);
                    break;
            }
            return error;
        }

        private static string ValidateName(string entity, string entityName)
        {
            if (IsStringMissing(entity))
            {
                return "Поле " + entityName + " не может быть пустым";
            }
            if (!IsValidName(entity))
            {
                return "Значение поля " + entityName + " некорректно";
            }
            return null;
        }

        private string ValidateSecondName()
        {
            if (SecondName != null)
            {
                if (!IsValidName(SecondName))
                {
                    return "Значение поля Отчество некорректно";
                }
            }
            return null;
        }

        private static bool IsValidName(string name)
        {
            name = name.ToUpper();
            return name.All(symbol => symbol is >= 'А' and <= 'Я' or >= 'A' and <= 'Z' or >= '0' and <= '9' or ' ' or '-');
        }

        private static bool IsStringMissing(string value)
        {
            return IsNullOrEmpty(value) || value.Trim() == Empty;
        }

        public override string ToString()
        {
            return SecondName == null ? Format(Surname + " " + Name[0] + ".") : Format(Surname + " " + Name[0] + "." + SecondName[0] + ".");
        }
    }
}
