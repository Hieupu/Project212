using Microsoft.EntityFrameworkCore;
using Project212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Project212.View.BookingManagement;

namespace Project212.View
{
    /// <summary>
    /// Interaction logic for RecordManagement.xaml
    /// </summary>
    public partial class RecordManagement : UserControl
    {
        private readonly Prn212AssignmentContext _context;
        public RecordManagement()
        {
            _context = new Prn212AssignmentContext();
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {

                var rs = _context.Timetables.Where(x => x.Status.Trim().ToLower().Equals("đã duyệt"))
                .Select(b => new RecordViewModel
                {
                    Id = b.Id,
                    StationName = _context.InspectionStations.FirstOrDefault(x => x.Id == b.InspectionId).Name,
                    User = _context.Citizens.FirstOrDefault(x => b.AccId.Equals(x.Id)).Name,
                    AppointmentDate = b.InspectTime.ToString("yyyy-MM-dd HH:mm"),
                    Status = b.Status,
                    RecordStatus = _context.Records.Any(x => x.TimeId == b.Id)
    ? (_context.Records.FirstOrDefault(x => x.TimeId == b.Id).Result == false ? "Chưa đạt" : "Đạt")
    : "Chưa kiểm tra"
                })
                            .ToList();

                if (rs == null || !rs.Any())
                {
                    MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                lsProduct.ItemsSource = null;
                lsProduct.ItemsSource = rs;

                var type = _context.Standards.ToList();
                cbType.ItemsSource = null;
                cbType.ItemsSource = type;
                cbType.DisplayMemberPath = "Id";
                cbType.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (
                    string.IsNullOrWhiteSpace(txtVihicle.Text) ||
                    string.IsNullOrWhiteSpace(txtCo.Text) ||
                    string.IsNullOrWhiteSpace(txtHc.Text) ||
                    string.IsNullOrWhiteSpace(txtNOx.Text) ||
                    cbType.SelectedItem == null)
                {
                    MessageBox.Show("All fields must be filled.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Parse and validate the input

                if (!double.TryParse(txtCo.Text, out double co) || !double.TryParse(txtHc.Text, out double hc) || !double.TryParse(txtNOx.Text, out double nox))
                {
                    MessageBox.Show("Invalid numeric values for CO, HC, or NOx.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var vehi = _context.Vehicles.FirstOrDefault(x => x.Id == int.Parse(txtVihicle.Text));

                if (vehi == null)
                {
                    MessageBox.Show("Invalid Vehicle ID. Please enter a valid number for Vehicle ID.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get the selected vehicle type (assuming it's an integer)
                var selectedVehicleType = (int)cbType.SelectedValue;
                var time = int.Parse(txtTimeId.Text);
                // Create the new record object
                var newRecord = new Record()
                {
                    VehicleId = int.Parse(txtVihicle.Text),
                    Co = co,
                    Hc = hc,
                    Nox = nox,
                    Note = txtNote.Text,
                    StandardId = selectedVehicleType,
                    TimeId = int.Parse(txtTimeId.Text),
                    Result = true,
                };
                var standard = _context.Standards.FirstOrDefault(s => s.Id == newRecord.StandardId);
                if (standard == null)
                {
                    MessageBox.Show("Standard not found.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Compare the values and set Result to 0 if the Record's values are higher
                if (newRecord.Co > standard.Co || newRecord.Hc > standard.Hc || newRecord.Nox > standard.Nox)
                {
                    newRecord.Result = false;  // The record's value is higher than the standard's value
                }

                // Save the new record to the database (or ObservableCollection)
                _context.Records.Add(newRecord);  // Assuming _context is your DbContext
                _context.SaveChanges();

                // Optionally, update the ListView
                LoadData(); // Load data method to refresh the ListView
                SentNotice(newRecord.Result, time);
                // Clear the form fields
                txtVihicle.Clear();
                txtCo.Clear();
                txtHc.Clear();
                txtNOx.Clear();
                txtNote.Clear();
                cbType.SelectedIndex = -1; // Reset ComboBox
                LoadData();

                MessageBox.Show("Record added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void SentNotice(bool result, int time)
        {
            try
            {
                var acc = _context.Timetables.FirstOrDefault(x => x.Id == time);
                int newId = _context.Notices.OrderByDescending(n => n.Id).FirstOrDefault()?.Id + 1 ?? 1;
                var notice = new Notice()
                {
                    Id = newId,
                    AccId = acc.AccId,
                    Detail = result ? "Đạt" : "Chưa đạt",
                    SentDate = DateTime.Now,
                    IsRead = false,
                };
                _context.Notices.Add(notice);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class RecordViewModel
        {
            public int Id { get; set; }
            public string StationName { get; set; }
            public string User { get; set; }
            public string AppointmentDate { get; set; }
            public string Status { get; set; }
            public string RecordStatus { get; set; }
        }
    }
}
