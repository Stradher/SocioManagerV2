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
    public class MemberSubscriptionStatus : INotifyPropertyChanged
    {
        private bool _isPaid;
        public int SocioId { get; set; }
        public string MemberName { get; set; } = string.Empty;

        public bool IsPaid 
        { 
            get => _isPaid; 
            set 
            {
                _isPaid = value;
                OnPropertyChanged(nameof(IsPaid));
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(ActionText));
            } 
        }

        public string StatusText => IsPaid ? "Paid" : "Pending";
        public string ActionText => IsPaid ? "❌ Undo Payment" : "✅ Mark as Paid";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class FinanceViewModel : INotifyPropertyChanged
    {
        private decimal _currentBalance;
        private string _transactionConcept = string.Empty;
        private string _transactionAmount = string.Empty;
        private DateTime _transactionDate = DateTime.Now;
        private bool _isIncome = true;
        private string _selectedMonth = string.Empty;

        public ObservableCollection<FinanceTransaction> TreasuryTransactions { get; set; }
        public ObservableCollection<string> AvailableMonths { get; set; }
        public ObservableCollection<MemberSubscriptionStatus> MemberSubscriptions { get; set; }

        public decimal CurrentBalance
        {
            get => _currentBalance;
            set { _currentBalance = value; OnPropertyChanged(nameof(CurrentBalance)); }
        }

        public string TransactionConcept
        {
            get => _transactionConcept;
            set { _transactionConcept = value; OnPropertyChanged(nameof(TransactionConcept)); }
        }

        public string TransactionAmount
        {
            get => _transactionAmount;
            set { _transactionAmount = value; OnPropertyChanged(nameof(TransactionAmount)); }
        }

        public DateTime TransactionDate
        {
            get => _transactionDate;
            set { _transactionDate = value; OnPropertyChanged(nameof(TransactionDate)); }
        }

        public bool IsIncome
        {
            get => _isIncome;
            set { _isIncome = value; OnPropertyChanged(nameof(IsIncome)); }
        }

        public string SelectedMonth
        {
            get => _selectedMonth;
            set 
            { 
                _selectedMonth = value; 
                OnPropertyChanged(nameof(SelectedMonth)); 
                _ = LoadSubscriptions();
            }
        }

        public ICommand AddTransactionCommand { get; }
        public ICommand TogglePaymentCommand { get; }

        public FinanceViewModel()
        {
            TreasuryTransactions = new ObservableCollection<FinanceTransaction>();
            AvailableMonths = new ObservableCollection<string>();
            MemberSubscriptions = new ObservableCollection<MemberSubscriptionStatus>();

            AddTransactionCommand = new RelayCommand(async _ => await AddTransaction());
            TogglePaymentCommand = new RelayCommand(async param => await TogglePayment(param as MemberSubscriptionStatus));

            InitializeMonths();
            LoadTreasuryData();
        }

        private void InitializeMonths()
        {
            var current = DateTime.Now;
            for (int i = -12; i <= 12; i++)
            {
                var month = current.AddMonths(i);
                AvailableMonths.Add(month.ToString("yyyy-MM"));
            }
            SelectedMonth = current.ToString("yyyy-MM");
        }

        private async void LoadTreasuryData()
        {
            try
            {
                using (var context = new SociosContext())
                {
                    context.Database.EnsureCreated();

                    var allTransactions = await context.FinanceTransactions.ToListAsync();

                    var totalIncome = allTransactions.Where(t => t.Type == "income").Sum(t => t.Amount);
                    var totalExpense = allTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
                    CurrentBalance = totalIncome - totalExpense;

                    var treasuryLog = allTransactions.Where(t => t.Category == "treasury").OrderByDescending(t => t.Date).ToList();

                    TreasuryTransactions.Clear();
                    foreach (var t in treasuryLog)
                    {
                        TreasuryTransactions.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading treasury data: {ex.Message}", "Error");
            }
        }

        private async Task AddTransaction()
        {
            if (string.IsNullOrWhiteSpace(TransactionConcept) || !decimal.TryParse(TransactionAmount, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid concept and a positive amount.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var tx = new FinanceTransaction
                {
                    Concept = TransactionConcept,
                    Amount = amount,
                    Date = TransactionDate,
                    Type = IsIncome ? "income" : "expense",
                    Category = "treasury"
                };

                using (var context = new SociosContext())
                {
                    context.FinanceTransactions.Add(tx);
                    await context.SaveChangesAsync();
                }

                TransactionConcept = string.Empty;
                TransactionAmount = string.Empty;
                TransactionDate = DateTime.Now;
                IsIncome = true;

                LoadTreasuryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding transaction: {ex.Message}", "Error");
            }
        }

        private async Task LoadSubscriptions()
        {
            if (string.IsNullOrEmpty(SelectedMonth)) return;

            try
            {
                using (var context = new SociosContext())
                {
                    // Active members = Baja == null or future
                    var activeMembers = await context.Socios.Where(s => s.FechaDeBaja == null).ToListAsync();
                    var paymentsThisMonth = await context.SubscriptionPayments
                        .Where(p => p.MonthYear == SelectedMonth)
                        .Select(p => p.SocioId)
                        .ToListAsync();

                    MemberSubscriptions.Clear();
                    foreach (var member in activeMembers)
                    {
                        MemberSubscriptions.Add(new MemberSubscriptionStatus
                        {
                            SocioId = member.Id,
                            MemberName = $"{member.Nombre} {member.Apellido1}",
                            IsPaid = paymentsThisMonth.Contains(member.Id)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subscriptions: {ex.Message}", "Error");
            }
        }

        private async Task TogglePayment(MemberSubscriptionStatus? status)
        {
            if (status == null || string.IsNullOrEmpty(SelectedMonth)) return;

            try
            {
                using (var context = new SociosContext())
                {
                    if (status.IsPaid)
                    {
                        var payment = await context.SubscriptionPayments.FirstOrDefaultAsync(p => p.SocioId == status.SocioId && p.MonthYear == SelectedMonth);
                        if (payment != null)
                        {
                            context.SubscriptionPayments.Remove(payment);
                        }

                        string conceptMatch = $"Subscription {SelectedMonth} - {status.MemberName}";
                        var tx = await context.FinanceTransactions.FirstOrDefaultAsync(t => t.Concept == conceptMatch && t.Category == "subscription");
                        if (tx != null)
                        {
                            context.FinanceTransactions.Remove(tx);
                        }

                        await context.SaveChangesAsync();
                        status.IsPaid = false;
                    }
                    else
                    {
                        var payment = new SubscriptionPayment
                        {
                            SocioId = status.SocioId,
                            MonthYear = SelectedMonth,
                            PaymentDate = DateTime.Now,
                            Amount = 10.0m // Example default amount
                        };
                        context.SubscriptionPayments.Add(payment);

                        var tx = new FinanceTransaction
                        {
                            Concept = $"Subscription {SelectedMonth} - {status.MemberName}",
                            Amount = payment.Amount,
                            Date = DateTime.Now,
                            Type = "income",
                            Category = "subscription"
                        };
                        context.FinanceTransactions.Add(tx);

                        await context.SaveChangesAsync();
                        status.IsPaid = true;
                    }
                }

                LoadTreasuryData(); // to update balance
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error recording payment toggle: {ex.Message}", "Error");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}