using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace QUIZAPP
{
    public partial class MainWindow : Window
    {
        private string category;
        private string correctAnswer;
        private string question;

        private JObject jsonData; // Class-level variable to store JSON data

        public MainWindow()
        {
            InitializeComponent();
            // Call InitializeAsync when the window is initialized
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // Fetch categories and populate ComboBox
            await FetchCategories();

            // Fetch difficulties and populate ComboBox
            await FetchDifficulties();
        }

        private async Task FetchCategories()
        {
            string[] categories = null;
            string url = "https://opentdb.com/api_category.php";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JObject jsonData = JObject.Parse(json);
                        categories = jsonData["trivia_categories"].Select(c => (string)c["name"]).ToArray();
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch categories. Status code: " + response.StatusCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (categories != null)
            {
                categoryComboBox.ItemsSource = categories;
            }
        }

        private async Task FetchDifficulties()
        {
            string[] difficulties = { "easy", "medium", "hard" }; // Pre-defined difficulty levels
            difficultyComboBox.ItemsSource = difficulties;
        }

        private async Task FetchAPIData(string selectedCategory, string selectedDifficulty)
        {
            string url = $"https://opentdb.com/api.php?amount=1&category={selectedCategory}&difficulty={selectedDifficulty}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        jsonData = JObject.Parse(json); // Store the JSON data
                        category = (string)jsonData["results"][0]["category"];
                        question = (string)jsonData["results"][0]["question"];
                        correctAnswer = (string)jsonData["results"][0]["correct_answer"];
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

        private string DetermineQuestionType()
        {
            string questionType = (string)jsonData["results"][0]["type"];
            switch (questionType)
            {
                case "multiple":
                    return "Multiple Choice";
                case "boolean":
                    return "True/False";
                default:
                    return "Unknown"; // Add more cases as needed
            }
        }

        private async void FetchQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedCategory = categoryComboBox.SelectedItem as string;
            string selectedDifficulty = difficultyComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedCategory) && !string.IsNullOrEmpty(selectedDifficulty))
            {
                await FetchAPIData(selectedCategory, selectedDifficulty);
                string questionType = DetermineQuestionType();
                pagina2 page2 = new pagina2(category, question, correctAnswer, questionType, selectedCategory, selectedDifficulty);
                page2.Show();
                this.Hide(); // Hide the main window
            }
            else
            {
                MessageBox.Show("Please select both a category and a difficulty level.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
