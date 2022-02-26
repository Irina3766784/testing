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
    /// Логика взаимодействия для EditTestsWindow.xaml
    /// </summary>
    public partial class EditTestsWindow : Window
    {
        TestingDBEntities db = new TestingDBEntities();
        public static int selectedTestId = -1;
        public EditTestsWindow()
        {
            InitializeComponent();
            Refresh();
        }
        public void Refresh()
        {
            db = new TestingDBEntities();
            TestsListBox.Items.Clear();
            var testsMas = db.Tests.ToList();
            string sort = (SortComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            switch (sort)
            {
                case "По названию теста А-Я": testsMas = testsMas.OrderBy(test => test.Name).ToList(); break;
                case "По названию теста Я-А": testsMas = testsMas.OrderByDescending(test => test.Name).ToList(); break;
                case "Количеству вопросов по возр.": testsMas = testsMas.OrderBy(test => test.TestQuestions.Count).ToList(); break;
                case "Количеству вопросов по убыв.": testsMas = testsMas.OrderByDescending(test => test.TestQuestions.Count).ToList(); break;
                default: break;
            }
            foreach (var item in testsMas) //Заполнение листбокса после сортировки
            {
                TestsListBox.Items.Add(item.Name);
            }
        }
        private void TestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestsListBox.SelectedIndex>-1) //если выбран один элемент 
            {
                selectedTestId = (from test
                                  in db.Tests
                                  where test.Name == TestsListBox.SelectedItem.ToString()
                                  select test.Id).FirstOrDefault(); 
            }
            else
            {
                selectedTestId = -1;
            }
            if (selectedTestId == -1)
            {
                TestNameTextBox.Text = "";
                QuestionsNumberTextBox.Text = "";
            }
            else
            {
                var selectedTest = (from test
                                    in db.Tests
                                    where test.Id == selectedTestId
                                    select test).FirstOrDefault();
                TestNameTextBox.Text = selectedTest.Name;
                QuestionsNumberTextBox.Text = (from testquestion
                                               in db.TestQuestions
                                               where testquestion.TestId == selectedTestId
                                               select testquestion).Count().ToString();
            }
        }

        private void NewTestButton_Click(object sender, RoutedEventArgs e)
        {
            selectedTestId = -1;
            new EditTestWindow(selectedTestId).ShowDialog();
            Refresh();
        }

        private void DeleteTestButton_Click(object sender, RoutedEventArgs e)
        {
            if (TestsListBox.SelectedIndex > -1)
            {
                var deleteTest = (from test
                                  in db.Tests
                                  where test.Name == TestsListBox.SelectedItem.ToString()
                                  select test).FirstOrDefault();
                if (MessageBox.Show("Вы действительно хотите удалить тест \"" + deleteTest.Name + "\"? Вместе с тестом удалятся все результаты пользователей по этому тесту.", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Tests.Remove(deleteTest);
                    db.SaveChanges();
                    Refresh();
                }
            }
            else
            {
                MessageBox.Show("Не выбран тест");
            }
        }

        private void EditTestButton_Click(object sender, RoutedEventArgs e)
        {
            if (TestsListBox.SelectedIndex>-1)
            {
                selectedTestId = (from test
                                  in db.Tests
                                  where test.Name == TestsListBox.SelectedItem.ToString()
                                  select test.Id).FirstOrDefault();
                new EditTestWindow(selectedTestId).ShowDialog();
                Refresh();
            }
            else
            {
                MessageBox.Show("Не выбран тест");
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
    }
}
