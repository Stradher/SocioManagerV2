using Microsoft.EntityFrameworkCore;
using SocioManagerV2.Data;
using SocioManagerV2.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SocioManagerV2.ViewModels
{
    public class BoardGamesViewModel : INotifyPropertyChanged
    {
        private BoardGame? _selectedGame;
        private string _gameName = string.Empty;
        private Socio? _selectedOwner;

        private BoardGame? _selectedLoanGame;
        private Socio? _selectedBorrower;
        private BoardGameLoan? _selectedLoan;

        public ObservableCollection<BoardGame> BoardGames { get; set; }
        public ObservableCollection<Socio> Socios { get; set; }
        public ObservableCollection<BoardGameLoan> Loans { get; set; }

        public BoardGame? SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));
            }
        }

        public string GameName
        {
            get => _gameName;
            set
            {
                _gameName = value;
                OnPropertyChanged(nameof(GameName));
            }
        }

        public Socio? SelectedOwner
        {
            get => _selectedOwner;
            set
            {
                _selectedOwner = value;
                OnPropertyChanged(nameof(SelectedOwner));
            }
        }

        public BoardGame? SelectedLoanGame
        {
            get => _selectedLoanGame;
            set
            {
                _selectedLoanGame = value;
                OnPropertyChanged(nameof(SelectedLoanGame));
            }
        }

        public Socio? SelectedBorrower
        {
            get => _selectedBorrower;
            set
            {
                _selectedBorrower = value;
                OnPropertyChanged(nameof(SelectedBorrower));
            }
        }

        public BoardGameLoan? SelectedLoan
        {
            get => _selectedLoan;
            set
            {
                _selectedLoan = value;
                OnPropertyChanged(nameof(SelectedLoan));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddLoanCommand { get; }
        public ICommand ReturnGameCommand { get; }

        public BoardGamesViewModel()
        {
            BoardGames = new ObservableCollection<BoardGame>();
            Socios = new ObservableCollection<Socio>();
            Loans = new ObservableCollection<BoardGameLoan>();

            AddCommand = new RelayCommand(async _ => await AddGame());
            DeleteCommand = new RelayCommand(async _ => await DeleteGame(), _ => SelectedGame != null);
            AddLoanCommand = new RelayCommand(async _ => await AddLoan());
            ReturnGameCommand = new RelayCommand(async _ => await ReturnGame(), _ => SelectedLoan != null && SelectedLoan.ReturnDate == null);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                using (var context = new SociosContext())
                {
                    context.Database.EnsureCreated();
                    var gamesList = await context.BoardGames.Include(b => b.Owner).ToListAsync();
                    var sociosList = await context.Socios.AsNoTracking().ToListAsync();
                    var loansList = await context.BoardGameLoans
                        .Include(l => l.BoardGame)
                        .Include(l => l.Borrower)
                        .OrderByDescending(l => l.LoanDate)
                        .ToListAsync();

                    BoardGames.Clear();
                    foreach (var game in gamesList)
                        BoardGames.Add(game);

                    Socios.Clear();
                    foreach (var socio in sociosList)
                        Socios.Add(socio);

                    Loans.Clear();
                    foreach (var loan in loansList)
                        Loans.Add(loan);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddGame()
        {
            if (string.IsNullOrWhiteSpace(GameName))
            {
                MessageBox.Show("Game name is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newGame = new BoardGame
                {
                    Name = GameName,
                    OwnerId = SelectedOwner?.Id
                };

                using (var context = new SociosContext())
                {
                    context.BoardGames.Add(newGame);
                    await context.SaveChangesAsync();
                }

                GameName = string.Empty;
                SelectedOwner = null;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteGame()
        {
            if (SelectedGame == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedGame.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new SociosContext())
                    {
                        var gameToDelete = await context.BoardGames.FindAsync(SelectedGame.Id);
                        if (gameToDelete != null)
                        {
                            context.BoardGames.Remove(gameToDelete);
                            await context.SaveChangesAsync();
                        }
                    }

                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task AddLoan()
        {
            if (SelectedLoanGame == null || SelectedBorrower == null)
            {
                MessageBox.Show("Please select a game and a borrower.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new SociosContext())
                {
                    // Check if the game is already borrowed and not returned
                    var isBorrowed = await context.BoardGameLoans.AnyAsync(l => l.BoardGameId == SelectedLoanGame.Id && l.ReturnDate == null);
                    if (isBorrowed)
                    {
                        MessageBox.Show("This game is currently borrowed.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newLoan = new BoardGameLoan
                    {
                        BoardGameId = SelectedLoanGame.Id,
                        BorrowerId = SelectedBorrower.Id,
                        LoanDate = DateTime.Now
                    };

                    context.BoardGameLoans.Add(newLoan);
                    await context.SaveChangesAsync();
                }

                SelectedLoanGame = null;
                SelectedBorrower = null;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding loan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ReturnGame()
        {
            if (SelectedLoan == null || SelectedLoan.ReturnDate != null) return;

            var result = MessageBox.Show($"Mark '{SelectedLoan.BoardGame.Name}' as returned?", "Confirm Return", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new SociosContext())
                    {
                        var loanToUpdate = await context.BoardGameLoans.FindAsync(SelectedLoan.Id);
                        if (loanToUpdate != null)
                        {
                            loanToUpdate.ReturnDate = DateTime.Now;
                            await context.SaveChangesAsync();
                        }
                    }

                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error returning game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
