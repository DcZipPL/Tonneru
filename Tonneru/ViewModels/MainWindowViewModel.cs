using Avalonia.Media;
using Color = System.Drawing.Color;

namespace Tonneru.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public string Status => "Disconnected";
		public IBrush StatusDot => new SolidColorBrush(Colors.LimeGreen);
		public ushort LocalPort => 8080;
		public ushort RemotePort => 80;
	}
}