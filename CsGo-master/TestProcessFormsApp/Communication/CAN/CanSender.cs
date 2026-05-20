using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProcessFormsApp.Communication.CAN
{
    public class CanSender
    {
        public void SendMessage(Message message, Dictionary<string, double> signalValues)
        {
            byte[] data = new byte[message.Dlc];
            foreach (var signal in message.Signals)
            {
                if (signalValues.TryGetValue(signal.Name, out double physicalValue))
                {
                    // 计算原始值
                    double rawValue = (physicalValue - signal.Offset) / signal.Factor;
                    // 将rawValue编码到data的对应位置
                    EncodeSignal(data, signal, (int)rawValue);
                }
            }
            // 使用CAN库发送数据
            // 示例：CANInterface.Send(message.Id, data);
        }

        /// <summary>
        /// 将信号原始值编码到CAN数据帧
        /// </summary>
        /// <param name="data">目标字节数组（将被修改）</param>
        /// <param name="signal">信号定义</param>
        /// <param name="rawValue">待编码的原始值</param>
        /// <exception cref="ArgumentOutOfRangeException">数值超出可表示范围</exception>
        private void EncodeSignal(byte[] data, Signal signal, int rawValue)
        {
            // 参数校验
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (signal.StartBit < 0) throw new ArgumentException("起始位不能为负");
            if (signal.Length <= 0 || signal.Length > 32)
                throw new ArgumentException("信号长度必须在1-32位之间");

            // 计算有效值范围
            int maxValue = signal.IsSigned ?
                (1 << (signal.Length - 1)) - 1 :
                (1 << signal.Length) - 1;
            int minValue = signal.IsSigned ?
                -(1 << (signal.Length - 1)) : 0;

            if (rawValue < minValue || rawValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(rawValue),
                    $"值必须在{minValue}到{maxValue}之间");

            ulong valueToEncode = (ulong)(rawValue & ((1 << signal.Length) - 1));

            if (signal.IsMotorolaByteOrder)
            {
                // Motorola格式（大端字节序）
                int currentByte = signal.StartBit / 8;
                int bitCursor = signal.StartBit % 8;
                int bitsRemaining = signal.Length;

                while (bitsRemaining > 0 && currentByte < data.Length)
                {
                    // 计算当前字节可写入的位数
                    int bitsToWrite = Math.Min(bitCursor + 1, bitsRemaining);

                    // 创建掩码并获取位段
                    ulong mask = (1UL << bitsToWrite) - 1;
                    byte bits = (byte)((valueToEncode >> (bitsRemaining - bitsToWrite)) & mask);

                    // 计算移位量并更新目标字节
                    int shift = bitCursor - bitsToWrite + 1;
                    data[currentByte] = (byte)(
                    ((ulong)data[currentByte] & ~(mask << shift)) |
                    ((ulong)bits << shift));
                    // 更新状态
                    bitsRemaining -= bitsToWrite;
                    bitCursor -= bitsToWrite;

                    // 处理字节边界
                    if (bitCursor < 0)
                    {
                        currentByte++;
                        bitCursor = 7;
                    }
                }
            }
            else
            {
                // Intel格式（小端字节序）
                int currentBit = signal.StartBit;
                for (int i = 0; i < signal.Length; i++)
                {
                    int byteIndex = currentBit / 8;
                    int bitIndex = currentBit % 8;

                    if (byteIndex >= data.Length) break;

                    // 设置或清除对应位
                    if ((valueToEncode & (1UL << i)) != 0)
                        data[byteIndex] |= (byte)(1 << bitIndex);
                    else
                        data[byteIndex] &= (byte)~(1 << bitIndex);

                    currentBit++;
                }
            }
        }
    }
}
