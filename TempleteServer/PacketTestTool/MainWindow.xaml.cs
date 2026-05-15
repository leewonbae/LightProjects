using GameServer.Handlers;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snowpipe.Commons.Packets;
using System.CodeDom;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
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
    public enum E_PACKET_TYPE
    {
        REQ,
        RES
    }

    public partial class MainWindow : Window
    {
        private string NEED_SELECT_PACKET_TEXT = "--패킷을 선택해주세요 --";

        private Dictionary<string, Type> _reqPacketTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, Type> _resPacketTypeDict = new Dictionary<string, Type>();

        private string _currentPacketName; // 현재 편집 중인 객체를 보관
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

            _reqPacketTypeDict.Clear();
            _resPacketTypeDict.Clear();

            var serverProjectAssembly = Assembly.Load("GameServer");

            var handlerTypeList = serverProjectAssembly.GetTypes()
                .Where(i => i.IsClass && !i.IsAbstract)
                .Where(i => i.GetInterfaces().Any(s => s.IsGenericType && s.GetGenericTypeDefinition() == typeof(IHandler<,>)))
                .ToList();

            if (handlerTypeList.Count == 0)
            {
                MessageBox.Show("패킷 클래스를 찾지 못했습니다.");
            }

            // 빈 오브젝트 미리 추가
            cbPacketTypes.Items.Add(NEED_SELECT_PACKET_TEXT);
            _reqPacketTypeDict.Add(NEED_SELECT_PACKET_TEXT, typeof(object));
            _resPacketTypeDict.Add(NEED_SELECT_PACKET_TEXT, typeof(object));

            // 패킷 이름 별로, req, res 패킷 추가 
            foreach (var handlerType in handlerTypeList)
            {
                var handlerInterface = handlerType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>));

                var arguments = handlerInterface.GenericTypeArguments;

                var reqType = arguments[0];
                var resType = arguments[1];

                var key = handlerType.Name.Replace("Handler", "").ToLower();

                _reqPacketTypeDict.Add(key, reqType);
                _resPacketTypeDict.Add(key, resType);

                cbPacketTypes.Items.Add(key);
            }

            cbPacketTypes.SelectedIndex = 0;
        }

        private void cbPacketTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var packetName = cbPacketTypes.SelectedItem as string;
            if (packetName.IsNullOrEmpty())
            {
                pgRequest.SelectedObject = null;
                _currentRequestInstance = null;
                _currentPacketName = string.Empty;
            }
            else
            {
                var selectedPacketType = _reqPacketTypeDict[packetName];

                _currentPacketName = packetName;
                _currentRequestInstance = Activator.CreateInstance(selectedPacketType);
                pgRequest.SelectedObject = _currentRequestInstance;
            }
        }

        private async void btnSendPacket_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentPacketName) || _currentPacketName.Equals(NEED_SELECT_PACKET_TEXT))
            {
                MessageBox.Show("패킷을 선택해주세요");

                return;
            }

            string packetBody = JsonConvert.SerializeObject(_currentRequestInstance);
            var baseReqPacket = new BaseReqPacket()
            {
                PacketBody = packetBody
            };

            var jsonPayLoad = JsonConvert.SerializeObject(baseReqPacket);

            var url = $"http://{txtBaseServerIp.Text}:{txtBaseServerPort.Text}/action/DoJson?packetName={_currentPacketName}&jsonBody={packetBody}";

            HttpClient httpClient = new HttpClient();

            var content = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();



            try
            {
                // 1. 전체 베이스 패킷을 JObject로 먼저 파싱
                var root = JObject.Parse(result);

                if (root["errorCode"] != null)
                {
                    txtErrorCode.Text = Enum.GetName(typeof(E_PACKET_ERROR_CODE), root["errorCode"].Value<int>());
                }

                // 2. packetBody가 문자열 형태의 JSON으로 들어가 있다면 실제 JSON 객체로 복원
                if (root["packetBody"] != null && root["packetBody"].Type == JTokenType.String)
                {
                    string bodyStr = root["packetBody"].ToString();
                    try
                    {
                        // 이 단계에서 \u0022나 역슬래시 이스케이프가 완전히 제거된 깨끗한 객체가 됩니다.
                        root["packetBody"] = JToken.Parse(bodyStr);
                    }
                    catch
                    {
                        // 만약 packetBody 내부가 JSON 형식이 아니면 파싱하지 않고 원문 그대로 둡니다.
                    }
                }

                // 3. Newtonsoft.Json의 Formatting.Indented 옵션으로 이쁘게 줄바꿈하여 출력
                txtResponseData.Text = root.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                // 파싱에 실패하면 디버깅을 위해 일단 원문이라도 출력합니다.
                txtResponseData.Text = $"[Parsing Error]: {ex.Message}\n\n[Raw Data]:\n{result}";
            }
        }
    }
}