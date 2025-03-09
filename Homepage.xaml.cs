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
            //MessageBox.Show("Homepage opened!");
            account = acc;
            SetUserInterface(acc.Role);
        }

        private void SetUserInterface(string role)
        {
            role = role.Trim(); // Xóa khoảng trắng thừa ở đầu và cuối

            //MessageBox.Show($"Role after trim: '{role}'"); // Kiểm tra lại giá trị role
            if (role == "admin")
            {
                AdminPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
            }
            else if (role == "customer")
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

        //nút user information
        private void UserInfor_Click(object sender, RoutedEventArgs e)
        {
            UserInformation userInfo = new UserInformation(account);
            userInfo.Width = ContainerUser.ActualWidth;  // Gán kích thước theo container
            userInfo.Height = ContainerUser.ActualHeight;

            ContainerUser.Child = userInfo;
        }


        
        private void TrackingAllFile(object sender, RoutedEventArgs e)
        {
            ContainerUser.Child = new Timetable(); 
        }

        //nút Book
        private void BookLich_Click(object sender, RoutedEventArgs e)
        {
            // Tạo một thể hiện của BookLich
            BookLich bookLichControl = new BookLich();

            // Thiết lập kích thước theo kích thước của ContainerUser
            bookLichControl.Width = ContainerUser.ActualWidth;
            bookLichControl.Height = ContainerUser.ActualHeight;

            // Gán vào ContainerUser (giống cách bạn làm với UserInformation)
            ContainerUser.Child = bookLichControl;
        }
    }
}
