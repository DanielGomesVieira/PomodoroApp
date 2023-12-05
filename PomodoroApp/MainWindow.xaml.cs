using PomodoroApp;
using System;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        private TimeSpan workTime = TimeSpan.FromMinutes(25);
        private TimeSpan breakTime = TimeSpan.FromMinutes(1);
        private DispatcherTimer timer;
        private TimeSpan currentTime;
        private bool isWorking = true;
        private SoundPlayer alarmSoundPlayer;
        private SoundPlayer clickSoundPlayer;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            ResetTimer();

            string alarmSoundFilePath = @"C:\projects\pomodoroTimer\PomodoroApp\PomodoroApp\assets\alarm.wav";
            alarmSoundPlayer = new SoundPlayer(alarmSoundFilePath);
            string clickSoundFilePath = @"C:\projects\pomodoroTimer\PomodoroApp\PomodoroApp\assets\click.wav";
            clickSoundPlayer = new SoundPlayer(clickSoundFilePath);
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
        private void SettingsWindow_SettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            ResetTimer();
        }
    }
}
