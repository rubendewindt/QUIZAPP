using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows;

namespace QUIZAPP
{
    public partial class pagina2 : Window
    {
        private string correctAnswer;
        private string category;
        private string selectedDifficulty;
        private int categoryId;
        private int score; // Added score field

        public pagina2()
        {
            InitializeComponent();
        }

        public void SetQuestionData(string category, string question, List<string> options, string difficulty, string correctAnswer, string questionType)
        {
            this.correctAnswer = correctAnswer;
            this.category = category;
            this.selectedDifficulty = difficulty;

            CategoryTextBlock.Text = $"Category: {category}";
            QuestionTextBlock.Text = $"Question: {question}";

            OptionsStackPanel.Children.Clear();
            foreach (var option in options)
            {
                var radioButton = new RadioButton
                {
                    Content = option,
                    GroupName = "Options"
                };
                OptionsStackPanel.Children.Add(radioButton);
            }

            DifficultyTextBlock.Text = $"Difficulty: {difficulty}";
        }

        private async void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            await FetchNextQuestion();
        }

        private async Task FetchNextQuestion()
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(selectedDifficulty))
            {
                MessageBox.Show("Category or difficulty is not selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await FetchQuestionAndNavigate(categoryId, selectedDifficulty);
        }

        private async Task FetchQuestionAndNavigate(int categoryId, string difficulty)
        {
            string fetchedCategory = category;

            string url = $"https://opentdb.com/api.php?amount=1&category={categoryId}&difficulty={difficulty}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JObject jsonData = JObject.Parse(json);

                        string question = (string)jsonData["results"][0]["question"];
                        correctAnswer = (string)jsonData["results"][0]["correct_answer"];

                        JArray incorrectAnswers = (JArray)jsonData["results"][0]["incorrect_answers"];
                        List<string> multipleChoiceOptions = new List<string>();
                        multipleChoiceOptions.Add(correctAnswer);
                        foreach (var option in incorrectAnswers)
                        {
                            multipleChoiceOptions.Add((string)option);
                        }

                        ShuffleOptions();

                        SetQuestionData(fetchedCategory, question, multipleChoiceOptions, selectedDifficulty, correctAnswer, "multiple");
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch question. Status code: " + response.StatusCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRadioButton = OptionsStackPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);

            if (checkedRadioButton != null)
            {
                string selectedOption = checkedRadioButton.Content.ToString();
                if (selectedOption == correctAnswer)
                {
                    MessageBox.Show("Correct!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    score++; // Increase the score by 1 for correct answer
                    ScoreTextBlock.Text = $"Score: {score}"; // Update the score display

                    // Call FetchQuestionAndNavigate method to fetch the next question
                    await FetchQuestionAndNavigate(categoryId, selectedDifficulty);
                }
                else
                {
                    MessageBox.Show($"Wrong! The correct answer is: {correctAnswer}", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
                    await FetchQuestionAndNavigate(categoryId, selectedDifficulty);
                }
            }
            else
            {
                MessageBox.Show("Please select an option!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void ShuffleOptions()
        {
            Random rng = new Random();
            int n = OptionsStackPanel.Children.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                UIElement value = OptionsStackPanel.Children[k];
                OptionsStackPanel.Children.RemoveAt(k);
                OptionsStackPanel.Children.Insert(n, value);
            }
        }
    }
}



