using SocioManagerV2.Data;
using SocioManagerV2.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SocioManagerV2.ViewModels
{
    public class AddSocioViewModel : INotifyPropertyChanged
    {
        private readonly SociosContext _context = new SociosContext();
        private Socio _newSocio;

        public Socio NewSocio
        {
            get => _newSocio;
            set
            {
                _newSocio = value;
                OnPropertyChanged(nameof(NewSocio));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<Socio> SocioAdded;

        public AddSocioViewModel()
        {
            NewSocio = new Socio 
            { 
                FechaNacimiento = DateTime.Now, 
                FechaDeAlta = DateTime.Now 
            };

            SaveCommand = new RelayCommand(async _ => await SaveSocio());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private async Task SaveSocio()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewSocio.Nombre))
                {
                    MessageBox.Show("Name is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!isAdult(NewSocio.FechaNacimiento))
                {
                    MessageBox.Show("The socio must be at least 18 years old", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if(IsDniInDatabase(NewSocio.Dni))
                {
                    MessageBox.Show("This DNI is already registered", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!IsValidDni(NewSocio.Dni))
                {
                    MessageBox.Show("Invalid DNI format", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if(isBanned(NewSocio.Dni))
                {
                    string reason = reasonBanned(NewSocio.Dni);
                    MessageBox.Show("This DNI is banned and cannot be added, reason: " + reason, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _context.Socios.Add(NewSocio);
                await _context.SaveChangesAsync();

                MessageBox.Show("Socio added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                SocioAdded?.Invoke(this, NewSocio);

                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding socio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            var result = MessageBox.Show("Are you sure you want to cancel? All changes will be lost.", 
                "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }

        private bool isAdult(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }

        public static bool IsValidDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return false;

            dni = dni.ToUpper().Trim();

            // Check format: 8 digits + 1 letter
            if (!Regex.IsMatch(dni, @"^\d{8}[A-Z]$"))
                return false;

            string numberPart = dni.Substring(0, 8);
            char letterPart = dni[8];

            int number = int.Parse(numberPart);

            const string letters = "TRWAGMYFPDXBNJZSQVHLCKE";
            char correctLetter = letters[number % 23];

            return letterPart == correctLetter;
        }
        //Check if the DNI is already in the database
        public static bool IsDniInDatabase(string dni)
        {
            using var context = new SociosContext();
            return context.Socios.Any(s => s.Dni != null && s.Dni == dni);
        }

        // Check if the DNI is in the banned list
        public static bool isBanned(string dni)
        {
            using var context = new SociosContext();
            return context.Vetados.Any(v => v.dni != null && v.dni == dni);
        }

        public static string reasonBanned(string dni)
        {
            using var context = new SociosContext();
            var vetado = context.Vetados.FirstOrDefault(v => v.dni != null && v.dni == dni);
            return vetado != null ? vetado.motivo : "Unknown reason";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
