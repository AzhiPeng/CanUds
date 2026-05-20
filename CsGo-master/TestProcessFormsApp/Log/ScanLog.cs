using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLRScan
{
    public class ScanLog
    {
        private static ConcurrentQueue<string> Logs { set; get; } = new ConcurrentQueue<string>();

        public void InsertLog(string log)
        {
            Logs.Enqueue(log);
        }

        public bool GetLogIfExists(out string log)
        {
            if (Logs.Count > 0)
            {
                if (Logs.TryDequeue(out string value))
                {
                    log = value;
                    return true;
                }
                log = string.Empty;
                return false;
            }
            log = string.Empty;
            return false;
        }
    }
}
