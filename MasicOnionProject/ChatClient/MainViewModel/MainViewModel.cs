using ChatClient.NewFolder;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.MainViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private ChatReceiver _chatReceiver = new ChatReceiver();

        [ObservableProperty] private string _serverAddress = "localhost:5001";
        [ObservableProperty] private string _serverConnectStatus = "Disconnected";

        [RelayCommand]
        public async Task ConnectToServer()
        {
            // 여기에 구현
            var result = await _chatReceiver.OnConnected(_serverAddress);
            if (result)
            {
                ServerConnectStatus = "Connected";
            }
        }

    }
}
