using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QUIZAPP
{
    public partial class pagina2 : Window
    {
        private string correctAnswer;
        private string category;
        private string selectedDifficulty;
        private int categoryId;
        private int score; // Added score field
        private DispatcherTimer timer; // Timer declaration
        private int remainingTime = 300; // 10 minutes in seconds = 600

        public pagina2(string category, string selectedDifficulty, int categoryId)
        {
            InitializeComponent();
            InitializeTimer(); // Initialize the timer
            timer.Start();

            // Store the category, difficulty, and categoryId for later use
            this.category = category;
            this.selectedDifficulty = selectedDifficulty;
            this.categoryId = categoryId;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Timer ticks every second
            timer.Tick += Timer_Tick;
            UpdateTimerDisplay();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime--; // Decrement remaining time
            UpdateTimerDisplay();

            if (remainingTime == 0)
            {
                timer.Stop();
                MessageBox.Show("Time's up!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);

                // Navigate to the EindeQiuz.xaml page
                EindeQiuz eindeQiuzPage = new EindeQiuz(score, category); // Pass the score to the EindeQiuz page
                eindeQiuzPage.Show(); // Display the EindeQiuz window modally
                this.Close();
            }
        }

        private void UpdateTimerDisplay()
        {
            int minutes = remainingTime / 60;
            int seconds = remainingTime % 60;
            TimerTextBlock.Text = $"Time: {minutes:D2}:{seconds:D2}";
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to go to the next question?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await FetchNextQuestion();
            }
        }

        private async Task FetchNextQuestion()
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(selectedDifficulty))
            {
                MessageBox.Show("Category or difficulty is not selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Disable buttons and radio buttons
            NextQuestionButton.IsEnabled = false;
            SubmitButton.IsEnabled = false;
            foreach (var radioButton in OptionsStackPanel.Children.OfType<RadioButton>())
            {
                radioButton.IsEnabled = false;
            }

            // Add a delay before fetching the next question
            await Task.Delay(1000); // Adjust the delay time as needed (in milliseconds)
            await FetchQuestionAndNavigate(categoryId, selectedDifficulty); // Use the stored categoryId
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
                        foreach (var option in incorrectAnswers)
                        {
                            multipleChoiceOptions.Add((string)option);
                        }

                        // Add the correct answer
                        multipleChoiceOptions.Add(correctAnswer);

                        // Shuffle options to randomize order
                        ShuffleOptions(multipleChoiceOptions);

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

            // Enable buttons and radio buttons after fetching is complete
            NextQuestionButton.IsEnabled = true;
            SubmitButton.IsEnabled = true;
            foreach (var radioButton in OptionsStackPanel.Children.OfType<RadioButton>())
            {
                radioButton.IsEnabled = true;
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
                    await Task.Delay(1000); // Adjust the delay time as needed (in milliseconds)
                    await FetchQuestionAndNavigate(categoryId, selectedDifficulty);
                }
                else
                {
                    MessageBox.Show($"Wrong! The correct answer is: {correctAnswer}", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
                    await Task.Delay(1000); // Adjust the delay time as needed (in milliseconds)
                    await FetchQuestionAndNavigate(categoryId, selectedDifficulty);
                }
            }
            else
            {
                MessageBox.Show("Please select an option!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShuffleOptions(List<string> options)
        {
            Random rng = new Random();
            int n = options.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = options[k];
                options[k] = options[n];
                options[n] = value;
            }
        }
    }
}



