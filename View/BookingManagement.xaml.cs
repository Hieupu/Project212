using Project212.DAO;
using Project212.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Project212.View
{
    public partial class BookingManagement : UserControl
    {
        private ObservableCollection<BookingViewModel> Bookings;
        private readonly Prn212AssignmentContext _context;


        public BookingManagement()
        {
            _context = new Prn212AssignmentContext();
            InitializeComponent();
            this.DataContext = new BookingViewModel();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgBookings.Items.Clear();
                dgBookings.SelectedItem = null;
                dgBookings.SelectedIndex = -1;

                var rs = _context.Timetables
                            .Select(b => new BookingViewModel
                            {
                                Id = b.Id,
                                StationName = _context.InspectionStations.FirstOrDefault(x => x.Id == b.InspectionId).Name,
                                User = _context.Citizens.FirstOrDefault(x => b.AccId.Equals(x.Id)).Name,
                                AppointmentDate = b.InspectTime.ToString("yyyy-MM-dd HH:mm"),
                                Status = b.Status
                            })
                            .ToList();

                if (rs == null || !rs.Any())
                {
                    MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Bookings = new ObservableCollection<BookingViewModel>(rs);
                dgBookings.ItemsSource = null;
                dgBookings.ItemsSource = Bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void dgBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBookings.SelectedItem is Models.Timetable selectedBooking)
            {
                txtID.Text = selectedBooking.Id.ToString();
                cbStatus.SelectedValue = selectedBooking.Status;
            }
        }

        // Save updates to database
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtID.Text, out int bookingId))
            {
                string newStatus = "Đã " + cbStatus.Text;

                // Find booking in database
                var bookingToUpdate = _context.Timetables.FirstOrDefault(b => b.Id == bookingId);
                if (bookingToUpdate != null)
                {
                    bookingToUpdate.Status = newStatus;
                    _context.SaveChanges(); // Save change to database
                }

                // Update display list
                var updatedBooking = Bookings.FirstOrDefault(b => b.Id == bookingId);
                if (updatedBooking != null)
                {
                    updatedBooking.Status = newStatus;
                    dgBookings.Items.Refresh(); // Refresh UI
                }

                MessageBox.Show("Trạng thái đã được cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public class BookingViewModel
        {
            public int Id { get; set; }
            public string StationName { get; set; }
            public string User { get; set; }
            public string AppointmentDate { get; set; }
            public string Status { get; set; }
        }
    }
}
