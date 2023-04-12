using EcoFarm.AppConnection;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
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

namespace EcoFarm.CropProduction
{
    /// <summary>
    /// Логика взаимодействия для PageFields.xaml
    /// </summary>
    public partial class PageFields : Page
    {
        AccessVerification access = new AccessVerification();

        public PageFields()
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetFilter();
            SetSort();
            ListFields.ItemsSource = SortFilterFields();
        }

        Fields[] SortFilterFields()
        {
            List<Fields> fields = AppConnect.ModelDB.Fields.ToList();
            var CounterALL = fields;
            if (textBoxSearch.Text != null)
            {
                fields = fields.Where(x => x.Number.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 1:
                        fields = fields.OrderBy(x => x.Number).ToList();
                        break;
                    case 2:
                        fields = fields.OrderByDescending(x => x.Number).ToList();
                        break;
                }
            }

            if (comboBoxFilter.SelectedIndex > 0)
            {
                fields = fields.Where(x => x.Plants.Name == comboBoxFilter.SelectedItem.ToString()).ToList();
            }

            if (fields.Count != 0)
            {
                Counter.Text = "Показано: " + fields.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return fields.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("Без сортировки");
            comboBoxSort.Items.Add("По алфавиту А-Я");
            comboBoxSort.Items.Add("По алфавиту Я-А");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("Все растения");
            foreach (var plants in AppConnect.ModelDB.Plants)
            {
                comboBoxFilter.Items.Add(plants.Name);
            }


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListFields.ItemsSource = SortFilterFields();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListFields.ItemsSource = SortFilterFields();
        }

        private void comboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListFields.ItemsSource = SortFilterFields();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListFields.ItemsSource = SortFilterFields();
        }

        private void ListFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Fields row = ListFields.SelectedItem as Fields;
            AppFrame.frameMain.Navigate(new PageAddEditField(row));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditField((sender as Button).DataContext as Fields));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentRow = ListFields.SelectedItems.Cast<Fields>().ToList().ElementAt(0);
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AppConnect.ModelDB.Fields.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();
                    ListFields.ItemsSource = SortFilterFields();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
