using System.Diagnostics;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ReactiveUI;

namespace Tonneru.Views.Controls;

public class Hyperlink : Button
{
	public Hyperlink()
	{
		Click += (_, _) => LinkClick();
	}

	public static readonly StyledProperty<string> LinkProperty = AvaloniaProperty.Register<Hyperlink, string>(nameof(Link));

	public string Link
	{
		get => GetValue(LinkProperty);
		set => SetValue(LinkProperty, value);
	}

	public void LinkClick()
	{
		Process.Start(new ProcessStartInfo(Link) { UseShellExecute = true });
	}
}