using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Project212.DAO;
using Project212.Models;

namespace Project212
{
    public partial class PoliceView : UserControl
    {
        private RecordDAO recordDAO;

        public PoliceView()
        {
            InitializeComponent();
            recordDAO = new RecordDAO();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                List<Record> records = recordDAO.GetAllRecords();
                RecordDataGrid.ItemsSource = records;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RecordDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordDataGrid.SelectedItem is Record selectedRecord)
            {
                txtID.Text = selectedRecord.Id.ToString();
                txtLicensePlate.Text = selectedRecord.Vehicle.Plate;
                txtVehicleType.Text = selectedRecord.Vehicle.Engine;
                txtOwner.Text = selectedRecord.Vehicle.Citizen.Name;
            }
        }
    }
}
