namespace LteVideoPlayer.Api.Configs
{
    public class ConnectConfigs : IConfig
    {
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }

    public class Connection
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string RootDir { get; set; } = "";
    }
}
