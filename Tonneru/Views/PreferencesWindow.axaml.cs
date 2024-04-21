using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tonneru.Views;

public partial class PreferencesWindow : Window
{
	public PreferencesWindow()
	{
		InitializeComponent();
	}
	
	public static PreferencesWindow OpenPreferences()
	{
		var window = new PreferencesWindow();
		window.Show();
		return window;
	}
}