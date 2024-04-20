using AutoMapper;
using LteVideoPlayer.Api.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace LteVideoPlayer.Api.Hubs
{
    public class RemoteHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connectionProfile = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, int> _connectionChannel = new ConcurrentDictionary<string, int>();
        private static readonly ConcurrentDictionary<int, string> _channelConnection = new ConcurrentDictionary<int, string>();
        private static readonly ConcurrentDictionary<string, List<int>> _profileChannels = new ConcurrentDictionary<string, List<int>>();
        private static Random _rand = new Random(DateTime.Now.Second);
        private static int _maxChannels = 1000;

        private const string SEND_ADDCHANNEL = "AddChannel";
        private const string SEND_YOURCHANNEL = "YourChannel";
        private const string SEND_REMOVECHANNEL = "RemoveChannel";
        private const string SEND_RECEIVEVIDEOINFO = "ReceiveVideoInfo";
        private const string SEND_RECEIVESETSEEK = "ReceiveSetSeek";
        private const string SEND_RECEIVEMOVESEEK = "ReceiveMoveSeek";
        private const string SEND_RECEIVEVIDEOPAUSE = "ReceiveVideoPause";
        private const string SEND_RECEIVEVIDEOPLAY = "ReceiveVideoPlay";
        private const string SEND_RECEIVESETVOLUME = "ReceiveSetVolume";
        private const string SEND_ERROR = "Error";

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"OnConnected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var channel = 0;
            if (_connectionProfile.TryGetValue(Context.ConnectionId, out var profile) &&
                _connectionChannel.TryGetValue(Context.ConnectionId, out channel))
            {
                if (RemoveChannel(profile, channel))
                    await Clients.OthersInGroup(profile).SendAsync(SEND_REMOVECHANNEL, channel);
            }
            _connectionProfile.TryRemove(Context.ConnectionId, out var _);
            _connectionChannel.TryRemove(Context.ConnectionId, out var _);
            _channelConnection.TryRemove(channel, out var _);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, profile);

            Console.WriteLine($"OnDisconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinProfile(string profile)
        {
            if (NewChannel(profile, out var channel))
            {
                if (_connectionProfile.TryAdd(Context.ConnectionId, profile) &&
                    _connectionChannel.TryAdd(Context.ConnectionId, channel) &&
                    _channelConnection.TryAdd(channel, Context.ConnectionId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, profile);

                    await Clients.Client(Context.ConnectionId).SendAsync(SEND_YOURCHANNEL, channel);
                    await Clients.OthersInGroup(profile).SendAsync(SEND_ADDCHANNEL, channel);

                    await GetChannels();
                }
                else
                    await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Connection already joined profile");
            }
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Connection could not generate channel");
        }

        public async Task LeaveProfile()
        {
            if (GetConnectionInfo(out var profile, out var channel))
            {
                _connectionProfile.Remove(Context.ConnectionId, out var _);
                _connectionChannel.Remove(Context.ConnectionId, out var _);
                _channelConnection.Remove(channel, out var _);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, profile);

                if (RemoveChannel(profile, channel))
                    await Clients.OthersInGroup(profile).SendAsync(SEND_REMOVECHANNEL, channel);
            }
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Connection must be join profile first");
        }

        public async Task GetChannels()
        {
            if (GetConnectionInfo(out var profile, out var channel))
            {
                if (_profileChannels.TryGetValue(profile, out var allChannels))
                {
                    foreach (var c in allChannels.Where(x => x != channel))
                        await Clients.Client(Context.ConnectionId).SendAsync(SEND_ADDCHANNEL, c);
                }
                else
                    await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Profile has no channels");
            }
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task ResetChannel()
        {
            if (GetConnectionInfo(out var profile, out var channel))
            {
                if (RemoveChannel(profile, channel))
                {
                    _connectionChannel.Remove(Context.ConnectionId, out var _);
                    _channelConnection.Remove(channel, out var _);
                    await Clients.Client(profile).SendAsync(SEND_REMOVECHANNEL, channel);
                }

                if (NewChannel(profile, out channel))
                {
                    if (_connectionChannel.TryAdd(Context.ConnectionId, channel) &&
                        _channelConnection.TryAdd(channel, Context.ConnectionId))
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync(SEND_YOURCHANNEL, channel);
                        await Clients.OthersInGroup(profile).SendAsync(SEND_ADDCHANNEL, channel);
                    }
                    else
                        await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Connection could not keep channel");
                }
                else
                    await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Connection could not generate channel");
            }
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task VideoInfo(RemoteData_VideoInfoDto dto)
        {
            if (GetConnectionInfo(out var profile, out var channel))
                await Clients.OthersInGroup(profile).SendAsync(SEND_RECEIVEVIDEOINFO, dto);
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task SetSeek(RemoteData_SetSeekDto dto)
        {
            if (GetConnectionInfo(out var profile, out var channel))
                await Clients.OthersInGroup(profile).SendAsync(SEND_RECEIVESETSEEK, dto);
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task MoveSeek(RemoteData_MoveSeekDto dto)
        {
            if (GetConnectionInfo(out var profile, out var channel))
                await Clients.OthersInGroup(profile).SendAsync(SEND_RECEIVEMOVESEEK, dto);
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task VideoPause(RemoteData_PauseDto dto)
        {
            if (GetConnectionInfo(out var profile, out var channel))
                await Clients.OthersInGroup(profile).SendAsync(SEND_RECEIVEVIDEOPAUSE, dto);
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task VideoPlay(RemoteData_PlayDto dto)
        {
            if (GetConnectionInfo(out var profile, out var channel))
                await Clients.OthersInGroup(profile).SendAsync(SEND_RECEIVEVIDEOPLAY, dto);
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        public async Task SetVolume(RemoteData_SetVolumeDto dto)
        {
            if (GetConnectionInfo(out var profile, out var channel))
                await Clients.OthersInGroup(profile).SendAsync(SEND_RECEIVESETVOLUME, dto);
            else
                await Clients.Client(Context.ConnectionId).SendAsync(SEND_ERROR, "Must be join profile first");
        }

        private bool GetConnectionInfo(out string profile, out int channel)
        {
            profile = "";
            channel = -1;

            return _connectionProfile.TryGetValue(Context.ConnectionId, out profile) &&
                _connectionChannel.TryGetValue(Context.ConnectionId, out channel);
        }

        private bool NewChannel(string profile, out int channel)
        {
            channel = -1;
            if (!_profileChannels.TryGetValue(profile, out var channels))
            {
                channels = new List<int>();
                if (!_profileChannels.TryAdd(profile, channels))
                    return false;
            }

            if (channels.Count == _maxChannels - 1)
                return false;

            while (channel == -1)
            {
                channel = _rand.Next(1, _maxChannels);
                if (channels.Contains(channel))
                    channel = -1;
            }
            channels.Add(channel);

            return true;
        }

        private bool RemoveChannel(string profile, int channel)
        {
            if (!_profileChannels.TryGetValue(profile, out var channels))
                return false;

            return channels.Remove(channel);
        }

        private class ProfileChannels
        {
            public string Profile { get; set; }
            public List<int> Channels { get; set; }
        }
    }
}
