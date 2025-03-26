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
        private const int maxAttempts = 5; 
        private int attemptCount = 0;       
        private static DateTime? lockoutEndTime = null; 
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
            

            if (username.Length < 2 || username.Length > 10)
            {
                MessageBox.Show("Username must be between 2 and 10 characters.");
                return;
            }

            if (username.Contains(" "))
            {
                MessageBox.Show("Username must not contain spaces.");
                return;
            }

            if (!IsValidUsername(username))
            {
                MessageBox.Show("Username must not contain special characters.");
                return;
            }

            if (lockoutEndTime.HasValue && DateTime.Now < lockoutEndTime.Value)
            {
                TimeSpan remainingTime = lockoutEndTime.Value - DateTime.Now;
                MessageBox.Show($"Too many failed login attempts. Try again in {remainingTime.Minutes} minutes {remainingTime.Seconds} seconds.");
                return;
            }

            if (isRegisterMode)
            {
                string CitizenID = CitizenIDBox.Text;

                if (context.Accounts.Any(a => a.Username == username))
                {
                    MessageBox.Show("Username already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (CitizenID.Length != 12 || !CitizenID.All(char.IsDigit))
                {
                    MessageBox.Show("Invalid CitizenID! It must be exactly 12 digits.");
                    return;
                }


                var newAccount = new Account { Username = username, Password = password, Role = "customer" };
                context.Accounts.Add(newAccount);
                context.SaveChanges();

                var newCitizen = new Citizen { AccId = newAccount.Id, Id = CitizenID };
                context.Citizens.Add(newCitizen);
                context.SaveChanges();
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            

            var account = context.Accounts
                                 .FirstOrDefault(a => a.Username == username && a.Password == password);

            if (account != null)
            {
                attemptCount = 0; 
                lockoutEndTime = null; 

                UserSession.CurrentUser = account; 

                Homepage homepage = new Homepage(account);
                this.Close();
                homepage.Show();
            }
            else
            {
                attemptCount++; 

                if (attemptCount >= maxAttempts)
                {
                    lockoutEndTime = DateTime.Now.AddMinutes(1); 
                    MessageBox.Show("Too many failed login attempts. Your account is locked for 1 minutes.");
                }
                else
                {
                    MessageBox.Show($"Invalid username or password. Attempts left: {maxAttempts - attemptCount}");
                }
            }
        }
        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, "^[a-zA-Z0-9]+$");
        }
    }
}
