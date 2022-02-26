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
    /// Логика взаимодействия для PassWindow.xaml
    /// </summary>
    public partial class PassWindow : Window
    {
        TestingDBEntities db=new TestingDBEntities();
        public string ImagePath { get; set; }
        public Tests Test { get; set; }
        public List<TestQuestions> TestQuestions { get; set; }
        public List<QuestionAnswer> questionAnswers { get; set; }
        public ResultAnswers[] resultAnswers { get; set; }
        public int currentQuestionNumber;
        public PassWindow(Tests test)
        {
            InitializeComponent();
            Test = test;
            currentQuestionNumber = 0;
            TestQuestions=(from testQuestion
                           in db.TestQuestions
                           where testQuestion.TestId==Test.Id
                           select testQuestion).ToList();
            resultAnswers=new ResultAnswers[TestQuestions.Count];
            Refresh();
        }
        public void Refresh()
        {
            if (currentQuestionNumber<1)
            {
                BackButton.IsEnabled = false;
            }
            else
            {
                BackButton.IsEnabled=true;
            }
            if (currentQuestionNumber > TestQuestions.Count-2)
            {
                NextButton.IsEnabled = false;
            }
            else
            {
                NextButton.IsEnabled = true;
            }
            var currentQuestion=TestQuestions[currentQuestionNumber];
            QuestionNumberLabel.Content=(currentQuestionNumber+1).ToString();
            QuestionTextLabel.Text=currentQuestion.Text;
            ImagePath = currentQuestion.Image;
            DataContext = null;
            DataContext = this;

            questionAnswers = new List<QuestionAnswer>();
            if (currentQuestion.Answers != null)
            {
                foreach (var item in currentQuestion.Answers.Split(';'))
                {
                    questionAnswers.Add(new QuestionAnswer(null, item));
                }
            } 
            RefillQuestionAnswerStack();
        }
        public void RefillQuestionAnswerStack()
        {
            AnswersStackPanel.Children.Clear();
            foreach (var item in questionAnswers)
            {
                AnswersStackPanel.Children.Add(item);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            int answerNumber = -1;
            int i = 0;
            foreach (var item in questionAnswers)
            {
                if (item.IsChecked)
                {
                    answerNumber = i;
                }
                i++;
            }
            resultAnswers[currentQuestionNumber] = new ResultAnswers { QuestionId = TestQuestions[currentQuestionNumber].Id, Answer = answerNumber };
            currentQuestionNumber--;
            Refresh();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int answerNumber = -1;
            int i = 0;
            foreach (var item in questionAnswers)
            {
                if (item.IsChecked)
                {
                    answerNumber = i;
                }
                i++;
            }
            resultAnswers[currentQuestionNumber] = new ResultAnswers { QuestionId = TestQuestions[currentQuestionNumber].Id, Answer = answerNumber };
            currentQuestionNumber++;
            Refresh();
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            int answerNumber = -1;
            int i = 0;
            foreach (var item in questionAnswers)
            {
                if (item.IsChecked)
                {
                    answerNumber = i;
                }
                i++;
            }
            resultAnswers[currentQuestionNumber] = new ResultAnswers { QuestionId = TestQuestions[currentQuestionNumber].Id, Answer = answerNumber };
            currentQuestionNumber++;

            foreach (var item in resultAnswers)
            {
                if (item == null)
                {
                    MessageBox.Show("Нет ответа на все вопросы");
                    return;
                }
            }

            var newResult = new Results();
            newResult.UserId=MainWindow.User.Id;
            newResult.TestId = Test.Id;
            newResult.Date = DateTime.Now;
            db.Results.Add(newResult);
            db.SaveChanges();
            foreach (var item in resultAnswers)
            {
                db.ResultAnswers.Add(item);
                item.ResultId=newResult.Id;
            }
            db.SaveChanges();
            MessageBox.Show("Тест пройден");
            this.Close();
        }
    }
}
