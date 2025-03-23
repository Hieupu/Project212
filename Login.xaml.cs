using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using Project212.Models;

namespace Project212
{
    public partial class Login : Window
    {
        readonly Prn212AssignmentContext context;
        private const int maxAttempts = 5;  // Hằng số số lần nhập sai tối đa
        private int attemptCount = 0;       // Biến đếm số lần nhập sai
        private static DateTime? lockoutEndTime = null; // Biến lưu thời gian mở khóa
        private bool isRegisterMode = false;

        public Login()
        {
            InitializeComponent();
            context = new Prn212AssignmentContext();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isRegisterMode = !isRegisterMode;
            TitleText.Text = isRegisterMode ? "Register" : "Login";
            AuthButton.Content = isRegisterMode ? "Register" : "Login";
            ToggleButton.Content = isRegisterMode ? "Switch to Login" : "Switch to Register";
            CitizenIDLabel.Visibility = isRegisterMode ? Visibility.Visible : Visibility.Collapsed;
            CitizenIDBox.Visibility = isRegisterMode ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (isRegisterMode)
            {
                string CitizenID = CitizenIDBox.Text;
                

                if (context.Accounts.Any(a => a.Username == username))
                {
                    MessageBox.Show("Username already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newAccount = new Account { Username = username, Password = password, Role = "customer" };
                context.Accounts.Add(newAccount);
                context.SaveChanges();

                var newCitizen = new Citizen { AccId = newAccount.Id, Id = CitizenID };
                context.Citizens.Add(newCitizen);
                context.SaveChanges();
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Kiểm tra username có độ dài hợp lệ không (từ 2 đến 10 ký tự)
            if (username.Length < 2 || username.Length > 10)
            {
                MessageBox.Show("Username must be between 2 and 10 characters.");
                return;
            }

            //check khoang trắng username
            if (username.Contains(" "))
            {
                MessageBox.Show("Username must not contain spaces.");
                return;
            }

            // Kiểm tra username có chứa ký tự đặc biệt không
            if (!IsValidUsername(username))
            {
                MessageBox.Show("Username must not contain special characters.");
                return;
            }

            // Kiểm tra nếu tài khoản đang bị khóa
            if (lockoutEndTime.HasValue && DateTime.Now < lockoutEndTime.Value)
            {
                TimeSpan remainingTime = lockoutEndTime.Value - DateTime.Now;
                MessageBox.Show($"Too many failed login attempts. Try again in {remainingTime.Minutes} minutes {remainingTime.Seconds} seconds.");
                return;
            }

            var account = context.Accounts
                                 .FirstOrDefault(a => a.Username == username && a.Password == password);

            if (account != null)
            {
                attemptCount = 0; // Reset số lần nhập sai nếu đăng nhập thành công
                lockoutEndTime = null; // Hủy bỏ khóa nếu có

                UserSession.CurrentUser = account;      //lưu tk trên session

                var newAccount = new Account { Username = username, Password = password, Role = "customer" };
                context.Accounts.Add(newAccount);
                context.SaveChanges();

                Homepage homepage = new Homepage(account);
                this.Close();
                homepage.Show();
                
            }
            else
            {
                attemptCount++; // Tăng số lần nhập sai

                if (attemptCount >= maxAttempts)
                {
                    lockoutEndTime = DateTime.Now.AddMinutes(1); // Khóa trong 5 phút
                    MessageBox.Show("Too many failed login attempts. Your account is locked for 5 minutes.");
                }
                else
                {
                    MessageBox.Show($"Invalid username or password. Attempts left: {maxAttempts - attemptCount}");
                }
            }
        }

        // Hàm kiểm tra username không chứa ký tự đặc biệt
        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, "^[a-zA-Z0-9]+$");
        }
    }
}
