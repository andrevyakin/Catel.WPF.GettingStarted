namespace WPF.GettingStarted.Models
{
    using System.Collections.ObjectModel;
    using Catel.Data;

    public class Settings : SavableModelBase<Settings>
    {
        /// <summary>
        /// Gets or sets all the families.
        /// Получает или задает все семьи.
        /// </summary>
        public ObservableCollection<Family> Families
        {
            get { return GetValue<ObservableCollection<Family>>(FamiliesProperty); }
            set { SetValue(FamiliesProperty, value); }
        }

        /// <summary>
        /// Register the Families property so it is known in the class.
        /// Зарегистрируйте свойство Families, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FamiliesProperty = RegisterProperty("Families", typeof(ObservableCollection<Family>), () => new ObservableCollection<Family>());
    }
}
