using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestProcessFormsApp.Communication.CAN;
using System.IO;
namespace TestProcessFormsApp.Frm
{
    public partial class UserControlCanMessage : UserControl
    {
        //public UserControlCanMessage()
        //{
        //    InitializeComponent();
        //}
        public static bool StopFlag = false;
        private readonly System.Timers.Timer Refalshtimer = new System.Timers.Timer();
        private readonly object _listLock = new object();
        private int _isTimerProcessing;
        private bool _isClosing;
        // 2026-04-23: 虚拟列表只显示过滤后的索引，避免频繁拷贝消息对象。
        private readonly List<int> _filteredIndices = new List<int>();
        private Label _lblCanIdFilter;
        private TextBox _txtCanIdFilter;
        private Label _lblSidFilter;
        private TextBox _txtSidFilter;
        private CCWin.SkinControl.SkinButton _btnExport;

        public class CanMessageItem
        {
            public DateTime Timestamp { get; set; }
            public string Direction { get; set; } // TX/RX
            public string ID { get; set; }
            public byte[] Data { get; set; }
            public string DataHex => ToHex(Data);
            // 2026-04-23: 以下字段由FillParsedFields从UDP帧中解析后填充，用于监控界面展示。
            public string UdpType { get; set; } = "-";
            public string FrameFormat { get; set; } = "-";
            public string Flags { get; set; } = "-";
            public string ParsedId { get; set; } = "-";
            public string Dlc { get; set; } = "-";
            public string Service { get; set; } = "-";
            public string PayloadHex { get; set; } = "-";
            public string CrcInfo { get; set; } = "-";
            public bool IsCrcOk { get; set; } = true;
        }

        public UserControlCanMessage()
        {
            InitializeComponent();

            EnableDoubleBuffering();
            InitializeMessageListView();
            // 动态添加过滤和导出控件，尽量不改Designer文件。
            InitializeFilterControls();
            // 配置定时器
            Refalshtimer.Interval = 100;
            Refalshtimer.Elapsed += Refalshtimer_Elapsed; // 使用跨线程事件
            Refalshtimer.AutoReset = true; // 自动重置
            Refalshtimer.Start();
            CanMessageBus.MessagePublished += CanMessageBus_MessagePublished;
            Disposed += UserControlCanMessage_Disposed;
        }

        private void EnableDoubleBuffering()
        {
            var prop = typeof(ListView).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            prop?.SetValue(lvMessages, true, null);
        }

        private readonly BindingList<CanMessageItem> _canMessages = new BindingList<CanMessageItem>();
        private readonly ConcurrentQueueWrapper<CanMessageItem> _canMessagesTemp = new ConcurrentQueueWrapper<CanMessageItem>();

        private void CanMessageBus_MessagePublished(CanMessageBus.RawCanMessage raw)
        {
            if (raw == null || _isClosing || _canMessagesTemp.Count >= 10000)
            {
                return;
            }

            _canMessagesTemp.Enqueue(new CanMessageItem
            {
                Timestamp = raw.Timestamp,
                Direction = raw.Direction,
                ID = raw.ID,
                Data = raw.Data
            });
        }

        private void UserControlCanMessage_Disposed(object sender, EventArgs e)
        {
            _isClosing = true;
            CanMessageBus.MessagePublished -= CanMessageBus_MessagePublished;
            Refalshtimer.Stop();
            Refalshtimer.Elapsed -= Refalshtimer_Elapsed;
            Refalshtimer.Dispose();
        }

