using Microsoft.Identity.Client;
using Project212.DAO;
using Project212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;

namespace Project212
{
    /// <summary>
    /// Interaction logic for AccountManagement.xaml
    /// </summary>
    public partial class AccountManagement : UserControl
    {
        private AccountDAO accountDAO;
        public AccountManagement()
        {
            InitializeComponent();
            accountDAO = new AccountDAO();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var accounts = accountDAO.GetAllAccountsWithCitizens();
                UserList.ItemsSource = accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        public void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList.SelectedItem is Account selectedAccount)
            {
                Username.Text = selectedAccount.Username ?? "Trống";
                Password.Text = selectedAccount.Password ?? "Trống";

                var firstCitizen = selectedAccount.Citizens?.FirstOrDefault();

                Email.Text = firstCitizen?.Mail ?? "Chưa điền mail";
                Name.Text = firstCitizen?.Name ?? "Trống";
                CitizenId.Text = firstCitizen?.Id;

                DoB.Text = firstCitizen?.Dob != null
                    ? firstCitizen.Dob.Value.ToString("dd/MM/yyyy")
                    : "Trống";
                foreach (ComboBoxItem item in Role.Items)
                {
                    string itemRole = item.Content?.ToString()?.ToLower();
                    string selectedRole = selectedAccount.Role?.Trim().ToLower();

                    if (itemRole == selectedRole)
                    {
                        Role.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                DoB.Text = string.Empty;
                Name.Text = string.Empty;
                CitizenId.Text = string.Empty;
                Password.Text = string.Empty;
            }
        }

        private void UpdateRole_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem is Account selectedAccount)
            {
                if (Role.SelectedItem is ComboBoxItem selectedRoleItem)
                {
                    string newRole = selectedRoleItem.Content?.ToString().ToLower();

                    selectedAccount.Role = newRole;
                    accountDAO.UpdateAccount(selectedAccount);
                    LoadData();
                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn vai trò!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người dùng!");
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCitizenId.Text))
            {
                MessageBox.Show("Vui lòng nhập số CCCD.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ComboboxRole.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn vai trò.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                string citizenId = txtCitizenId.Text; 
                string role = ((ComboBoxItem)ComboboxRole.SelectedItem).Content.ToString().ToLower();

                var newCitizen = new Citizen
                {
                    Id = citizenId,
                };

                var newAccount = new Account
                {
                    Username = username,
                    Password = password,
                    Role = role,
                    Citizens = new List<Citizen> { newCitizen } 
                };

                accountDAO.AddAccount(newAccount);

                LoadData();
                ClearInputFields();

                MessageBox.Show("Thêm người dùng và công dân thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("CCCD phải là số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputFields()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtCitizenId.Text = "";
            ComboboxRole.SelectedItem = null;
        }
    }
}

