using JLRScan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static TestProcessFormsApp.Frm.FrmCanNode;

namespace TestProcessFormsApp.Communication.CAN
{
    public class ParseDbc
    {

    }
    public class CANInitSetting
    {
        [DisplayName("CAN选择")]
        public CANSelectType CAN_Select { set; get; }
        [DisplayName("仲裁段的波特率")]
        /// <summary>
        /// 仲裁段的波特率 单位K
        /// </summary>
        public UInt16 Arbitration { set; get; }
        [DisplayName("数据段的波特率")]
        /// <summary>
        /// 数据段的波特率
        /// </summary>
        public UInt16 DataSegment { set; get; }
        [DisplayName("仲裁段采样点")]
        /// <summary>
        /// 仲裁段采样点
        /// </summary>
        public byte SamplingPoint { set; get; }
        [DisplayName("数据段采样点")]
        /// <summary>
        /// 仲裁段采样点
        /// </summary>
        public byte DataPoint { set; get; }
       
        [DisplayName("诊断报文接收ID")]
        /// <summary>
        /// 诊断报文接收ID
        /// </summary>
        public string DiagnosisRecvID { set; get; }
      
    }
    // 枚举定义
    public enum SendType
    {
        /// <summary>
        /// 应用
        /// </summary>
        Apply,
        /// <summary>
        /// 诊断
        /// </summary>
        Diagnosis,
    }
    public enum WriteOrRead
    {
        /// <summary>
        /// 写入
        /// </summary>
        Write,
        /// <summary>
        /// 读取
        /// </summary>
        Read,
    }
    public enum CANSelectType
    {
        CAN,
        CANFD,
    }
    public enum FrameType
    {
        /// <summary>
        /// 标准帧
        /// </summary>
        StandardFrame,
        /// <summary>
        /// 拓展帧
        /// </summary>
        ExtendedFrame,
    }
    public enum ReadType
    {
        Signals,
        Bytes,
    }
    // 可扩展的注释系统（对应DBC中的CM_）
    public class Comment
    {
        public string TargetType { get; set; }  // 注释对象类型（BO_/SG_）
        public string TargetName { get; set; }  // 关联的对象名称
        public string Text { get; set; }        // 注释内容
    }

    // 属性值定义（对应DBC中的BA_）
    public class AttributeValue
    {
        public string AttributeName { get; set; }
        public string ValueType { get; set; }    // ENUM/INT/FLOAT/STRING
        public object Value { get; set; }        // 实际值
    }
    // [JsonObject(MemberSerialization.OptIn)]
    //  [TypeConverter(typeof(ClassConverter<Message>))]
    public class Message
    {
        private string _Id;
        private string _Name;
        private int _Dlc;
        private string _Transmitter;
        private SendType _SendType;
        private FrameType _FrameType;
        private WriteOrRead _writeOrRead;
        private ReadType _readType;
        [Browsable(false)]
        public bool IsPositiveResponse { get; set; } = false;
        private int _CycleTime { get; set; }
        public string Id
        {
            get => _Id;
            set { _Id = value; OnPropertyChanged(); }
        }         // CAN报文ID (对应DBC中的 `BO_` 标识符)
        public string Name
        {
            get => _Name;
            set { _Name = value; OnPropertyChanged(); }
        }     // 报文名称 (对应DBC中的 `BO_` 名称)
        [DisplayName("长度")]
        public int Dlc
        {
            get => _Dlc;
            set { _Dlc = value; OnPropertyChanged(); }
        }         // 数据长度 (单位：字节，范围1-8或64)
        [DisplayName("报文内容")]
        public string Transmitter
        {
            get => _Transmitter;
            set { _Transmitter = value; OnPropertyChanged(); }
        } // 发送节点 (对应DBC中的发送ECU) 修改为报文内容
        [TypeConverter(typeof(ClassConverter<Signal>))]
        public ObservableCollection<Signal> Signals { set; get; } = new ObservableCollection<Signal>();// 信号列表 (对应DBC中的 `SG_` 定义)
                                                                                                       //   public List<Signal> Signals { get; set; } // 信号列表 (对应DBC中的 `SG_` 定义)