        private void InitializeMessageListView()
        {
            // 防止重复订阅导致回调被调用多次
            lvMessages.RetrieveVirtualItem -= LvMessages_RetrieveVirtualItem;

            lvMessages.View = View.Details;
            lvMessages.FullRowSelect = true;
            lvMessages.HeaderStyle = ColumnHeaderStyle.Clickable;
            //    lvMessages.DoubleBuffered = true; // 启用双缓冲

            // 列定义
            if (lvMessages.Columns.Count == 0)
            {
                lvMessages.Columns.AddRange(new[] {
                    new ColumnHeader { Text = "时间", Width = 140 },
                    new ColumnHeader { Text = "方向", Width = 60 },
                    new ColumnHeader { Text = "UDP类型", Width = 90 },
                    new ColumnHeader { Text = "CAN ID", Width = 100 },
                    new ColumnHeader { Text = "DLC", Width = 50 },
                    new ColumnHeader { Text = "SID", Width = 60 },
                    new ColumnHeader { Text = "载荷", Width = 280 },
                    new ColumnHeader { Text = "CRC", Width = 120 },
                    new ColumnHeader { Text = "原始UDP", Width = 650 }
                });
            }

            // 虚拟模式配置（处理大量数据）
            lvMessages.VirtualMode = true;
            lvMessages.RetrieveVirtualItem += LvMessages_RetrieveVirtualItem;
            // 使用系统默认绘制，避免OwnerDraw在部分环境导致表头不显示。
            lvMessages.OwnerDraw = false;

            lock (_listLock)
            {
                // 初始化时先构建一次过滤索引，保证VirtualListSize一致。
                ApplyFilterLocked();
            }
            lvMessages.VirtualListSize = _filteredIndices.Count;
        }

