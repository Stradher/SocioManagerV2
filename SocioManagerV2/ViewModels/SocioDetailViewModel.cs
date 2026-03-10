using SocioManagerV2.Data;
using SocioManagerV2.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SocioManagerV2.ViewModels
{
    public class SocioDetailViewModel : INotifyPropertyChanged
    {
        private readonly SociosContext _context = new SociosContext();
        private Socio _socio;
        private bool _isEditMode = false;
        private bool _isBanned = false;
        private string _banReason = string.Empty;

        public Socio Socio
        {
            get => _socio;
            set
            {
                _socio = value;
                OnPropertyChanged(nameof(Socio));
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public bool IsReadOnly => !IsEditMode;

        public bool IsBanned
        {
            get => _isBanned;
            set
            {
                _isBanned = value;
                OnPropertyChanged(nameof(IsBanned));
            }
        }

        public string BanReason
        {
            get => _banReason;
            set
            {
                _banReason = value;
                OnPropertyChanged(nameof(BanReason));
            }
        }

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }

        public event EventHandler<Socio> SocioUpdated;

        public SocioDetailViewModel(Socio socio)
        {
            Socio = new Socio
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido1 = socio.Apellido1,
                Apellido2 = socio.Apellido2,
                Alta = socio.Alta,
                Dni = socio.Dni,
                FechaNacimiento = socio.FechaNacimiento,
                NumeroDeSocio = socio.NumeroDeSocio,
                NumeroDeTelefono = socio.NumeroDeTelefono,
                CorreoElectronico = socio.CorreoElectronico,
                DireccionPostal = socio.DireccionPostal,
                FechaDeAlta = socio.FechaDeAlta,
                FechaDeBaja = socio.FechaDeBaja
            };

            EditCommand = new RelayCommand(_ => EnableEditMode());
            SaveCommand = new RelayCommand(async _ => await SaveSocio(), _ => IsEditMode);
            CancelCommand = new RelayCommand(_ => CancelEdit(), _ => IsEditMode);
            CloseCommand = new RelayCommand(_ => CloseWindow());

            CheckIfBanned();
        }

        private void CheckIfBanned()
        {
            if (!string.IsNullOrWhiteSpace(Socio.Dni))
            {
                IsBanned = _context.Vetados.Any(v => v.dni != null && v.dni == Socio.Dni);
                if (IsBanned)
                {
                    var vetado = _context.Vetados.FirstOrDefault(v => v.dni != null && v.dni == Socio.Dni);
                    BanReason = vetado?.motivo ?? "Unknown reason";
                }
            }
        }

        private void EnableEditMode()
        {
            IsEditMode = true;
        }

        private async Task SaveSocio()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Socio.Nombre))
                {
                    MessageBox.Show("Name is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var socioToUpdate = await _context.Socios.FindAsync(Socio.Id);
                if (socioToUpdate != null)
                {
                    socioToUpdate.Nombre = Socio.Nombre;
                    socioToUpdate.Apellido1 = Socio.Apellido1;
                    socioToUpdate.Apellido2 = Socio.Apellido2;
                    socioToUpdate.Alta = Socio.Alta;
                    socioToUpdate.Dni = Socio.Dni;
                    socioToUpdate.FechaNacimiento = Socio.FechaNacimiento;
                    socioToUpdate.NumeroDeSocio = Socio.NumeroDeSocio;
                    socioToUpdate.NumeroDeTelefono = Socio.NumeroDeTelefono;
                    socioToUpdate.CorreoElectronico = Socio.CorreoElectronico;
                    socioToUpdate.DireccionPostal = Socio.DireccionPostal;
                    socioToUpdate.FechaDeAlta = Socio.FechaDeAlta;
                    socioToUpdate.FechaDeBaja = Socio.FechaDeBaja;

                    await _context.SaveChangesAsync();

                    MessageBox.Show("Socio updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    SocioUpdated?.Invoke(this, socioToUpdate);

                    IsEditMode = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating socio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit()
        {
            var result = MessageBox.Show("Are you sure you want to cancel? All changes will be lost.",
                "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsEditMode = false;
            }
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
