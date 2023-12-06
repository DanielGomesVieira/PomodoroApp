using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace PomodoroApp
{
    /// <summary>
    /// Interaction logic for AnalyticsWindow.xaml
    /// </summary>
    public partial class AnalyticsWindow : Window
    {
        private SoundPlayer clickSoundPlayer;

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
    }
}