        private void LvMessages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (_listLock)
            {
                if (e.ItemIndex >= 0 && e.ItemIndex < _filteredIndices.Count)
                {
                    int sourceIndex = _filteredIndices[e.ItemIndex];
                    if (sourceIndex < 0 || sourceIndex >= _canMessages.Count)
                    {
                        e.Item = new ListViewItem(new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
                        return;
                    }

                    var item = _canMessages[sourceIndex];
                    var row = new ListViewItem(new[] {
                        item.Timestamp.ToString("HH:mm:ss.fff"),
                        item.Direction,
                        item.UdpType,
                        item.ParsedId,
                        item.Dlc,
                        item.Service,
                        item.PayloadHex,
                        item.CrcInfo,
                        item.DataHex
                    });

                    // 使用系统绘制时，在这里设置背景色保持监控可读性。
                    if (!item.IsCrcOk)
                    {
                        row.BackColor = Color.MistyRose;
                    }
                    else if (string.Equals(item.Direction, "ERR", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(item.UdpType, "ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        row.BackColor = Color.LemonChiffon;
                    }
                    else if (string.Equals(item.Direction, "TX", StringComparison.OrdinalIgnoreCase))
                    {
                        row.BackColor = Color.LightBlue;
                    }
                    else
                    {
                        row.BackColor = Color.Honeydew;
                    }

                    e.Item = row;
                }
                else
                {
                    e.Item = new ListViewItem(new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
                }
            }
        }

        private DateTime _lastUpdateTime = DateTime.MinValue;

        public void AddCanMessage(CanMessageItem msg)
        {
            if (msg == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke((MethodInvoker)delegate { AddCanMessage(msg); });
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                return;
            }

            FillParsedFields(msg);
            lock (_listLock)
            {
                _canMessages.Add(msg);
                // 每次新增都重建过滤索引，保证筛选结果实时刷新。
                ApplyFilterLocked();
            }

            // 限制刷新频率（例如每50ms刷新一次）
            if ((DateTime.Now - _lastUpdateTime).TotalMilliseconds >= 50)
            {
                if (_isClosing || IsDisposed || !IsHandleCreated)
                {
                    return;
                }

                try
                {
                    lvMessages.BeginInvoke((MethodInvoker)delegate
                    {
                        if (_isClosing || IsDisposed)
                        {
                            return;
                        }

                        lock (_listLock)
                        {
                            lvMessages.VirtualListSize = _filteredIndices.Count;
                        }
                        AutoScrollToListEnd();
                    });
                    _lastUpdateTime = DateTime.Now;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private void AutoScrollToListEnd()
        {
            int count;
            lock (_listLock)
            {
                count = _filteredIndices.Count;
            }

            if (count > 0)
            {
                lvMessages.EnsureVisible(count - 1);
            }
        }

        private void Refalshtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_isClosing || StopFlag)
            {
                return;
            }

            // 避免定时器重入导致并发消费
            if (Interlocked.Exchange(ref _isTimerProcessing, 1) == 1)
            {
                return;
            }

            try
            {
                int maxProcess = 50;
                int processed = 0;

                while (processed < maxProcess && _canMessagesTemp.TryDequeue(out var canmessTemp))
                {
                    if (_isClosing || StopFlag || IsDisposed || !IsHandleCreated)
                    {
                        break;
                    }

                    try
                    {
                        // 跨线程更新UI
                        lvMessages.BeginInvoke((MethodInvoker)delegate
                        {
                            if (_isClosing || IsDisposed)
                            {
                                return;
                            }
                            AddCanMessage(canmessTemp);
                        });
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }

                    processed++;
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isTimerProcessing, 0);
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
          //  Close();
        }

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    _isClosing = true;
        //    Refalshtimer.Stop();
        //    Refalshtimer.Elapsed -= Refalshtimer_Elapsed;
        //    Refalshtimer.Dispose();
        //    base.OnFormClosing(e);
        //}

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            StopFlag = true;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            StopFlag = false;
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            Refalshtimer.Stop();
            lock (_listLock)
            {
                _canMessages.Clear();
                _filteredIndices.Clear();
                lvMessages.VirtualListSize = 0;
            }
            _canMessagesTemp.Clear();
            _lastUpdateTime = DateTime.MinValue;
            lvMessages.Invalidate();
            Refalshtimer.Start();
        }

        public class ConcurrentQueueWrapper<T>
        {
            private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
            public int Count => _queue.Count;
            public void Enqueue(T item)
            {
                _queue.Enqueue(item); // 安全入队
            }

            public bool TryDequeue(out T item)
            {
                return _queue.TryDequeue(out item); // 安全出队
            }

            public void Clear()
            {
                // 原子替换为新队列
                var newQueue = new ConcurrentQueue<T>();
                Interlocked.Exchange(ref _queue, newQueue);
            }
        }

        private static void FillParsedFields(CanMessageItem item)
        {
            if (item == null)
            {
                return;
            }

            // 回退值：若解析失败，至少保留原有ID显示。
            item.ParsedId = string.IsNullOrWhiteSpace(item.ID) ? "-" : item.ID;

            var frame = item.Data;
            if (frame == null || frame.Length < 3)
            {
                item.UdpType = "RAW";
                item.FrameFormat = "-";
                item.Flags = "-";
                item.Dlc = "-";
                item.Service = "-";
                item.PayloadHex = "-";
                item.CrcInfo = "-";
                item.IsCrcOk = true;
                return;
            }

            if (frame[0] != 0x55 || frame[1] != 0xAA)
            {
                item.UdpType = "RAW";
                item.FrameFormat = "-";
                item.Flags = "-";
                item.Dlc = "-";
                item.Service = "-";
                item.PayloadHex = "-";
                item.CrcInfo = "-";
                item.IsCrcOk = true;
                return;
            }

            byte udpType = frame[2];
            // 解析当前自定义UDP协议的帧类型。
            switch (udpType)
            {
                case 0x01:
                    item.UdpType = "CAN TX";
                    break;
                case 0x02:
                    item.UdpType = "CAN RX";
                    break;
                case 0x03:
                    item.UdpType = "CAN INIT";
                    break;
                case 0x05:
                    item.UdpType = "ID INIT";
                    break;
                case 0xFF:
                    item.UdpType = "ERROR";
                    break;
                default:
                    item.UdpType = $"TYPE 0x{udpType:X2}";
                    break;
            }

            if (udpType == 0xFF)
            {
                item.FrameFormat = "-";
                item.Flags = "-";
                item.Dlc = "-";

                if (frame.Length >= 7)
                {
                    ushort errorCode = BitConverter.ToUInt16(frame, 3);
                    item.Service = $"0x{errorCode:X4}";
                    item.PayloadHex = GetUdpErrorDescription(errorCode);

                    try
                    {
                        ushort recvCrc = BitConverter.ToUInt16(frame, frame.Length - 2);
                        ushort calcCrc = CANUDPClient.CalculateCRC16(frame, 0, frame.Length - 3);
                        bool ok = recvCrc == calcCrc;
                        item.IsCrcOk = ok;
                        item.CrcInfo = ok
                            ? $"OK 0x{recvCrc:X4}"
                            : $"ERR recv=0x{recvCrc:X4} calc=0x{calcCrc:X4}";
                    }
                    catch
                    {
                        item.IsCrcOk = false;
                        item.CrcInfo = "CRC异常";
                    }
                }
                else
                {
                    item.Service = "-";
                    item.PayloadHex = "错误帧长度不足";
                    item.CrcInfo = "长度不足";
                    item.IsCrcOk = false;
                }
                return;
            }

            if (udpType != 0x01 && udpType != 0x02)
            {
                item.FrameFormat = "-";
                item.Flags = "-";
                item.Dlc = "-";
                item.Service = "-";
                item.PayloadHex = "-";
                item.CrcInfo = "-";
                item.IsCrcOk = true;
                return;
            }

            if (frame.Length < 11)
            {
                item.FrameFormat = "INVALID";
                item.Flags = "-";
                item.Dlc = "-";
                item.Service = "-";
                item.PayloadHex = "-";
                item.CrcInfo = "长度不足";
                item.IsCrcOk = false;
                return;
            }

            int dataLen = frame[7] | (frame[8] << 8);
            int crcOffset = 9 + dataLen;
            int expectedLen = crcOffset + 2;
            if (dataLen < 0 || expectedLen > frame.Length)
            {
                item.FrameFormat = "INVALID";
                item.Flags = "-";
                item.Dlc = dataLen.ToString();
                item.Service = "-";
                item.PayloadHex = "-";
                item.CrcInfo = "DLC越界";
                item.IsCrcOk = false;
                return;
            }

            uint canIdRaw = (uint)(frame[3] | (frame[4] << 8) | (frame[5] << 16) | (frame[6] << 24));
            //bool isExt = (canIdRaw & 0x80000000) != 0;
            //bool isDiag = (canIdRaw & 0x40000000) != 0;
            //bool isCanFd = (canIdRaw & 0x20000000) != 0;

            uint pureId = canIdRaw & 0x1FFFFFFF;
            //if (!isExt)
            //{
            //    pureId &= 0x7FF;
            //}

            //item.FrameFormat = isExt ? "EXT" : "STD";
            //item.ParsedId = isExt ? $"0x{pureId:X8}" : $"0x{pureId:X3}";
            item.ParsedId = $"0x{pureId:X8}";
            var flagParts = new List<string>();
            //if (isDiag) flagParts.Add("Diag");
            //if (isCanFd) flagParts.Add("CANFD");
            item.Flags = flagParts.Count == 0 ? "-" : string.Join("|", flagParts);
            item.Dlc = dataLen.ToString();

            byte[] payload = new byte[dataLen];
            if (dataLen > 0)
            {
                Buffer.BlockCopy(frame, 9, payload, 0, dataLen);
                item.PayloadHex = ToHex(payload);
                item.Service = $"0x{payload[0]:X2}";
            }
            else
            {
                item.PayloadHex = "-";
                item.Service = "-";
            }

            try
            {
                ushort recvCrc = BitConverter.ToUInt16(frame, crcOffset);
                ushort calcCrc = CANUDPClient.CalculateCRC16(frame, 0, crcOffset - 1);
                bool ok = recvCrc == calcCrc;
                item.IsCrcOk = ok;
                item.CrcInfo = ok
                    ? $"OK 0x{recvCrc:X4}"
                    : $"ERR recv=0x{recvCrc:X4} calc=0x{calcCrc:X4}";
            }
            catch
            {
                item.IsCrcOk = false;
                item.CrcInfo = "CRC异常";
            }
        }

        private static string ToHex(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }
            return BitConverter.ToString(data).Replace("-", " ");
        }

        private static string GetUdpErrorDescription(ushort code)
        {
            switch (code)
            {
                case 0x0000: return "CRC校验错误";
                case 0x0001: return "格式错误";
                case 0x0002: return "UDP发送失败";
                case 0x0003: return "CAN发送失败";
                case 0x0004: return "数据长度错误";
                case 0x0005: return "CAN初始化失败";
                case 0x0006: return "诊断发送失败";
                default: return $"未知错误码(0x{code:X4})";
            }
        }

        private void InitializeFilterControls()
        {
            // 2026-04-23: 动态创建过滤输入框和导出按钮。
            _lblCanIdFilter = new Label
            {
                Text = "CAN ID",
                AutoSize = true,
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(852, 11),
            };

            _txtCanIdFilter = new TextBox
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(896, 8),
                Size = new Size(80, 21),
                Name = "txtCanIdFilter"
            };
            _txtCanIdFilter.TextChanged += FilterTextChanged;

            _lblSidFilter = new Label
            {
                Text = "SID",
                AutoSize = true,
                ForeColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(981, 11),
            };

            _txtSidFilter = new TextBox
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(1014, 8),
                Size = new Size(55, 21),
                Name = "txtSidFilter"
            };
            _txtSidFilter.TextChanged += FilterTextChanged;

            _btnExport = new CCWin.SkinControl.SkinButton
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(1076, 1),
                Size = new Size(85, 38),
                Name = "btn_export",
                Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134))),
                ForeColor = System.Drawing.Color.White,
                Text = "导出CSV",
                Radius = 20,
                IsDrawGlass = false,
                InheritColor = true
            };
            _btnExport.Click += BtnExport_Click;

            splitContainer1.Panel2.Controls.Add(_lblCanIdFilter);
            splitContainer1.Panel2.Controls.Add(_txtCanIdFilter);
            splitContainer1.Panel2.Controls.Add(_lblSidFilter);
            splitContainer1.Panel2.Controls.Add(_txtSidFilter);
            splitContainer1.Panel2.Controls.Add(_btnExport);
        }

        private void FilterTextChanged(object sender, EventArgs e)
        {
            RefreshFilterAndView();
        }

        private void RefreshFilterAndView()
        {
            if (_isClosing || IsDisposed)
            {
                return;
            }

            lock (_listLock)
            {
                ApplyFilterLocked();
                lvMessages.VirtualListSize = _filteredIndices.Count;
            }

            lvMessages.Invalidate();
        }

        private void ApplyFilterLocked()
        {
            _filteredIndices.Clear();

            string canIdFilter = _txtCanIdFilter?.Text?.Trim() ?? string.Empty;
            string sidFilter = NormalizeHexToken(_txtSidFilter?.Text?.Trim() ?? string.Empty);

            for (int i = 0; i < _canMessages.Count; i++)
            {
                var item = _canMessages[i];
                if (item == null)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(canIdFilter))
                {
                    var target = item.ParsedId ?? string.Empty;
                    if (target.IndexOf(canIdFilter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrWhiteSpace(sidFilter))
                {
                    var currentSid = NormalizeHexToken(item.Service ?? string.Empty);
                    if (string.IsNullOrWhiteSpace(currentSid) ||
                        currentSid.IndexOf(sidFilter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                }

                _filteredIndices.Add(i);
            }
        }

        private static string NormalizeHexToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string t = value.Trim();
            if (t.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                t = t.Substring(2);
            }

            return t.ToUpperInvariant();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            List<CanMessageItem> snapshot;
            lock (_listLock)
            {
                // 只导出当前过滤结果，保持“所见即所得”。
                snapshot = _filteredIndices
                    .Where(idx => idx >= 0 && idx < _canMessages.Count)
                    .Select(idx => _canMessages[idx])
                    .ToList();
            }

            if (snapshot.Count == 0)
            {
                MessageBox.Show("当前过滤结果为空，暂无可导出数据。");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV文件|*.csv";
                sfd.FileName = $"CanMonitor_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Timestamp,Direction,UdpType,CANID,DLC,SID,Payload,CRC,RawUDP");
                foreach (var item in snapshot)
                {
                    sb.AppendLine(string.Join(",",
                        CsvEscape(item.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                        CsvEscape(item.Direction),
                        CsvEscape(item.UdpType),
                        CsvEscape(item.ParsedId),
                        CsvEscape(item.Dlc),
                        CsvEscape(item.Service),
                        CsvEscape(item.PayloadHex),
                        CsvEscape(item.CrcInfo),
                        CsvEscape(item.DataHex)
                    ));
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                MessageBox.Show($"导出成功，共 {snapshot.Count} 条。");
            }
        }

        private static string CsvEscape(string value)
        {
            if (value == null)
            {
                return "\"\"";
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
