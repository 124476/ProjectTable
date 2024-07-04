using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

namespace ProjectTable
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int sizeInt;
        public string[] LabelX { get; set; }
        public string[] LabelY { get; set; }
        public string[] Labels { get; set; }
        public SeriesCollection Series { get; set; }
        DateTime dateNow;
        public MainWindow()
        {
            InitializeComponent();

            PoiskCombo.ItemsSource = new string[] { "1 неделя", "2 недели", "1 месяц", "1 год" };
            PoiskCombo.SelectedIndex = 0;
            sizeInt = 5;
            TextSize.Text = sizeInt.ToString();
            dateNow = DateTime.Now;
            Refresh();
        }

        private void Refresh()
        {
            DataContext = null;

            var weekends = new List<string>();
            var dateStart = dateNow;
            DateTime dateEnd;

            var times = new List<string>();
            var timeStart = TimeSpan.Parse("23:00");
            bool flag = true;
            var timeNow = new TimeSpan(dateNow.Second, dateNow.Minute, dateNow.Hour);

            while (timeStart.Hours >= 0)
            {
                if (flag && timeStart <= timeNow)
                {
                    flag = false;
                    times.Add(timeStart.ToString());
                    continue;
                }

                times.Add(timeStart.ToString());
                timeStart -= TimeSpan.FromHours(1);
            }

            if (PoiskCombo.SelectedIndex == 0)
                dateEnd = dateStart.AddDays(7);
            else if (PoiskCombo.SelectedIndex == 1)
                dateEnd = dateStart.AddDays(14);
            else if (PoiskCombo.SelectedIndex == 2)
                dateEnd = dateStart.AddMonths(1);
            else
                dateEnd = dateStart.AddYears(1);

            TextPeriod.Text = dateStart.ToString("dd.MM.yyyy") + " - " + dateEnd.ToString("dd.MM.yyyy");


            while (dateStart < dateEnd)
            {
                weekends.Add(dateStart.ToString("dddd"));

                dateStart += TimeSpan.FromDays(1);
            }

            var values = new ChartValues<GanttPoint>();

            var tasks = App.DB.Task.Where(x => x.DateStart < dateStart || x.DateEnd > dateEnd).ToList();
            var labels = new List<string>();

            foreach (var task in tasks)
            {
                try
                {
                    values.Add(new GanttPoint(task.CreatedTime.Value.Ticks, task.FinishActualTime.Value.Ticks));
                    labels.Add(task.ShortTitle);
                }
                catch { }
            }

            Labels = labels.ToArray();

            Series = new SeriesCollection()
            {
                new RowSeries
                {
                    Values = values,
                },
            };

            LabelX = weekends.ToArray();
            LabelY = times.ToArray();

            DataContext = this;
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PoiskCombo.SelectedIndex == 0)
                dateNow = dateNow.AddDays(-7);
            else if (PoiskCombo.SelectedIndex == 1)
                dateNow = dateNow.AddDays(-14);
            else if (PoiskCombo.SelectedIndex == 2)
                dateNow = dateNow.AddMonths(-1);
            else
                dateNow = dateNow.AddYears(-1);
            Refresh();
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PoiskCombo.SelectedIndex == 0)
                dateNow = dateNow.AddDays(7);
            else if (PoiskCombo.SelectedIndex == 1)
                dateNow = dateNow.AddDays(14);
            else if (PoiskCombo.SelectedIndex == 2)
                dateNow = dateNow.AddMonths(1);
            else
                dateNow = dateNow.AddYears(1);
            Refresh();
        }

        private void PoiskCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DownSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            sizeInt--;
            if (sizeInt < 1)
                sizeInt = 1;
            TextSize.Text = sizeInt.ToString();

            Cartes.MinWidth = sizeInt * 140;
            Cartes.MinHeight = sizeInt * 140;
        }

        private void UpSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            sizeInt++;
            if (sizeInt > 10)
                sizeInt = 10;
            TextSize.Text = sizeInt.ToString();

            Cartes.MinWidth = sizeInt * 140;
            Cartes.MinHeight = sizeInt * 140;
        }
    }
}
