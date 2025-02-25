using LteVideoPlayer.Api.Configs;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace LteVideoPlayer.Api.Services
{
    public class ConnectService : IDisposable
    {
        private readonly ConnectConfigs _connectConfig;
        private readonly List<NetworkConnection> _connections;

        public ConnectService(ConnectConfigs config)
        {
            _connectConfig = config;
            _connections = new List<NetworkConnection>();
        }

        public void ConnectAll()
        {
            foreach (var connection in _connections)
                connection.Dispose();
            _connections.Clear();

            try
            {
                foreach (var config in _connectConfig.Connections)
                    _connections.Add(new NetworkConnection(config));
            }
            catch (Exception ex)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            foreach (var connection in _connections)
                connection.Dispose();
            _connections.Clear();
        }

        public class NetworkConnection : IDisposable
        {
            public Connection Connection { get; private set; }

            public NetworkConnection(Connection connection)
            {
                Connection = connection;

                var netResource = new NetResource()
                {
                    Scope = ResourceScope.GlobalNetwork,
                    ResourceType = ResourceType.Disk,
                    DisplayType = ResourceDisplaytype.Share,
                    RemoteName = connection.RootDir
                };

                var credentials = new NetworkCredential(
                        @$"\{Connection.Username}",
                        Connection.Password);

                var userName = string.IsNullOrEmpty(credentials.Domain)
                    ? credentials.UserName
                    : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

                var result = WNetAddConnection2(
                    netResource,
                    credentials.Password,
                    userName,
                    0);

                if (result != 0)
                {
                    throw new Win32Exception(result);
                }
            }

            NetworkConnection()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                WNetCancelConnection2(Connection.RootDir, 0, true);
            }

            [DllImport("mpr.dll")]
            private static extern int WNetAddConnection2(NetResource netResource,
                string password, string username, int flags);

            [DllImport("mpr.dll")]
            private static extern int WNetCancelConnection2(string name, int flags,
                bool force);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }
    }
}
