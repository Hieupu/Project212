using Microsoft.EntityFrameworkCore;
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
            LoadComboboxCoso();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                int selectedStationId = cbCoso.SelectedValue is int id ? id : -1;

                var query = _context.Timetables.AsQueryable();

                if (selectedStationId > 0)
                {
                    query = query.Where(b => b.InspectionId == selectedStationId);
                }

                var rs = (from b in _context.Timetables
                          join c in _context.Citizens on b.AccId equals c.AccId into citizenGroup
                          from citizen in citizenGroup.DefaultIfEmpty() 
                          where selectedStationId <= 0 || b.InspectionId == selectedStationId
                          select new BookingViewModel
                          {
                              Id = b.Id,
                              StationName = b.Inspection.Name,
                              User = citizen != null ? citizen.Name : "Không xác định",
                              AppointmentDate = b.InspectTime.ToString("yyyy-MM-dd HH:mm"),
                              Status = b.Status
                          }).ToList();

                if (rs == null || !rs.Any())
                {
                    MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Bookings = new ObservableCollection<BookingViewModel>();
                }
                else
                {
                    Bookings = new ObservableCollection<BookingViewModel>(rs);
                }

                dgBookings.ItemsSource = Bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBookings.SelectedItem is BookingViewModel selectedBooking)
            {
                txtID.Text = selectedBooking.Id.ToString();
                if(selectedBooking.Status.Trim().Equals("Đã hủy"))
                {
                    cbStatus.SelectedItem = cbStatus.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == "Hủy");
                }
                else if(selectedBooking.Status.Trim().Equals("Đã duyệt"))
                {
                    cbStatus.SelectedItem = cbStatus.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == "Duyệt");
                }
                else if (selectedBooking.Status.Trim().Equals("Đang duyệt"))
                {
                    cbStatus.SelectedValue = "";
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtID.Text, out int bookingId))
            {
                string newStatus = "Đã " + (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString().ToLower();

                var bookingToUpdate = _context.Timetables.FirstOrDefault(b => b.Id == bookingId);
                if (bookingToUpdate != null)
                {
                    bookingToUpdate.Status = newStatus;
                    _context.SaveChanges();
                }

                var updatedBooking = Bookings.FirstOrDefault(b => b.Id == bookingId);
                if (updatedBooking != null)
                {
                    updatedBooking.Status = newStatus;
                    dgBookings.Items.Refresh();
                }

                MessageBox.Show("Trạng thái đã được cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        void LoadComboboxCoso()
        {
            var stations = CosoDAO.GetInspectionStations();
            if (stations != null && stations.Count > 0)
            {
                var allStations = new System.Collections.Generic.List<dynamic>
                {
                    new { Id = -1, Name = "Tất cả" }
                };

                allStations.AddRange(stations);

                cbCoso.ItemsSource = allStations;
                cbCoso.DisplayMemberPath = "Name";
                cbCoso.SelectedValuePath = "Id";
                cbCoso.SelectedIndex = 0;
            }
            else
            {
                cbCoso.ItemsSource = null;
            }
        }

        private void cbCoso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData(); 
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