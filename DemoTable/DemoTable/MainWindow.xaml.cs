using Phidget22;
using Phidget22.Events;
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

namespace DemoTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer countDown = new DispatcherTimer();

        public delegate void GameEndEventHandler(object sender, EventArgs e);
        public event GameEndEventHandler GameEnd;
        private List<Player> players = new List<Player>();
        public bool isCountdownStarted = false;
        public bool isGameStarted = false;

        DigitalInput diPlayer1Btn = new DigitalInput();
        DigitalInput diStartBtn = new DigitalInput();

        public double timer = 10;
        public MainWindow()
        {
            InitializeComponent();
            for(int i = 0; i < 4; i++)
            {
                players.Add(new Player(this));
                (MainGrid.Children[i + 1] as Frame).Content = players[i];
            }
            countDown.Interval = new TimeSpan(0, 0, 0, 0, 1);
            countDown.Tick += CountDown_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Phidget.InvokeEventCallbacks = true;

            diPlayer1Btn.Channel = 0;
            diPlayer1Btn.StateChange += DiPlayer1Btn_StateChange;
            diPlayer1Btn.Open();

            diStartBtn.Channel = 1;
            diStartBtn.StateChange += DiPlayer1StartBtn_StateChange;
            diStartBtn.Open();
        }

        private void DiPlayer1StartBtn_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if(!isCountdownStarted && !isGameStarted && e.State)
            {
                players[0].isPlayerJoined = true;
                GameStart();
            }
        }
        private void DiPlayer1Btn_StateChange(object sender, DigitalInputStateChangeEventArgs e)
        {
            if(isGameStarted && e.State)
            {
                players[0].Score++;
            }
        }

        public void GameStart()
        {
            if(!isCountdownStarted)
            {
                countDown.Start();
                isCountdownStarted = true;
            }
        }

        private void CountDown_Tick(object sender, EventArgs e)
        {
            if(isCountdownStarted)
            {
                timer -= 1;
                if(timer < 0)
                {
                    isCountdownStarted = false;
                    isGameStarted = true;
                    timer = 5;
                }
            }

            if(isGameStarted)
            {
                timer -= 1;
                if(timer < 0)
                {
                    isGameStarted = false;
                    countDown.Stop();
                    GameEnd.Invoke(this, new EventArgs());
                    timer = 10;
                }
            }
        }
    }
}