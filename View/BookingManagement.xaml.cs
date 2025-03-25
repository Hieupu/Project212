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
        private int _selectedStationId; //id của "TRẠM KIỂM ĐỊNH"
        private int _currentUserId;

        public BookingManagement()
        {
            _context = new Prn212AssignmentContext();
            InitializeComponent();
            this.DataContext = new BookingViewModel();
            //LoadComboboxCoso();
            //FilterTimetableHistory();
            LoadData();

        }

        //private void LoadData()
        //{
        //    try
        //    {
        //        dgBookings.Items.Clear();
        //        dgBookings.SelectedItem = null;
        //        dgBookings.SelectedIndex = -1;

        //        var rs = _context.Timetables
        //                    .Select(b => new BookingViewModel
        //                    {
        //                        Id = b.Id,
        //                        StationName = _context.InspectionStations.FirstOrDefault(x => x.Id == b.InspectionId).Name,
        //                        User = _context.Citizens.FirstOrDefault(x => b.AccId.ToString() == x.Id).Name,
        //                        AppointmentDate = b.InspectTime.ToString("yyyy-MM-dd HH:mm"),
        //                        Status = b.Status
        //                    })
        //                    .ToList();

        //        if (rs == null || !rs.Any())
        //        {
        //            MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }

        //        Bookings = new ObservableCollection<BookingViewModel>(rs);
        //        dgBookings.ItemsSource = null;
        //        dgBookings.ItemsSource = Bookings;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void LoadData()
        {
            try
            {
                var rs = _context.Timetables
                            .Select(b => new BookingViewModel
                            {
                                Id = b.Id,
                                StationName = _context.InspectionStations.FirstOrDefault(x => x.Id == b.InspectionId).Name,
                                User = _context.Citizens.FirstOrDefault(x => b.AccId.ToString() == x.Id).Name,
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
                    _context.SaveChanges();
                }

                // Update display list
                var updatedBooking = Bookings.FirstOrDefault(b => b.Id == bookingId);
                if (updatedBooking != null)
                {
                    updatedBooking.Status = newStatus;
                    dgBookings.Items.Refresh();
                }

                MessageBox.Show("Trạng thái đã được cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //void LoadComboboxCoso()
        //{
        //    var stations = CosoDAO.GetInspectionStations();
        //    if (stations != null && stations.Count > 0)
        //    {
        //        // Create a new list that includes an "All" option at the beginning
        //        var allStations = new List<dynamic>();

        //        // Add an "All" option with ID -1 (or any value that doesn't conflict with your actual IDs)
        //        allStations.Add(new { Id = -1, Name = "Tất cả" });

        //        // Add all the actual stations
        //        allStations.AddRange(stations);

        //        cbCoso.ItemsSource = allStations;
        //        cbCoso.DisplayMemberPath = "Name";
        //        cbCoso.SelectedValuePath = "Id";
        //        cbCoso.SelectedIndex = 0; // Select "All" by default
        //    }
        //    else
        //    {
        //        cbCoso.ItemsSource = null;
        //    }
        //}

        //private void FilterTimetableHistory()
        //{
        //    try
        //    {
        //        // Get the current user ID
        //        int userId = UserSession.CurrentUser?.Id ?? -1;
        //        if (userId == -1)
        //        {
        //            MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        // Start with the base query for the current user
        //        var query = _context.Timetables.Where(t => t.AccId == userId);

        //        // Apply station filter if selected (and not "All")
        //        if (cbCoso.SelectedValue is int selectedStationId && selectedStationId > 0) // -1 là "Tất cả"
        //        {
        //            query = query.Where(t => t.InspectionId == selectedStationId);
        //        }

        //        // Execute query and update DataGrid
        //        var filteredResults = query
        //            .Select(t => new BookingViewModel
        //            {
        //                Id = t.Id,
        //                StationName = t.Inspection.Name,
        //                User = _context.Citizens.FirstOrDefault(c => c.Id == t.AccId.ToString()).Name,
        //                AppointmentDate = t.InspectTime.ToString("yyyy-MM-dd HH:mm"),
        //                Status = t.Status
        //            })
        //            .ToList();

        //        // Update the ObservableCollection
        //        Bookings = new ObservableCollection<BookingViewModel>(filteredResults);
        //        dgBookings.ItemsSource = Bookings;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void FilterTimetableHistory()
        //{
        //    try
        //    {
        //        int userId = UserSession.CurrentUser?.Id ?? -1;
        //        if (userId == -1)
        //        {
        //            MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        var query = _context.Timetables.Where(t => t.AccId == userId);

        //        if (cbCoso.SelectedValue is int selectedStationId && selectedStationId > 0)
        //        {
        //            query = query.Where(t => t.InspectionId == selectedStationId);
        //        }

        //        var filteredResults = query
        //            .Select(t => new BookingViewModel
        //            {
        //                Id = t.Id,
        //                StationName = t.Inspection.Name,
        //                User = _context.Citizens.FirstOrDefault(c => c.Id == t.AccId.ToString()).Name,
        //                AppointmentDate = t.InspectTime.ToString("yyyy-MM-dd HH:mm"),
        //                Status = t.Status
        //            })
        //            .ToList();

        //        Bookings = new ObservableCollection<BookingViewModel>(filteredResults);
        //        dgBookings.ItemsSource = Bookings;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void cbCoso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //FilterTimetableHistory();
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
