using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Project212.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;


namespace Project212
{
    public partial class History : UserControl
    {
        private readonly Prn212AssignmentContext _context = new Prn212AssignmentContext();
        private DispatcherTimer _timer;
        private int _currentUserId1;
        

        public History()
        {
            InitializeComponent();
            LoadNotifications();
            _currentUserId1 = UserSession.CurrentUser?.Id ?? -1;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _timer.Tick += (s, e) => LoadNotifications();
            _timer.Start();
            LoadDatagridResult();
        }

        private void LoadNotifications()
        {
            try
            {
                //// Hiển thị tất cả thông báo (để kiểm tra)
                //var allNotifications = _context.Notices.OrderByDescending(n => n.SentDate).ToList();
                //MessageBox.Show($"Total notifications in system: {allNotifications.Count}");

                //// Sau đó lọc theo người dùng
                //var userNotifications = allNotifications.Where(n => n.AccId == _currentUserId1).ToList();
                //MessageBox.Show($"User notifications: {userNotifications.Count}");

                var userNotifications = _context.Notices.Where(n => n.AccId == _currentUserId1).ToList();

                var allNotifications = userNotifications.OrderByDescending(n => n.SentDate).ToList();

                dgNotifications.ItemsSource = allNotifications;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void dgNotifications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notice selectedNotice)
            {
                // Hiển thị chi tiết thông báo
                tbDetail.Text = selectedNotice.Detail;
                tbSentDate.Text = selectedNotice.SentDate.ToString("dd/MM/yyyy HH:mm");

                // Nếu thông báo chưa đọc, cập nhật thành đã đọc
                if (!selectedNotice.IsRead)
                {
                    selectedNotice.IsRead = true;
                    _context.Notices.Update(selectedNotice);
                    _context.SaveChanges();
                    LoadNotifications(); // Cập nhật lại danh sách
                }
            }
        }
        private void LoadDatagridResult()
        {
            try
            {
                var citizenId = _context.Citizens.Where(c => c.AccId == _currentUserId1).Select(c => c.Id).FirstOrDefault();

                List<int> a = _context.Vehicles
                    .Where(v => v.CitizenId == citizenId)
                    .Select(v => v.Id)
                    .ToList();

                var userRecords = _context.Records
                .Where(r => a.Contains(r.VehicleId)) // Lọc chỉ các xe của tài khoản
                .OrderByDescending(r => r.VehicleId) // Sắp xếp theo ID xe giảm dần
                .ToList();

                //MessageBox.Show($"Số bản ghi lấy được: {userRecords.Count}");

                if (userRecords.Count == 0)
                    {
                        MessageBox.Show("Không có dữ liệu để hiển thị.");
                    }

                    DgResult.ItemsSource = userRecords.Select(r => new
                    {
                        Result = r.Result,
                        Co = r.Co,
                        Hc = r.Hc,
                        Nox = r.Nox,
                        Note = r.Note,
                        TimeId = r.TimeId
                    }).ToList();

                DgResult.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnExportTxt_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (DgResult.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi để xuất", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Save Selected Record to Text File",
                FileName = "XuatFileRecord.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    using StreamWriter sw = new StreamWriter(filePath);

                    // Lấy bản ghi đã chọn
                    dynamic selectedItem = DgResult.SelectedItem;

                    // Ghi thông tin bản ghi được chọn ra file
                    sw.WriteLine($"Kết quả: {selectedItem.Result}\nCO: {selectedItem.Co}\nHC: {selectedItem.Hc}\nNOx: {selectedItem.Nox}\nGhi chú: {selectedItem.Note}");

                    MessageBox.Show($"Exported Selected Record to {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static BaseFont LoadFont()
        {
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            return BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }


        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            if (DgResult.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Lưu kết quả ra file PDF",
                FileName = "KetQuaXe.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        Document document = new Document(PageSize.A4);
                        PdfWriter.GetInstance(document, fs);
                        document.Open();

                        // Load font hỗ trợ tiếng Việt
                        BaseFont baseFont = LoadFont();
                        Font font = new Font(baseFont, 12, Font.NORMAL);
                        Font titleFont = new Font(baseFont, 16, Font.BOLD);

                        // Tiêu đề
                        Paragraph title = new Paragraph("BÁO CÁO KẾT QUẢ KIỂM TRA XE", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        document.Add(title);
                        document.Add(new Paragraph("\n"));

                        // Bảng PDF
                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        float[] columnWidths = { 15f, 10f, 10f, 10f, 25f, 10f };
                        table.SetWidths(columnWidths);

                        // Thêm tiêu đề cột
                        string[] headers = { "Kết quả", "CO", "HC", "NOx", "Ghi chú", "Thời gian" };
                        foreach (string header in headers)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(header, font));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }

                        // Lấy bản ghi được chọn
                        dynamic row = DgResult.SelectedItem;
                        table.AddCell(new Phrase(row.Result.ToString(), font));
                        table.AddCell(new Phrase(row.Co.ToString(), font));
                        table.AddCell(new Phrase(row.Hc.ToString(), font));
                        table.AddCell(new Phrase(row.Nox.ToString(), font));
                        table.AddCell(new Phrase(row.Note ?? "", font));
                        table.AddCell(new Phrase(row.TimeId.ToString(), font));

                        document.Add(table);
                        document.Close();
                    }

                    MessageBox.Show($"Xuất file PDF thành công tại: {filePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
