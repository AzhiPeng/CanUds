using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProcessFormsApp.Communication.CAN
{
    public class CanReceiver
    {
        private Dictionary<uint, Message> _messages;

        //public CanReceiver(Dictionary<uint, Message> messages)
        //{
        //    _messages = messages;
        //}
      //  new SecurityUnlocker Security = new SecurityUnlocker();
        public void OnMessageReceived(Message mes, byte[] data)
        {
          //  if (_messages.TryGetValue(id, out Message message))
            {
                foreach (var signal in mes.Signals)
                {
                    int rawValue = DecodeSignal(data, signal);
                    double physicalValue = rawValue * signal.Factor + signal.Offset;
                    signal.value = physicalValue;
                    Console.WriteLine($"{signal.Name}: {physicalValue} {signal.Unit}");
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
            // 参数校验
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (signal.StartBit < 0) throw new ArgumentException("起始位不能为负");
            if (signal.Length <= 0) throw new ArgumentException("信号长度必须大于0");

            ulong rawValue = 0;
            int bitsRemaining = signal.Length;

            if (signal.IsMotorolaByteOrder)
            {
                // Motorola格式（大端字节序+高位优先）
                int currentByte = signal.StartBit / 8;
                int bitCursor = signal.StartBit % 8;

                while (bitsRemaining > 0 && currentByte < data.Length)
                {
                    // 计算当前字节可取的位数
                    int bitsToTake = Math.Min(bitCursor + 1, bitsRemaining);

                    // 创建位掩码并提取位段
                    byte mask = (byte)((1 << bitsToTake) - 1);
                    byte chunk = (byte)((data[currentByte] >> (bitCursor - bitsToTake + 1)) & mask);

                    // 合并到原始值
                    rawValue = (rawValue << bitsToTake) | (uint)chunk;

                    // 更新状态
                    bitsRemaining -= bitsToTake;
                    bitCursor -= bitsToTake;

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
                // Intel格式（小端字节序+低位优先）
                int currentBit = signal.StartBit;

                for (int i = 0; i < signal.Length; i++)
                {
                    int byteIndex = currentBit / 8;
                    int bitIndex = currentBit % 8;

                    if (byteIndex >= data.Length) break;

                    // 设置对应位
                    if ((data[byteIndex] & (1 << bitIndex)) != 0)
                    {
                        rawValue |= (1UL << i);
                    }
                    currentBit++;
                }
            }

            // 符号扩展处理（适用于有符号类型）
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
    }
}
