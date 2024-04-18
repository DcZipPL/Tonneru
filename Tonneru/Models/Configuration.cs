namespace Tonneru.Models;

public class Configuration
{
	public SshTable? Ssh { get; set; }
	public TunnelTable? Tunnel { get; set; }
	
	public class SshTable
	{
		public string Host { get; set; } = "";
		public uint Port { get; set; } = 22;
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";
	}
	
	public class TunnelTable
	{
		public string LocalHost { get; set; } = "localhost";
		public string RemoteHost { get; set; } = "localhost";
	
		public uint LocalPort { get; set; } = 8080;
		public uint RemotePort { get; set; } = 80;
	}
}