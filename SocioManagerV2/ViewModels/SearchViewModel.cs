using Microsoft.EntityFrameworkCore;
using SocioManagerV2.Data;
using SocioManagerV2.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SocioManagerV2.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private string _searchName;
        private string _searchDni;
        private string _searchNumeroDeSocio;
        private Socio _selectedSocio;
        private ICollectionView _searchResultsView;

        public ObservableCollection<Socio> SearchResults { get; set; }

        public ICollectionView SearchResultsView
        {
            get => _searchResultsView;
            set
            {
                _searchResultsView = value;
                OnPropertyChanged(nameof(SearchResultsView));
            }
        }

        public string SearchName
        {
            get => _searchName;
            set
            {
                _searchName = value;
                OnPropertyChanged(nameof(SearchName));
            }
        }

        public string SearchDni
        {
            get => _searchDni;
            set
            {
                _searchDni = value;
                OnPropertyChanged(nameof(SearchDni));
            }
        }

        public string SearchNumeroDeSocio
        {
            get => _searchNumeroDeSocio;
            set
            {
                _searchNumeroDeSocio = value;
                OnPropertyChanged(nameof(SearchNumeroDeSocio));
            }
        }

        public Socio SelectedSocio
        {
            get => _selectedSocio;
            set
            {
                _selectedSocio = value;
                OnPropertyChanged(nameof(SelectedSocio));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SelectCommand { get; }

        public event EventHandler<Socio> SocioSelected;

        public SearchViewModel()
        {
            SearchResults = new ObservableCollection<Socio>();
            SearchResultsView = CollectionViewSource.GetDefaultView(SearchResults);

            SearchCommand = new RelayCommand(async _ => await PerformSearch());
            ClearCommand = new RelayCommand(_ => ClearSearch());
            SelectCommand = new RelayCommand(_ => SelectSocio(), _ => SelectedSocio != null);
        }

        private async Task PerformSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchName) && 
                    string.IsNullOrWhiteSpace(SearchDni) && 
                    string.IsNullOrWhiteSpace(SearchNumeroDeSocio))
                {
                    MessageBox.Show("Please enter at least one search criteria", "Validation", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var context = new SociosContext())
                {
                    // Load all socios from database
                    var allSocios = await context.Socios.AsNoTracking().ToListAsync();

                    // Debug: Show total count
                    System.Diagnostics.Debug.WriteLine($"Total socios loaded: {allSocios.Count}");

                    if (allSocios.Count == 0)
                    {
                        MessageBox.Show("No members found in database. Please add members first.", "Search", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Filter in memory for better compatibility
                    var results = allSocios.AsEnumerable();

                    if (!string.IsNullOrWhiteSpace(SearchName))
                    {
                        var nameLower = SearchName.ToLower().Trim();
                        System.Diagnostics.Debug.WriteLine($"Searching for name: {nameLower}");
                        results = results.Where(s => 
                            (s.Nombre != null && s.Nombre.ToLower().Contains(nameLower)) ||
                            (s.Apellido1 != null && s.Apellido1.ToLower().Contains(nameLower)) ||
                            (s.Apellido2 != null && s.Apellido2.ToLower().Contains(nameLower)));
                    }

                    if (!string.IsNullOrWhiteSpace(SearchDni))
                    {
                        var dniTrimmed = SearchDni.Trim();
                        System.Diagnostics.Debug.WriteLine($"Searching for DNI: {dniTrimmed}");
                        results = results.Where(s => s.Dni != null && s.Dni.Contains(dniTrimmed, StringComparison.OrdinalIgnoreCase));
                    }

                    if (!string.IsNullOrWhiteSpace(SearchNumeroDeSocio))
                    {
                        if (int.TryParse(SearchNumeroDeSocio.Trim(), out int numero))
                        {
                            System.Diagnostics.Debug.WriteLine($"Searching for member number: {numero}");

                            // Debug: Show all member numbers in database
                            var allNumbers = allSocios.Select(s => s.NumeroDeSocio).OrderBy(n => n).ToList();
                            System.Diagnostics.Debug.WriteLine($"All member numbers in DB: {string.Join(", ", allNumbers)}");

                            results = results.Where(s => s.NumeroDeSocio == numero);
                        }
                        else
                        {
                            MessageBox.Show("Member number must be a valid number", "Validation", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    var finalResults = results.ToList();
                    System.Diagnostics.Debug.WriteLine($"Results after filtering: {finalResults.Count}");

                    SearchResults.Clear();
                    foreach (var socio in finalResults)
                    {
                        SearchResults.Add(socio);
                    }

                    if (SearchResults.Count == 0)
                    {
                        MessageBox.Show("No results found matching your search criteria.", "Search Results", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    SearchResultsView?.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing search: {ex.Message}\n\nDetails: {ex.InnerException?.Message}\n\nStack: {ex.StackTrace}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchName = string.Empty;
            SearchDni = string.Empty;
            SearchNumeroDeSocio = string.Empty;
            SearchResults.Clear();
            SelectedSocio = null;
        }

        private void SelectSocio()
        {
            if (SelectedSocio != null)
            {
                SocioSelected?.Invoke(this, SelectedSocio);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
