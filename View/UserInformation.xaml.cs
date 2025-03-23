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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Identity.Client;
using Project212.DAO;
using Project212.Models;

namespace Project212
{
    /// <summary>
    /// Interaction logic for UserInformation.xaml
    /// </summary>
    public partial class UserInformation : UserControl
    {
        private readonly CitizenDAO citizenDAO;
        private Citizen currentCitizen;
        private Account currentAccount;
        private readonly VehicleDAO vehicleDAO;

        public UserInformation()
        {
            InitializeComponent();
            citizenDAO = new CitizenDAO();
            vehicleDAO = new VehicleDAO();
        }

        public UserInformation(Account account) : this()
        {
            currentAccount = account;
            LoadUserInformation();
            LoadDataGridVehicle();
            LoadInforChinhSua();
        }

        private void LoadUserInformation()
        {
            if (currentAccount != null)
            {
                currentCitizen = citizenDAO.GetCitizenByAccountId(currentAccount.Id);

                if (currentCitizen != null)
                {
                    ten.Content = currentCitizen.Name ?? "Không có tên";
                    ngaysinh.Content = currentCitizen.Dob != null
                        ? currentCitizen.Dob.Value.ToString("dd/MM/yyyy")
                        : "Không có ngày sinh";
                    diachi.Content = currentCitizen.Address ?? "Không có địa chỉ";
                    phone.Content = currentCitizen.Phone?.ToString() ?? "Không có số điện thoại";
                    mail.Content = currentCitizen.Mail ?? "Không có email";
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng cho tài khoản này.");
                }
            }
        }

        private void LoadDataGridVehicle()
        {
            if (currentCitizen != null)
            {
                var vehicles = vehicleDAO.GetVehiclesByCitizenId(currentCitizen.Id);
                this.dgVehicles.ItemsSource = vehicles;
            }
        }

        private void LoadInforChinhSua()
        {
            if (currentAccount != null)
            {
                currentCitizen = citizenDAO.GetCitizenByAccountId(currentAccount.Id);
                if (currentCitizen != null)
                {
                    // Display citizen information in the labels
                    tbTen.Text = currentCitizen.Name;
                    dtNgaysinh.SelectedDate = currentCitizen.Dob.HasValue
    ? currentCitizen.Dob.Value.ToDateTime(TimeOnly.MinValue)
    : null;

                    tbDiachi.Text = currentCitizen.Address;
                    tbPhone.Text = currentCitizen.Phone.ToString();
                    tbMail.Text = currentCitizen.Mail;
                }
            }

        }

        private void btnChinhsua_Click(object sender, RoutedEventArgs e)
        {
            tbTen.IsEnabled = true;
            dtNgaysinh.IsEnabled = true;
            tbDiachi.IsEnabled = true;
            tbPhone.IsEnabled = true;
            tbMail.IsEnabled = true;

            if (currentCitizen != null)
            {
                btnUpdate.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = true;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input data
                if (string.IsNullOrWhiteSpace(tbTen.Text) ||
                    dtNgaysinh.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(tbDiachi.Text) ||
                    string.IsNullOrWhiteSpace(tbPhone.Text) ||
                    string.IsNullOrWhiteSpace(tbMail.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate phone number
                if (!int.TryParse(tbPhone.Text, out int phoneNumber))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create a new Citizen object
                Citizen newCitizen = new Citizen
                {
                    Name = tbTen.Text,
                    Dob = DateOnly.FromDateTime(dtNgaysinh.SelectedDate.Value),
                    Address = tbDiachi.Text,
                    Phone = phoneNumber,
                    Mail = tbMail.Text,
                    AccId = currentAccount.Id
                };

                using (Prn212AssignmentContext context = new Prn212AssignmentContext())
                {
                    context.Citizens.Add(newCitizen);
                    context.SaveChanges();

                    // Update the current citizen reference
                    currentCitizen = newCitizen;

                    MessageBox.Show("Thêm thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload user information
                    LoadUserInformation();

                    // Disable editing fields
                    DisableEditFields();

                    // Enable Update button instead of Add for future edits
                    btnAdd.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input data
                if (string.IsNullOrWhiteSpace(tbTen.Text) ||
                    dtNgaysinh.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(tbDiachi.Text) ||
                    string.IsNullOrWhiteSpace(tbPhone.Text) ||
                    string.IsNullOrWhiteSpace(tbMail.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate phone number
                if (!int.TryParse(tbPhone.Text, out int phoneNumber))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update citizen data
                currentCitizen.Name = tbTen.Text;
                currentCitizen.Dob = DateOnly.FromDateTime(dtNgaysinh.SelectedDate.Value);
                currentCitizen.Address = tbDiachi.Text;
                currentCitizen.Phone = phoneNumber;
                currentCitizen.Mail = tbMail.Text;

                bool result = citizenDAO.UpdateCitizen(currentCitizen);
                if (result)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload user information to display updated data
                    LoadUserInformation();

                    // Disable all fields after successful update
                    DisableEditFields();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DisableEditFields()
        {
            tbTen.IsEnabled = false;
            dtNgaysinh.IsEnabled = false;
            tbDiachi.IsEnabled = false;
            tbPhone.IsEnabled = false;
            tbMail.IsEnabled = false;
            btnUpdate.IsEnabled = false;
        }
    }
}
