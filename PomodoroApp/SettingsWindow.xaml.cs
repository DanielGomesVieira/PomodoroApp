using PomodoroTimer;
using System;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace PomodoroApp
{
    public partial class SettingsWindow : Window
    {
        public TimeSpan WorkingTime { get; set; }
        public TimeSpan BreakTime { get; set; }
        private SoundPlayer clickSoundPlayer;

        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

        public SettingsWindow()
        {
            InitializeComponent();
            string clickSoundFilePath = @"C:\projects\pomodoroTimer\PomodoroApp\PomodoroApp\assets\click.wav";
            clickSoundPlayer = new SoundPlayer(clickSoundFilePath);
        }

        public void UpdateSettings(TimeSpan currentWorkingTime, TimeSpan currentBreakTime)
        {
            WorkingTime = currentWorkingTime;
            BreakTime = currentBreakTime;

            workingTimeTextBox.Text = WorkingTime.TotalMinutes.ToString();
            breakTimeTextBox.Text = BreakTime.TotalMinutes.ToString();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveButton_Click(sender, e);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            if (int.TryParse(workingTimeTextBox.Text, out int newWorkingMinutes) &&
                int.TryParse(breakTimeTextBox.Text, out int newBreakMinutes))
            {
                WorkingTime = TimeSpan.FromMinutes(newWorkingMinutes);
                BreakTime = TimeSpan.FromMinutes(newBreakMinutes);

                SettingsChanged?.Invoke(this, new SettingsChangedEventArgs(WorkingTime, BreakTime));

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter valid time values (e.g., 25 for 25 minutes).");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            DialogResult = false;
            Close();
        }
    }

    public class SettingsChangedEventArgs : EventArgs
    {
        public TimeSpan NewWorkingTime { get; }
        public TimeSpan NewBreakTime { get; }

        public SettingsChangedEventArgs(TimeSpan newWorkingTime, TimeSpan newBreakTime)
        {
            NewWorkingTime = newWorkingTime;
            NewBreakTime = newBreakTime;
        }
    }
    

}
