using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Project212.DAO;
using Project212.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Project212
{
    /// <summary>
    /// Interaction logic for BookingHistory.xaml
    /// </summary>
    public partial class BookingHistory : UserControl
    {
        private TimetableDAO timetableDAO;
        public BookingHistory()
        {
            InitializeComponent();
            timetableDAO = new TimetableDAO();
            LoadBookingData();
        }

        private void UpdateOrderCounts(List<Timetable> bookingList)
        {
            txtPassedOrders.Text = bookingList.SelectMany(b => b.Records)
                                              .Count(r => r.ResultText.Trim() == "Đạt")
                                              .ToString();

            txtNotPassedOrders.Text = bookingList.SelectMany(b => b.Records)
                                                 .Count(r => r.ResultText.Trim() == "Không đạt")
                                                 .ToString();

            txtPendingOrders.Text = bookingList.Count(b => b.Status.Trim() == "Đang duyệt").ToString();
            txtApprovedOrders.Text = bookingList.Count(b => b.Status.Trim() == "Đã Duyệt").ToString();
            txtCanceledOrders.Text = bookingList.Count(b => b.Status.Trim() == "Đã hủy").ToString();
        }

        private void FilterBookings(object sender, RoutedEventArgs e)
        {
            List<Timetable> bookingList = timetableDAO.GetAllTimetables();
            if (dpFrom.SelectedDate == null || dpTo.SelectedDate == null)
            {
                MessageBox.Show("Can not be null", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime fromDate = dpFrom.SelectedDate.Value;
            DateTime toDate = dpTo.SelectedDate.Value;

            if (fromDate > toDate)
            {
                MessageBox.Show("From date can't greater than To date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filteredList = bookingList.FindAll(b => b.InspectTime >= fromDate && b.InspectTime <= toDate);
            dgBookingHistory.ItemsSource = filteredList;
            UpdateOrderCounts(filteredList);
        }

        private void LoadBookingData()
        {
            List<Timetable> bookingList = timetableDAO.GetAllTimetables();
            dgBookingHistory.ItemsSource = bookingList;
            UpdateOrderCounts(bookingList);
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            string filePath = "TimetableReport.xlsx";
            List<Timetable> timetables = timetableDAO.GetAllTimetables();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Timetable Report");

                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Customer";
                worksheet.Cells["C1"].Value = "Vehicle";
                worksheet.Cells["D1"].Value = "Station";
                worksheet.Cells["E1"].Value = "Time";
                worksheet.Cells["F1"].Value = "Status";
                worksheet.Cells["G1"].Value = "CO";
                worksheet.Cells["H1"].Value = "HC";
                worksheet.Cells["I1"].Value = "NOx";
                worksheet.Cells["J1"].Value = "Result";

                using (var range = worksheet.Cells["A1:J1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 2; 

                foreach (var timetable in timetables)
                {
                    worksheet.Cells[row, 1].Value = timetable.Id;
                    worksheet.Cells[row, 2].Value = timetable.Acc?.Citizens.FirstOrDefault()?.Name ?? "N/A";
                    worksheet.Cells[row, 3].Value = timetable.Vehicle?.Plate ?? timetable.Records.FirstOrDefault()?.Vehicle?.Plate ?? "N/A";
                    worksheet.Cells[row, 4].Value = timetable.Inspection?.Name ?? "N/A";
                    worksheet.Cells[row, 5].Value = timetable.InspectTime.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 6].Value = timetable.Status;
                    worksheet.Cells[row, 7].Value = timetable.Records.FirstOrDefault()?.Co.ToString() ?? "N/A";
                    worksheet.Cells[row, 8].Value = timetable.Records.FirstOrDefault()?.Hc.ToString() ?? "N/A";
                    worksheet.Cells[row, 9].Value = timetable.Records.FirstOrDefault()?.Nox.ToString() ?? "N/A";
                    worksheet.Cells[row, 10].Value = timetable.Records.FirstOrDefault()?.ResultText ?? "Chưa kiểm định";

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);
            }

            Console.WriteLine("Xuất Excel thành công tại: " + filePath);
        }
    }
}
