using LteVideoPlayer.Api.Configs;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace LteVideoPlayer.Api
{
    public class ShareConnect : IDisposable
    {
        public string ConnectOutput = "";
        private readonly VideoConfig _videoConfig;
        private NetworkConnection? _connection;

        public ShareConnect(VideoConfig videoConfig)
        {
            _videoConfig = videoConfig;
        }

        public void Connect()
        {
            if (_connection != null)
                _connection.Dispose();

            try
            {
                _connection = new NetworkConnection(_videoConfig.RootPath,
                    new NetworkCredential(
                        @"\sambauser",
                        "SecretPassword1"));
            }
            catch (Exception ex)
            {
                ConnectOutput = ex.Message;
                _connection?.Dispose();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            _connection = null;
        }

        public class NetworkConnection : IDisposable
        {
            string _networkName;

            public NetworkConnection(string networkName,
                NetworkCredential credentials)
            {
                _networkName = networkName;

                var netResource = new NetResource()
                {
                    Scope = ResourceScope.GlobalNetwork,
                    ResourceType = ResourceType.Disk,
                    DisplayType = ResourceDisplaytype.Share,
                    RemoteName = networkName
                };

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
                WNetCancelConnection2(_networkName, 0, true);
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

        /*public void AttemptShareConnect(ConfigurationManager configuration)
        {
            var config = configuration.GetSection(nameof(VideoConfig)).Get<VideoConfig>();
            var hasAccess = false;
            try
            {
                hasAccess = Directory.Exists(config.RootPath);
            }
            catch
            {
                // Tring to check access threw error
            }

            if (!hasAccess)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $@"net use s: ""{config.RootPath}"" /user:\sambauser SecretPassword1 /persistent:no",
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                    };

                    var output = new StringBuilder();
                    using (var process = new Process())
                    {
                        process.StartInfo = startInfo;
                        process.ErrorDataReceived += (s, e) =>
                        {
                            lock (output)
                            {
                                output.AppendLine(e.Data);
                            }
                        };

                        process.Start();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                    ConnectOutput = "Map created";
                    ;
                }
                catch (Exception ex)
                {
                    ConnectOutput = ex.Message;
                }
            }
            else
                ConnectOutput = "Already have access";
        }

        public void Dispose()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $@"net delete s: ""{config.RootPath}"" /user:\sambauser SecretPassword1 /persistent:no",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };

                var output = new StringBuilder();
                using (var process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += (s, e) =>
                    {
                        lock (output)
                        {
                            output.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
                ConnectOutput = "Map created";
                ;
            }
            catch (Exception ex)
            {
                ConnectOutput = ex.Message;
            }
        }*/
    }
}
