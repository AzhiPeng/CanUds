using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JModbusClient.Log
{
    public sealed class StoreLog
    {
        private string filename = null;
        private static volatile StoreLog instance;
        private static readonly Object objectsync = new Object();
        private static readonly Object writesync = new Object();
        public static StoreLog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objectsync)
                    {
                        if (instance == null)
                        {
                            instance = new StoreLog();
                        }
                    }
                }
                return instance;
            }
        }
        private StoreLog()
        {

        }
        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }

        public void Store(string message)
        {
            try
            {
                if (filename != null)
                {
                    lock (writesync)
                    {
                        using (StreamWriter sw = new StreamWriter(filename, true))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + ": " + message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while writing to log file: " + ex.Message);
            }
        }        
    }
}
