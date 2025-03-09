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
        }

        private void LoadUserInformation()
        {
            if (currentAccount != null)
            {
                currentCitizen = citizenDAO.GetCitizenByAccountId(currentAccount.Id);

                if (currentCitizen != null)
                {
                    // Display citizen information in the labels
                    ten.Content = currentCitizen.Name;
                    ngaysinh.Content = currentCitizen.Dob.ToString("dd/MM/yyyy");
                    diachi.Content = currentCitizen.Address;
                    phone.Content = currentCitizen.Phone.ToString();
                    mail.Content = currentCitizen.Mail;
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
    }
}
