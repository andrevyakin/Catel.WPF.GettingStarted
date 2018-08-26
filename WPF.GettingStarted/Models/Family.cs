﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.GettingStarted.Models
{
    using System.Collections.ObjectModel;
    using Catel.Data;

    public class Family : ModelBase
    {
        /// <summary>
        /// Gets or sets the family name.
        /// Получает или задает семью.
        /// </summary>
        public string FamilyName
        {
            get { return GetValue<string>(FamilyNameProperty); }
            set { SetValue(FamilyNameProperty, value); }
        }

        /// <summary>
        /// Register the FamilyName property so it is known in the class.
        /// Зарегистрируйте свойство FamilyName, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FamilyNameProperty = RegisterProperty("FamilyName", typeof(string), null);

        /// <summary>
        /// Gets or sets the list of persons in this family.
        /// Получает или задает список лиц в этом семействе.
        /// </summary>
        public ObservableCollection<Person> Persons
        {
            get { return GetValue<ObservableCollection<Person>>(PersonsProperty); }
            set { SetValue(PersonsProperty, value); }
        }

        /// <summary>
        /// Register the Persons property so it is known in the class.
        /// Зарегистрируйте список лиц, чтобы он был известен в классе.
        /// </summary>
        public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<Person>), () => new ObservableCollection<Person>());

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(FamilyName))
            {
                validationResults.Add(FieldValidationResult.CreateError(FamilyNameProperty, "Фамилия обязательна"));
            }
        }

        public override string ToString()
        {
            return FamilyName;
        }
    }
}
