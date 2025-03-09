using System;
using System.Collections.Generic;
using System.Data;
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
using Project212.Models;

namespace Project212
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        readonly Prn212AssignmentContext context;
        public Login()
        {

            InitializeComponent();
            context = new Prn212AssignmentContext();
           
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var account = context.Accounts
                                 .FirstOrDefault(a => a.Username == username && a.Password == password);
            


            if (account != null)
            {
                Homepage homepage = new Homepage(account);
                homepage.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }
    }
}
