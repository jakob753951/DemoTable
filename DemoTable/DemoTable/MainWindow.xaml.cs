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

		public double timer = 10;
		public MainWindow()
		{
			InitializeComponent();
			for (int i = 0; i < 4; i++)
			{
				players.Add(new Player(this));
				(MainGrid.Children[i+1] as Frame).Content = players[i];
			}
			countDown.Interval = new TimeSpan(0,0,0,0,1);
			countDown.Tick += CountDown_Tick;
		}
		private void CountDown_Tick(object sender, EventArgs e)
		{
			if(timer > 0)
			{
				timer -= 0.001;
			}
			else
			{
				countDown.Stop();
				GameEnd.Invoke(this, new EventArgs());
				timer = 10;
			}
		}
	}
}
