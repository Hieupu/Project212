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
            role = role.Trim(); 

            if (role == "admin")
            {
                AdminPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
                PolicePanel.Visibility = Visibility.Collapsed;
            }
            else if (role.Trim() == "customer")
            {
                UserPanel.Visibility = Visibility.Visible;
                AdminPanel.Visibility = Visibility.Collapsed;
                PolicePanel.Visibility = Visibility.Collapsed;
            }
            else if (role.Trim() == "police")
            {
                UserPanel.Visibility = Visibility.Collapsed;
                AdminPanel.Visibility = Visibility.Collapsed;
                PolicePanel.Visibility = Visibility.Visible;
            }
            else if (role.Trim() == "receptionist")
            {
                UserPanel.Visibility = Visibility.Visible;
                AdminPanel.Visibility = Visibility.Collapsed;
                PolicePanel.Visibility = Visibility.Collapsed;
            }
            else if (role.Trim() == "engineer")
            {
                UserPanel.Visibility = Visibility.Visible;
                AdminPanel.Visibility = Visibility.Collapsed;
                PolicePanel.Visibility = Visibility.Collapsed;
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

        private void UserInfor_Click(object sender, RoutedEventArgs e)
        {
            UserInformation userInfo = new UserInformation(account);
            userInfo.Width = ContainerUser.ActualWidth; 
            userInfo.Height = ContainerUser.ActualHeight;

            ContainerUser.Child = userInfo;
        }

        private void ManageTimeable_Click(object sender, RoutedEventArgs e)
        {
            Timetable timetableControl = new Timetable();
            timetableControl.Width = ContainerAdmin.ActualWidth;
            timetableControl.Height = ContainerAdmin.ActualHeight;
            ContainerAdmin.Child = timetableControl; 
        }

        private void ViewVehicleList_Click(object sender, RoutedEventArgs e)
        {
            PoliceView policeView = new PoliceView();
            policeView.Width = ContainerPolice.ActualWidth;
            policeView.Height = ContainerPolice.ActualHeight;

            ContainerPolice.Child = policeView;
        }
        
        private void BookLich_Click(object sender, RoutedEventArgs e)
        {
            BookLich bookLichControl = new BookLich();
            bookLichControl.Width = ContainerUser.ActualWidth;
            bookLichControl.Height = ContainerUser.ActualHeight;

            ContainerUser.Child = bookLichControl;
        }
    }
}
