using Phidget22;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Phidget22.Events;
using System.IO;

namespace DemoTable
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DigitalInput PointButton = new DigitalInput();
        public DigitalInput JoinButton = new DigitalInput();
        public DigitalOutput Output = new DigitalOutput();

        private MainWindow mw;

        public Player(MainWindow mainWindow, int pointChannel, int joinChannel, int outputChannel)
        {
            InitializeComponent();
            mw = mainWindow;

            DataContext = this;
            LabelTimer.DataContext = mw;

            PointButton.Channel = pointChannel;

            JoinButton.Channel = joinChannel;

            Output.Channel = outputChannel;

            GetBackground(outputChannel);

            LabelTimer.Visibility = Visibility.Hidden;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PointButton.StateChange += PointButton_StateChange;
            PointButton.Open();

            JoinButton.StateChange += JoinButton_StateChange;
            JoinButton.Open();

            Output.Open();
        }

        private void GetBackground(int id)
        {
            if(File.Exists($@"{Environment.CurrentDirectory}\Resources\Background{id}.png"))
                Background = new ImageBrush(new BitmapImage(new Uri($@"{Environment.CurrentDirectory}\Resources\Background{id}.png")));
            else
            {
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\Resources");
            }
        }

        private bool isPlayerJoined = false;
        private int score = 0;
        private string timer = "";
        private string status = "Tryk for at starte!";

        public bool IsPlayerJoined
        {
            get => isPlayerJoined;
            set
            {
                Dispatcher.Invoke(() =>
                {
                    LabelTimer.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                });
                isPlayerJoined = value;
            }
        }
        public string Timer
        {
            get => timer;
            set
            {
                timer = value;
                OnPropertyChanged("Timer");
            }
        }
        public string Status
        {
            get
            {
                if(int.TryParse(status, out int statusScore))
                {
                    LabelStatus.FontSize = 100;
                    return status;
                }
                else
                {
                    LabelStatus.FontSize = 50;
                    return status;
                }
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public void ResetScore() => score = 0;

        private async void PointButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if (e.State)
            {
                if (mw.isGameStarted && IsPlayerJoined)
                {
                    score++;
                    Status = score.ToString();
                }
                await Task.Delay(MainWindow.outputDelay1);
                OutputFire();
            }
        }

        public async void OutputFire()
        {
            Output.State = true;
            await Task.Delay(MainWindow.outputDelay2);
            Output.State = false;
        }

        private void JoinButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if(e.State)
            {
                IsPlayerJoined = true;
                Status = "0";
                if(!mw.isCountdownStarted && !mw.isGameStarted)
                    mw.StartGame();
            }
        }

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
