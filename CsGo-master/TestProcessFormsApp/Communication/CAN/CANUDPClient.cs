using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JLRScan;
//using TestProcessFormsApp.Boards;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TestProcessFormsApp.Frm;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Collections.Concurrent;
using System.Threading;
using System.Security.Cryptography;
using System.Collections;
using CCWin.SkinClass;
using System.Globalization;
using System.Windows.Forms;
using static JLRScan.AppGlobal;
using System.Windows.Input;
using TestProcessFormsApp.Log;
using JLRScan.Log;
using FastReport.Utils;

namespace TestProcessFormsApp.Communication.CAN
{

    //    [JsonObject(MemberSerialization.OptIn)]
    public class CANUDPClient
    {

        #region 属性
        public class UpdateMessage
        {
            public string Name { get; set; }
            public bool Sendflag { get; set; }
            public DateTime UpdateTime { get; set; }
        }
        private UdpClient udpClient;
        private UdpClient udpRevClient;
        private IPEndPoint remoteEndPoint;
        private IPEndPoint remoteRevEndPoint;
        public CanReceiver canReceiver = new CanReceiver();
        public string ip { get; set; }
        [DisplayName("发送端口")]
        public int port { get; set; }
        [DisplayName("监听端口")]
        public int Listingport { get; set; }
        public static bool StopSendfalg = false;
        public static int RevFlag = 0;
        //public CANUDPClient(string ip, int port)
        //{
        //    udpClient = new UdpClient();
        //    remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        //}
        private string _name;
        [TypeConverter(typeof(ClassConverter<CANInitSetting>))]
        public CANInitSetting CAN初始化设置 { set; get; } = new CANInitSetting();
        [TypeConverter(typeof(ClassConverter<Message>))]
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
        private readonly ConcurrentDictionary<string, (byte[] data, string hash)> _snapshot
       = new ConcurrentDictionary<string, (byte[], string)>();
        public Dictionary<UpdateMessage, byte[]> TransmitterDictionary = new Dictionary<UpdateMessage, byte[]>();
        public Dictionary<string, byte[]> SignalReadBytes = new Dictionary<string, byte[]>();
        public Dictionary<string, double> SignalDictionary = new Dictionary<string, double>();
        public Dictionary<string, Message> ReadMesDictionary = new Dictionary<string, Message>();
        public static Dictionary<string, DateTime> CanIdReadTime = new Dictionary<string, DateTime>();
        // 2026-04-23: UDS请求上下文池。发送线程发出请求后，在接收线程中按规则匹配响应。
        private readonly object _udsRequestLock = new object();
        private readonly List<UdsRequestContext> _pendingUdsRequests = new List<UdsRequestContext>();
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
        public void Init(string ip, int port, int listingport)
        {
            this.ip = ip;
            this.port = port;
            InitSignal();
            udpClient = new UdpClient();
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            udpRevClient = new UdpClient();

            StartListening(listingport);
        }
        #region 接收方法
        private bool isListening = false;
        public void InitSignal()
        {
            try
            {
                SignalDictionary.Clear();
                TransmitterDictionary.Clear();
                SignalReadBytes.Clear();
                CanIdReadTime.Clear();
                string name = "";
                foreach (var mes in Messages)
                {
                    if (mes.WriteOrRead == WriteOrRead.Read)
                    {
                        Console.WriteLine(mes.Name);
                        ReadMesDictionary.Add(mes.Name, mes);
                        foreach (var signal in mes.Signals)
                        {
                            if(mes.ReadType == ReadType.Signals)
                            {
                                Console.WriteLine(mes.Name + " " + signal.Name);
                                SignalDictionary.Add(signal.Name, 0);
                            }
                        }
                    }
                    if (mes.WriteOrRead == WriteOrRead.Write)
                    {
                        Console.WriteLine(mes.Name);
                        TransmitterDictionary.Add(new CANUDPClient.UpdateMessage() { Name = mes.Name, Sendflag = false, UpdateTime = DateTime.Now }, StringToBytes(mes.Transmitter));
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        // 初始化并启动监听
        public void StartListening(int port)
        {
            try
            {
                udpRevClient = new UdpClient(port);
                remoteRevEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port); // 监听任意来源
             //   udpRevClient.Client.ReceiveBufferSize = 65536 * 4; // 256KB                                                            //   udpRevClient.Connect(IPAddress.Parse("127.0.0.1"), 17212);
                isListening = true;

                // 启动异步接收线程
                ThreadPool.QueueUserWorkItem(ReceiveLoop);
                Console.WriteLine($"开始监听端口 {port}...");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"监听失败: {ex.Message}");
            }
        }

        // 停止监听
        public void StopListening()
        {
            isListening = false;
            udpClient?.Close();
            Console.WriteLine("监听已停止");
        }

        // 接收循环
        private void ReceiveLoop(object state)
        {
            while (isListening)
            {
             //   Thread.Sleep(1);
                try
                {
                    // 同步接收数据（可改用异步 BeginReceive/EndReceive）
                    byte[] receivedBytes = udpRevClient.Receive(ref remoteRevEndPoint);
                    //   Console.WriteLine($"received {receivedBytes.Length} bytes");
                    // 处理接收到的数据
                    ReceiveCallback(receivedBytes);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    // 正常退出时忽略
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"接收异常: {ex.Message}");
                }
            }
        }
        private void ReceiveCallback(byte[] receivedBytes)
        {
            // 基础验证
            if (receivedBytes.Length < 5)
                return; // 最小错误帧长度
          //  if(ip == "192.168.1.2")
         //   Console.WriteLine("result：" + string.Join(" ", receivedBytes.Select(it => it.ToString("X").PadLeft(2, '0'))));
            if ( string.Join(" ", receivedBytes.Select(it => it.ToString("X").PadLeft(2, '0'))).Contains("22 F0 B4"))
            {

            }
            if (receivedBytes[0] != 0x55 || receivedBytes[1] != 0xAA)
                return;
             
            byte frameType = receivedBytes[2];
            try
            {
                switch (frameType)
                {
                    case 0x02: // CAN数据帧
                        RevFlag++;
                        // 2026-04-23: 先尝试走UDS上下文分发，不影响原有业务报文解析逻辑。
                        if (TryExtractUdsPayload(receivedBytes, out uint udsCanId, out byte[] udsPayload))
                        {
                            DispatchUdsResponse(udsCanId, udsPayload);
                        }
                        //if (receivedBytes[4] == 0x67)
                        //{
                        //    SecurityUnlocker.Unlock(receivedBytes[4], this, receivedBytes);
                        //}
                        byte[] newArray = new byte[3]
                        {
                            receivedBytes[9],
                            receivedBytes[10],
                            receivedBytes[11]
                        };
                        
                        //SendMesTemp.TryPeek(out var mes2);
                        //Console.WriteLine("SendMesTemp：" + string.Join(" ", StringToBytes(mes2.Transmitter).Select(it => it.ToString("X").PadLeft(2, '0'))));
                        //Console.WriteLine(SendMesTemp.Count);



                        ///{屏蔽
                        //foreach (var rev in SendMesTemp)
                        //{
                        //    if (IsPositiveResponse(rev.Name, newArray) && rev.SendType == SendType.Diagnosis)
                        //    {
                        //        if (rev.WriteOrRead == WriteOrRead.Write)
                        //            rev.IsPositiveResponse = true;
                        //        foreach (var findmes in ReadMesDictionary)//查找是哪条节点的正响应
                        //        {
                        //            if (rev.ReadType == ReadType.Signals && findmes.Key == rev.Name)
                        //            {
                        //                ParseCANFrame(findmes.Value, receivedBytes, OnMessageReceived);
                        //                rev.IsPositiveResponse = true;
                        //                break;
                        //            }
                        //            if (rev.ReadType == ReadType.Bytes && findmes.Key == rev.Name)
                        //            {
                        //                if(SignalReadBytes.ContainsKey(rev.Name))
                        //                {
                        //                    SignalReadBytes[rev.Name] = ExtractData(receivedBytes);
                        //                }
                        //                else
                        //                {
                        //                    SignalReadBytes.Add(rev.Name, ExtractData(receivedBytes));
                        //                }
                        //                rev.IsPositiveResponse = true;
                        //                break;
                        //            }
                        //        }
                        //        //rev.IsPositiveResponse = true;
                        //        break;
                        //    }
                        //}
                       ///屏蔽}
                        uint canId = (uint)(receivedBytes[3] << 24) + (uint)(receivedBytes[4] << 16) + (uint)(receivedBytes[5] << 8) + (uint)(receivedBytes[6]);
                        uint canIdRead = ((uint)(receivedBytes[6] << 24) + (uint)(receivedBytes[5] << 16) + (uint)(receivedBytes[4] << 8) + (uint)(receivedBytes[3])) & 0x1FFFFFFF;
                        if (CanIdReadTime.ContainsKey($"0x{canIdRead:X8}"))
                        {
                            CanIdReadTime[$"0x{canIdRead:X8}"] = DateTime.Now;
                        }
                        else
                        {
                            CanIdReadTime.Add($"0x{canIdRead:X8}", DateTime.Now);
                        }
                        CanMessageBus.Publish(new CanMessageBus.RawCanMessage
                        {
                            Timestamp = DateTime.Now,
                            Direction = "RX",
                            ID = $"0x{canId:X8}",
                            Data = receivedBytes
                        });
                      //  Console.WriteLine($"0x{canId:X8}");
                        break;
                    case 0xFF: // 错误帧
                        ParseErrorFrame(receivedBytes);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }
        byte[] ExtractData(byte[] receivedBytes)
        {
            // 验证数组长度至少为 14（索引12 + 最小数据长度0 + 末尾CRC的2字节）
            if (receivedBytes.Length < 14)
                throw new ArgumentException("receivedBytes must be at least 14 bytes long");

            // 计算数据长度（小端字节序：receivedBytes[7]为低位，receivedBytes[8]为高位）
            int dataLength = receivedBytes[7] + (receivedBytes[8] << 8) - 3;

            // 数据结束位置 = 起始位置 + 数据长度
            int endIndex = 9 + dataLength;

            // 验证数据长度不超过数组范围（必须留出最后2字节给CRC）
            //if (endIndex >( receivedBytes.Length - 2) )
            //    throw new ArgumentException($"Data length exceeds available space. Expected max: {receivedBytes.Length - 14}, Actual: {dataLength}");

            // 提取数据（从索引12开始，截取dataLength个字节）
            byte[] result = new byte[dataLength];
            Array.Copy(receivedBytes, 12, result, 0, dataLength);
            return result;
        }
        private void ParseCANFrame(Message Mes, byte[] data, Action<Message, byte[]> handler)
        {
            // 数据长度验证
            int dataLen = data[7];
            int expectedLength = 2 + 1 + 4 + 2 + dataLen + 2; // 帧头+类型+CANID+DataLen+Data+CRC
            if (data.Length != expectedLength) return;

            // CRC验证
            int crcOffset = 9 + dataLen;
            ushort receivedCRC = BitConverter.ToUInt16(data, crcOffset);
            ushort calculatedCRC = CalculateCRC16(data, 0, crcOffset - 1);

            if (receivedCRC != calculatedCRC) return;

            // 解析数据
            uint canId = BitConverter.ToUInt32(data, 3);
            byte[] payload = new byte[dataLen];
            Buffer.BlockCopy(data, 9 + 3, payload, 0, dataLen - 3);

            handler?.Invoke(Mes, payload);
        }
        public static bool IsPositiveResponse(string receivedString, byte[] originalRequest)
        {
            // Step 1: 将字符串转换为字节数组
            byte[] receivedBytes;
            try
            {
                receivedBytes = receivedString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(s => Convert.ToByte(s, 16))
                                              .ToArray();
            }
            catch
            {
                // 格式错误（非十六进制或无效字符）
                return false;
            }

            // Step 2: 验证长度和正响应标识符
            //if (receivedBytes.Length != 3 || originalRequest.Length != 3)
            //{
            //    return false;
            //}

            // 第一个字节是正响应标识符 0x62，且后两个字节与原请求一致
            return receivedBytes[0] + 0x40 == originalRequest[0] &&
                   receivedBytes[1] == originalRequest[1] &&
                   receivedBytes[2] == originalRequest[2];
        }

        // 2026-04-23: 单次UDS请求的等待状态。负责处理正响应、负响应、0x78等待中。
        private class UdsRequestContext
        {
            private readonly AutoResetEvent _signal = new AutoResetEvent(false);
            private readonly object _stateLock = new object();

            public byte RequestSid { get; private set; }
            public byte ExpectedPositiveSid { get; private set; }
            public byte? ExpectedPositiveSubFunction { get; private set; }
            public uint? ExpectedResponseCanId { get; private set; }

            public bool IsCompleted { get; private set; }
            public bool IsResponsePending { get; private set; }
            public bool IsNegativeResponse { get; private set; }
            public byte NegativeResponseCode { get; private set; }
            public byte[] ResponsePayload { get; private set; }

            public UdsRequestContext(byte requestSid, byte expectedPositiveSid, byte? expectedPositiveSubFunction, uint? expectedResponseCanId)
            {
                RequestSid = requestSid;
                ExpectedPositiveSid = expectedPositiveSid;
                ExpectedPositiveSubFunction = expectedPositiveSubFunction;
                ExpectedResponseCanId = expectedResponseCanId;
            }

            public bool TryAccept(uint canId, byte[] udsPayload)
            {
                if (udsPayload == null || udsPayload.Length == 0)
                {
                    return false;
                }

                if (ExpectedResponseCanId.HasValue && ExpectedResponseCanId.Value != canId)
                {
                    return false;
                }

                int udsOffset = DetectUdsStartIndex(udsPayload);
                if (udsOffset < 0 || udsOffset >= udsPayload.Length)
                {
                    return false;
                }

                bool matched = false;
                lock (_stateLock)
                {
                    if (IsCompleted)
                    {
                        return false;
                    }

                    if (udsPayload[udsOffset] == 0x7F)
                    {
                        if (udsPayload.Length < udsOffset + 3 || udsPayload[udsOffset + 1] != RequestSid)
                        {
                            return false;
                        }

                        matched = true;
                        IsResponsePending = udsPayload[udsOffset + 2] == 0x78;
                        IsNegativeResponse = !IsResponsePending;
                        NegativeResponseCode = udsPayload[udsOffset + 2];
                        ResponsePayload = (byte[])udsPayload.Clone();
                        IsCompleted = !IsResponsePending;
                    }
                    else
                    {
                        if (udsPayload[udsOffset] != ExpectedPositiveSid)
                        {
                            return false;
                        }

                        if (ExpectedPositiveSubFunction.HasValue)
                        {
                            if (udsPayload.Length < udsOffset + 2 || udsPayload[udsOffset + 1] != ExpectedPositiveSubFunction.Value)
                            {
                                return false;
                            }
                        }

                        matched = true;
                        IsResponsePending = false;
                        IsNegativeResponse = false;
                        NegativeResponseCode = 0;
                        ResponsePayload = (byte[])udsPayload.Clone();
                        IsCompleted = true;
                    }
                }

                if (matched)
                {
                    _signal.Set();
                }

                return matched;
            }

            public bool Wait(int timeoutMs)
            {
                return _signal.WaitOne(timeoutMs);
            }

            public void GetState(out bool isCompleted, out bool isResponsePending, out bool isNegativeResponse, out byte negativeResponseCode, out byte[] responsePayload)
            {
                lock (_stateLock)
                {
                    isCompleted = IsCompleted;
                    isResponsePending = IsResponsePending;
                    isNegativeResponse = IsNegativeResponse;
                    negativeResponseCode = NegativeResponseCode;
                    responsePayload = ResponsePayload == null ? null : (byte[])ResponsePayload.Clone();
                }
            }
        }

        // 支持三种UDS数据格式:
        // 1) 纯UDS: [SID, ...]
        // 2) 单帧PCI: [Len(0x0N), SID, ...]
        // 3) 下位机扩展格式: [0x00, Len, SID, ...]
        private static int DetectUdsStartIndex(byte[] payload)
        {
            if (payload == null || payload.Length == 0)
            {
                return -1;
            }

            // 2026-05-08: 兼容下位机返回的 "00 xx + UDS" 格式。
            // 例如: 00 21 67 0D ...
            if (payload.Length >= 3 && payload[0] == 0x00)
            {
                int wrapperLen = payload[1];
                if (wrapperLen > 0 && wrapperLen <= payload.Length - 2)
                {
                    TraceUdsCompat($"[UDS] wrapper matched(00 xx + UDS), len=0x{wrapperLen:X2}, payload={FormatBytesForLog(payload)}");
                    return 2;
                }

                // 某些设备会在后面补0xFF，长度字段与UDP载荷不完全一致，这里放宽处理。
                if (IsLikelyUdsSid(payload[2]))
                {
                    TraceUdsCompat($"[UDS] wrapper matched(00 xx + UDS, relaxed), len=0x{wrapperLen:X2}, payload={FormatBytesForLog(payload)}");
                    return 2;
                }
            }

            if (payload.Length >= 2)
            {
                int pciType = (payload[0] >> 4) & 0x0F;
                int singleFrameLen = payload[0] & 0x0F;
                if (pciType == 0x0 && singleFrameLen > 0 && singleFrameLen <= payload.Length - 1)
                {
                    return 1;
                }
            }

            return 0;
        }

        private static bool IsLikelyUdsSid(byte b)
        {
            // UDS SID常见范围，包含负响应0x7F。
            return (b >= 0x10 && b <= 0x3E) || b == 0x7F || (b >= 0x50 && b <= 0x7E);
        }

        private static string FormatBytesForLog(byte[] data, int maxBytes = 24)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            int take = Math.Min(maxBytes, data.Length);
            string hex = BitConverter.ToString(data, 0, take).Replace("-", " ");
            if (take < data.Length)
            {
                hex += $" ...(+{data.Length - take}B)";
            }
            return hex;
        }

        private static void TraceUdsCompat(string message)
        {
            try
            {
                Console.WriteLine(message);
            }
            catch
            {
            }

            try
            {
                LogHelper.Info(message);
            }
            catch
            {
            }
        }

        private static bool TryGetUdsSid(byte[] payload, out int udsOffset, out byte sid)
        {
            udsOffset = DetectUdsStartIndex(payload);
            sid = 0;
            if (udsOffset < 0 || payload == null || payload.Length <= udsOffset)
            {
                return false;
            }

            sid = payload[udsOffset];
            return true;
        }

        private void RegisterUdsRequest(UdsRequestContext context)
        {
            lock (_udsRequestLock)
            {
                _pendingUdsRequests.Add(context);
            }
        }

        private void UnregisterUdsRequest(UdsRequestContext context)
        {
            lock (_udsRequestLock)
            {
                _pendingUdsRequests.Remove(context);
            }
        }

        private void DispatchUdsResponse(uint canId, byte[] udsPayload)
        {
            UdsRequestContext[] contexts;
            lock (_udsRequestLock)
            {
                contexts = _pendingUdsRequests.ToArray();
            }

            if (contexts.Length == 0)
            {
                return;
            }

            bool matchedAny = false;
            foreach (var context in contexts)
            {
                // 每条响应最多命中一个上下文，命中后立即退出。
                if (!context.TryAccept(canId, udsPayload))
                {
                    continue;
                }

                matchedAny = true;
                context.GetState(out bool completed, out bool pending, out bool negative, out byte nrc, out byte[] response);
                if (completed && !pending)
                {
                    UnregisterUdsRequest(context);
                }
                break;
            }

            if (!matchedAny)
            {
                TraceUdsCompat($"[UDS] response not matched, canId=0x{canId:X8}, contexts={contexts.Length}, payload={FormatBytesForLog(udsPayload)}");
            }
        }

        private bool TryExtractUdsPayload(byte[] frame, out uint canId, out byte[] udsPayload)
        {
            canId = 0;
            udsPayload = null;

            if (frame == null || frame.Length < 11)
            {
                return false;
            }

            int dataLen = frame[7] + (frame[8] << 8);
            int expectedLength = 2 + 1 + 4 + 2 + dataLen + 2;
            if (dataLen <= 0 || frame.Length < expectedLength)
            {
                return false;
            }

            int crcOffset = 9 + dataLen;
            if (crcOffset + 1 >= frame.Length)
            {
                return false;
            }

            ushort receivedCrc = BitConverter.ToUInt16(frame, crcOffset);
            ushort calculatedCrc = CalculateCRC16(frame, 0, crcOffset - 1);
            if (receivedCrc != calculatedCrc)
            {
                return false;
            }

            // 解析为纯CAN ID（屏蔽扩展位/诊断位/CANFD位标志）。
            canId = ((uint)(frame[6] << 24) + (uint)(frame[5] << 16) + (uint)(frame[4] << 8) + (uint)frame[3]) & 0x1FFFFFFF;
            udsPayload = new byte[dataLen];
            Buffer.BlockCopy(frame, 9, udsPayload, 0, dataLen);
            return true;
        }

        // 2026-04-23: 通用UDS请求-响应等待入口（支持0x7F负响应和0x78延时响应）。
        public bool TrySendUdsAndWait(
            FrameType frameType,
            uint requestCanId,
            SendType sendType,
            CANSelectType canSelectType,
            byte[] requestPayload,
            byte expectedPositiveSid,
            byte? expectedPositiveSubFunction,
            out byte[] responsePayload,
            out byte negativeResponseCode,
            out string error,
            int p2TimeoutMs = 500,
            int p2StarTimeoutMs = 5000,
            uint? expectedResponseCanId = null)
        {
            responsePayload = Array.Empty<byte>();
            negativeResponseCode = 0;
            error = string.Empty;

            if (requestPayload == null || requestPayload.Length == 0)
            {
                error = "UDS请求数据为空";
                return false;
            }

            if (p2TimeoutMs <= 0)
            {
                p2TimeoutMs = 500;
            }
            if (p2StarTimeoutMs <= 0)
            {
                p2StarTimeoutMs = 5000;
            }

            if (!TryGetUdsSid(requestPayload, out int requestUdsOffset, out byte requestSid))
            {
                error = "UDS请求格式异常，无法解析SID";
                return false;
            }
            _ = requestUdsOffset;

            var context = new UdsRequestContext(requestSid, expectedPositiveSid, expectedPositiveSubFunction, expectedResponseCanId);
            RegisterUdsRequest(context);
            try
            {
                TraceUdsCompat($"[UDS] send req canId=0x{requestCanId:X8}, reqSid=0x{requestSid:X2}, expPosSid=0x{expectedPositiveSid:X2}, expSub={(expectedPositiveSubFunction.HasValue ? $"0x{expectedPositiveSubFunction.Value:X2}" : "null")}, expRespCanId={(expectedResponseCanId.HasValue ? $"0x{expectedResponseCanId.Value:X8}" : "null")}, payload={FormatBytesForLog(requestPayload)}");
                SendCANMessage(frameType, requestCanId, sendType, canSelectType, requestPayload);

                // waitPending = true 代表收到了0x78，此时超时窗口切换到P2*。
                bool waitPending = false;
                DateTime deadline = DateTime.Now.AddMilliseconds(p2TimeoutMs);
                while (true)
                {
                    int remain = (int)(deadline - DateTime.Now).TotalMilliseconds;
                    if (remain <= 0)
                    {
                        error = waitPending
                            ? $"UDS等待超时（P2*={p2StarTimeoutMs}ms）"
                            : $"UDS等待超时（P2={p2TimeoutMs}ms）";
                        TraceUdsCompat($"[UDS] timeout(deadline), reqSid=0x{requestSid:X2}, expPosSid=0x{expectedPositiveSid:X2}, expRespCanId={(expectedResponseCanId.HasValue ? $"0x{expectedResponseCanId.Value:X8}" : "null")}");
                        return false;
                    }

                    if (!context.Wait(remain))
                    {
                        error = waitPending
                            ? $"UDS等待超时（P2*={p2StarTimeoutMs}ms）"
                            : $"UDS等待超时（P2={p2TimeoutMs}ms）";
                        TraceUdsCompat($"[UDS] timeout(wait), reqSid=0x{requestSid:X2}, expPosSid=0x{expectedPositiveSid:X2}, expRespCanId={(expectedResponseCanId.HasValue ? $"0x{expectedResponseCanId.Value:X8}" : "null")}");
                        return false;
                    }

                    context.GetState(out bool completed, out bool pending, out bool negative, out byte nrc, out byte[] payload);
                    if (pending)
                    {
                        TraceUdsCompat($"[UDS] response pending(0x78), reqSid=0x{requestSid:X2}, payload={FormatBytesForLog(payload)}");
                        waitPending = true;
                        deadline = DateTime.Now.AddMilliseconds(p2StarTimeoutMs);
                        continue;
                    }

                    if (!completed)
                    {
                        continue;
                    }

                    responsePayload = payload ?? Array.Empty<byte>();
                    if (negative)
                    {
                        negativeResponseCode = nrc;
                        TraceUdsCompat($"[UDS] negative response nrc=0x{nrc:X2}, payload={FormatBytesForLog(responsePayload)}");
                        error = $"UDS负响应 NRC=0x{nrc:X2} {GetNrcDescription(nrc)}";
                        return false;
                    }

                    TraceUdsCompat($"[UDS] positive response, reqSid=0x{requestSid:X2}, payload={FormatBytesForLog(responsePayload)}");
                    return true;
                }
            }
            finally
            {
                UnregisterUdsRequest(context);
            }
        }

        public bool TryUdsSecurityAccess(
            FrameType frameType,
            uint requestCanId,
            CANSelectType canSelectType,
            int securityLevel,
            Func<byte[], int, byte[]> keyGenerator,
            out string error,
            uint? expectedResponseCanId = null,
            int p2TimeoutMs = 500,
            int p2StarTimeoutMs = 5000,
            bool enterExtendedSessionBeforeUnlock = true,
            bool useSingleFramePci = true)
        {
            error = string.Empty;

            if (keyGenerator == null)
            {
                error = "未提供Seed-Key算法";
                return false;
            }

            if (enterExtendedSessionBeforeUnlock)
            {
                byte[] sessionRequest = useSingleFramePci
                    ? new byte[] { 0x10, 0x03 }
                    : new byte[] { 0x10, 0x03 };

                if (!TrySendUdsAndWait(
                    frameType,
                    requestCanId,
                    SendType.Diagnosis,
                    canSelectType,
                    sessionRequest,
                    expectedPositiveSid: 0x50,
                    expectedPositiveSubFunction: 0x03,
                    responsePayload: out byte[] sessionResponse,
                    negativeResponseCode: out byte sessionNrc,
                    error: out string sessionError,
                    p2TimeoutMs: p2TimeoutMs,
                    p2StarTimeoutMs: p2StarTimeoutMs,
                    expectedResponseCanId: expectedResponseCanId))
                {
                    error = $"进入扩展会话失败: {sessionError}";
                    return false;
                }
            }

            byte subFunction = (byte)(securityLevel & 0xFF);
            if ((subFunction & 0x01) == 0)
            {
                error = "securityLevel必须是奇数（请求Seed阶段）";
                return false;
            }

            byte[] seedRequest = useSingleFramePci
                ? new byte[] {  0x27, subFunction }
                : new byte[] { 0x27, subFunction };

            // 步骤1: 请求seed（奇数子功能）
            byte[] seedResponse;
            if (!TrySendUdsAndWait(
                frameType,
                requestCanId,
                SendType.Diagnosis,
                canSelectType,
                seedRequest,
                expectedPositiveSid: 0x67,
                expectedPositiveSubFunction: subFunction,
                responsePayload: out seedResponse,
                negativeResponseCode: out byte seedNrc,
                error: out string seedError,
                p2TimeoutMs: p2TimeoutMs,
                p2StarTimeoutMs: p2StarTimeoutMs,
                expectedResponseCanId: expectedResponseCanId))
            {
                error = seedError;
                return false;
            }

            if (seedResponse.Length < 3)
            {
                error = "Seed响应长度异常";
                return false;
            }

            int seedOffset = DetectUdsStartIndex(seedResponse);
            if (seedOffset < 0 || seedResponse.Length < seedOffset + 3)
            {
                error = "Seed响应格式异常";
                return false;
            }

            // 正响应格式:
            // 纯UDS: [67, SubFunction, Seed...]
            // 单帧PCI: [Len, 67, SubFunction, Seed...]
            byte[] seed = seedResponse.Skip(seedOffset + 2).ToArray();
            byte[] key = keyGenerator(seed, securityLevel);
            if (key == null || key.Length == 0)
            {
                error = "Seed-Key算法未返回有效密钥";
                return false;
            }

            byte keySubFunction = (byte)(subFunction + 1);
            byte[] keyRequest;
            if (useSingleFramePci)
            {
                int udsLen = 2 + key.Length;
                //if (udsLen > 0x0F)
                //{
                //    error = "Key长度超过单帧PCI限制，请切换ISO-TP多帧";
                //    return false;
                //}

                keyRequest = new byte[udsLen];
             //   keyRequest[0] = (byte)(udsLen - 4);
                keyRequest[0] = 0x27;
                keyRequest[1] = keySubFunction;
                Buffer.BlockCopy(key, 0, keyRequest, 2, key.Length);
            }
            else
            {
                keyRequest = new byte[2 + key.Length];
                keyRequest[0] = 0x27;
                keyRequest[1] = keySubFunction;
                Buffer.BlockCopy(key, 0, keyRequest, 2, key.Length);
            }

            // 步骤2: 发送key（偶数子功能）
            if (!TrySendUdsAndWait(
                frameType,
                requestCanId,
                SendType.Diagnosis,
                canSelectType,
                keyRequest,
                expectedPositiveSid: 0x67,
                expectedPositiveSubFunction: keySubFunction,
                responsePayload: out byte[] keyResponse,
                negativeResponseCode: out byte keyNrc,
                error: out string keyError,
                p2TimeoutMs: p2TimeoutMs,
                p2StarTimeoutMs: p2StarTimeoutMs,
                expectedResponseCanId: expectedResponseCanId))
            {
                error = keyError;
                return false;
            }

            return true;
        }

        // 2026-04-24: UDS 0x2F (InputOutputControlByIdentifier)。
        // controlOptionRecord通常包含 controlParameter(00/01/02/03) 及可选控制数据。
        public bool TryUds2F(
            FrameType frameType,
            uint requestCanId,
            CANSelectType canSelectType,
            ushort did,
            byte[] controlOptionRecord,
            out byte[] responsePayload,
            out byte negativeResponseCode,
            out string error,
            uint? expectedResponseCanId = null,
            int p2TimeoutMs = 500,
            int p2StarTimeoutMs = 5000,
            bool useSingleFramePci = true)
        {
            responsePayload = Array.Empty<byte>();
            negativeResponseCode = 0;
            error = string.Empty;

            if (controlOptionRecord == null || controlOptionRecord.Length == 0)
            {
                error = "2F请求至少需要1字节controlOptionRecord";
                return false;
            }

            byte didHigh = (byte)(did >> 8);
            byte didLow = (byte)(did & 0xFF);

            byte[] udsCore = new byte[3 + controlOptionRecord.Length];
            udsCore[0] = 0x2F;
            udsCore[1] = didHigh;
            udsCore[2] = didLow;
            Buffer.BlockCopy(controlOptionRecord, 0, udsCore, 3, controlOptionRecord.Length);

            byte[] requestPayload;
            if (useSingleFramePci)
            {
                //if (udsCore.Length > 0x0F)
                //{
                //    error = "2F请求超过单帧PCI长度限制，请切换ISO-TP多帧";
                //    return false;
                //}
                requestPayload = udsCore;
              //  requestPayload = new byte[1 + udsCore.Length];
                //requestPayload[0] = (byte)udsCore.Length;
               // Buffer.BlockCopy(udsCore, 0, requestPayload, 1, udsCore.Length);
            }
            else
            {
                requestPayload = udsCore;
            }

            if (!TrySendUdsAndWait(
                frameType,
                requestCanId,
                SendType.Diagnosis,
                canSelectType,
                requestPayload,
                expectedPositiveSid: 0x6F,
                expectedPositiveSubFunction: null,
                responsePayload: out byte[] rawResponse,
                negativeResponseCode: out negativeResponseCode,
                error: out string callError,
                p2TimeoutMs: p2TimeoutMs,
                p2StarTimeoutMs: p2StarTimeoutMs,
                expectedResponseCanId: expectedResponseCanId))
            {
                error = callError;
                return false;
            }

            int respOffset = DetectUdsStartIndex(rawResponse);
            if (respOffset < 0 || rawResponse.Length < respOffset + 3)
            {
                error = "2F正响应格式异常";
                return false;
            }

            if (rawResponse[respOffset] != 0x6F)
            {
                error = $"2F正响应SID异常: 0x{rawResponse[respOffset]:X2}";
                return false;
            }

            if (rawResponse[respOffset + 1] != didHigh || rawResponse[respOffset + 2] != didLow)
            {
                error = $"2F正响应DID不匹配: recv=0x{rawResponse[respOffset + 1]:X2}{rawResponse[respOffset + 2]:X2}, expect=0x{did:X4}";
                return false;
            }

            responsePayload = rawResponse;
            return true;
        }

        // 2026-04-24: 2F便捷重载。
        // controlParameter常用: 0x00(ReturnControlToECU), 0x01(ResetToDefault), 0x02(FreezeCurrentState), 0x03(ShortTermAdjustment)
        public bool TryUds2F(
            FrameType frameType,
            uint requestCanId,
            CANSelectType canSelectType,
            ushort did,
            byte controlParameter,
            byte[] controlStateRecord,
            out byte[] responsePayload,
            out byte negativeResponseCode,
            out string error,
            uint? expectedResponseCanId = null,
            int p2TimeoutMs = 500,
            int p2StarTimeoutMs = 5000,
            bool useSingleFramePci = true)
        {
            byte[] record = controlStateRecord ?? Array.Empty<byte>();
            byte[] controlOptionRecord = new byte[1 + record.Length];
            controlOptionRecord[0] = controlParameter;
            if (record.Length > 0)
            {
                Buffer.BlockCopy(record, 0, controlOptionRecord, 1, record.Length);
            }

            return TryUds2F(
                frameType,
                requestCanId,
                canSelectType,
                did,
                controlOptionRecord,
                out responsePayload,
                out negativeResponseCode,
                out error,
                expectedResponseCanId,
                p2TimeoutMs,
                p2StarTimeoutMs,
                useSingleFramePci);
        }

        private static string GetNrcDescription(byte nrc)
        {
            // 常见NRC翻译，便于产线直接定位失败原因。
            switch (nrc)
            {
                case 0x35:
                    return "(InvalidKey)";
                case 0x36:
                    return "(ExceedNumberOfAttempts)";
                case 0x37:
                    return "(RequiredTimeDelayNotExpired)";
                case 0x78:
                    return "(ResponsePending)";
                case 0x7E:
                    return "(SubFunctionNotSupportedInActiveSession)";
                case 0x7F:
                    return "(ServiceNotSupportedInActiveSession)";
                default:
                    return string.Empty;
            }
        }

        #endregion


        #region 发送方法
        /// <summary>
        /// CAN初始化
        /// </summary>
        /// <param name="cANInitSetting"></param>
        public void CanInitSend(/*CANInitSetting cANInitSetting,*/ushort Arbitration, ushort DataSegment,byte lenth/*,byte SamplingPoint, byte DataPoint*/)//CRC
        {
            byte[] buffer = new byte[1 + 2 + 2 + 1 + 4  + 1 + 1 ]; // CAN_Select+仲裁段的波特率+数据段的波特率+采样点+诊断报文发送ID+诊断报文接收ID+加速状态+CRC16
            int offset = 0;
            // 固定帧头
            buffer[offset++] = 0x55;
            buffer[offset++] = 0xAA;
            // 帧类型
            buffer[offset++] = 0x03;
            // 仲裁段波特率
            buffer[offset++] = (byte)((Arbitration) & 0xFF);
            buffer[offset++] = (byte)((Arbitration >> 8) & 0xFF);
            // 数据段波特率
            buffer[offset++] = (byte)((DataSegment) & 0xFF);
            buffer[offset++] = (byte)((DataSegment >> 8) & 0xFF);
            //仲裁段采样点、
            buffer[offset++] = 80;//SamplingPoint;
            //数据段采样点、
            buffer[offset++] = 80;//DataPoint;
            buffer[offset++] = lenth;//接收长度
            // 数据内容
            // CRC16 (计算范围：帧头+帧类型+数据部分)
            ushort crc = CalculateCRC16(buffer, 0, offset - 1);
            byte[] crcBytes = BitConverter.GetBytes(crc);
            Buffer.BlockCopy(crcBytes, 0, buffer, offset, 2);

            udpClient.Send(buffer, buffer.Length, remoteEndPoint);
        }
        /// <summary>
        /// CAN诊断接收ID设置
        /// </summary>
        /// <param name="cANInitSetting"></param>
        public void CanIDInitSend(/*CANInitSetting cANInitSetting*/string DiagnosisRecvID)//CRC
        {
            byte[] buffer = new byte[1 + 2 + 2 + 1  + 1 + 2]; // CAN_Select+仲裁段的波特率+数据段的波特率+采样点+诊断报文发送ID+诊断报文接收ID+加速状态+CRC16
            int offset = 0;
            // 固定帧头
            buffer[offset++] = 0x55;
            buffer[offset++] = 0xAA;
            // 帧类型
            buffer[offset++] = 0x05;
            TryHexToUInt(DiagnosisRecvID, out uint id);
            buffer[offset++] = (byte)((id) & 0xFF);
            buffer[offset++] = (byte)((id >> 8) & 0xFF);
            buffer[offset++] = (byte)((id >> 16) & 0xFF);
            buffer[offset++] = (byte)((id >> 24) & 0xFF);
            //诊断报文接收ID、

            // 数据内容
            // CRC16 (计算范围：帧头+帧类型+数据部分)
            ushort crc = CalculateCRC16(buffer, 0, offset - 1);
            byte[] crcBytes = BitConverter.GetBytes(crc);
            Buffer.BlockCopy(crcBytes, 0, buffer, offset, 2);

            udpClient.Send(buffer, buffer.Length, remoteEndPoint);
        }
        public void SendCANMessage(FrameType frameType, uint canId, SendType sendType, CANSelectType cANSelectType, byte[] data)
        {
            byte[] buffer = new byte[2 + 1 + 4 + 2 + data.Length + 2]; // 固定帧头+类型+数据+CRC
            int offset = 0;

            // 固定帧头
            buffer[offset++] = 0x55;
            buffer[offset++] = 0xAA;

            // 帧类型
            buffer[offset++] = 0x01;

            // CAN ID (小端)
            if (frameType == FrameType.ExtendedFrame)//分辨标准帧还是拓展帧
                canId |= 0x80000000;
            if (sendType == SendType.Diagnosis)//分辨应用还是诊断
                canId |= 0x40000000;
            if (cANSelectType == CANSelectType.CANFD)//分辨CAN还是CANFD
                canId |= 0x20000000;
            byte[] canIdBytes = BitConverter.GetBytes(canId);
            //   if (!BitConverter.IsLittleEndian) 
            //      Array.Reverse(canIdBytes);
            Buffer.BlockCopy(canIdBytes, 0, buffer, offset, 4);
            offset += 4;
            // 数据长度

            buffer[offset++] = (byte)(data.Length & 0xFF);
            buffer[offset++] = (byte)(data.Length >> 8 & 0xFF);
            // 数据内容
            Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
            offset += data.Length;

            // CRC16 (计算范围：帧头+帧类型+数据部分)
            ushort crc = CalculateCRC16(buffer, 0, offset - 1);
            byte[] crcBytes = BitConverter.GetBytes(crc);
            /*   if (!BitConverter.IsLittleEndian) */
            //    Array.Reverse(crcBytes);
            Buffer.BlockCopy(crcBytes, 0, buffer, offset, 2);

            udpClient.Send(buffer, buffer.Length, remoteEndPoint);
            CanMessageBus.Publish(new CanMessageBus.RawCanMessage
            {
                Timestamp = DateTime.Now,
                Direction = "TX",
                ID = $"0x{canId:X8}",
                Data = buffer
            });
        }

        // 接收线程
        SecurityUnlocker SecurityUnlocker = new SecurityUnlocker();

        #region  CRC16表
        public static byte[] aucCRCHi = new byte[]
            {
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40
            };

        public static byte[] aucCRCLo = new byte[]
            {
        0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
        0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
        0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
        0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
        0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
        0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
        0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
        0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
        0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
        0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
        0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
        0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
        0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
        0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
        0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
        0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
        0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
        0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
        0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
        0x41, 0x81, 0x80, 0x40
            };

        #endregion
        // CRC16计算（优化版）
        public static ushort CalculateCRC16(byte[] data, int start, int end)
        {
            // 参数合法性校验
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (start < 0 || start >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end < 0 || end >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(end));
            if (start > end)
                throw new ArgumentException("start index must be <= end index");

            // 预计算的CRC高字节和低字节表（与C代码完全一致）

            // 初始化CRC寄存器（0xFFFF）
            byte ucCRCHi = 0xFF;
            byte ucCRCLo = 0xFF;

            // 遍历指定范围内的每个字节
            for (int i = start; i <= end; i++)
            {
                byte currentByte = data[i];

                // 计算查表索引
                int index = ucCRCLo ^ currentByte;

                // 更新CRC高低位
                ucCRCLo = (byte)(ucCRCHi ^ aucCRCHi[index]);
                ucCRCHi = aucCRCLo[index];
            }

            // 组合结果（高字节在前）
            return (ushort)((ucCRCHi << 8) | ucCRCLo);
        }

        public void ParseErrorFrame(byte[] data)
        {
            // 错误帧解析逻辑
            if (data.Length < 5 + 2) return; // 帧头+类型+错误码+CRC
            ushort errorCode = BitConverter.ToUInt16(data, 3);
            Console.WriteLine($"收到错误码：0x{errorCode:X4}");
            // 2026-05-01: 错误帧也发布到监控总线，窗体/控件可统一订阅显示。
            CanMessageBus.Publish(new CanMessageBus.RawCanMessage
            {
                Timestamp = DateTime.Now,
                Direction = "ERR",
                ID = "-",
                Data = data
            });
            //if(errorCode == 0x7F) 预留
            //{
            //     SendCANMessage(FrameType.ExtendedFrame,0x7DF, SendType.Diagnosis,CANSelectType.CANFD,new byte[] { 0x02, 0x27, 0x01 });
            //}
            // 根据新错误码表处理
            switch (errorCode)
            {
                case 0x0000:
                    Console.WriteLine("CRC校验错误");
                    break;
                case 0x0001:
                    Console.WriteLine("格式错误");
                    break;
                case 0x0002:
                    Console.WriteLine("UDP发送失败");
                    break;
                case 0x0003:
                    Console.WriteLine("CAN发送失败");
                    break;
                case 0x0004:
                    Console.WriteLine("数据长度错误");
                    break;
                case 0x0005:
                    Console.WriteLine("CAN初始化失败");
                    break;
                case 0x0006:
                    Console.WriteLine("诊断发送失败");
                    break;
                default:
                    Console.WriteLine($"未知错误码：0x{errorCode:X4}");
                    break;
            }

        }
        #endregion

        #region 线程
        public bool StopFlag = false;
        public List<Message> 周期发送List = new List<Message>();
        public ConcurrentQueue<Message> 发送Queue = new ConcurrentQueue<Message>();
        // 2026-05-06: 周期调度与待发去重，避免重复入队导致周期失真。
        private readonly object _periodicScheduleLock = new object();
        private readonly Dictionary<string, DateTime> _periodicNextDue = new Dictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, byte> _pendingSendKeys = new ConcurrentDictionary<string, byte>();
        private DateTime _lastSendAt = DateTime.MinValue;
        private const int ApplyMinGapMs = 2;
        private const int DiagnosisMinGapMs = 25;

        public void RunningThread(object o)
        {

            while (!StopFlag)
            {
                try
                {
                    CANWriteSend();
                    CycleSend();
                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(1);
            }
        }
        public void SendThread(object o)
        {

            while (!StopFlag)
            {
                try
                {
                   // CheckChanges();
                    SendStep();
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {

                }
            }

        }
        public void CheckChangesThread(object o)
        {

            //while (!StopFlag)
            //{
            //    try
            //    {

            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

        }
        public void EnSendQueue(Message message)
        {
            发送Queue.Enqueue(message);
        }
        public void Clear()
        {
            while(SendMesTemp.Count > 0) 
            {
                if (SendMesTemp.TryDequeue(out var tr))
                {
                    RemovePendingMessage(tr);
                }
            }
            _pendingSendKeys.Clear();
            lock (_periodicScheduleLock)
            {
                _periodicNextDue.Clear();
            }
        }
        public void CANWriteSend()
        {
            // 2026-05-06: 单次批量搬运上限，避免实时写队列长期霸占导致周期报文饥饿。
            int budget = 20;
            while (budget-- > 0 && 发送Queue.TryDequeue(out var mes))
            {
                try
                {
                    TryEnqueueSendMessage(mes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CANWriteSend异常: {ex.Message}");
                }
            }
        }
        public void CycleSend()
        {
            if (StopSendfalg == true)
                return;

            var now = DateTime.Now;
            // 快照遍历，避免遍历期间外部修改周期列表导致异常。
            var cycleSnapshot = 周期发送List.ToArray();
            foreach (var m in cycleSnapshot)
            {
                if (StopSendfalg || m == null)
                {
                    return;
                }

                int periodMs = m.CycleTime;
                if (periodMs <= 0)
                {
                    continue;
                }

                bool due = false;
                string key = GetMessageKey(m);
                lock (_periodicScheduleLock)
                {
                    if (!_periodicNextDue.TryGetValue(key, out DateTime nextDue))
                    {
                        // 首次调度：立即可发，并登记下一次到期。
                        _periodicNextDue[key] = now.AddMilliseconds(periodMs);
                        due = true;
                    }
                    else if (now >= nextDue)
                    {
                        // 补偿模式：如果线程抖动导致错过多个周期，推进到“当前时刻之后”的下一周期。
                        while (nextDue <= now)
                        {
                            nextDue = nextDue.AddMilliseconds(periodMs);
                        }
                        _periodicNextDue[key] = nextDue;
                        due = true;
                    }
                }

                if (due)
                {
                    TryEnqueueSendMessage(m);
                }
            }
        }
        public void ReadSend(string s)
        {
            if (StopSendfalg == true)
                return;
            
            foreach (var m in Messages)
            {
                if (StopSendfalg == true)
                    return;
                //TryHexToUInt(m.Id, out uint id);
                //SendCANMessage(m.FrameType, id, m.SendType, CAN初始化设置.CAN_Select, StringToBytes(m.Transmitter));
                if (s == m.Name)
                {
                    TryEnqueueSendMessage(m);
                }


            }
        }
        public int step = 0;
        public DateTime LastSendTime;
        public ConcurrentQueue<Message> SendMesTemp = new ConcurrentQueue<Message>();
        public DateTime SendSpen;
        void SendStep()
        {
            if (!SendMesTemp.TryPeek(out var mes))
            {
                return;
            }

            int gap = mes.SendType == SendType.Diagnosis ? DiagnosisMinGapMs : ApplyMinGapMs;
            if (_lastSendAt != DateTime.MinValue && (DateTime.Now - _lastSendAt).TotalMilliseconds < gap)
            {
                return;
            }

            if (!TryHexToUInt(mes.Id, out uint id))
            {
                Console.WriteLine($"发送失败: 非法CAN ID={mes.Id}");
                if (SendMesTemp.TryDequeue(out var bad))
                {
                    RemovePendingMessage(bad);
                }
                return;
            }

            try
            {
                SendCANMessage(mes.FrameType, id, mes.SendType, WinForm.CANType, StringToBytes(mes.Transmitter));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendStep发送异常: {ex.Message}");
            }
            finally
            {
                if (SendMesTemp.TryDequeue(out var sent))
                {
                    sent.IsPositiveResponse = false;
                    RemovePendingMessage(sent);
                }
                _lastSendAt = DateTime.Now;
            }
        }

        private bool TryEnqueueSendMessage(Message mes)
        {
            if (mes == null)
            {
                return false;
            }

            string key = GetMessageKey(mes);
            if (!_pendingSendKeys.TryAdd(key, 0))
            {
                return false;
            }

            SendMesTemp.Enqueue(mes);
            return true;
        }

        private void RemovePendingMessage(Message mes)
        {
            if (mes == null)
            {
                return;
            }

            _pendingSendKeys.TryRemove(GetMessageKey(mes), out _);
        }

        private static string GetMessageKey(Message mes)
        {
            if (mes == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(mes.Name))
            {
                return mes.Name;
            }

            return $"{mes.Id}|{mes.Transmitter}|{(int)mes.SendType}|{(int)mes.FrameType}";
        }



        public byte[] StringToBytes(string input)
        {
            string[] hexParts = input.Split(' ');
            byte[] result = new byte[hexParts.Length];

            for (int i = 0; i < hexParts.Length; i++)
            {
                result[i] = Convert.ToByte(hexParts[i], 16);
            }
            return result;
        }
        /// <summary>
        /// 安全转换方法（推荐）
        /// </summary>
        public static bool TryHexToUInt(string input, out uint value)
        {
            value = 0;
            string hexPart = input.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? input.Substring(2)
                : input;

            return uint.TryParse(
                hexPart,
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out value
            );
        }
        // 案例2：跨字节设置第6-9位（4位）为0b1011
        // transmitter.ModifyBits("FlagReg",
        // bitOffset: 5,
        //bitLength: 4,
        // value: 0b1011);
        /* 结果（大端表示）：
        原数据: 00110000 00000000
        修改后: 00110010 11000000
        */
        /// <summary>
        /// 比特级数据修改方法
        /// </summary>
        /// <param name="bitOffset">比特偏移量（从0开始）</param>
        /// <param name="bitLength">要写入的比特长度（1-32）</param>
        /// <param name="isBigEndian">是否按大端序处理多字节数据</param>
        public void ModifyBits(string nodeName, uint bitOffset, byte bitLength, int value, bool isBigEndian = true)
        {
            // 参数基础校验
            if (bitLength < 1 || bitLength > 32)
                throw new ArgumentException("Bit length must be between 1 and 32");

            // 获取目标字节数组
            var key = TransmitterDictionary.Keys.FirstOrDefault(k => k.Name == nodeName);

            if (key != null && TransmitterDictionary.TryGetValue(key, out byte[] data))
            {
                // 找到数据
            }
            else
            {
                throw new KeyNotFoundException($"Node '{nodeName}' not found");
            }
            //if (!TransmitterDictionary.TryGetValue(nodeName, out byte[] data))
            //    throw new KeyNotFoundException($"Node '{nodeName}' not found");

            // 计算字节边界
            int totalBits = data.Length * 8;
            if (bitOffset + bitLength > totalBits)
                throw new ArgumentOutOfRangeException(
                    $"Operation range [{bitOffset}-{bitOffset + bitLength}) exceeds buffer size {totalBits} bits");

            // 处理整数值的位范围
            uint mask = bitLength == 32 ? 0xFFFFFFFF : (1u << bitLength) - 1;
            uint bitsToWrite = (uint)(value & (int)mask);

            // 大端序调整
            if (isBigEndian && bitLength > 8)
            {
                bitsToWrite = ReverseBitOrder(bitsToWrite, bitLength);
            }

            // 执行位写入操作
            WriteBits(data, bitOffset, bitLength, bitsToWrite);
            //key.Sendflag = true;
            //key.UpdateTime = DateTime.Now;
        }
        public void UpSendflag(string nodeName)
        {
            var key = TransmitterDictionary.Keys.FirstOrDefault(k => k.Name == nodeName);

            if (key != null && TransmitterDictionary.TryGetValue(key, out byte[] data))
            {
                // 找到数据
                key.Sendflag = true;
                key.UpdateTime = DateTime.Now;
            }
        }
        private static uint ReverseBitOrder(uint value, int bitLength)
        {
            uint result = 0;
            for (int i = 0; i < bitLength; i++)
            {
                result <<= 1;
                result |= (value & 1);
                value >>= 1;
            }
            return result;
        }

        private static void WriteBits(byte[] buffer, uint bitOffset, int bitLength, uint bits)
        {
            int currentBit = 0;
            while (currentBit < bitLength)
            {
                int bytePos = (int)((bitOffset + currentBit) / 8);
                int bitPos = (int)(7 - ((bitOffset + currentBit) % 8)); // 高位在前

                // 修复1：计算连续可写入的位数（从高位到低位）
                int bitsInByte = Math.Min(bitPos + 1, bitLength - currentBit);

                // 调整位移量
                int shift = bitLength - currentBit - bitsInByte;

                // 生成掩码并提取值
                uint mask = ((1u << bitsInByte) - 1) << shift;
                uint valuePart = (bits & mask) >> shift;

                // 修复2：调用修正后的 UpdateByte
                buffer[bytePos] = UpdateByte(buffer[bytePos], bitPos - (bitsInByte - 1), bitsInByte, (byte)valuePart);

                currentBit += bitsInByte;
            }
        }

        private static byte UpdateByte(byte original, int startBit, int bitCount, byte value)
        {
            // 修复3：正确计算掩码和位移
            int shift = 8 - startBit - bitCount;
            byte mask = (byte)~(((1 << bitCount) - 1) << shift); // 清零目标位
            byte adjustedValue = (byte)(value << shift);             // 对齐数值位
            return (byte)((original & mask) | adjustedValue);
        }



        #endregion
        #region Message修改通知
        private readonly Dictionary<string, System.Threading.Timer> _debounceTimers = new Dictionary<string, System.Threading.Timer>();
        private readonly object _lock = new object();
        public void CheckChanges()
        {
            foreach (var kvp in TransmitterDictionary)
            {
                if (kvp.Key != null && kvp.Key.Sendflag == true && (DateTime.Now - kvp.Key.UpdateTime).TotalMilliseconds > 1500)
                {
                    foreach (var mes in Messages)
                    {
                        if (mes.Name == kvp.Key.Name)
                        {
                            // 更新 Transmitter 的值
                            mes.Transmitter = BitConverter.ToString(kvp.Value).Replace("-", " ");
                            Console.WriteLine($"修改报文 {mes.Transmitter}");
                            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"修改报文 {mes.Transmitter}");
                            发送Queue.Enqueue(mes);
                            // 使用防抖机制延迟发送
                            // DebounceEnqueue(mes, kvp.Key);
                            break;
                        }
                    }
                    kvp.Key.Sendflag = false;
                    kvp.Key.UpdateTime = DateTime.Now;
                }
            }
            //Thread.Sleep(10);
        }
        private void DebounceEnqueue(Message mes, string key)
        {
            lock (_lock)
            {
                // 检查并停止之前的计时器
                if (_debounceTimers.TryGetValue(key, out var existingTimer))
                {
                    existingTimer.Dispose();
                }

                // 创建新计时器
                var newTimer = new System.Threading.Timer(
                    callback: _ =>
                    {
                        lock (_lock)
                        {
                            // 修正：分两步操作（兼容旧版本）
                            if (_debounceTimers.TryGetValue(key, out var timer))
                            {
                                _debounceTimers.Remove(key); // 1. 移除键
                                timer.Dispose();             // 2. 释放资源
                                Console.WriteLine($"发送 {key}");
                                发送Queue.Enqueue(mes);
                            }
                        }
                    },
                    state: null,
                    dueTime: 300,
                    period: Timeout.Infinite
                );

                _debounceTimers[key] = newTimer;
            }
        }
        private static string ComputeHash(byte[] data)
        {
            var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(data));
        }
        // 添加 Message 到字典并订阅事件
        public void AddMessage(string key, Message message)
        {
            if (ReadMesDictionary.ContainsKey(key)) return;

            ReadMesDictionary.Add(key, message);

            // 订阅 Message 的 Signal 值变更事件
            message.SignalValueChanged += signal =>
            {
                // 更新 SignalDictionary
                SignalDictionary[signal.Name] = signal.value;
            };

            // 初始化现有 Signals 到字典
            foreach (var signal in message.Signals)
            {
                SignalDictionary[signal.Name] = signal.value;
            }
        }

        // 移除 Message 并清理字典
        public void RemoveMessage(string key)
        {
            if (ReadMesDictionary.TryGetValue(key, out var message))
            {
                // 清理相关 Signal 数据（需根据业务逻辑调整）
                foreach (var signal in message.Signals)
                {
                    SignalDictionary.Remove(signal.Name);
                }

                ReadMesDictionary.Remove(key);
            }
        }
        #endregion

        #region 信号量转换
        public void OnMessageReceived(Message mes, byte[] data)
        {
            int k = 0;
            //  if (_messages.TryGetValue(id, out Message message))
            {
                foreach (var signal in mes.Signals)
                {
                    int rawValue = DecodeSignal(data, signal);
                    double physicalValue = rawValue * signal.Factor + signal.Offset;
                    signal.value = physicalValue;
                    //   Console.WriteLine($"{signal.Name}: {physicalValue} {signal.Unit}");
                    if (signal.Name == "VBAT1")
                    {
                        k = 0;
                        Console.WriteLine(DateTime.Now.ToString()+$" {ip}");
                    }
                    if (ReadMesDictionary.TryGetValue(mes.Name, out var message))
                    {
                        SignalDictionary[signal.Name] = physicalValue;
                    }
                }
            }
        }

        /// <summary>
        /// 解码CAN信号原始值
        /// </summary>
        /// <param name="data">CAN数据帧字节数组</param>
        /// <param name="signal">信号定义</param>
        /// <returns>解码后的整数值</returns>
        private int DecodeSignal(byte[] data, Signal signal)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (signal.StartBit < 0) throw new ArgumentException("起始位不能为负");
            if (signal.Length <= 0) throw new ArgumentException("信号长度必须大于0");

            ulong rawValue = 0;
            int bitsRemaining = signal.Length;

            if (signal.IsMotorolaByteOrder)
            {
                // Motorola格式：大端字节序 + 高位优先
                int currentByte = signal.StartBit / 8;
                int bitCursor = 7 - (signal.StartBit % 8); // 关键修正点：从高位开始

                while (bitsRemaining > 0 && currentByte < data.Length)
                {
                    int bitsToTake = Math.Min(bitCursor + 1, bitsRemaining);
                    byte mask = (byte)((1 << bitsToTake) - 1);
                    byte chunk = (byte)((data[currentByte] >> (bitCursor - bitsToTake + 1)) & mask);

                    rawValue = (rawValue << bitsToTake) | chunk;
                    bitsRemaining -= bitsToTake;
                    bitCursor -= bitsToTake;

                    if (bitCursor < 0)
                    {
                        currentByte++;
                        bitCursor = 7; // 下一个字节继续从高位开始
                    }
                }
            }
            else
            {
                // Intel格式：小端字节序 + 低位优先（代码保持不变）
                int currentBit = signal.StartBit;
                for (int i = 0; i < signal.Length; i++)
                {
                    int byteIndex = currentBit / 8;
                    int bitIndex = currentBit % 8;

                    if (byteIndex >= data.Length) break;

                    if ((data[byteIndex] & (1 << bitIndex)) != 0)
                    {
                        rawValue |= (1UL << i);
                    }
                    currentBit++;
                }
            }

            // 符号扩展处理（保持不变）
            if (signal.IsSigned && signal.Length < 64)
            {
                ulong signMask = 1UL << (signal.Length - 1);
                if ((rawValue & signMask) != 0)
                {
                    ulong extension = 0xFFFFFFFFFFFFFFFFUL << signal.Length;
                    rawValue |= extension;
                }
            }

            return (int)rawValue;
        }
        #endregion
    }
}
