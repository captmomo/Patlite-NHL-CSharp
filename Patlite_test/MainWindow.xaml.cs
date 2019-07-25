using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Windows;

namespace Patlite_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int ReadTimeOut = 1000;

        private string IP_ADDRESS = "192.168.10.1";

        private int PORT = 10000;

        private List<ComboxMode> _lightModeList;

        private List<ComboxMode> _buzzerModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public List<ComboxMode> LightModeList
        {
            get => _lightModeList;
            set
            {
                if (_lightModeList != value)
                {
                    _lightModeList = value;
                    RaisePropertyChanged(nameof(LightModeList));
                }
            }
        }

        public List<ComboxMode> BuzzerModeList
        {
            get => _buzzerModeList;
            set
            {
                if (_buzzerModeList != value)
                {
                    _buzzerModeList = value;
                    RaisePropertyChanged(nameof(BuzzerModeList));
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            LightModeList = new List<ComboxMode>()
            {
                new ComboxMode(){ Mode = LightMode.NO_FLASH, Name= "Off"},
                new ComboxMode(){ Mode = LightMode.STEADY, Name= "Steady"},
                new ComboxMode(){ Mode = LightMode.FLASHING_PATTERN_1, Name= "Pattern 1"},
                new ComboxMode(){ Mode = LightMode.FLASHING_PATTERN_2, Name= "Pattern 2"},
                new ComboxMode(){ Mode = LightMode.LIGHT_REMAIN, Name= "Remain"},
            };

            BuzzerModeList = new List<ComboxMode>()
            {
                new ComboxMode(){ Mode = BuzzerMode.STOP_BUZZER, Name= "Off"},
                new ComboxMode(){ Mode = BuzzerMode.BUZZ_PATTERN_1, Name= "Pattern 1"},
                new ComboxMode(){ Mode = BuzzerMode.BUZZ_PATTERN_2, Name= "Pattern 2"},
                new ComboxMode(){ Mode = BuzzerMode.BUZZ_PATTERN_3, Name= "Pattern 3"},
                new ComboxMode(){ Mode = BuzzerMode.BUZZ_PATTERN_4, Name= "Pattern 4"},
                new ComboxMode(){ Mode = BuzzerMode.BUZZ_REMAIN, Name= "Remain"},
            };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //CommandResult result = SendWriteCommand(LightMode.FLASHING_PATTERN_1, LightMode.FLASHING_PATTERN_2, LightMode.NO_FLASH, BuzzerMode.BUZZ_PATTERN_3, "192.168.10.1", 10000);
            //if (result.result)
            //{
            //    ReadResult status = SendReadCommand("192.168.10.1", 10000);
            //    if (status.result)
            //    {
            //        Console.WriteLine(status.responseString);
            //    }
            //}
        }



        private string[] SetData(string red, string amber, string green, string buzzer)
        {
            string[] DataControl = new string[] { red, amber, green, "00", "00", buzzer };

            return DataControl;
        }



        private ReadResult GetStatus(string[] response)
        {
            LightMode.STATUS red = (LightMode.STATUS)(int.Parse(response[0]));
            LightMode.STATUS amber = (LightMode.STATUS)(int.Parse(response[1]));
            LightMode.STATUS green = (LightMode.STATUS)(int.Parse(response[2]));
            BuzzerMode.STATUS buzzer = (BuzzerMode.STATUS)(int.Parse(response[5]));
            ReadResult result = new ReadResult(red.ToString(), amber.ToString(), green.ToString(), buzzer.ToString())
            {
                result = true
            };
            result.responseString = result.ToString();

            return result;

        }

        private CommandResult GetResponse(string response)
        {
            string[] resCode = response.Split('-').ToArray();

            if (resCode[0] == "41" && resCode[1] == "43" && resCode[2] == "4B" || resCode[0] == "06")
            {
                return new CommandResult { result = true, responseString = "ACK" };
            }
            if (resCode[0] == "15")
            {
                return new CommandResult { result = false, responseString = "NAK" };
            }

            return new CommandResult { result = false, responseString = "NAK" };
        }
        private byte[] BasicCommand(string[] code)
        {
            return code.Select(x => Convert.ToByte(x, 16)).ToArray();

        }

        private ReadResult SendReadCommand(string ipAddress, int port)
        {
            ReadResult returnResult = new ReadResult(LightMode.LIGHT_REMAIN, LightMode.LIGHT_REMAIN, LightMode.LIGHT_REMAIN, BuzzerMode.BUZZ_REMAIN)
            {
                result = false
            };

            try
            {
                using (TcpClient tcpClient = CreateClient(ipAddress, port))
                {
                    NetworkStream stream = tcpClient.GetStream();

                    stream.ReadTimeout = ReadTimeOut; //1 second read timeout

                    byte[] commandBytes = BasicCommand(BasicCode.READ);

                    stream.Write(commandBytes, 0, commandBytes.Length);

                    Console.WriteLine(BitConverter.ToString(commandBytes));

                    byte[] data = new byte[6];

                    int bitLength = stream.Read(data, 0, data.Length);

                    string response = BitConverter.ToString(data);

                    return GetStatus(response.Split('-').ToArray());
                }
            }

            catch (SocketException SocketEx)
            {
                Console.WriteLine($"{SocketEx.GetBaseException().Message}");
                returnResult.responseString = SocketEx.GetBaseException().Message;
                return returnResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetBaseException().Message}");
                returnResult.responseString = ex.GetBaseException().Message;
                return returnResult;
            }

        }

        private CommandResult SendClearCommand(string ipAddress, int port)
        {
            CommandResult returnResult = new CommandResult { result = false };
            try
            {
                using (TcpClient tcpClient = CreateClient(ipAddress, port))
                {
                    NetworkStream stream = tcpClient.GetStream();

                    stream.ReadTimeout = ReadTimeOut; //1 second read timeout

                    byte[] commandBytes = BasicCommand(BasicCode.CLEAR);

                    stream.Write(commandBytes, 0, commandBytes.Length);

                    Console.WriteLine(BitConverter.ToString(commandBytes));

                    byte[] data = new byte[6];

                    int bitLength = stream.Read(data, 0, data.Length);

                    string response = BitConverter.ToString(data);

                    return GetResponse(response);
                }
            }

            catch (SocketException SocketEx)
            {
                Console.WriteLine($"{SocketEx.GetBaseException().Message}");
                returnResult.responseString = SocketEx.GetBaseException().Message;
                return returnResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetBaseException().Message}");
                returnResult.responseString = ex.GetBaseException().Message;
                return returnResult;
            }

        }



        private CommandResult SendWriteCommand(string red, string amber, string green, string buzzer, string IpAddress, int port)
        {
            CommandResult returnResult = new CommandResult
            {
                result = false
            };
            try
            {
                using (TcpClient tcpClient = CreateClient(IpAddress, port))
                {
                    NetworkStream stream = tcpClient.GetStream();

                    stream.ReadTimeout = ReadTimeOut; //1 second read timeout

                    byte[] clearCommand = BasicCode.CLEAR.Select(x => Convert.ToByte(x, 16)).ToArray();

                    stream.Write(clearCommand, 0, clearCommand.Length);

                    Console.WriteLine(BitConverter.ToString(clearCommand));

                    byte[] data = new byte[6];

                    int bitLength = stream.Read(data, 0, data.Length);

                    string response = BitConverter.ToString(data);

                    if (!GetResponse(response).result)
                    {
                        returnResult.responseString = "Failed to carry out clear command";
                        return returnResult;
                    }

                    string[] DataCommand = SetData(red, amber, green, buzzer);

                    string[] commandArray = BasicCode.WRITE_HEADER.Concat(DataCommand).ToArray();

                    Console.WriteLine(string.Join("-", commandArray));

                    byte[] commandBytes = commandArray.Select(x => Convert.ToByte(x, 16)).ToArray();

                    Console.WriteLine(BitConverter.ToString(commandBytes));


                    stream.Write(commandBytes, 0, commandBytes.Length);

                    data = new byte[1];

                    bitLength = stream.Read(data, 0, data.Length);

                    response = BitConverter.ToString(data);

                    return GetResponse(response);
                }
            }
            catch (SocketException SocketEx)
            {
                Console.WriteLine($"{SocketEx.GetBaseException().Message}");
                returnResult.responseString = SocketEx.GetBaseException().Message;
                return returnResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetBaseException().Message}");
                returnResult.responseString = ex.GetBaseException().Message;
                return returnResult;

            }
        }

        private TcpClient CreateClient(string IpAddress, int port)
        {
            return new TcpClient(IpAddress, port);
        }
        private void Connect(string server, int port)
        {


            TcpClient client = new TcpClient(server, port);

            string[] DataCommand = SetData(LightMode.FLASHING_PATTERN_1, LightMode.STEADY, LightMode.FLASHING_PATTERN_2, BuzzerMode.BUZZ_PATTERN_2);

            string[] commandArray = BasicCode.WRITE_HEADER.Concat(DataCommand).ToArray();

            //var stringArray = string.Join("-", commandArray);
            Console.WriteLine(string.Join("-", commandArray));
            byte[] testCommand = commandArray.Select(x => Convert.ToByte(x, 16)).ToArray();


            NetworkStream stream = client.GetStream();

            stream.Write(testCommand, 0, testCommand.Length);
            Console.WriteLine(BitConverter.ToString(testCommand));
            byte[] data = new byte[6];

            int bitLength = stream.Read(data, 0, data.Length);

            string response = BitConverter.ToString(data);

            Console.WriteLine($"{response}");
            string[] resCode = response.Split('-').ToArray();
            ReadResult reply = GetStatus(resCode);
            Console.WriteLine(reply);
            if (resCode[0] == "41" && resCode[1] == "43" && resCode[2] == "4B" || resCode[0] == "06")
            {
                Console.WriteLine("ACK");
            }
            if (resCode[0] == "15")
            {
                Console.WriteLine("NAK");
            }

            byte[] clearCommand = BasicCode.CLEAR.Select(x => Convert.ToByte(x, 16)).ToArray();


            stream.Write(clearCommand, 0, clearCommand.Length);

            Console.WriteLine(BitConverter.ToString(clearCommand));

            data = new byte[6];

            bitLength = stream.Read(data, 0, data.Length);

            response = BitConverter.ToString(data);

            Console.WriteLine($"{response}");
            resCode = response.Split('-').ToArray();
            reply = GetStatus(resCode);
            Console.WriteLine(reply);
            if (resCode[0] == "41" && resCode[1] == "43" && resCode[2] == "4B" || resCode[0] == "06")
            {
                Console.WriteLine("ACK");
            }
            if (resCode[0] == "15")
            {
                Console.WriteLine("NAK");
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = SendWriteCommand(RedCombo.SelectedValue.ToString(), AmberCombo.SelectedValue.ToString(), GreenCombo.SelectedValue.ToString(), BuzzerCombo.SelectedValue.ToString(), IP_ADDRESS, PORT).responseString;
        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = SendReadCommand(IP_ADDRESS, PORT).responseString;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            statusText.Text = SendClearCommand(IP_ADDRESS, PORT).responseString;
        }
    }
}
