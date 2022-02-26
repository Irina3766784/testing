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
using System.Windows.Shapes;

namespace Тестирование_персонала
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        TestingDBEntities db = new TestingDBEntities();

        public LoginWindow()
        {
            InitializeComponent();
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            if (login == "")
            {
                ErrorLabel.Content = "Логин не введён";
                return;
            }
            if (password == "")
            {
                ErrorLabel.Content = "Пароль не введён";
                return;
            }
            var currentuser = (from user in db.Users
                               where user.Name == login
                               select user).FirstOrDefault();
            if (currentuser != null && password==Crypto.Decode(currentuser.Password))
            {
                MainWindow.User = currentuser;
                this.DialogResult = true;
            }
            else
            {
                ErrorLabel.Content = "Неправильный логин или пароль";
                return;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().ShowDialog();
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLabel.Content = "";
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLabel.Content = "";
        }
    }
}
