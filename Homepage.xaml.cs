using Project212.Models;
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
using System.Windows.Shapes;

namespace Project212
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : Window
    {
        private Account account;
        public Homepage(Account acc)
        {
            InitializeComponent();
            account = acc;
            SetUserInterface(acc.Role);
        }

        private void SetUserInterface(string role)
        {
            if (role.Trim() == "admin")
            {
                AdminPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
            }
            else if (role.Trim() == "customer")
            {
                UserPanel.Visibility = Visibility.Visible;
                AdminPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Invalid role detected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

            private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ButtonUSB_Click(object sender, RoutedEventArgs e)
        {
            ContainerUser.Child = new Timetable();
        }

        private void TrackingAllFile(object sender, RoutedEventArgs e)
        {
            ContainerUser.Child = new UserInformation();
        }
    }
}
