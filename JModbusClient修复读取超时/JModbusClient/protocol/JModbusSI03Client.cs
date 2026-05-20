using JModbusClient.Communicate;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient.protocol
{
    public class JModbusSI03Client : JModbusTcpClient, IJModbusSI03Client
    {
        private DateTime _lastReceiveTime = DateTime.Now;
        private Timer _activeCheckTimer;
        public int ActiveReceiveInterval { get; set; } = 10;
        public bool IsActive { get; private set; } = true;
        public override event ReceiveActiveDataChangedHandler ReceiveActiveDataChanged;
        private void InitializeActiveCheckTimer()
        {
            _activeCheckTimer = new Timer(CheckActiveStatus, null, 0, 1000);
        }
        private void CheckActiveStatus(object state)
        {
            try
            {
                TimeSpan elapsed = DateTime.Now - _lastReceiveTime;
                if (elapsed.TotalSeconds >= ActiveReceiveInterval)
                {
                    IsActive = false;
                    DebugRecord($"数据接收超时: {elapsed.TotalSeconds:F2}秒");
                }
            }
            catch (Exception ex)
            {
                DebugRecord($"检查活动状态异常: {ex.Message}");
            }
        }
        public JModbusSI03Client(string ip, int port, bool useActiveMode = false) : base(ip, port)
        {
            ActiveModeOn = useActiveMode;
            ReceiveDataChangedEvent += MotorReceiveDataHandler;
            if (ActiveModeOn)
                InitializeActiveCheckTimer();
        }
        /// <summary>
        /// 主动模式接收数据处理事件
        /// </summary>
        /// <param name="sender"></param>
        protected override void MotorReceiveDataHandler(object sender)
        {
            try
            {
                byte[] array = ReadMsgOnActiveMode(_readTimeout);
                int num = array.Length;
                //最后更新时间
                _lastReceiveTime = DateTime.Now;
                IsActive = true;
                if (VerifyDataCompetition(array, ref num))
                    while (ParseCurrentByte(array, ref num, out byte[] currentdata))
                    {
                        if (ValidatecurResponse(currentdata))
                        {
                            try
                            {
                                int[] array2 = ParseModbusResponse(currentdata);
                                if (array2 != null && array2.Count() > 0)
                                    TryAddCollection(array2);
                            }
                            catch (Exception ex)
                            {
                                DebugRecord($"主动模式数据转换溢出: {ex.Message}");
                            }
                        }

                    }
                ReceiveActiveDataChanged?.Invoke(this);
            }
            catch (Exception ex)
            {
                DebugRecord($"主动模式接收的数据异常{ex.Message}");
            }
        }

        private bool ValidatecurResponse(byte[] response)
        {
            if (response == null || response.Length < 33 || response[8] != 0x18)
            {
                byte[] currentdata = new byte[response.Length];
                Array.Copy(response, 0, currentdata, 0, response.Length);
                //                DebugRecord("Read ModbusTCPActiveMore3-Data:  " + BitConverter.ToString(currentdata) + ", bufLen: " + response.Length);
                return false;
            }
            return true;
        }

        private bool ParseCurrentByte(byte[] array, ref int num, out byte[] currentdata)
        {

            if (num < 33)
            {
                currentdata = new byte[num];
                Array.Copy(array, 0, currentdata, 0, num);
                //                DebugRecord("Read ModbusTCPActiveMore2-Data:  " + BitConverter.ToString(currentdata) + ", bufLen: " + num);
                return false;
            }
            int tmpLen = array[8] + 9;
            currentdata = new byte[tmpLen];
            Array.Copy(array, 0, currentdata, 0, tmpLen);
            num -= tmpLen;
            Array.Copy(array, tmpLen, array, 0, tmpLen);
            return true;
        }

        private bool VerifyDataCompetition(byte[] array, ref int num)
        {
            //至少两组数据
            if (num > 66)
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 33; i++)
                {
                    bytes[0] = array[i + 1];
                    bytes[1] = array[i];
                    bytes[2] = array[i + 34];
                    bytes[3] = array[i + 33];
                    if (BitConverter.ToInt16(bytes, 0) + 1 == BitConverter.ToInt16(bytes, 2) && array[i + 8] == 0x18)
                    {
                        num = num - i;
                        Array.Copy(array, i, array, 0, num);
                        return true;
                    }
                }
            }
            else
            if (num == 33 && array[8] == 0x18)
            {
                return true;
            }
            else
            {
                byte[] bytes1 = new byte[num];
                Array.Copy(array, 0, bytes1, 0, num);
                //                DebugRecord("Read ModbusTCPActiveMore1-Data:  " + BitConverter.ToString(bytes1) + ", bufLen: " + num);
            }
            return false;
        }

        private static int[] ParseModbusResponse(byte[] array)
        {
            if (array == null || array.Length < 9)
                return new int[0];
            int datalen = array[8] / 2;
            if (datalen <= 0 || 9 + datalen * 2 > array.Length)
                return new int[0];
            int[] array2 = new int[datalen];
            for (int i = 0; i < datalen; i++)
            {
                byte b2 = array[9 + i * 2];
                byte b3 = array[9 + i * 2 + 1];
                array[9 + i * 2] = b3;
                array[9 + i * 2 + 1] = b2;
                array2[i] = BitConverter.ToInt16(array, 9 + i * 2);
            }
            return array2;
        }

        private byte[] ReadMsgOnActiveMode(int timeoutMs)
        {
            var receive = _motorTcpClient.Read(timeoutMs);
            if (receive.Status == ReadStatus.Error)
                throw new ArgumentException(receive.Message);
            if (receive.Status == ReadStatus.Timeout)
                throw new TimeoutException(receive.Message);
            //byte[] bytes1 = new byte[receive];
            //Array.Copy(bytes, 0, bytes1, 0, receive);
            //            DebugRecord("Read ModbusTCPActive-Data:  " + BitConverter.ToString(bytes1) + ", bufLen: " + receive);
            return receive.Data;
        }
        public override void Dispose()
        {
            ReceiveDataChangedEvent -= MotorReceiveDataHandler;
            _activeCheckTimer?.Dispose();
            _activeCheckTimer = null;
            base.Dispose();
        }
    }
}
