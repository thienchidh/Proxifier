namespace Proxifier.Models
{
    public class ServerInfo
    {
        public string ServerIp { get; set; } = string.Empty;
        public int ServerPort { get; set; }
        public ServerType ServerType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}