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
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;

namespace Тестирование_персонала
{
    /// <summary>
    /// Логика взаимодействия для TestsListWindow.xaml
    /// </summary>
    public partial class TestsListWindow : Window
    {
        TestingDBEntities db = new TestingDBEntities();
        public Tests selectedTest;
        public TestsListWindow()
        {
            InitializeComponent();
            UserNameLabel.Content = MainWindow.User.Name;
            RefreshListBox();
        }
        public void RefreshListBox()
        {
            TestsListBox.Items.Clear();
            foreach (var item in db.Tests.ToList())
            {
                TestsListBox.Items.Add(item.Name);
            }
        }
        public void ExportToWord()
        {
            try
            {
                // Создаём объект приложения
                Word.Application app = new Word.Application();
                // Путь до шаблона документа
                string source = Assembly.GetExecutingAssembly().Location + @"\..\Сертификат.dotx";

                // Открываем
                Word.Document doc = app.Documents.Add(source);
                doc.Activate();

                // Добавляем информацию
                // wBookmarks содержит все закладки
                Word.Bookmarks wBookmarks = doc.Bookmarks;

                int questionsCount = (from question
                                      in db.TestQuestions
                                      where question.TestId == selectedTest.Id
                                      select question).Count();
                Results lastResult = (from result
                                          in db.Results
                                      where result.UserId == MainWindow.User.Id
                                      select result).ToList().Last();
                List<ResultAnswers> lastResultAnswers = (from resultAnswer
                                                         in db.ResultAnswers
                                                         where resultAnswer.ResultId == lastResult.Id
                                                         select resultAnswer).ToList();
                int correctAnswers = 0;
                foreach (var item in lastResultAnswers)
                {
                    if ((from question
                         in db.TestQuestions
                         where question.Id == item.QuestionId
                         select question).FirstOrDefault().AnswerNumber == item.Answer)
                    {
                        correctAnswers++;
                    }
                }

                foreach (Word.Bookmark mark in wBookmarks)
                {
                    switch (mark.Name)
                    {
                        case "НазваниеТеста": mark.Range.Text = selectedTest.Name; break;
                        case "ФИО": mark.Range.Text = MainWindow.User.Name; break;
                        case "Дата": mark.Range.Text = lastResult.Date.Value.Date.ToString().Split(' ')[0]; break;
                        case "Балл": mark.Range.Text = correctAnswers.ToString(); break;
                        case "МаксимумБаллов": mark.Range.Text = questionsCount.ToString(); break;
                        case "ДатаВыдачи": mark.Range.Text = DateTime.Now.Date.ToString().Split(' ')[0]; break;
                    }
                }

                // Закрываем документ
                doc.Close();
                doc = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void PassTestButton_Click(object sender, RoutedEventArgs e)
        {
            new PassWindow(selectedTest).ShowDialog();
            RefreshListBox();
            TestsListBox.SelectedIndex = -1;
        }

        private void TestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestsListBox.SelectedIndex!=-1)
            {
                selectedTest = (from test
                                in db.Tests
                                where test.Name == TestsListBox.SelectedItem.ToString()
                                select test).FirstOrDefault();
                TestNameTextBox.Text = selectedTest.Name;
                int questionsCount = (from question
                                      in db.TestQuestions
                                      where question.TestId == selectedTest.Id
                                      select question).Count();
                if (questionsCount>0)
                {
                    PassTestButton.IsEnabled = true;
                }
                else
                {
                    PassTestButton.IsEnabled=false;
                }
                QuestionsNumberTextBox.Text = questionsCount.ToString();
                if ((from result
                     in db.Results
                     where result.UserId==MainWindow.User.Id && selectedTest.Id==result.TestId
                     select result).Any())
                {
                    Results lastResult= (from result
                                         in db.Results
                                         where result.UserId == MainWindow.User.Id
                                         select result).ToList().Last();
                    List<ResultAnswers> lastResultAnswers = (from resultAnswer
                                                             in db.ResultAnswers
                                                             where resultAnswer.ResultId == lastResult.Id
                                                             select resultAnswer).ToList();
                    int correctAnswers = 0;
                    foreach (var item in lastResultAnswers)
                    {
                        if ((from question
                             in db.TestQuestions
                             where question.Id==item.QuestionId
                             select question).FirstOrDefault().AnswerNumber==item.Answer)
                        {
                            correctAnswers++;
                        }
                    }
                    TestResultLabel.Content = $"{correctAnswers} из {questionsCount}";
                    ExportTestResultButton.IsEnabled = true;
                }
                else
                {
                    TestResultLabel.Content = "Тест не пройден";
                    ExportTestResultButton.IsEnabled = false;
                }
            }
            else
            {

            }
        }

        private void ExportTestResultButton_Click(object sender, RoutedEventArgs e)
        {
            ExportToWord();
        }
    }
}
