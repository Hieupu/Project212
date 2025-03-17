using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using Project212.Models;

namespace Project212
{
    public partial class Login : Window
    {
        readonly Prn212AssignmentContext context;
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
            ConfirmPasswordLabel.Visibility = isRegisterMode ? Visibility.Visible : Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = isRegisterMode ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (isRegisterMode)
            {
                string confirmPassword = ConfirmPasswordBox.Password;
                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (context.Accounts.Any(a => a.Username == username))
                {
                    MessageBox.Show("Username already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newAccount = new Account { Username = username, Password = password, Role = "customer" };
                context.Accounts.Add(newAccount);
                context.SaveChanges();

                var newCitizen = new Citizen { AccId = newAccount.Id };
                context.Citizens.Add(newCitizen);
                context.SaveChanges();
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var account = context.Accounts.FirstOrDefault(a => a.Username == username && a.Password == password);
                if (account != null)
                {
                    Homepage homepage = new Homepage(account);
                    this.Close();
                    homepage.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
