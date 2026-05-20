using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestProcessFormsApp.Communication.CAN
{
    public class SecurityUnlocker
    {
        private int _timeDelay = 2000; // 2秒延迟

        // 2026-04-23: 解锁前等待时间，可用于满足部分ECU的时间窗要求。
        public int TimeDelayMs
        {
            get => _timeDelay;
            set => _timeDelay = value < 0 ? 0 : value;
        }

        // 2026-04-23: 新入口。调用CANUDPClient中的UDS状态机完成完整0x27解锁流程。
        public bool TryUnlock(
            CANUDPClient can,
            uint requestCanId,
            int securityLevel,
            out string error,
            FrameType frameType = FrameType.ExtendedFrame,
            CANSelectType canSelectType = CANSelectType.CANFD,
            uint? expectedResponseCanId = null,
            int p2TimeoutMs = 500,
            int p2StarTimeoutMs = 5000,
            bool enterExtendedSessionBeforeUnlock = true,
            bool useSingleFramePci = true)
        {
            error = string.Empty;
            if (can == null)
            {
                error = "CAN客户端不能为空";
                return false;
            }

            if (_timeDelay > 0)
            {
                Thread.Sleep(_timeDelay);
            }

            return can.TryUdsSecurityAccess(
                frameType,
                requestCanId,
                canSelectType,
                securityLevel,
                GenerateKey,
                out error,
                expectedResponseCanId,
                p2TimeoutMs,
                p2StarTimeoutMs,
                enterExtendedSessionBeforeUnlock,
                useSingleFramePci);
        }

        // 2026-04-23: 兼容旧接口。
        // 只负责“seed -> key报文”发送，不包含负响应/超时处理，建议新代码优先使用TryUnlock。
        public bool Unlock(int securityLevel, CANUDPClient can, byte[] seedResponse)
        {
            if (can == null || seedResponse == null || seedResponse.Length < 3)
            {
                return false;
            }

            byte[] seed = seedResponse.Skip(2).ToArray();
            byte[] key = GenerateKey(seed, securityLevel);
            if (key == null || key.Length == 0)
            {
                return false;
            }

            byte subFunction = (byte)(securityLevel & 0xFF);
            if ((subFunction & 0x01) == 1)
            {
                subFunction++;
            }

            byte[] request = new byte[2 + key.Length];
            request[0] = 0x27;
            request[1] = subFunction;
            Buffer.BlockCopy(key, 0, request, 2, key.Length);

            if (_timeDelay > 0)
            {
                Thread.Sleep(_timeDelay);
            }

            can.SendCANMessage(FrameType.ExtendedFrame, 0x7DF, SendType.Diagnosis, CANSelectType.CANFD, request);
            return true;
        }

        // 暴露默认算法，便于上层测试或替换算法前做联调。
        public byte[] DefaultGenerateKey(byte[] seed, int level)
        {
            return GenerateKey(seed, level);
        }

        private byte[] GenerateKey(byte[] seed, int level)
        {
            return new byte[12] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
            // 2026-04-23: 占位算法（联调用）。正式量产需替换为真实Seed-Key算法。
         //   return seed.Select(b => (byte)(b ^ 0xA5)).ToArray();
        }
    }
}
