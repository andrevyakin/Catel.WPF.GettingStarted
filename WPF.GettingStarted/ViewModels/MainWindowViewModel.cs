using System.Threading.Tasks;

namespace WPF.GettingStarted.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using WPF.GettingStarted.Models;
    using WPF.GettingStarted.Services;

    /// <summary>
    /// MainWindow view model.
    /// Модель главного окна
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Переменная для хранения Интерфейса сериализации, реализованного в этом проекте
        /// </summary>
        private readonly IFamilyService familyService;
        /// <summary>
        /// Переменная для храниения Интерфейса показа диалоговых окон, реализованного в Catel
        /// </summary>
        private readonly IUIVisualizerService uiVisualizerService;
        /// <summary>
        /// Переменная для хранения Интерфейса показа сообщений, реализованного в Catel
        /// </summary>
        private readonly IMessageService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Инициализирует новый экземпляр класса <see cref = "MainWindowViewModel" />.
        /// </summary>
        public MainWindowViewModel(IFamilyService familyService, IUIVisualizerService uiVisualizerService, IMessageService messageService)
        {
            //Проверка входящих значений
            Argument.IsNotNull(() => familyService);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => messageService);

            this.familyService = familyService;
            this.uiVisualizerService = uiVisualizerService;
            this.messageService = messageService;

            //Команды
            AddFamily = new TaskCommand(OnAddFamilyExecuteAsync);
            EditFamily = new TaskCommand(OnEditFamilyExecute, OnEditFamilyCanExecute);
            RemoveFamily = new TaskCommand(OnRemoveFamilyExecute, OnRemoveFamilyCanExecute);
        }

        #region Properties Свойства
        /// <summary>
        /// Gets the title of the view model.
        /// Возвращает название модели представления.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return "WPF Getting Started example"; } }

        /// <summary>
        /// Gets the families.
        /// Получает семьи.
        /// </summary>
        public ObservableCollection<Family> Families
        {
            get { return GetValue<ObservableCollection<Family>>(FamiliesProperty); }
            private set { SetValue(FamiliesProperty, value); }
        }

        /// <summary>
        /// Register the Families property so it is known in the class.
        /// Зарегистрируйте свойство Families, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FamiliesProperty = RegisterProperty("Families", typeof(ObservableCollection<Family>), null);

        /// <summary>
        /// Gets the filtered families.
        /// Получает фильтрованные семейства.
        /// </summary>
        public ObservableCollection<Family> FilteredFamilies
        {
            get { return GetValue<ObservableCollection<Family>>(FilteredFamiliesProperty); }
            private set { SetValue(FilteredFamiliesProperty, value); }
        }

        /// <summary>
        /// Register the FilteredFamilies property so it is known in the class.
        /// Зарегистрируйте свойство FilteredFamilies, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FilteredFamiliesProperty = RegisterProperty("FilteredFamilies", typeof(ObservableCollection<Family>));

        /// <summary>
        /// Gets or sets the search filter.
        /// Получает или задает фильтр поиска.
        /// </summary>
        public string SearchFilter
        {
            get { return GetValue<string>(SearchFilterProperty); }
            set { SetValue(SearchFilterProperty, value); }
        }

        /// <summary>
        /// Register the SearchFilter property so it is known in the class.
        /// Зарегистрируйте свойство SearchFilter, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData SearchFilterProperty = RegisterProperty("SearchFilter", typeof(string), null, 
            (sender, e) => ((MainWindowViewModel)sender).UpdateSearchFilter());

        /// <summary>
        /// Gets or sets the selected family.
        /// Получает или задает выбранное семейство.
        /// </summary>
        public Family SelectedFamily
        {
            get { return GetValue<Family>(SelectedFamilyProperty); }
            set { SetValue(SelectedFamilyProperty, value); }
        }

        /// <summary>
        /// Register the SelectedFamily property so it is known in the class.
        /// Зарегистрируйте свойство SelectedFamily, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData SelectedFamilyProperty = RegisterProperty("SelectedFamily", typeof(Family), null);
        #endregion

        #region Commands
        /// <summary>
        /// Gets the AddFamily command.
        /// Возвращает команду AddFamily.
        /// </summary>
        public TaskCommand AddFamily { get; private set; }

        /// <summary>
        /// Method to invoke when the AddFamily command is executed.
        /// Метод, который вызывается, когда команда AddFamily активирована.
        /// </summary>
        private async Task OnAddFamilyExecuteAsync()
        {
            var family = new Family();

            // Note that we use the type factory here because it will automatically take care of any dependencies
            // that the FamilyWindowViewModel will add in the future
            // Обратите внимание, что мы используем фабрику типов здесь,
            //потому что она автоматически позаботится о любых зависимостях,
            //которые FamilyWindowViewModel будет добавлять в будущем
            var typeFactory = this.GetTypeFactory();
            var familyWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<FamilyWindowViewModel>(family);
            if (await uiVisualizerService.ShowDialogAsync(familyWindowViewModel) ?? false)
            {
                Families.Add(family);

                UpdateSearchFilter();
            }
        }

        /// <summary>
        /// Gets the EditFamily command.
        /// Получает команду EditFamily.
        /// </summary>
        public TaskCommand EditFamily { get; private set; }

        /// <summary>
        /// Method to check whether the EditFamily command can be executed.
        /// Метод проверки выбрана команды EditFamily.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnEditFamilyCanExecute()
        {
            return SelectedFamily != null;
        }

        /// <summary>
        /// Method to invoke when the EditFamily command is executed.
        /// Метод вызова, когда выбрана команда EditFamily.
        /// </summary>
        private async Task OnEditFamilyExecute()
        {
            // Note that we use the type factory here because it will automatically take care of any dependencies
            // that the PersonViewModel will add in the future
            // Обратите внимание, что мы используем фабрику типов здесь,
            // потому что она автоматически позаботится о любых зависимостях,
            // которые PersonViewModel будет добавлять в будущем
            var typeFactory = this.GetTypeFactory();
            var familyWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<FamilyWindowViewModel>(SelectedFamily);
            await uiVisualizerService.ShowDialogAsync(familyWindowViewModel);
        }

        /// <summary>
        /// Gets the RemoveFamily command.
        /// Получает команду RemoveFamily.
        /// </summary>
        public TaskCommand RemoveFamily { get; private set; }

        /// <summary>
        /// Method to check whether the RemoveFamily command can be executed.
        /// Метод проверки возможности выполнения команды RemoveFamily.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnRemoveFamilyCanExecute()
        {
            return SelectedFamily != null;
        }

        /// <summary>
        /// Method to invoke when the RemoveFamily command is executed.
        /// Метод вызова, когда выбрана команда RemoveFamily.
        /// </summary>
        private async Task OnRemoveFamilyExecute()
        {
            if (await messageService.ShowAsync(string.Format("Are you sure you want to delete the family '{0}'?", SelectedFamily),
                "Are you sure?", MessageButton.YesNo, MessageImage.Question) == MessageResult.Yes)
            {
                Families.Remove(SelectedFamily);
                SelectedFamily = null;
            }
        }

        /// <summary>
        /// Updates the filtered items.
        /// Обновляет фильтрованные элементы.
        /// </summary>
        private void UpdateSearchFilter()
        {
            if (FilteredFamilies == null)
            {
                FilteredFamilies = new ObservableCollection<Family>();
            }

            if (string.IsNullOrWhiteSpace(SearchFilter))
            {
                FilteredFamilies.ReplaceRange(Families);
            }
            else
            {
                var lowerSearchFilter = SearchFilter.ToLower();

                FilteredFamilies.ReplaceRange(from family in Families
                                                where !string.IsNullOrWhiteSpace(family.FamilyName) && family.FamilyName.ToLower().Contains(lowerSearchFilter)
                                                select family);
            }
        }
        #endregion

        #region Methods

        protected override async Task InitializeAsync()
        {
            var families = familyService.LoadFamilies();

            Families = new ObservableCollection<Family>(families);

            UpdateSearchFilter();
        }

        protected override async Task CloseAsync()
        {
            familyService.SaveFamilies(Families);
        }

        #endregion
    }
}
