using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.EntityFrameworkCore;
using Project212.DAO;
using Project212.Models;

namespace Project212
{
    /// <summary>
    /// Interaction logic for BookLich.xaml
    /// </summary>
    public partial class BookLich : UserControl
    {
        private readonly TimetableDAO _timetableDAO;
        private readonly Prn212AssignmentContext _context;
        private DateTime _selectedInspectTime;
        private int _selectedStationId; 
        private int _currentUserId;
        private Vehicle _vehicle;
        private Citizen currentCitizen;
        private readonly VehicleDAO vehicleDAO;
        public BookLich()
        {
            _context = new Prn212AssignmentContext();
            _currentUserId = UserSession.CurrentUser?.Id ?? -1;
            _timetableDAO = new TimetableDAO(_context);
            vehicleDAO = new VehicleDAO();
            _vehicle = new Vehicle();
            InitializeComponent();
            LoadComboboxRoles();
            LoadTimetableHistory();
            LoadComboboxCoso();
            FilterTimetableHistory();
            LoadVehicle();
        }

        //Load cb trạm
        void LoadComboboxRoles()
        {
            var station = CosoDAO.GetInspectionStations();
            if (station != null && station.Count > 0)
            {
                cbRoles1.ItemsSource = station;
                cbRoles1.DisplayMemberPath = "Name"; 
                cbRoles1.SelectedValuePath = "Id";   
                cbRoles1.SelectedIndex = 0;          
            }
            else
            {
                cbRoles1.ItemsSource = null; 
            }
        }
        //load cb xe
        void LoadVehicle()
        {
            currentCitizen = _context.Citizens.FirstOrDefault(c => c.AccId == _currentUserId);
            

            var vehicle = vehicleDAO.GetVehiclesByCitizenId(currentCitizen.Id);
            if (vehicle != null && vehicle.Count > 0)
            {
                cbVehicle.ItemsSource = vehicle;
                cbVehicle.DisplayMemberPath = "Model";
                cbVehicle.SelectedValuePath = "Id";
                cbVehicle.SelectedIndex = 0;
            }
            else
            {
                cbVehicle.ItemsSource = null; 
            }
        }

        //load filter trạm
        void LoadComboboxCoso()
        {
            var stations = CosoDAO.GetInspectionStations();
            if (stations != null && stations.Count > 0)
            {
                
                var allStations = new List<dynamic>();

                // thêm "tất cả" có id= -1, tránh xung đột id
                allStations.Add(new { Id = -1, Name = "Tất cả" });

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


        // nút "ĐẶT LỊCH"
        private void btnDatlich_Click(object sender, RoutedEventArgs e)
        {
            if (cbRoles1.SelectedValue == null || dpThoigian.SelectedDate == null || cbHours.SelectedItem == null || cbVehicle.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn xe của bạn, cơ sở, ngày và giờ kiểm định!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int vehicleId = (int)cbVehicle.SelectedValue;
            int inspectionId = (int)cbRoles1.SelectedValue;
            DateTime gioihan = dpThoigian.SelectedDate.Value;       // ngày đc chọn

            if (gioihan < DateTime.Today)
            {
                MessageBox.Show("Không thể đặt lịch trong quá khứ. Vui lòng chọn ngày hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool hasVehicle = _context.Vehicles.Any(v => v.Id == _currentUserId);

            if (!hasVehicle)
            {
                MessageBox.Show("Bạn chưa có xe, vui lòng thêm xe trước khi đặt lịch!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime selectedDate = dpThoigian.SelectedDate.Value;
            int selectedHour = int.Parse(((ComboBoxItem)cbHours.SelectedItem).Content.ToString());
            DateTime inspectTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedHour, 0, 0);
          

            bool result = _timetableDAO.AddTimetable(inspectionId, _currentUserId, inspectTime, vehicleId);

            if (result)
            {
                MessageBox.Show("Đặt lịch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadTimetableHistory();
            }
            else
            {
                MessageBox.Show("Đã có người khác đặt trùng giờ lịch với bạn. Vui lòng chọn giờ khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //reload dữ liệu mới nhất
        private void LoadTimetableHistory()
        {
            int userId = UserSession.CurrentUser?.Id ?? -1;

            if (_currentUserId != userId)
            {
                _currentUserId = userId;
            }

            dgVehiclesLichsu.ItemsSource = _context.Timetables
                .Include(t => t.Vehicle) 
                .Where(t => t.AccId == _currentUserId)
                .Select(t => new
                {
                    StationID = t.InspectionId,
                    StationName = t.Inspection.Name,
                    AppointmentDate = t.InspectTime,
                    Status = t.Status,
                    VehicleModel = t.Vehicle != null ? t.Vehicle.Model : "Không xác định"
                }).ToList();
        }
        private void FilterTimetableHistory()
        {
            try
            {
                // Get the current user ID
                int userId = UserSession.CurrentUser?.Id ?? -1;

                var query = _context.Timetables.Where(t => t.AccId == _currentUserId);

                if (cbCoso.SelectedValue != null && cbCoso.SelectedIndex > 0) // Assuming index 0 is "All"
                {
                    int selectedStationId = (int)cbCoso.SelectedValue;
                    query = query.Where(t => t.InspectionId == selectedStationId);
                }

                if (txtSearch.SelectedDate != null)
                {
                    DateTime selectedDate = txtSearch.SelectedDate.Value.Date;
                    query = query.Where(t => t.InspectTime.Date == selectedDate);
                }

                dgVehiclesLichsu.ItemsSource = query
                    .Select(t => new
                    {
                        StationID = t.InspectionId,
                        StationName = t.Inspection.Name,
                        AppointmentDate = t.InspectTime,
                        Status = t.Status,
                        VehicleModel = t.Vehicle != null ? t.Vehicle.Model : "Không xác định"
                    }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //check cancel lịch
        private void dgVehiclesLichsu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVehiclesLichsu.SelectedItem != null)
            {
                var selectedRow = (dynamic)dgVehiclesLichsu.SelectedItem;

                _selectedInspectTime = selectedRow.AppointmentDate;
                _selectedStationId = Convert.ToInt32(selectedRow.StationID);
                string selectedStatus = selectedRow.Status;

                cbHours.SelectedItem = cbHours.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == _selectedInspectTime.Hour.ToString("00"));

                btnHuylich.IsEnabled = !(selectedStatus.Trim() == "Đã hủy" || selectedStatus.Trim() == "Đã Duyệt");

            }
            else
            {
                btnHuylich.IsEnabled = false;
            }
        }


        //nút "HỦY LỊCH"
        private void btnHuylich_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedInspectTime == default || _selectedStationId == 0)
            {
                MessageBox.Show("Vui lòng chọn một lịch cần hủy!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult confirmResult = MessageBox.Show(
                "Bạn có chắc chắn muốn hủy lịch này không?",
                "Xác nhận hủy lịch",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult == MessageBoxResult.No)
            {
                return;
            }

            bool result = _timetableDAO.CancelTimetable(_selectedStationId, _currentUserId, _selectedInspectTime);

            if (result)
            {
                MessageBox.Show("Hủy lịch thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadTimetableHistory(); // Cập nhật lại danh sách
            }
            else
            {
                MessageBox.Show("Hủy lịch thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbCoso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTimetableHistory();
        }
        private void txtSearch_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTimetableHistory();
        }

    }
}
