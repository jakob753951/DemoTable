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

namespace DemoTable
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool isPlayerJoined = false;

        public DigitalInput PointButton = new DigitalInput();
        public DigitalInput JoinButton = new DigitalInput();

        private MainWindow mw;

        public Player(MainWindow mainWindow, int pointChannel, int joinChannel)
        {
            InitializeComponent();

            LabelScore.DataContext = this;
            LabelStatus.DataContext = this;

            mainWindow.GameStart += Reset;

            PointButton.Channel = pointChannel;
            PointButton.StateChange += PointButton_StateChange;
            PointButton.Open();

            JoinButton.Channel = joinChannel;
            JoinButton.StateChange += JoinButton_StateChange;
            JoinButton.Open();

            mw = mainWindow;
        }

        private int score = 0;
        private string status = "Tryk for at starte!";

        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged("Score");
            }
        }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private void Reset(object sender, EventArgs e) => Score = 0;

        private void ButtonScore_Click(object sender, RoutedEventArgs e)
        {
            if (mw.isGameStarted && isPlayerJoined)
                Score++;
        }

        private void ButtonJoin_Click(object sender, RoutedEventArgs e)
        {
            if(!mw.isGameStarted)
            {
                isPlayerJoined = true;
                Status = "Klar";
                if(!mw.isCountdownStarted)
                    mw.StartGame();
            }
        }

        private void PointButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if (mw.isGameStarted && isPlayerJoined && e.State)
                Score++;
        }

        private void JoinButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if(e.State && !mw.isGameStarted)
            {
                isPlayerJoined = true;
                Status = "Klar";
                if(!mw.isCountdownStarted)
                    mw.StartGame();
            }
        }

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
