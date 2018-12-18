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
        
        // The Phidget buttons to score, and join a game, respectively
        public DigitalInput PointButton = new DigitalInput();
        public DigitalInput JoinButton = new DigitalInput();

        //the reference to mainwindow
        private MainWindow mw;

        public Player(MainWindow mainWindow, int pointChannel, int joinChannel)
        {
            InitializeComponent();
            
            //Needed for Databinding
            LabelScore.DataContext = this;
            LabelStatus.DataContext = this;

            //Reset when the game starts
            mainWindow.GameStart += Reset;

            //Setup and open PointButton
            PointButton.Channel = pointChannel;
            PointButton.StateChange += PointButton_StateChange;
            PointButton.Open();

            //Setup and open JoinButton
            JoinButton.Channel = joinChannel;
            JoinButton.StateChange += JoinButton_StateChange;
            JoinButton.Open();

            //Set reference to mainwindow
            mw = mainWindow;
        }

        //Set default values
        private int score = 0;
        private string status = "Tryk for at starte!";

        /// <summary>
        /// The player's current score
        /// </summary>
        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged("Score");
            }
        }

        /// <summary>
        /// The player's status ("Tryk for at starte!")
        /// </summary>
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

        /*
         * Only needed for debugging
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
        */


        /// <summary>
        /// Executed when the PointButton is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if (mw.isGameStarted && isPlayerJoined && e.State)
                Score++;
        }

        /// <summary>
        /// Executed when the JoinButton is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
