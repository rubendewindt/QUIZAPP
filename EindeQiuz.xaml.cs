using QUIZAPP.Data;
using System;
using System.Windows;
using System.Windows.Controls;


namespace QUIZAPP
{
    public partial class EindeQiuz : Window
    {
        private int score;
        private string category;
 
        public EindeQiuz(int score, string category)
        {
            InitializeComponent();
            this.score = score;
            this.category = category;

            ScoreTextBlock.Text = $"Score: {score}"; // Display the score
            CategoryTextBlock.Text = $"Category: {category}"; // Display the category

            // Fetch and display user data
          
        }
 


        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            string name = naam.Text;
            // Save data to the database
            try
            {
                using (var context = new QuizAppContext())
                {
                    var user = new User
                    {
                        Score = score,
                        Name = name,
                        category = category
                    };

                    context.Users.Add(user);
                    context.SaveChanges();

                    MessageBox.Show("Data saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);


                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data to the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}

