using CommunityToolkit.Mvvm.ComponentModel;
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
        [ObservableProperty] private string _serverAddress = "localhost:5001";
        [ObservableProperty] private string _serverConnectStatus = "Disconnected";


    }
}
