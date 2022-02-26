using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Тестирование_персонала
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestingDBEntities db = new TestingDBEntities();
        public static Users User;
        public MainWindow()
        {
            InitializeComponent();
            if (new LoginWindow().ShowDialog() == false)
            {
                this.Close();
            }
            if (User.IsAdmin!=true)
            {
                EditTestsButton.IsEnabled = false;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditTestsButton_Click(object sender, RoutedEventArgs e)
        {
            new EditTestsWindow().ShowDialog();
        }

        private void PassTestButton_Click(object sender, RoutedEventArgs e)
        {
            new TestsListWindow().ShowDialog();
        }

    }
}
