using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using PomodoroTimer;
using Path = System.IO.Path;
using System.Xml.Schema;


namespace PomodoroApp
{
    /// <summary>
    /// Interaction logic for AnalyticsWindow.xaml
    /// </summary>
    public partial class AnalyticsWindow : Window
    {
        private SoundPlayer clickSoundPlayer;
        public int CompletedMinutes { get; set; }

        public AnalyticsWindow()
        {
            InitializeComponent();
            string clickSoundFilePath = "C:\\projects\\pomodoroTimer\\PomodoroApp\\PomodoroApp\\assets\\SFX\\click.wav";
            clickSoundPlayer = new SoundPlayer(clickSoundFilePath);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            DialogResult = false;
            Close();
        }

        public void UpdateAnalytics()
        {
            string folderPath = "bin";
            string filePath = Path.Combine(folderPath, "cycles.json");

            // Read the content of the cycles.json file
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);

                    // Deserialize the JSON content into a list of CompletedCycle objects
                    List<CompletedCycle> allCycles = JsonConvert.DeserializeObject<List<CompletedCycle>>(jsonContent);

                    // Calculate total minutes for today
                    DateTime today = DateTime.Now.Date;
                    int minutesToday = allCycles
                        .Where(c => c.CompletionDate.Date == today)
                        .Sum(c => c.CompletedMinutes);

                    minutesTodayBox.Text = $"Total minutes today: {minutesToday}";

                    // Calculate total minutes for the past 7 days
                    DateTime sevenDaysAgo = today.AddDays(-6);
                    int minutesLast7Days = allCycles
                        .Where(c => c.CompletionDate.Date >= sevenDaysAgo && c.CompletionDate.Date <= today)
                        .Sum(c => c.CompletedMinutes);

                    minutesWeekBox.Text = $"Total minutes in the past 7 days: {minutesLast7Days}";

                    // Calculate total minutes this month
                    DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                    int minutesThisMonth = allCycles
                        .Where(c => c.CompletionDate.Date >= startOfMonth && c.CompletionDate.Date <= today)
                        .Sum(c => c.CompletedMinutes);

                    minutesMonthBox.Text = $"Total minutes this month: {minutesThisMonth}";
                }
                else
                {
                    // Handle the case where the file doesn't exist
                    Console.WriteLine("cycles.json file not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions during file reading or JSON parsing
                Console.WriteLine($"Error updating analytics: {ex.Message}");
            }
        }

    }
}
