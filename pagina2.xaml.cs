using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace QUIZAPP
{
    public partial class pagina2 : Window
    {
        private string correctAnswer;

        public pagina2()
        {
            InitializeComponent();
        }

        public void SetQuestionData(string category, string question, List<string> options, string difficulty, string correctAnswer, string questionType)
        {
            this.correctAnswer = correctAnswer;

            CategoryTextBlock.Text = $"Category: {category}";
            QuestionTextBlock.Text = $"Question: {question}";

            // Clear previous options and wire up event handlers for new options
            OptionsStackPanel.Children.Clear();
            foreach (var option in options)
            {
                var radioButton = new RadioButton
                {
                    Content = option,
                    GroupName = "Options"
                };
                radioButton.Checked += RadioButton_Checked; // Wire up the event handler
                OptionsStackPanel.Children.Add(radioButton);
            }

            // Display difficulty
            DifficultyTextBlock.Text = $"Difficulty: {difficulty}";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedOption = radioButton.Content.ToString();
                if (selectedOption == correctAnswer)
                {
                    // Display correct answer message
                    MessageBox.Show("Correct!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Display incorrect answer message
                    MessageBox.Show("Wrong!", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}


