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
        private int _selectedStationId; //id của "TRẠM KIỂM ĐỊNH"
        private int _currentUserId;
        public BookLich()
        {
            _context = new Prn212AssignmentContext();
            _currentUserId = UserSession.CurrentUser?.Id ?? -1;
            MessageBox.Show($"Current User ID: {_currentUserId}");
            _timetableDAO = new TimetableDAO(_context);
            InitializeComponent();
            LoadComboboxRoles();
            LoadTimetableHistory();
        }

        //Load dữ liệu "TRẠM KIỂM ĐỊNH"
        void LoadComboboxRoles()
        {
            var station = CosoDAO.GetInspectionStations();
            if (station != null && station.Count > 0)
            {
                cbRoles1.ItemsSource = station;
                cbRoles1.DisplayMemberPath = "Name"; // Hiển thị tên trạm
                cbRoles1.SelectedValuePath = "Id";   // Giá trị thực nhận là Id
                cbRoles1.SelectedIndex = 0;          // Chọn phần tử đầu tiên
            }
            else
            {
                cbRoles1.ItemsSource = null; // Tránh lỗi khi danh sách rỗng
            }
        }

        // nút "ĐẶT LỊCH"
        private void btnDatlich_Click(object sender, RoutedEventArgs e)
        {
            if (cbRoles1.SelectedValue == null || dpThoigian.SelectedDate == null || cbHours.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn cơ sở, ngày và giờ kiểm định!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int inspectionId = (int)cbRoles1.SelectedValue;
            DateTime gioihan = dpThoigian.SelectedDate.Value;

            // Kiểm tra nếu ngày được chọn nhỏ hơn ngày hiện tại
            if (gioihan < DateTime.Today)
            {
                MessageBox.Show("Không thể đặt lịch trong quá khứ. Vui lòng chọn ngày hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime selectedDate = dpThoigian.SelectedDate.Value;
            int selectedHour = int.Parse(((ComboBoxItem)cbHours.SelectedItem).Content.ToString());
            DateTime inspectTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, selectedHour, 0, 0);

            bool result = _timetableDAO.AddTimetable(inspectionId, _currentUserId, inspectTime);

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
            // Debug to confirm the current user ID
            int userId = UserSession.CurrentUser?.Id ?? -1;

            // Check if current user ID matches what we expect
            if (_currentUserId != userId)
            {
                // Update current user ID if it doesn't match
                _currentUserId = userId;
                //MessageBox.Show($"User ID mismatch corrected. Current ID: {_currentUserId}");
            }

            // Check if there are any timetables for this user at all
            var allTimetables = _context.Timetables.Where(t => t.AccId == _currentUserId).ToList();

            if (allTimetables.Count == 0)
            {
                // No timetables found for this user
                MessageBox.Show($"Không tìm thấy lịch trình cho ID người dùng: {_currentUserId}");
                dgVehiclesLichsu.ItemsSource = null; // Clear the data grid
                return;
            }

            // Proceed with loading timetables as before
            dgVehiclesLichsu.ItemsSource = _context.Timetables
                .Where(t => t.AccId == _currentUserId)
                .Select(t => new
                {
                    StationID = t.InspectionId,
                    StationName = t.Inspection.Name,
                    AppointmentDate = t.InspectTime,
                    Status = t.Status
                }).ToList();
        }

        

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

                btnHuylich.IsEnabled = !(selectedStatus.Trim() == "Đã hủy" || selectedStatus.Trim() == "Đã duyệt");

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

       
    }
}
