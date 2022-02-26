using Microsoft.Win32;
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
    /// Логика взаимодействия для EditTestWindow.xaml
    /// </summary>
    public partial class EditTestWindow : Window
    {
        TestingDBEntities db = new TestingDBEntities();
        public EditTestWindow(int testId)
        {
            InitializeComponent();
            TestId = testId;
            TestQuestions=new List<TestQuestions>();
            questionAnswers = new List<QuestionAnswer>();

            if (TestId>-1)
            {
                Test = (from test
                        in db.Tests
                        where test.Id == TestId
                        select test).FirstOrDefault();
                TestNameTextBox.Text = Test.Name;
                TestQuestions = (from testQuestion
                                 in db.TestQuestions
                                 where testQuestion.TestId == Test.Id
                                 select testQuestion).ToList();
                Refresh();
            }
            else
            {
                Test = new Tests();
            }
        }
        public List<TestQuestions> TestQuestions { get; set; }
        public int TestId { get; set; }
        public Tests Test { get; set; }
        public string ImagePath { get; set; }
        public TestQuestions selectedTestQuestion { get; set; }
        public List<QuestionAnswer> questionAnswers { get; set; }
        public void Refresh()
        {
            TestQuestions = (from testQuestion
                                 in db.TestQuestions
                             where testQuestion.TestId == Test.Id
                             select testQuestion).ToList();
            QuestionsListBox.SelectedIndex = -1;
            QuestionsListBox.Items.Clear();
            int i = 1;
            foreach (var item in TestQuestions)
            {
                QuestionsListBox.Items.Add(i.ToString());
                i++;
            }
        }
        public void RefillQuestionAnswerStack()
        {
            QuestionAnswersStack.Children.Clear();
            foreach (var item in questionAnswers)
            {
                QuestionAnswersStack.Children.Add(item);
            }
        }
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (TestId==-1)
            {
                Test = db.Tests.Add(new Tests { Name=TestNameTextBox.Text});
                db.SaveChanges();
                TestId = Test.Id;
            }
            db.TestQuestions.Add(new TestQuestions { TestId = TestId });/////////////////////
            questionAnswers = new List<QuestionAnswer>();////////////////////
            questionAnswers.Add(new QuestionAnswer(this,""));////////////////////
            questionAnswers[0].IsChecked = true;/////////////////////
            RefillQuestionAnswerStack();////////////////////////
            db.SaveChanges();
            Refresh();
        }

        private void DeleteQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsListBox.SelectedIndex==-1)
            {
                MessageBox.Show("Не выбран вопрос");
            }
            else
            {
                db.TestQuestions.Remove(TestQuestions[QuestionsListBox.SelectedIndex]);
                db.SaveChanges();
                Refresh();
                QuestionsListBox_SelectionChanged(this, null);
            }
        }
        private void QuestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionsListBox.SelectedIndex != -1)
            {
                selectedTestQuestion = TestQuestions[QuestionsListBox.SelectedIndex];
                if (selectedTestQuestion.Image!=null)
                {
                    ImagePath = selectedTestQuestion.Image;
                    DataContext = null;
                    DataContext = this;
                }
                else
                {
                    ImagePath=null;
                    DataContext = null;
                    DataContext = this;
                }
                if (selectedTestQuestion.Text != null)
                {
                    QuestionTextBox.Text = selectedTestQuestion.Text;
                }
                else
                {
                    QuestionTextBox.Text = "";
                }
                questionAnswers = new List<QuestionAnswer>();///////////////////
                if (selectedTestQuestion.Answers != null)
                {
                    foreach (var item in selectedTestQuestion.Answers.Split(';'))
                    {
                        questionAnswers.Add(new QuestionAnswer(this, item));
                    }
                    questionAnswers[(int)selectedTestQuestion.AnswerNumber].IsChecked = true;
                }
                RefillQuestionAnswerStack();
            }
            else
            {
                selectedTestQuestion = null;
                QuestionTextBox.Text = "";
                questionAnswers = new List<QuestionAnswer>();
                RefillQuestionAnswerStack();
                ImagePath = null;
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран вопрос");
            }
            else
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg";
                if (openDialog.ShowDialog() != true)
                    return;
                ImagePath = "images/" + openDialog.FileName.Split('\\').Last();
                selectedTestQuestion.Image = ImagePath;
            }
        }

        private void AddQuestionAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран вопрос");
            }
            else
            {
                questionAnswers.Add(new QuestionAnswer(this, ""));
                RefillQuestionAnswerStack();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string answers = "";
            int i = 0;
            if (questionAnswers.Count==0)/////////////////////////
            {
                MessageBox.Show("Нельзя сохранить вопрос без ответов");
                return;
            }
            selectedTestQuestion.AnswerNumber = -1;
            foreach (var item in questionAnswers)
            {
                if (item.Text.Length<1)
                {
                    MessageBox.Show("Один из вопросов пуст");
                    return;
                }
                answers += item.Text + ";";
                if (item.IsChecked)
                {
                    selectedTestQuestion.AnswerNumber = i;
                }
                i++;
            }
            if (selectedTestQuestion.AnswerNumber==-1)
            {
                MessageBox.Show("Нельзя сохранить вопрос правильного ответа");
                return;
            }
            Test.Name = TestNameTextBox.Text;
            if (selectedTestQuestion!=null)
            {
                selectedTestQuestion.Text=QuestionTextBox.Text;
                selectedTestQuestion.Answers = answers.Trim(';');
            }
            if (Test.Id==-1)
            {
                db.Tests.Add(Test);
            }
            db.SaveChanges();
            MessageBox.Show("Тест и вопросы к нему успешно сохранены");
        }
    }
}
