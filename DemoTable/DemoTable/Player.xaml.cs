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

namespace DemoTable
{
	/// <summary>
	/// Interaction logic for Player.xaml
	/// </summary>
	public partial class Player : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public Player(MainWindow mainwindow)
		{
			InitializeComponent();
			LabelScore.DataContext = this;
			mainwindow.GameEnd += Reset;
		}

		private int score = 0;
		public int Score
		{
			get => score;
			set
			{
				score = value;
				OnPropertyChanged("Score");
			}
		}

		private void Reset(object sender, EventArgs e) => Score = 0;

		private void ButtonScore_Click(object sender, RoutedEventArgs e) => Score++;

		private void ButtonJoin_Click(object sender, RoutedEventArgs e)
		{

		}
		private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
	}
}
