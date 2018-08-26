namespace WPF.GettingStarted.Models
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    public class Person : ModelBase
    {
        /// <summary>
        /// Gets or sets the first name.
        /// Получает или задает имя.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// Зарегистрируйте свойство FirstName, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

        /// <summary>
        /// Gets or sets the last name.
        /// Получает или задает фамилию.
        /// </summary>
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// Зарегистрируйте свойство LastName, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "Требуется имя"));
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                validationResults.Add(FieldValidationResult.CreateError(LastNameProperty, "Требуется фамилия"));
            }
        }

        public override string ToString()
        {
            string fullName = string.Empty;

            if (!string.IsNullOrEmpty(FirstName))
            {
                fullName += FirstName;
            }

            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrWhiteSpace(LastName))
            {
                fullName += " ";
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                fullName += LastName;
            }

            return fullName;
        }
    }
}
