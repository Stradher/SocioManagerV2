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
        public ICommand SearchCommand { get; }
        public ICommand OpenBoardGamesCommand { get; }
        public ICommand OpenRoomsCommand { get; }
        public ICommand OpenFinanceCommand { get; }

        public MainViewModel()
        {
            using (var context = new SociosContext())
            {
                context.Database.EnsureCreated();
                try 
                {
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS board_games (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, owner_id INTEGER, FOREIGN KEY(owner_id) REFERENCES socios(id) ON DELETE SET NULL);");
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS board_game_loans (id INTEGER PRIMARY KEY AUTOINCREMENT, board_game_id INTEGER NOT NULL, borrower_id INTEGER NOT NULL, loan_date TEXT NOT NULL, return_date TEXT, FOREIGN KEY(board_game_id) REFERENCES board_games(id) ON DELETE CASCADE, FOREIGN KEY(borrower_id) REFERENCES socios(id) ON DELETE CASCADE);");
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS rooms (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, max_capacity INTEGER NOT NULL);");
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS room_bookings (id INTEGER PRIMARY KEY AUTOINCREMENT, room_id INTEGER NOT NULL, booker_id INTEGER NOT NULL, booking_date TEXT NOT NULL, duration_minutes INTEGER NOT NULL, FOREIGN KEY(room_id) REFERENCES rooms(id) ON DELETE CASCADE, FOREIGN KEY(booker_id) REFERENCES socios(id) ON DELETE CASCADE);");
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS finance_transactions (id INTEGER PRIMARY KEY AUTOINCREMENT, amount DECIMAL(18,2) NOT NULL, date TEXT NOT NULL, concept TEXT NOT NULL, type TEXT NOT NULL, category TEXT NOT NULL);");
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS subscription_payments (id INTEGER PRIMARY KEY AUTOINCREMENT, socio_id INTEGER NOT NULL, month_year TEXT NOT NULL, payment_date TEXT NOT NULL, amount DECIMAL(18,2) NOT NULL, FOREIGN KEY(socio_id) REFERENCES socios(id) ON DELETE CASCADE);");
                } 
                catch { }
            }

            Socios = new ObservableCollection<Socio>();
            CurrentSocio = new Socio { FechaNacimiento = DateTime.Now, FechaDeAlta = DateTime.Now };

            AddCommand = new RelayCommand(_ => OpenAddSocioWindow());
            UpdateCommand = new RelayCommand(async _ => await UpdateSocio(), _ => SelectedSocio != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteSocio(), _ => SelectedSocio != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
            ViewSocioDetailsCommand = new RelayCommand(_ => OpenSocioDetailWindow(), _ => SelectedSocio != null);
            SearchCommand = new RelayCommand(_ => OpenSearchWindow());
            OpenBoardGamesCommand = new RelayCommand(_ => OpenBoardGamesWindow());
            OpenRoomsCommand = new RelayCommand(_ => OpenRoomsWindow());
            OpenFinanceCommand = new RelayCommand(_ => OpenFinanceWindow());

            LoadSocios();
        }

        private async void LoadSocios()
        {
            try
            {
                using (var context = new SociosContext())
                {
                    var list = await context.Socios.AsNoTracking().ToListAsync();
                    Socios.Clear();
                    foreach (var socio in list)
                    {
                        Socios.Add(socio);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenSearchWindow()
        {
            var searchWindow = new SearchWindow
            {
                Owner = Application.Current.MainWindow
            };

            var viewModel = searchWindow.DataContext as SearchViewModel;
            if (viewModel != null)
            {
                viewModel.SocioSelected += (sender, selectedSocio) =>
                {
                    var existingSocio = Socios.FirstOrDefault(s => s.Id == selectedSocio.Id);
                    if (existingSocio != null)
                    {
                        SelectedSocio = existingSocio;
                    }
                    searchWindow.DialogResult = true;
                    searchWindow.Close();
                };
            }

            searchWindow.ShowDialog();
        }

        private void OpenBoardGamesWindow()
        {
            var boardGamesWindow = new BoardGamesWindow
            {
                Owner = Application.Current.MainWindow
            };
            boardGamesWindow.ShowDialog();
        }

        private void OpenRoomsWindow()
        {
            var roomsWindow = new RoomsWindow
            {
                Owner = Application.Current.MainWindow
            };
            roomsWindow.ShowDialog();
        }

        private void OpenFinanceWindow()
        {
            var financeWindow = new FinanceWindow
            {
                Owner = Application.Current.MainWindow
            };
            financeWindow.ShowDialog();
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

                using (var context = new SociosContext())
                {
                    var socioToUpdate = await context.Socios.FindAsync(CurrentSocio.Id);
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

                        await context.SaveChangesAsync();

                        var index = Socios.IndexOf(SelectedSocio);
                        if (index >= 0)
                        {
                            Socios[index] = socioToUpdate;
                        }

                        ClearForm();
                        MessageBox.Show("Socio updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
                    using (var context = new SociosContext())
                    {
                        var socioToDelete = await context.Socios.FindAsync(SelectedSocio.Id);
                        if (socioToDelete != null)
                        {
                            context.Socios.Remove(socioToDelete);
                            await context.SaveChangesAsync();

                            Socios.Remove(SelectedSocio);
                            ClearForm();

                            MessageBox.Show("Socio deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error deleting socio: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                errorMessage += $"\n\nStack Trace: {ex.StackTrace}";
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
