using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient
{
    public delegate void ReceiveActiveDataChangedHandler(object sender);

    public delegate void ReceiveDataChangedHandler(object sender);

    public delegate void SendDataChangedHandler(object sender);

    public delegate void ConnectedChangedHandler(object sender);
    public interface IJModbusClient:IDisposable
    {
        /// <summary>
        /// 获取连接状态
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 设置连接超时时间
        /// </summary>
        int ConnectTimeout { get; set; }
        /// <summary>
        /// 获取数据缓冲区数量
        /// </summary>
        int DataActivebufCount { get; }
        /// <summary>
        /// 判断缓冲区是否有数据
        /// </summary>
        bool DataAvailable { get; }
        /// <summary>
        /// 设置主动模式
        /// </summary>
        bool ActiveModeOn { get; set; }
        /// <summary>
        /// 设置日志地址
        /// </summary>
        string Logfilename { get; set; }
        /// <summary>
        /// 设置modbus接口重试最大值默认3
        /// </summary>
        int NumberOfRetries { get; set; }
        /// <summary>
        /// 设置读取超时时间
        /// </summary>
        int ReadTimeout { get; set; }
        /// <summary>
        /// 设置写入超时时间
        /// </summary>
        int WriteTimeout { get; set; }
        /// <summary>
        /// 设置modbus从站地址
        /// </summary>
        byte UnitIdentifier { get; set; }
        /// <summary>
        /// 设置rtu波特率
        /// </summary>
        int BaudRate { get; set; }
        /// <summary>
        /// 设置rtu停止位
        /// </summary>
        StopBits StopBits { get; set; }
        /// <summary>
        /// 设置rtu奇偶校验位
        /// </summary>
        Parity Parity { get; set; }
        /// <summary>
        /// 连接事件
        /// </summary>
        event ConnectedChangedHandler ConnectedChanged;
        /// <summary>
        /// 接受主动模式事件
        /// </summary>
        event ReceiveActiveDataChangedHandler ReceiveActiveDataChanged;
        /// <summary>
        /// 接受主从模式事件
        /// </summary>
        event ReceiveDataChangedHandler ReceiveDataChanged;
        /// <summary>
        /// 数据发送事件
        /// </summary>
        event SendDataChangedHandler SendDataChanged;
        /// <summary>
        /// 连接方法
        /// </summary>
        void Connect();
        /// <summary>
        /// 异步连接方法
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync(Action action = null);
        /// <summary>
        /// 清除所有缓冲区数据
        /// </summary>
        void DiscardInBuffer();
        /// <summary>
        /// 断开modbus连接
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 设置从站地址
        /// </summary>
        /// <param name="slaveAddress"></param>
        void SetSlaveAddress(byte slaveAddress);
        /// <summary>
        /// 读取主动模式数据
        /// </summary>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        int[] Read(int timeoutMs = 1000);
        /// <summary>
        /// 读取线圈数据
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <param name="countRetries"></param>
        /// <returns></returns>
        bool[] ReadCoil(int startingAddress, int quantity, int countRetries = 0);
        Task<bool[]> ReadCoilAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 读取只读线圈数据
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <param name="countRetries"></param>
        /// <returns></returns>
        bool[] ReadDiscreteInputs(int startingAddress, int quantity, int countRetries = 0);
        Task<bool[]> ReadDiscreteInputsAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 读取保持寄存器
        /// </summary>
        /// <param name="startingAddress">开始地址</param>
        /// <param name="quantity">数量</param>
        /// <param name="countRetries">当前重试的次数</param>
        /// <returns></returns>
        int[] ReadHoldingRegister(int startingAddress, int quantity, int countRetries = 0);
        Task<int[]> ReadHoldingRegisterAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <param name="startingAddress">开始地址</param>
        /// <param name="quantity">数量</param>
        /// <param name="countRetries">当前重试的次数</param>
        /// <returns></returns>
        int[] ReadInputRegisters(int startingAddress, int quantity, int countRetries = 0);
        Task<int[]> ReadInputRegistersAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 写入多保持寄存器
        /// </summary>
        /// <param name="startingAddress">开始地址</param>
        /// <param name="values">值</param>
        /// <param name="countRetries">当前重试次数</param>
        void WriteMultipleRegisters(int startingAddress, int[] values, int countRetries = 0);
        Task WriteMultipleRegistersAsync(int startingAddress, int[] values, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 写入单个保持寄存器
        /// </summary>
        /// <param name="startingAddress">开始地址</param>
        /// <param name="value">值</param>
        /// <param name="countRetries">当前重试次数</param>
        void WriteSingleRegister(int startingAddress, int value, int countRetries = 0);
        Task WriteSingleRegisterAsync(int startingAddress, int value, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 写入单个线圈
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <param name="countRetries"></param>
        void WriteSingleCoil(int startingAddress, bool value, int countRetries = 0);
        Task WriteSingleCoilAsync(int startingAddress, bool value, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 写入多个线圈
        /// </summary>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        /// <param name="countRetries"></param>
        void WriteMultipleCoil(int startingAddress, bool[] values, int countRetries = 0);
        Task WriteMultipleCoilAsync(int startingAddress, bool[] values, CancellationToken token = default, int countRetries = 0);
        /// <summary>
        /// 在主动模式下面安全读取保持寄存器
        /// </summary>
        /// <param name="slaveAddress">切换主动和主从模式的地址</param>
        /// <param name="startingAddress">读取保持寄存器的地址</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        int[] ReadsafeRegister(int slaveAddress, int startingAddress, int quantity);
    }
}