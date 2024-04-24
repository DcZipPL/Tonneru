using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Tomlyn;
using Tonneru.Models;
using Tonneru.ViewModels;
using Tonneru.Views;

namespace Tonneru
{
	public partial class App : Application
	{
		public static readonly CultureInfo[] SupportedCultures = {
			CultureInfo.GetCultureInfo("en-US"),
			CultureInfo.GetCultureInfo("pl-PL"),
		};
		
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainWindowViewModel(),
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}