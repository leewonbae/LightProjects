using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacketTestTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, Type> _packetTypeDict = new Dictionary<string, Type>();
        private object _currentRequestInstance; // 현재 편집 중인 객체를 보관
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPacketTypes();
        }

        private void LoadPacketTypes()
        {
            cbPacketTypes.Items.Clear(); // 기존 아이템 초기화
            _packetTypeDict.Clear();

            var serverProjectAssembly = Assembly.Load("pray-server");

            var packetTypeList = serverProjectAssembly.GetTypes()
                .Where(i => i.IsClass && !i.IsAbstract)
                .Where(i => i.Name.StartsWith("Req"))
                .ToList();

            if (packetTypeList.Count == 0)
            {
                MessageBox.Show("패킷 클래스를 찾지 못했습니다.");
            }

            cbPacketTypes.Items.Add("-- 패킷을 선택해주세요 --");
            _packetTypeDict.Add("-- 패킷을 선택해주세요 --", typeof(object));
            foreach (var type in packetTypeList)
            {
                _packetTypeDict.Add(type.Name, type);
                cbPacketTypes.Items.Add(type.Name);
            }

            cbPacketTypes.SelectedIndex = 0;
        }

        private void cbPacketTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var packetName = cbPacketTypes.SelectedItem as string;
            if(packetName.IsNullOrEmpty())
            {
                pgRequest.SelectedObject = null;
            }
            else
            {
                var selectedPacketType = _packetTypeDict[packetName];

                _currentRequestInstance = Activator.CreateInstance(selectedPacketType);
                pgRequest.SelectedObject = _currentRequestInstance;
                // 2. 인스턴스를 JSON 문자열로 직렬화
                //var options = new JsonSerializerOptions
                //{
                //    WriteIndented = true // 보기 좋게 들여쓰기 적용
                //};

                //string jsonString = JsonSerializer.Serialize(instance, options);
            }
        }

        private void btnSendPacket_Click(object sender, RoutedEventArgs e)
        {
            string jsonPayload = JsonSerializer.Serialize(_currentRequestInstance);

            MessageBox.Show(jsonPayload);
        }
    }
}