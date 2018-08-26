using System.Threading.Tasks;

namespace WPF.GettingStarted.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel;
    using Catel.Data;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using WPF.GettingStarted.Models;

    public class FamilyWindowViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService uiVisualizerService;
        private readonly IMessageService messageService;

        public FamilyWindowViewModel(Family family, IUIVisualizerService uiVisualizerService, IMessageService messageService)
        {
            Argument.IsNotNull(() => family);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => messageService);

            Family = family;
            this.uiVisualizerService = uiVisualizerService;
            this.messageService = messageService;

            AddPerson = new TaskCommand(OnAddPersonExecuteAsync);
            EditPerson = new TaskCommand(OnEditPersonExecuteAsync, OnEditPersonCanExecute);
            RemovePerson = new TaskCommand(OnRemovePersonExecuteAsync, OnRemovePersonCanExecute);
        }

        /// <summary>
        /// Gets the family.
        /// Получает семью.
        /// </summary>
        [Model]
        public Family Family
        {
            get { return GetValue<Family>(FamilyProperty); }
            private set { SetValue(FamilyProperty, value); }
        }

        /// <summary>
        /// Register the Family property so it is known in the class.
        /// Зарегистрируйте свойство Family так, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FamilyProperty = RegisterProperty("Family", typeof(Family), null);

        /// <summary>
        /// Gets the family members.
        /// Получает членов семьи.
        /// </summary>
        [ViewModelToModel("Family")]
        public ObservableCollection<Person> Persons
        {
            get { return GetValue<ObservableCollection<Person>>(PersonsProperty); }
            private set { SetValue(PersonsProperty, value); }
        }

        /// <summary>
        /// Register the Persons property so it is known in the class.
        /// Зарегистрируйте xktyjd ctvmb, чтобы они были известна в классе.
        /// </summary>
        public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<Person>), null);

        /// <summary>
        /// Gets or sets the family name.
        /// Получает или задает фамилию.
        /// </summary>
        [ViewModelToModel("Family")]
        public string FamilyName
        {
            get { return GetValue<string>(FamilyNameProperty); }
            set { SetValue(FamilyNameProperty, value); }
        }

        /// <summary>
        /// Register the FamilyName property so it is known in the class.
        /// Зарегистрируйте свойство FamilyName, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData FamilyNameProperty = RegisterProperty("FamilyName", typeof(string));

        /// <summary>
        /// Gets or sets the selected person.
        /// Получает или задает выбранного человека.
        /// </summary>
        public Person SelectedPerson
        {
            get { return GetValue<Person>(SelectedPersonProperty); }
            set { SetValue(SelectedPersonProperty, value); }
        }

        /// <summary>
        /// Register the SelectedPerson property so it is known in the class.
        /// Зарегистрируйте свойство SelectedPerson, чтобы оно было известно в классе.
        /// </summary>
        public static readonly PropertyData SelectedPersonProperty = RegisterProperty("SelectedPerson", typeof(Person), null);

        /// <summary>
        /// Gets the AddPerson command.
        /// Получает команду AddPerson.
        /// </summary>
        public TaskCommand AddPerson { get; private set; }

        /// <summary>
        /// Method to invoke when the AddPerson command is executed.
        /// Метод вызова при выполнении команды AddPerson.
        /// </summary>
        private async Task OnAddPersonExecuteAsync()
        {
            var person = new Person();
            person.LastName = FamilyName;

            // Note that we use the type factory here because it will automatically take care of any dependencies
            // that the PersonViewModel will add in the future
            // Обратите внимание, что мы используем фабрику типов здесь,
            // потому что она автоматически позаботится о любых зависимостях,
            // которые PersonViewModel будет добавлять в будущем
            var typeFactory = this.GetTypeFactory();
            var personViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PersonViewModel>(person);
            if (await uiVisualizerService.ShowDialogAsync(personViewModel) ?? false)
            {
                Persons.Add(person);
            }
        }

        /// <summary>
        /// Gets the EditPerson command.
        /// Получает команду EditPerson.
        /// </summary>
        public TaskCommand EditPerson { get; private set; }

        /// <summary>
        /// Method to check whether the EditPerson command can be executed.
        /// Метод проверки возможности выполнения команды EditPerson.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnEditPersonCanExecute()
        {
            return SelectedPerson != null;
        }

        /// <summary>
        /// Method to invoke when the EditPerson command is executed.
        /// Способ вызова, когда выполняется команда EditPerson.
        /// </summary>
        private async Task OnEditPersonExecuteAsync()
        {
            // Note that we use the type factory here because it will automatically take care of any dependencies
            // that the PersonViewModel will add in the future
            // Обратите внимание, что мы используем фабрику типов здесь,
            // потому что она автоматически позаботится о любых зависимостях,
            // которые PersonViewModel будет добавлять в будущем
            var typeFactory = this.GetTypeFactory();
            var personViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PersonViewModel>(SelectedPerson);
            await uiVisualizerService.ShowDialogAsync(personViewModel);
        }

        /// <summary>
        /// Gets the RemovePerson command.
        /// Получает команду RemovePerson.
        /// </summary>
        public TaskCommand RemovePerson { get; private set; }

        /// <summary>
        /// Method to check whether the RemovePerson command can be executed.
        /// Способ проверки выполнения команды RemovePerson.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnRemovePersonCanExecute()
        {
            return SelectedPerson != null;
        }

        /// <summary>
        /// Method to invoke when the RemovePerson command is executed.
        /// Способ вызова, когда выполняется команда RemovePerson.
        /// </summary>
        private async Task OnRemovePersonExecuteAsync()
        {
            if (await messageService.ShowAsync(string.Format("Are you sure you want to delete the person '{0}'?", SelectedPerson), 
                "Are you sure?", MessageButton.YesNo, MessageImage.Question) == MessageResult.Yes)
            {
                Persons.Remove(SelectedPerson);
                SelectedPerson = null;
            }
        }
    }
}
