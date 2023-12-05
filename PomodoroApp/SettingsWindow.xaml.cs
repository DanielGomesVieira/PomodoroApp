using PomodoroTimer;
using System;
using System.Windows;

namespace PomodoroApp
{
    public partial class SettingsWindow : Window
    {
        public TimeSpan WorkingTime { get; set; }
        public TimeSpan BreakTime { get; set; }

        // Define an event to notify when settings are changed
        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        // Method to update the UI elements with current work and break times
        public void UpdateSettings(TimeSpan currentWorkingTime, TimeSpan currentBreakTime)
        {
            WorkingTime = currentWorkingTime;
            BreakTime = currentBreakTime;

            // Fill in the text boxes with current time values
            workingTimeTextBox.Text = WorkingTime.TotalMinutes.ToString();
            breakTimeTextBox.Text = BreakTime.TotalMinutes.ToString();
        }

        // Event handlers for Save and Cancel buttons
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(workingTimeTextBox.Text, out int newWorkingMinutes) &&
                int.TryParse(breakTimeTextBox.Text, out int newBreakMinutes))
            {
                WorkingTime = TimeSpan.FromMinutes(newWorkingMinutes);
                BreakTime = TimeSpan.FromMinutes(newBreakMinutes);

                // Raise the event to notify the changes in settings
                SettingsChanged?.Invoke(this, new SettingsChangedEventArgs(WorkingTime, BreakTime));

                DialogResult = true; // Set dialog result to true to indicate changes were saved
                Close();
            }
            else
            {
                MessageBox.Show("Please enter valid time values (e.g., 25 for 25 minutes).");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    // Custom event argument class to hold the updated settings
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