        [DisplayName("读写类型")]
        public WriteOrRead WriteOrRead
        {
            get => _writeOrRead;
            set { _writeOrRead = value; OnPropertyChanged(); }
        }         // 读写类型
        // 新增扩展属性
        [DisplayName("发送周期")]
        public int CycleTime
        {
            get => _CycleTime;
            set { _CycleTime = value; OnPropertyChanged(); }
        }         // 读写类型
                  //     public int CycleTime { get; set; }         // 发送周期（毫秒，对应BA_ "GenMsgCycleTime"）
                  //public bool IsExtendedId { get; set; }     // 是否为扩展帧
                  //public string Comment { get; set; }        // 报文注释（对应CM_）
                  //public Dictionary<string, string> Attributes { get; set; } // 自定义属性（对应BA_）
        [DisplayName("发送类型")]
        public SendType SendType
        {
            get => _SendType;
            set { _SendType = value; OnPropertyChanged(); }
        }     // 发送类型（周期型/事件型等）
        [DisplayName("帧类型")]
        public FrameType FrameType
        {
            get => _FrameType;
            set { _FrameType = value; OnPropertyChanged(); }
        }     // 帧类型
        [DisplayName("读取类型")]
        public ReadType ReadType
        {
            get => _readType;
            set { _readType = value; OnPropertyChanged(); }
        }     // 读取类型
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        // 定义事件：当 Signal 的值变化时触发
        public event Action<Signal> SignalValueChanged;

        public Message()
        {
            // 监听 Signals 集合的变化（添加/删除）
            Signals.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Signal signal in e.NewItems)
                    {
                        // 订阅 Signal 的属性变更事件
                        signal.PropertyChanged += OnSignalPropertyChanged;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (Signal signal in e.OldItems)
                    {
                        signal.PropertyChanged -= OnSignalPropertyChanged;
                    }
                }
            };
        }
        // 处理 Signal 的属性变更
        private void OnSignalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Signal.value))
            {
                var signal = (Signal)sender;
                SignalValueChanged?.Invoke(signal); // 触发事件
            }
        }
        // 通过 Signal 名称修改值
        public bool TryUpdateSignalValue(string signalName, double newValue)
        {
            // 查找对应的 Signal
            var signal = Signals.FirstOrDefault(s => s.Name == signalName);
            if (signal != null)
            {
                signal.value = newValue; // 通过属性设置器触发事件
                return true;
            }
            return false;
        }
    }
  //  [TypeConverter(typeof(ClassConverter<Signal>))]
   // [Editor(typeof(MyCollectionEditor), typeof(UITypeEditor))]
    public class Signal
    {
        private string _name;
        private int _StartBit;
        private int _Length;
        private bool _IsMotorolaByteOrder;
        private double _Factor;
        private double _Offset;
        private string _Unit;
        private double _Minimum;
        private double _Maximum;
        private bool _IsSigned;
        private double _value;

        [Browsable(false)]
        public double value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }     // 信号名称 (对应 `SG_` 后的名称)
        [DisplayName("起始位")]
        public int StartBit
        {
            get => _StartBit;
            set { _StartBit = value; OnPropertyChanged(); }
        }     // 起始位 (0-based, 最低有效位)
        [DisplayName("信号长度")]
        public int Length
        {
            get => _Length;
            set { _Length = value; OnPropertyChanged(); }
        }       // 信号长度 (单位：bit)
        [DisplayName("字节序")]
        public bool IsMotorolaByteOrder
        {
            get => _IsMotorolaByteOrder;
            set { _IsMotorolaByteOrder = value; OnPropertyChanged(); }
        }  // 字节序 (true=Intel小端, false=Motorola大端)
        [DisplayName("缩放因子")]
        public double Factor
        {
            get => _Factor;
            set { _Factor = value; OnPropertyChanged(); }
        }   // 缩放因子 (对应DBC中的 `Factor`)
        [DisplayName("偏移量")]
        public double Offset
        {
            get => _Offset;
            set { _Offset = value; OnPropertyChanged(); }
        }    // 偏移量 (对应DBC中的 `Offset`)
        [DisplayName("单位")]
        public string Unit
        {
            get => _Unit;
            set { _Unit = value; OnPropertyChanged(); }
        }      // 单位 (对应DBC中的单位字符串)
               //    public List<string> Receivers { get; set; } // 接收节点列表 (对应DBC中的接收ECU)

        // 新增扩展属性
        [Browsable(false)]
        public double Minimum
        {
            get => _Minimum;
            set { _Minimum = value; OnPropertyChanged(); }
        }        // 物理最小值（对应[最小值|最大值]）
        [Browsable(false)]
        public double Maximum
        {
            get => _Maximum;
            set { _Maximum = value; OnPropertyChanged(); }
        }        // 物理最大值
        public bool IsSigned
        {
            get => _IsSigned;
            set { _IsSigned = value; OnPropertyChanged(); }
        }         // 是否为有符号数
                  //     public Dictionary<int, string> ValueDescriptions { get; set; } // 枚举值定义（对应VAL_）
                  //public bool IsMultiplexed { get; set; }    // 是否多路复用信号
                  //public int MultiplexId { get; set; }       // 多路复用标识符（0=主信号）
                  //public string Comment { get; set; }        // 信号注释（对应CM_）

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

  
    }

