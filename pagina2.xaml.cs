using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QUIZAPP
{
    public partial class pagina2 : Window
    {
        private string category;
        private string correctAnswer;
        private string question;
        private string questionType;
        private int score = 0; // Variable to keep track of the score
        private string selectedCategory; // Added to store selected category
        private string selectedDifficulty; // Added to store selected difficulty

        public pagina2(string category, string question, string correctAnswer, string questionType, string selectedCategory, string selectedDifficulty)
        {
            InitializeComponent();
            this.category = category;
            this.question = question;
            this.correctAnswer = correctAnswer;
            this.questionType = questionType;
            this.selectedCategory = selectedCategory; // Store selected category
            this.selectedDifficulty = selectedDifficulty; // Store selected difficulty
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Display the fetched question and category
            MessageBox.Show($"Category: {category}", "Category", MessageBoxButton.OK, MessageBoxImage.Information);
            vraag.Content = question;

            if (questionType == "multiple") // If it's a multiple-choice question
            {
                // Show the multiple-choice options (A, B, C, etc.)
                optionA.Visibility = Visibility.Visible;
                optionB.Visibility = Visibility.Visible;
                optionC.Visibility = Visibility.Visible;

                // Set the content of the options if available
                optionA.Content = "Option A";
                optionB.Content = "Option B";
                optionC.Content = "Option C";
            }
            else if (questionType == "boolean") // If it's a true/false question
            {
                // Show the true/false options
                trueOption.Visibility = Visibility.Visible;
                falseOption.Visibility = Visibility.Visible;

                // Set the content of the options
                trueOption.Content = "True";
                falseOption.Content = "False";
            }
            else
            {
                MessageBox.Show("Invalid question type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close(); // Close the window if the question type is invalid
            }
        }

        // Event handler for option clicks
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            // Get the content of the clicked option
            Button clickedButton = sender as Button;
            string selectedOption = clickedButton.Content.ToString();

            // Check if the selected option is correct
            if (selectedOption == correctAnswer)
            {
                MessageBox.Show("Correct Answer!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                score++; // Increase score if correct
            }
            else
            {
                MessageBox.Show("Incorrect Answer!", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
                score = (score > 0) ? score - 1 : 0; // Decrease score by 1, but keep it >= 0
            }

            // Update score display
            MessageBox.Show($"Your current score: {score}", "Score", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Check if any option is selected
            if (optionA.IsChecked == true || optionB.IsChecked == true || optionC.IsChecked == true ||
                trueOption.IsChecked == true || falseOption.IsChecked == true)
            {
                MessageBox.Show("Please click on the options to answer the question.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No option selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Call a method to fetch the new question with the same category and difficulty
            await FetchNewQuestion();
        }

        private async Task FetchNewQuestion()
        {
            // Fetch a new question using the same category and difficulty as selected in the MainWindow
            string url = $"https://opentdb.com/api.php?amount=1&category={selectedCategory}&difficulty={selectedDifficulty}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var jsonData = JObject.Parse(json);
                        category = (string)jsonData["results"][0]["category"];
                        question = (string)jsonData["results"][0]["question"];
                        correctAnswer = (string)jsonData["results"][0]["correct_answer"];
                        questionType = (string)jsonData["results"][0]["type"];

                        // Update the UI with the new question
                        vraag.Content = question;

                        // Update visibility of options based on question type
                        if (questionType == "multiple")
                        {
                            optionA.Visibility = Visibility.Visible;
                            optionB.Visibility = Visibility.Visible;
                            optionC.Visibility = Visibility.Visible;

                            optionA.Content = "Option A";
                            optionB.Content = "Option B";
                            optionC.Content = "Option C";
                        }
                        else if (questionType == "boolean")
                        {
                            trueOption.Visibility = Visibility.Visible;
                            falseOption.Visibility = Visibility.Visible;

                            trueOption.Content = "True";
                            falseOption.Content = "False";
                        }
                        else
                        {
                            MessageBox.Show("Invalid question type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            this.Close(); // Close the window if the question type is invalid
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch data. Status code: " + response.StatusCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}


