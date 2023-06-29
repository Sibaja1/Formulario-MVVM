using Form.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using static System.Net.Mime.MediaTypeNames;

namespace Form.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int id;
        private int age;
        private string name;
        private string email;
        private ObservableCollection<Person> lista;
        ListPerson personas;

        private Person selectedPerson;

        private bool isButtonEnabled;

        private bool isTextBoxEnabled;

        private ICommand saveCommand;
        private ICommand deleteCommand;
        private ICommand newCommand;
        private ICommand doubleClickCommand;

        public event EventHandler<Person> PersonAdded;
        public event EventHandler<Person> PersonEdit;


        public bool AgregarPersona(ObservableCollection<Person> personas, Person persona)
        {
            Person persona2 = personas.FirstOrDefault(p => p.id == persona.id);

            if (persona2 == null)
            {
                personas.Add(persona);
                PersonAdded?.Invoke(this, persona);
                return true;
            }
            return false;
        }

        public bool EliminarPersona(ObservableCollection<Person> personas, int id)
        {
            Person persona = personas.FirstOrDefault(p => p.id == id);
            if (persona != null)
            {
                personas.Remove(persona);
                return true;
            }
            return false;
        }

        public int SearchPerson(ObservableCollection<Person> personas, int personId)
        {
            int index = -1;
            for (int i = 0; i < personas.Count; i++)
            {
                if (personas[i].id == personId)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public bool EditPerson(ObservableCollection<Person> personas, Person persona)
        {
            int index = SearchPerson(personas, persona.id);

            if (index != -1)
            {
                personas.RemoveAt(index);
                personas.Insert(index, persona);
                PersonEdit?.Invoke(this, persona);
                return true;
            }
            else
            {
                return false;
            }
        }
        public ICommand DoubleClickCommand
        {
            get
            {
                if (doubleClickCommand == null)
                {
                    doubleClickCommand = new RelayCommand(ExecuteDoubleClick);
                }
                return doubleClickCommand;
            }
        }

        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set
            {
                isButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsTextBoxEnabled
        {
            get { return isTextBoxEnabled; }
            set
            {
                isTextBoxEnabled = value;
                OnPropertyChanged();
            }
        }

        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                OnPropertyChanged();
            }
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                OnPropertyChanged("Age");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public ObservableCollection<Person> Lista
        {
            get { return lista; }
            set
            {
                lista = value;
                OnPropertyChanged(nameof(lista));
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(p => this.Delete());
                }
                return deleteCommand;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(p => this.Save());
                }
                return saveCommand;
            }
        }

        public ICommand NewCommand
        {
            get
            {
                if (newCommand == null)
                {
                    newCommand = new RelayCommand(p => this.New());
                }
                return newCommand;
            }
        }

        public MainViewModel()
        {
            Lista = new ObservableCollection<Person>();
            IsButtonEnabled = true;
            isTextBoxEnabled = true;

            Person person1 = new Person(1, "Jose Alejandro", 23, "japs_misterio@hotmail.com");

            Lista.Add(person1);

            PersonAdded += PersonAddedChange;
            PersonEdit += PersonEditChange;

        }

        public void Save()
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
                          id <= 0 || age <= 0)
            {
                MessageBox.Show("Hay campos vacios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Person persona = new Person(id, name, age, email);

                if (SearchPerson(Lista, id) != -1)
                {
                    MessageBoxResult result = MessageBox.Show("¿Seguro que desea editar?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        EditPerson(Lista, persona);
                        Clean();
                        IsButtonEnabled = true;
                        IsTextBoxEnabled = true;
                    }

                }
                else
                {
                    if (AgregarPersona(Lista, persona))
                    {
                        Clean();
                    }
                    else
                    {
                        MessageBox.Show($"Esta persona ya se encuentra registrado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }

        }

        public void Delete()
        {
            if (SelectedPerson != null)
            {
                MessageBoxResult result = MessageBox.Show("¿Seguro que deseas eliminar ala persona?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    EliminarPersona(Lista, SelectedPerson.id);
                }
                else
                {
                    SelectedPerson = null;
                }
            }
            else
            {
                MessageBox.Show($"No se selecciono ninguna persona", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ExecuteDoubleClick(object parameter)
        {
            if (SelectedPerson != null)
            {
                id = SelectedPerson.id;
                age = SelectedPerson.age;
                name = SelectedPerson.name;
                email = SelectedPerson.email;

                update(id, age, name, email);
                IsButtonEnabled = false;
                IsTextBoxEnabled = false;
            }
        }
        public void Clean()
        {
            id = 0;
            age = 0;
            name = null;
            email = null;

            update(id,age,name,email);
        }

        public void update(int id, int age, string name, string email)
        {
            OnPropertyChanged("Id");
            OnPropertyChanged("Age");
            OnPropertyChanged("Name");
            OnPropertyChanged("Email");
        }

        public void New()
        {
            IsButtonEnabled = true;
            SelectedPerson = null;
            IsTextBoxEnabled = true;
            Clean();
        }

        public void PersonAddedChange(object s, Person e)
        {
            MessageBox.Show($"Se ha agredado una nueva persona exitosamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void PersonEditChange(object s, Person e)
        {
            MessageBox.Show($"Se editado a la persona exitosamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
