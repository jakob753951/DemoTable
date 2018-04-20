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
using System.Windows.Threading;
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

		//The physical IO-Buttons
        public DigitalInput PointButton = new DigitalInput();
        public DigitalInput JoinButton = new DigitalInput();
        public DigitalOutput Output = new DigitalOutput();
		private DispatcherTimer holdTimer = new DispatcherTimer();

		private bool buttonIsHeld = false;
		private int timePassed = 0;

		//Whether or not the player is in the game
		private bool isPlayerJoined = false;
		//The player's score
		private int score = 0;
		//What is shown on the players timerLabel
		private string timer = "";
		//The Player's current status, can contain the score
		private string status = "Tryk for at starte!";
		//Reference to MainWindow, Given in Constructor
		private MainWindow mw;

        public Player(MainWindow mainWindow, int pointChannel, int joinChannel, int outputChannel)
        {
			//Render
            InitializeComponent();

			//Create reference
            mw = mainWindow;

			//Setup bindings
            DataContext = this;
            LabelTimer.DataContext = mw;

			//Set button channels
            PointButton.Channel = pointChannel;
            JoinButton.Channel = joinChannel;
            Output.Channel = outputChannel;

			//Set background image
            GetBackground(outputChannel);

			//Hide Timer
            LabelTimer.Visibility = Visibility.Hidden;
			holdTimer.Interval = new TimeSpan(0, 0, 1);
			holdTimer.Tick += HoldTimer_tick;
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			//Bind Buttons
            PointButton.StateChange += PointButton_StateChange;
			JoinButton.StateChange += JoinButton_StateChange;

			//Open ports
			PointButton.Open();
            JoinButton.Open();
            Output.Open();
        }

        private void GetBackground(int id)
        {
			//Get the background image from [.exe folder]/Resources/Background[1-4].png
			//If it does not exist, just create the resources directory
            if(File.Exists($@"{Environment.CurrentDirectory}\Resources\Background{id}.png"))
                Background = new ImageBrush(new BitmapImage(new Uri($@"{Environment.CurrentDirectory}\Resources\Background{id}.png")));
            else
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\Resources");
        }

        public bool IsPlayerJoined
        {
            get => isPlayerJoined;
            set
            {
				//Make sure to change the Timer visibility when Joining/Leaving
                Dispatcher.Invoke(() => { LabelTimer.Visibility = value ? Visibility.Visible : Visibility.Hidden; });
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
				//If getting a number, change FontSize to 100, else to 50
				LabelStatus.FontSize = int.TryParse(status, out int statusScore) ? 100 : 50;
				return status;
			}
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

		/// <summary>
		/// Fires the output
		/// Uses the outputDelay2 set in mainwindow
		/// </summary>
        public async void OutputFire()
        {
            Output.State = true;
            await Task.Delay(MainWindow.outputDelay2);
            Output.State = false;
        }

        public void ResetScore() => score = 0;

        private async void PointButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if (e.State)
            {
                if (mw.isGameStarted && IsPlayerJoined)
                    Status = score++.ToString();

				await Task.Delay(MainWindow.outputDelay1);
                OutputFire();
            }
        }

        private void JoinButton_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
			buttonIsHeld = e.State;
            if(e.State && !IsPlayerJoined)
            {
				if (!mw.isCountdownStarted && !mw.isGameStarted)
					mw.StartGame();

				IsPlayerJoined = true;
				Status = "0";
            }
        }

		private void HoldTimer_tick(object sender, EventArgs e)
		{

			if(buttonIsHeld)
			{
				if(timePassed == 10)
				{
					OutputFire();
					OutputFire();
					OutputFire();
				}
				else
					timePassed++;
			}
			else
				timePassed = 0;
		}

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
