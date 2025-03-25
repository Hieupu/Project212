using Project212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Project212.View
{
    /// <summary>
    /// Interaction logic for StandardManagement.xaml
    /// </summary>
    public partial class StandardManagement : UserControl
    {
        private readonly Prn212AssignmentContext _context;
        public ObservableCollection<StandardViewModel> Standards { get; set; }
        public StandardManagement()
        {
            _context = new Prn212AssignmentContext();
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            Standards = new ObservableCollection<StandardViewModel>(_context.Standards.Select(x => new StandardViewModel
            {
                Id = x.Id,
                Nox = x.Nox,
                Hc = x.Hc,
                Co = x.Co,
                Date = x.Date,
            }).ToList());
            lsProduct.ItemsSource = null;
            lsProduct.ItemsSource = Standards;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the fields before parsing
                if (string.IsNullOrWhiteSpace(txtId.Text) ||
                    string.IsNullOrWhiteSpace(txtCo.Text) ||
                    string.IsNullOrWhiteSpace(txtHc.Text) ||
                    string.IsNullOrWhiteSpace(txtNOx.Text))
                {
                    MessageBox.Show("All fields must be filled.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate if the Id can be parsed as an integer
                if (!int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show("Invalid Id. Please enter a valid integer.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }



                // Validate if the CO, HC, and NOx values are valid floats
                if (!float.TryParse(txtCo.Text, out float co))
                {
                    MessageBox.Show("Invalid CO value. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!float.TryParse(txtHc.Text, out float hc))
                {
                    MessageBox.Show("Invalid HC value. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!float.TryParse(txtNOx.Text, out float nox))
                {
                    MessageBox.Show("Invalid NOx value. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create a new standard object and save it to the database
                var standard = new Standard()
                {
                    Id = id,
                    Co = co,
                    Hc = hc,
                    Nox = nox,
                    Date = DateTime.Now,
                };

                _context.Standards.Add(standard);
                _context.SaveChanges();

                MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                txtId.Clear();
                txtCo.Clear();
                txtHc.Clear();
                txtNOx.Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text) ||
                    string.IsNullOrWhiteSpace(txtCo.Text) ||
                    string.IsNullOrWhiteSpace(txtHc.Text) ||
                    string.IsNullOrWhiteSpace(txtNOx.Text))
                {
                    MessageBox.Show("All fields must be filled.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show("Invalid Id. Please enter a valid integer.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }



                if (!float.TryParse(txtCo.Text, out float co))
                {
                    MessageBox.Show("Invalid CO value. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!float.TryParse(txtHc.Text, out float hc))
                {
                    MessageBox.Show("Invalid HC value. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!float.TryParse(txtNOx.Text, out float nox))
                {
                    MessageBox.Show("Invalid NOx value. Please enter a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var standard = _context.Standards.FirstOrDefault(x => x.Id == id);
                if (standard == null)
                {
                    MessageBox.Show("Invalid Id. Please enter a valid integer.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                standard.Hc = hc;
                standard.Nox = nox;
                standard.Co = co;
                standard.Date = DateTime.Now;

                _context.Standards.Update(standard);
                _context.SaveChanges();

                MessageBox.Show("Data Update Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                txtId.Clear();
                txtCo.Clear();
                txtHc.Clear();
                txtNOx.Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public class StandardViewModel
        {
            public int Id { get; set; }

            public double Co { get; set; }

            public double Hc { get; set; }

            public double Nox { get; set; }

            public DateTime Date { get; set; }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
