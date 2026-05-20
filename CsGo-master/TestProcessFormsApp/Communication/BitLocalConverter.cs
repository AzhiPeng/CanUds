using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadPlateSerialPort.Common
{
    /// <summary>
    /// 此计算机体系结构大小端处理后的BitConverter
    /// </summary>
    internal static class BitLocalConverter
    {
        /// <summary>
        /// 以字节数组的形式返回指定的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static byte[] GetBytes(dynamic value)
        {
            byte[] result = BitConverter.GetBytes(value);
            return BitConverter.IsLittleEndian ? result.Reverse().ToArray() : result;
        }
        internal static short ToInt16(byte[] value, int startIndex) => BitConverter.IsLittleEndian
                ? BitConverter.ToInt16(value.Skip(startIndex).Take(2).Reverse().ToArray(), 0)
                : BitConverter.ToInt16(value, startIndex);
        internal static int ToInt32(byte[] value, int startIndex) => BitConverter.IsLittleEndian
                ? BitConverter.ToInt32(value.Skip(startIndex).Take(4).Reverse().ToArray(), 0)
                : BitConverter.ToInt32(value, startIndex);
    }
}
