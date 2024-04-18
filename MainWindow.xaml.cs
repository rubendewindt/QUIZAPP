﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace QUIZAPP
{
    public partial class MainWindow : Window
    {
        private string category;
        private string selectedDifficulty; // Declare selectedDifficulty as a class-level variable
        private Dictionary<string, int> categoryDictionary = new Dictionary<string, int>();
        private int categoryId;
        private string question;
        private string correctAnswer;
        private List<string> multipleChoiceOptions;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
            PopulateCategoryComboBox();
        }

        private async Task InitializeAsync()
        {
            // Provide a default category ID (e.g., the ID for "General Knowledge")
            int defaultCategoryId = 9;
            // Provide a default difficulty level (e.g., "medium")
            selectedDifficulty = "medium"; // Initialize selectedDifficulty
            // Fetch a question initially using the default category ID and default difficulty
            await FetchQuestion(defaultCategoryId, selectedDifficulty);

        }

        private void PopulateCategoryComboBox()
        {
            // Populate the category dictionary with category names and their corresponding ids
            categoryDictionary.Add("General Knowledge", 9);
            categoryDictionary.Add("Entertainment: Books", 10);
            categoryDictionary.Add("Entertainment: Film", 11);
            categoryDictionary.Add("Entertainment: Music", 12);
            categoryDictionary.Add("Entertainment: Musicals & Theatres", 13);
            categoryDictionary.Add("Entertainment: Television", 14);
            categoryDictionary.Add("Entertainment: Video Games", 15);
            categoryDictionary.Add("Entertainment: Board Games", 16);
            categoryDictionary.Add("Science & Nature", 17);
            categoryDictionary.Add("Science: Computers", 18);
            categoryDictionary.Add("Science: Mathematics", 19);
            categoryDictionary.Add("Mythology", 20);
            categoryDictionary.Add("Geography", 22);
            categoryDictionary.Add("History", 23);
            categoryDictionary.Add("Politics", 24);
            categoryDictionary.Add("Art", 25);
            categoryDictionary.Add("Celebrities", 26);
            categoryDictionary.Add("Animals", 27);
            categoryDictionary.Add("Vehicles", 28);
            categoryDictionary.Add("Entertainment: Comics", 29);
            categoryDictionary.Add("Science: Gadgets", 30);
            categoryDictionary.Add("Entertainment: Japanese Anime & Manga", 31);
            categoryDictionary.Add("Entertainment: Cartoon & Animations", 32);

            // Add category names to the ComboBox
            foreach (string categoryName in categoryDictionary.Keys)
            {
                categoryComboBox.Items.Add(categoryName);
            }
        }

        private async void CategoryComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Set the categoryId based on the selected category
            string selectedCategory = (string)categoryComboBox.SelectedItem;
            if (categoryDictionary.ContainsKey(selectedCategory))
            {
                categoryId = categoryDictionary[selectedCategory];
                // Call FetchQuestion with the selected category ID and default difficulty

            }
        }

        private async void DifficultyComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Get the selected difficulty
            selectedDifficulty = ((ComboBoxItem)difficultyComboBox.SelectedItem)?.Content?.ToString()?.ToLower();

            // Enable the button if both category and difficulty are selected
            
        }

        private async Task FetchQuestion(int categoryId, string difficulty)
        {
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

                        category = (string)jsonData["results"][0]["category"];
                        question = (string)jsonData["results"][0]["question"];
                        correctAnswer = (string)jsonData["results"][0]["correct_answer"];

                        // Extract incorrect answers
                        JArray incorrectAnswers = (JArray)jsonData["results"][0]["incorrect_answers"];
                        multipleChoiceOptions = new List<string>();
                        multipleChoiceOptions.Add(correctAnswer); // Add the correct answer
                        foreach (var option in incorrectAnswers)
                        {
                            multipleChoiceOptions.Add((string)option);
                        }

                        // Shuffle options to randomize order
                        ShuffleOptions();

                        // Display the fetched question

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

        private void ShowQuestion()
        {
            // Display category, question, options, difficulty, and correct answer
            MessageBox.Show($"Category: {category}\n\nQuestion: {question}\n\nOptions:\n{string.Join("\n", multipleChoiceOptions)}\n\nDifficulty: {selectedDifficulty}\n\nCorrect Answer: {correctAnswer}", "Question", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void ShuffleOptions()
        {
            // Fisher-Yates shuffle algorithm
            Random rng = new Random();
            int n = multipleChoiceOptions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = multipleChoiceOptions[k];
                multipleChoiceOptions[k] = multipleChoiceOptions[n];
                multipleChoiceOptions[n] = value;
            }
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if both category and difficulty are selected
            if (categoryComboBox.SelectedItem == null || string.IsNullOrEmpty(selectedDifficulty))
            {
                // Display an alert message prompting the user to select both options
                MessageBox.Show("Please select both category and difficulty to start the quiz.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Exit the method early if either option is not selected
            }

            // Fetch the question if both options are selected
            int selectedCategoryId = categoryDictionary[(string)categoryComboBox.SelectedItem];
            FetchQuestion(selectedCategoryId, selectedDifficulty);
            ShowQuestion();
        }
    }
}

