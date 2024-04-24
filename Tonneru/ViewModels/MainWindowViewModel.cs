using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using Renci.SshNet;
using Renci.SshNet.Common;
using Tomlyn;
using Tonneru.Models;
using Tonneru.Views;

namespace Tonneru.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private const uint OPEN_HEIGHT = 436;
		private const uint COLLAPSED_HEIGHT = 80;
		
		private SshClient? client;
		private ForwardedPortRemote? portForwarder;
		private bool forceDisconnect = false;
		private ConnectionStatus _status = ConnectionStatus.Disconnect;
		private uint _height = COLLAPSED_HEIGHT;
		private uint _width = 500;
		private uint _port = 22, _localPort = 8080, _remotePort = 80;
		private string _host = "", _boundHost = "", _remoteHost = "", _username = "", _password = "";
		
		private ConnectionStatus Status
		{
			get => _status;
			set
			{
				_status = value;
				
				this.RaisePropertyChanged(nameof(StatusTitle));
				this.RaisePropertyChanged(nameof(StatusDot));
				this.RaisePropertyChanged(nameof(ConnectText));
				this.RaisePropertyChanged(nameof(CanEdit));
			}
		}
		private string ConnectionError { get; set; } = string.Empty;

		public ReactiveCommand<Unit, Unit> TryConnectCommand { get; }
		public ReactiveCommand<Unit, Unit> ToggleEditCommand { get; }
		public ReactiveCommand<Unit, Unit> SaveConfigCommand { get; }
		public ReactiveCommand<Unit, Unit> ClearCommand { get; }
		
		public MainWindowViewModel()
		{
			if (File.Exists("tunnel.toml"))
			{
				try
				{
					var toml = File.ReadAllText("tunnel.toml");
					var model = Toml.ToModel<Configuration>(toml);
					this.Host = model.Ssh!.Host;
					this.Port = model.Ssh!.Port;
					this.Username = model.Ssh!.Username;
					this.Password = model.Ssh!.Password;
					this.LocalHost = model.Tunnel!.LocalHost;
					this.RemoteHost = model.Tunnel!.RemoteHost;
					this.LocalPort = model.Tunnel!.LocalPort;
					this.RemotePort = model.Tunnel!.RemotePort;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					ConnectionError = e.Message;
					Status = ConnectionStatus.Error;
				}
			}
			
			TryConnectCommand = ReactiveCommand.Create(TryConnect);
			ToggleEditCommand = ReactiveCommand.Create(ToggleEdit);
			SaveConfigCommand = ReactiveCommand.Create(SaveConfig);
			ClearCommand = ReactiveCommand.Create(Clear);
		}
		
		public bool CanEdit => Status == ConnectionStatus.Disconnect || Status == ConnectionStatus.Error;

		public string StatusTitle
		{
			get
			{
				return Status switch
				{
					ConnectionStatus.Disconnect => "Disconnected",
					ConnectionStatus.Connecting => "Connecting...",
					ConnectionStatus.Waiting => "Waiting...",
					ConnectionStatus.Active => "Active",
					ConnectionStatus.Error => ConnectionError,
					_ => "Unknown"
				};
			}
		}

		public IBrush StatusDot => Status switch
		{
			ConnectionStatus.Disconnect => Brushes.Gray,
			ConnectionStatus.Connecting => Brushes.Yellow,
			ConnectionStatus.Waiting => Brushes.DodgerBlue,
			ConnectionStatus.Active => Brushes.LimeGreen,
			ConnectionStatus.Error => Brushes.Red,
			_ => Brushes.Black
		};

		public object ConnectText
		{
			get
			{
				return Status switch
				{
					ConnectionStatus.Disconnect => "Connect",
					ConnectionStatus.Connecting => "Force Disconnect",
					ConnectionStatus.Waiting => "Disconnect",
					ConnectionStatus.Active => "Disconnect",
					ConnectionStatus.Error => "Reconnect",
					_ => "Unknown"
				};
			}
		}
		
		public void TryConnect()
		{
			switch (Status)
			{
				case ConnectionStatus.Disconnect:
					Connect();
					break;
				case ConnectionStatus.Connecting:
					forceDisconnect = true;
					break;
				case ConnectionStatus.Waiting:
					Disconnect();
					break;
				case ConnectionStatus.Active:
					Disconnect();
					break;
				case ConnectionStatus.Error:
					Connect();
					break;
			}
		}

		public void ToggleEdit()
		{
			if (Height == COLLAPSED_HEIGHT)
			{
				Height = OPEN_HEIGHT;
			}
			else
			{
				Height = COLLAPSED_HEIGHT;
			}
		}
		
		public void SaveConfig()
		{
			var config = new Configuration
			{
				Ssh = new Configuration.SshTable
				{
					Host = Host,
					Port = Port,
					Username = Username,
					Password = Password
				},
				Tunnel = new Configuration.TunnelTable
				{
					LocalHost = LocalHost,
					RemoteHost = RemoteHost,
					LocalPort = LocalPort,
					RemotePort = RemotePort
				}
			};
			var toml = Toml.FromModel(config);
			File.WriteAllText("tunnel.toml", toml);
		}

		public void Clear()
		{
			this.Host = "";
			this.Port = 0;
			this.Username = "";
			this.Password = "";
			this.LocalHost = "";
			this.RemoteHost = "";
			this.LocalPort = 0;
			this.RemotePort = 0;
		}

		public SshClient CreateClient()
		{
			client = new SshClient(Host, (ushort)Port, Username, Password);
			client.ErrorOccurred += OnError;
			portForwarder = new ForwardedPortRemote(RemoteHost, (ushort)RemotePort, LocalHost, (ushort)LocalPort);
			return client;
		}

		public void Connect()
		{
			Status = ConnectionStatus.Connecting;

			Task.Run(WaitForConnection);
		}

		public void WaitForConnection()
		{
			try
			{
				CreateClient().Connect();
				Console.WriteLine("Connected!");
			}
			catch (ArgumentException e)
			{
				Console.WriteLine(e);
				ConnectionError = "Invalid argument: "+e.Message;
				Status = ConnectionStatus.Error;
				return;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				ConnectionError = e.Message;
				Status = ConnectionStatus.Error;
				return;
			}
			for (int i = 0; i < 100; i++)
			{
				if (forceDisconnect)
				{
					Disconnect();
					forceDisconnect = false;
					return;
				}
				if (client.IsConnected)
				{
					Status = ConnectionStatus.Active;
					client.AddForwardedPort(portForwarder);
					Console.WriteLine("Port Forwarding Started!");
					portForwarder.Start();
					if (forceDisconnect)
					{
						Disconnect();
						forceDisconnect = false;
					}
					return;
				}
				Task.Delay(1000);
			}
		}

		public void Disconnect()
		{
			Console.WriteLine("Disconnecting...");
			portForwarder.Stop();
			if (client == null)
			{
				Status = ConnectionStatus.Disconnect;
				return;
			}
			if (client.IsConnected)
			{
				client.RemoveForwardedPort(portForwarder);
				client.Disconnect();
			}
			Status = ConnectionStatus.Disconnect;
		}
		
		private void OnError(object? sender, ExceptionEventArgs e)
		{
			ConnectionError = e.Exception.Message;
			Console.WriteLine(e.Exception);
			Disconnect();
			Status = ConnectionStatus.Error;
		}
		
		// THE BOILERPLATE (my favorite part)
		public uint Width
		{
			get => _width;
			set => this.RaiseAndSetIfChanged(ref _width, value);
		}
		public uint Height
		{
			get => _height;
			set => this.RaiseAndSetIfChanged(ref _height, value);
		}
		public string Host
		{
			get => _host;
			set => this.RaiseAndSetIfChanged(ref _host, value);
		}
		[Range(0, 65535, ErrorMessage = "Invalid port, 0-65535")]
		public uint Port
		{
			get => _port;
			set => this.RaiseAndSetIfChanged(ref _port, value);
		}

		public string Username
		{
			get => _username;
			set => this.RaiseAndSetIfChanged(ref _username, value);
		}

		public string Password
		{
			get => _password;
			set => this.RaiseAndSetIfChanged(ref _password, value);
		}
		
		public string LocalHost
		{
			get => _boundHost;
			set => this.RaiseAndSetIfChanged(ref _boundHost, value);
		}
		public string RemoteHost
		{
			get => _remoteHost;
			set => this.RaiseAndSetIfChanged(ref _remoteHost, value);
		}
		[Range(0, 65535, ErrorMessage = "Invalid port, 0-65535")]
		public uint LocalPort
		{
			get => _localPort;
			set => this.RaiseAndSetIfChanged(ref _localPort, value);
		}
		[Range(0, 65535, ErrorMessage = "Invalid port, 0-65535")]
		public uint RemotePort
		{
			get => _remotePort;
			set => this.RaiseAndSetIfChanged(ref _remotePort, value);
		}

		public ComboBoxItem[] ProfileItems => new[]
		{
			new ComboBoxItem() { Content = "Profile 1" },
		};

		public void OpenPreferences() => PreferencesWindow.OpenPreferences();
	}
}