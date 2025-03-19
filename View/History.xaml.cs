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
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Project212.Models;

namespace Project212
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : UserControl
    {
        private readonly Prn212AssignmentContext _context = new Prn212AssignmentContext();
        private DispatcherTimer _timer;

        public History()
        {
            InitializeComponent();
            LoadNotifications();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _timer.Tick += (s, e) => LoadNotifications();
            _timer.Start();
        }
        private void LoadNotifications()
        {
            var notifications = _context.Notices.OrderByDescending(n => n.SentDate).ToList();
            dgNotifications.ItemsSource = notifications;
        }

        private void dgNotifications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notice selectedNotice)
            {
                tbDetail.Text = selectedNotice.Detail;
                tbSentDate.Text = selectedNotice.SentDate.ToString("dd/MM/yyyy HH:mm");

                if (!selectedNotice.IsRead)
                {
                    selectedNotice.IsRead = true;
                    _context.SaveChanges();
                    LoadNotifications();
                }
            }
        }
    }
}
