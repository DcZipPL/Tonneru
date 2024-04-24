using Avalonia;
using Avalonia.Controls;
using Tonneru.ViewModels;

namespace Tonneru.Views;

public partial class PreferencesWindow : Window
{
	public PreferencesWindow()
	{
		InitializeComponent();
	}
	
	public static PreferencesWindow OpenPreferences()
	{
		var window = new PreferencesWindow
		{
			DataContext = new PreferencesWindowViewModel()
		};
		window.Show();
		return window;
	}

	private void ChangeLanguage(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (DataContext is not PreferencesWindowViewModel model)
			return;
		model.ApplyCulture();
	}
}