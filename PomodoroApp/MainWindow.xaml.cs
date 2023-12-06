using PomodoroApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;


namespace PomodoroTimer
{
    public class CompletedCycle
    {
        public int CompletedMinutes { get; set; }
        public DateTime CompletionDate { get; set; }
    } 

    public partial class MainWindow : Window
    {
        private TimeSpan workTime = TimeSpan.FromMinutes(25);
        private TimeSpan breakTime = TimeSpan.FromMinutes(1);
        private DispatcherTimer timer;
        private TimeSpan currentTime;
        private bool isWorking = true;
        private SoundPlayer alarmSoundPlayer;
        private SoundPlayer clickSoundPlayer;
        public int CompletedMinutes { get; set; } = 0;
        public List<CompletedCycle> completedCycles { get; set; } = new List<CompletedCycle>();

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            ResetTimer();

            string alarmSoundFilePath = "C:\\projects\\pomodoroTimer\\PomodoroApp\\PomodoroApp\\assets\\SFX\\alarm.wav";
            alarmSoundPlayer = new SoundPlayer(alarmSoundFilePath);
            string clickSoundFilePath = "C:\\projects\\pomodoroTimer\\PomodoroApp\\PomodoroApp\\assets\\SFX\\click.wav";
            clickSoundPlayer = new SoundPlayer(clickSoundFilePath);
            LoadCompletedCycles();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentTime = currentTime.Subtract(TimeSpan.FromSeconds(1));
            if (currentTime <= TimeSpan.Zero)
            {
                timer.Stop();
                this.Topmost = true;
                this.Topmost = false;
                this.Focus();
                alarmSoundPlayer.Play();

                if (isWorking)
                {
                    MessageBox.Show("Take a break!", "Pomodoro Timer");
                    currentTime = breakTime;
                    isWorking = false;
                    startButton.Content = "Start";
                    CompletedMinutes += workTime.Minutes;

                    CompletedCycle completedCycle = new CompletedCycle
                    {
                        CompletedMinutes = workTime.Minutes,
                        CompletionDate = DateTime.Now
                    };

                    completedCycles.Add(completedCycle);
                    SaveCompletedCycles();

                }
                else
                {
                    MessageBox.Show("Back to work!", "Pomodoro Timer");
                    currentTime = workTime;
                    isWorking = true;
                    startButton.Content = "Start";
                }

                ResetTimer();
            }
            UpdateTimerText();
        }

        private void ResetTimer()
        {
            currentTime = isWorking ? workTime : breakTime;
            UpdateTimerText();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                StartButton_Click(sender, e);
            }
        }


        private void UpdateTimerText()
        {
            timerText.Dispatcher.Invoke(() =>
            {
                timerText.Text = currentTime.ToString(@"mm\:ss");
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            if (!timer.IsEnabled)
            {
                if (isWorking)
                {
                    currentTime = workTime;
                    startButton.Content = "Stop";
                }
                else
                {
                    currentTime = breakTime;
                    startButton.Content = "Skip Break";
                }
                timer.Start();
            }
            else
            {
                if (!isWorking)
                {
                    timer.Stop();
                    currentTime = workTime;
                    startButton.Content = "Start";
                    isWorking = true;
                    UpdateTimerText();
                }
                else
                {
                    timer.Stop();
                    currentTime = isWorking ? workTime : breakTime;
                    startButton.Content = "Start";
                    UpdateTimerText();
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            OpenSettings();
        }

        private void OpenSettings()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.UpdateSettings(workTime, breakTime);
            settingsWindow.SettingsChanged += SettingsWindow_SettingsChanged;

            if (settingsWindow.ShowDialog() == true)
            {
                workTime = settingsWindow.WorkingTime;
                breakTime = settingsWindow.BreakTime;
                timer.Stop();
                startButton.Content = "Start";
                ResetTimer();
            }
        }

        private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            OpenAnalytics();
        }

        private void OpenAnalytics()
        {
            AnalyticsWindow analyticsWindow = new AnalyticsWindow();
            analyticsWindow.UpdateAnalytics();
            if (analyticsWindow.ShowDialog() == true)
            {
                ResetTimer();
            }
        }
        private void SettingsWindow_SettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            ResetTimer();
        }

        private void LoadCompletedCycles()
        {
            string folderPath = "bin";
            string filePath = Path.Combine(folderPath, "cycles.json");

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "[]");
                }
                string existingContent = File.ReadAllText(filePath);
                completedCycles = JsonConvert.DeserializeObject<List<CompletedCycle>>(existingContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading completed cycles: {ex.Message}");
            }
        }

        private void SaveCompletedCycles()
        {
            string folderPath = "bin";
            string filePath = Path.Combine(folderPath, "cycles.json");

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string jsonContent = JsonConvert.SerializeObject(completedCycles);
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving completed cycles: {ex.Message}");
            }
        }
    }
}
