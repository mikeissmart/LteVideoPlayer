using AutoMapper;
using LteVideoPlayer.Api.Models.Dtos.Remote;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LteVideoPlayer.Api.Hubs
{
    public class RemoteHub : Hub
    {
        private static readonly List<Connection> _connections = new List<Connection>();
        private static readonly Random _rand = new Random(DateTime.Now.Second);

        private const string SEND_ADDCHANNEL = "AddChannel";
        private const string SEND_YOURCHANNEL = "YourChannel";
        private const string SEND_REMOVECHANNEL = "RemoveChannel";
        private const string SEND_RECEIVEVIDEOINFO = "ReceiveVideoInfo";
        private const string SEND_RECEIVEASKFORVIDEOINFO = "ReceiveAskForVideoInfo";
        private const string SEND_RECEIVESETSEEK = "ReceiveSetSeek";
        private const string SEND_RECEIVEMOVESEEK = "ReceiveMoveSeek";
        private const string SEND_RECEIVEPLAYPAUSE = "ReceivePlayPause";
        private const string SEND_RECEIVESETVOLUME = "ReceiveSetVolume";
        private const string SEND_ERROR = "Error";

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"OnConnected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"OnConnected: {Context.ConnectionId}");

            var con = RemoveConnection();
            if (con != null)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, con.Profile);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinProfile(string profile)
        {
            if (string.IsNullOrEmpty(profile) || string.IsNullOrWhiteSpace(profile))
            {
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Attempting to join NULL profile");
                return;
            }

            var con = CreateConnection(profile);
            if (con == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Connection already joined profile");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, profile);

            await Clients.Client(Context.ConnectionId).SendAsync(SEND_YOURCHANNEL, con.Channel);
            await Clients.OthersInGroup(profile).SendAsync(SEND_ADDCHANNEL, con.Channel);

            await GetChannels();
        }

        public async Task LeaveProfile()
        {
            var con = RemoveConnection();
            if (con == null)
            {
                await SendNoProfileErrorAsync();
                return;
            }

            await Groups.RemoveFromGroupAsync(con.ConnectionId, con.Profile);
            await Clients.OthersInGroup(con.Profile).SendAsync(SEND_REMOVECHANNEL, con.Channel);
        }

        public async Task ResetChannel()
        {
            var con = RemoveConnection();
            if (con == null)
            {
                await SendNoProfileErrorAsync();
                return;
            }
            await Clients.OthersInGroup(con.Profile).SendAsync(SEND_REMOVECHANNEL, con.Channel);

            con = CreateConnection(con.Profile)!;
            await Clients.Client(Context.ConnectionId).SendAsync(SEND_YOURCHANNEL, con.Channel);
            await Clients.OthersInGroup(con.Profile).SendAsync(SEND_ADDCHANNEL, con.Channel);
        }

        public async Task GetChannels()
        {
            var con = GetConnection();
            if (con == null)
            {
                await SendNoProfileErrorAsync();
                return;
            }

            foreach (var cc in GetAllConnections(con.Profile))
            {
                if (cc.ConnectionId == Context.ConnectionId)
                    continue;

                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ADDCHANNEL, cc.Channel);
            }
        }

        public async Task VideoInfo(RemoteVideoInfoDto dto)
        {
            await SendCommandAsync(dto, SEND_RECEIVEVIDEOINFO, dto);
        }

        public async Task AskForVideoInfo(RemoteDto dto)
        {
            await SendCommandAsync(dto, SEND_RECEIVEASKFORVIDEOINFO, dto);
        }

        public async Task SetSeek(RemoteSetSeekDto dto)
        {
            await SendCommandAsync(dto, SEND_RECEIVESETSEEK, dto);
        }

        public async Task MoveSeek(RemoteMoveSeekDto dto)
        {
            await SendCommandAsync(dto, SEND_RECEIVEMOVESEEK, dto);
        }

        public async Task SetVolume(RemoteSetVolumeDto dto)
        {
            await SendCommandAsync(dto, SEND_RECEIVESETVOLUME, dto);
        }

        public async Task PlayPause(RemoteDto dto)
        {
            await SendCommandAsync(dto, SEND_RECEIVEPLAYPAUSE, dto);
        }

        private async Task SendNoProfileErrorAsync()
            => await Clients.Client(Context.ConnectionId).SendAsync("Error", "Must be join profile first");

        private async Task SendCommandAsync(RemoteDto dto, string command, object? message)
        {
            var con = GetConnection();
            if (con == null)
            {
                await SendNoProfileErrorAsync();
                return;
            }

            if (dto.ToChannel != null)
            {
                con = GetAllConnections(dto.Profile).FirstOrDefault(x => x.Channel == dto.ToChannel);
                if (con == null)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("Error", $"Cant find channel '{dto.ToChannel}', to send command '{command}'");
                    return;
                }
                await Clients.Client(con.ConnectionId).SendAsync(command, dto);
            }
            else
                await Clients.Groups(dto.Profile).SendAsync(command, dto);
        }

        private Connection? CreateConnection(string profile)
        {
            lock (_connections)
            {
                if (_connections.Any(x => x.ConnectionId == Context.ConnectionId))
                    return null;

                var channels = _connections.Select(x => x.Channel).ToList();
                var channel = -1;

                while (channel == -1)
                {
                    channel = _rand.Next(1, 1000);
                    if (channels.Contains(channel))
                        channel = -1;
                }

                var con = new Connection
                {
                    Profile = profile,
                    ConnectionId = Context.ConnectionId,
                    Channel = channel
                };
                _connections.Add(con);

                return con;
            }
        }

        private Connection? RemoveConnection()
        {
            Connection? con;
            lock (_connections)
            {
                con = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (con != null)
                    _connections.Remove(con);
            }

            return con;
        }

        private Connection? GetConnection()
        {
            Connection? con;
            lock (_connections)
            {
                con = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            }

            return con;
        }

        private List<Connection> GetAllConnections(string profile)
        {
            lock (_connections)
            {
                return _connections.Where(x => x.Profile == profile).ToList();
            }
        }

        private class Connection
        {
            public required string Profile { get; set; }
            public required string ConnectionId { get; set; }
            public int Channel { get; set; }
        }
    }
}
