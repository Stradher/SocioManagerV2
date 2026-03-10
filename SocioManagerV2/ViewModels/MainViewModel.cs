using Microsoft.EntityFrameworkCore;
using SocioManagerV2.Data;
using SocioManagerV2.Models;
using SocioManagerV2.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SocioManagerV2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SociosContext _context = new SociosContext();
        private Socio _selectedSocio;
        private Socio _currentSocio;

        public ObservableCollection<Socio> Socios { get; set; }

        public Socio SelectedSocio
        {
            get => _selectedSocio;
            set
            {
                _selectedSocio = value;
                OnPropertyChanged(nameof(SelectedSocio));
                if (_selectedSocio != null)
                {
                    CurrentSocio = new Socio
                    {
                        Id = _selectedSocio.Id,
                        Nombre = _selectedSocio.Nombre,
                        Apellido1 = _selectedSocio.Apellido1,
                        Apellido2 = _selectedSocio.Apellido2,
                        Alta = _selectedSocio.Alta,
                        Dni = _selectedSocio.Dni,
                        FechaNacimiento = _selectedSocio.FechaNacimiento,
                        NumeroDeSocio = _selectedSocio.NumeroDeSocio,
                        NumeroDeTelefono = _selectedSocio.NumeroDeTelefono,
                        CorreoElectronico = _selectedSocio.CorreoElectronico,
                        DireccionPostal = _selectedSocio.DireccionPostal,
                        FechaDeAlta = _selectedSocio.FechaDeAlta,
                        FechaDeBaja = _selectedSocio.FechaDeBaja
                    };
                }
            }
        }

        public Socio CurrentSocio
        {
            get => _currentSocio;
            set
            {
                _currentSocio = value;
                OnPropertyChanged(nameof(CurrentSocio));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ViewSocioDetailsCommand { get; }

        public MainViewModel()
        {
            Socios = new ObservableCollection<Socio>();
            CurrentSocio = new Socio { FechaNacimiento = DateTime.Now, FechaDeAlta = DateTime.Now };

            AddCommand = new RelayCommand(_ => OpenAddSocioWindow());
            UpdateCommand = new RelayCommand(async _ => await UpdateSocio(), _ => SelectedSocio != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteSocio(), _ => SelectedSocio != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
            ViewSocioDetailsCommand = new RelayCommand(_ => OpenSocioDetailWindow(), _ => SelectedSocio != null);

            LoadSocios();
        }

        private async void LoadSocios()
        {
            try
            {
                var list = await _context.Socios.ToListAsync();
                Socios.Clear();
                foreach (var socio in list)
                {
                    Socios.Add(socio);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenAddSocioWindow()
        {
            var addWindow = new AddSocioWindow
            {
                Owner = Application.Current.MainWindow
            };

            var viewModel = addWindow.DataContext as AddSocioViewModel;
            if (viewModel != null)
            {
                viewModel.SocioAdded += (sender, newSocio) =>
                {
                    Socios.Add(newSocio);
                };
            }

            addWindow.ShowDialog();
        }

        private void OpenSocioDetailWindow()
        {
            if (SelectedSocio == null)
            {
                return;
            }

            var detailWindow = new SocioDetailWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new SocioDetailViewModel(SelectedSocio)
            };

            var viewModel = detailWindow.DataContext as SocioDetailViewModel;
            if (viewModel != null)
            {
                viewModel.SocioUpdated += (sender, updatedSocio) =>
                {
                    var index = Socios.IndexOf(SelectedSocio);
                    if (index >= 0)
                    {
                        Socios[index] = updatedSocio;
                        SelectedSocio = updatedSocio;
                    }
                };
            }

            detailWindow.ShowDialog();
        }

        private async Task UpdateSocio()
        {
            try
            {
                if (SelectedSocio == null)
                {
                    MessageBox.Show("Please select a socio to update", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentSocio.Nombre))
                {
                    MessageBox.Show("Name is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var socioToUpdate = await _context.Socios.FindAsync(CurrentSocio.Id);
                if (socioToUpdate != null)
                {
                    socioToUpdate.Nombre = CurrentSocio.Nombre;
                    socioToUpdate.Apellido1 = CurrentSocio.Apellido1;
                    socioToUpdate.Apellido2 = CurrentSocio.Apellido2;
                    socioToUpdate.Alta = CurrentSocio.Alta;
                    socioToUpdate.Dni = CurrentSocio.Dni;
                    socioToUpdate.FechaNacimiento = CurrentSocio.FechaNacimiento;
                    socioToUpdate.NumeroDeSocio = CurrentSocio.NumeroDeSocio;
                    socioToUpdate.NumeroDeTelefono = CurrentSocio.NumeroDeTelefono;
                    socioToUpdate.CorreoElectronico = CurrentSocio.CorreoElectronico;
                    socioToUpdate.DireccionPostal = CurrentSocio.DireccionPostal;
                    socioToUpdate.FechaDeAlta = CurrentSocio.FechaDeAlta;
                    socioToUpdate.FechaDeBaja = CurrentSocio.FechaDeBaja;

                    await _context.SaveChangesAsync();

                    var index = Socios.IndexOf(SelectedSocio);
                    if (index >= 0)
                    {
                        Socios[index] = socioToUpdate;
                    }

                    ClearForm();
                    MessageBox.Show("Socio updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating socio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteSocio()
        {
            try
            {
                if (SelectedSocio == null)
                {
                    MessageBox.Show("Please select a socio to delete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete {SelectedSocio.Nombre}?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var socioToDelete = await _context.Socios.FindAsync(SelectedSocio.Id);
                    if (socioToDelete != null)
                    {
                        _context.Socios.Remove(socioToDelete);
                        await _context.SaveChangesAsync();

                        Socios.Remove(SelectedSocio);
                        ClearForm();

                        MessageBox.Show("Socio deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting socio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            CurrentSocio = new Socio { FechaNacimiento = DateTime.Now, FechaDeAlta = DateTime.Now };
            SelectedSocio = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
