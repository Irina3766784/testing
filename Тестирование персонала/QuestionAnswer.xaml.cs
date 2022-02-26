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
    /// Логика взаимодействия для QuestionAnswer.xaml
    /// </summary>
    public partial class QuestionAnswer : UserControl
    {
        public QuestionAnswer(EditTestWindow editTestWindow, string text)
        {
            InitializeComponent();
            this.DataContext = this;
            IsChecked = false;
            EditTestWindow = editTestWindow;
            Text = text;
            if (editTestWindow!=null)
            {
                IsEditable = Visibility.Visible;
                IsReadOnly = false;
            }
            else
            {
                IsEditable=Visibility.Collapsed;
                IsReadOnly = true;
            }
        }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
        public bool IsReadOnly { get; set; }
        public Visibility IsEditable { get; set; }
        public EditTestWindow EditTestWindow { get; set; }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            EditTestWindow.questionAnswers.Remove(this);
            EditTestWindow.RefillQuestionAnswerStack();
        }
    }
}
