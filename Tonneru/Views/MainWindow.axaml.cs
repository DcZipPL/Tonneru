using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Tonneru.ViewModels;

namespace Tonneru.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		
		private void HandlePortInput(object? sender, KeyEventArgs e)
		{
			Regex regex = new Regex("(?:NumPad[0-9]|D[0-9])");
			e.Handled = !regex.IsMatch(e.Key.ToString());
		}
	}
}