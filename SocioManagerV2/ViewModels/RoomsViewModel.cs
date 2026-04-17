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
    public class RoomsViewModel : INotifyPropertyChanged
    {
        private string _roomName = string.Empty;
        private string _roomCapacity = string.Empty;
        private Room? _selectedRoom;

        private Room? _selectedBookingRoom;
        private Socio? _selectedBooker;
        private string _bookingDuration = string.Empty;
        private RoomBooking? _selectedBooking;

        public ObservableCollection<Room> Rooms { get; set; }
        public ObservableCollection<Socio> Socios { get; set; }
        public ObservableCollection<RoomBooking> Bookings { get; set; }

        public string RoomName
        {
            get => _roomName;
            set { _roomName = value; OnPropertyChanged(nameof(RoomName)); }
        }

        public string RoomCapacity
        {
            get => _roomCapacity;
            set { _roomCapacity = value; OnPropertyChanged(nameof(RoomCapacity)); }
        }

        public Room? SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(nameof(SelectedRoom)); }
        }

        public Room? SelectedBookingRoom
        {
            get => _selectedBookingRoom;
            set { _selectedBookingRoom = value; OnPropertyChanged(nameof(SelectedBookingRoom)); }
        }

        public Socio? SelectedBooker
        {
            get => _selectedBooker;
            set { _selectedBooker = value; OnPropertyChanged(nameof(SelectedBooker)); }
        }

        public string BookingDuration
        {
            get => _bookingDuration;
            set { _bookingDuration = value; OnPropertyChanged(nameof(BookingDuration)); }
        }

        public RoomBooking? SelectedBooking
        {
            get => _selectedBooking;
            set { _selectedBooking = value; OnPropertyChanged(nameof(SelectedBooking)); }
        }

        public ICommand AddRoomCommand { get; }
        public ICommand DeleteRoomCommand { get; }
        public ICommand AddBookingCommand { get; }
        public ICommand DeleteBookingCommand { get; }

        public RoomsViewModel()
        {
            Rooms = new ObservableCollection<Room>();
            Socios = new ObservableCollection<Socio>();
            Bookings = new ObservableCollection<RoomBooking>();

            AddRoomCommand = new RelayCommand(async _ => await AddRoom());
            DeleteRoomCommand = new RelayCommand(async _ => await DeleteRoom(), _ => SelectedRoom != null);
            AddBookingCommand = new RelayCommand(async _ => await AddBooking());
            DeleteBookingCommand = new RelayCommand(async _ => await DeleteBooking(), _ => SelectedBooking != null);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                using (var context = new SociosContext())
                {
                    context.Database.EnsureCreated();
                    var roomsList = await context.Rooms.ToListAsync();
                    var sociosList = await context.Socios.AsNoTracking().ToListAsync();
                    var bookingsList = await context.RoomBookings
                        .Include(b => b.Room)
                        .Include(b => b.Booker)
                        .OrderByDescending(b => b.BookingDate)
                        .ToListAsync();

                    Rooms.Clear();
                    foreach (var room in roomsList) Rooms.Add(room);

                    Socios.Clear();
                    foreach (var socio in sociosList) Socios.Add(socio);

                    Bookings.Clear();
                    foreach (var booking in bookingsList) Bookings.Add(booking);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddRoom()
        {
            if (string.IsNullOrWhiteSpace(RoomName) || !int.TryParse(RoomCapacity, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Please enter a valid room name and capacity.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newRoom = new Room { Name = RoomName, MaxCapacity = capacity };
                using (var context = new SociosContext())
                {
                    context.Rooms.Add(newRoom);
                    await context.SaveChangesAsync();
                }

                RoomName = string.Empty;
                RoomCapacity = string.Empty;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteRoom()
        {
            if (SelectedRoom == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedRoom.Name}'? This will also delete its bookings.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new SociosContext())
                    {
                        var roomToDelete = await context.Rooms.FindAsync(SelectedRoom.Id);
                        if (roomToDelete != null)
                        {
                            context.Rooms.Remove(roomToDelete);
                            await context.SaveChangesAsync();
                        }
                    }
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task AddBooking()
        {
            if (SelectedBookingRoom == null || SelectedBooker == null || !int.TryParse(BookingDuration, out int duration) || duration <= 0)
            {
                MessageBox.Show("Please select a room, a booker, and enter a valid duration in minutes.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newBooking = new RoomBooking
                {
                    RoomId = SelectedBookingRoom.Id,
                    BookerId = SelectedBooker.Id,
                    BookingDate = DateTime.Now,
                    DurationMinutes = duration
                };

                using (var context = new SociosContext())
                {
                    context.RoomBookings.Add(newBooking);
                    await context.SaveChangesAsync();
                }

                SelectedBookingRoom = null;
                SelectedBooker = null;
                BookingDuration = string.Empty;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error booking room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteBooking()
        {
            if (SelectedBooking == null) return;

            var result = MessageBox.Show($"Cancel booking for '{SelectedBooking.Room.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new SociosContext())
                    {
                        var bookingToDelete = await context.RoomBookings.FindAsync(SelectedBooking.Id);
                        if (bookingToDelete != null)
                        {
                            context.RoomBookings.Remove(bookingToDelete);
                            await context.SaveChangesAsync();
                        }
                    }
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
