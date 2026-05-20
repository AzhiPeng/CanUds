
using JLRScan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestProcessFormsApp;
using TestProcessFormsApp.Public;
using static HMIPLC.WebServer.GetSnInfo;

namespace HMIPLC
{
    internal static class WebServer
    {
        static ScanLog  log = new ScanLog();
        static string serverAddr = "192.168.1.5:19091";
        static string secret = "0D1EFD65E72B4B9FABDA9134775D7750";
        static string groupCode = "test_bench_config";
        static string pageMaxsize = "500";
        static readonly object pendingUploadLock = new object();
        static string PendingUploadFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PendingTraceUploads.json");

        class PendingUploadItem
        {
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public DateTime CreateTime { get; set; } = DateTime.Now;
            public DateTime LastTryTime { get; set; } = DateTime.MinValue;
            public int RetryCount { get; set; } = 0;
            public string LastError { get; set; } = "";
            public UploadTestData Data { get; set; }
        }

        static List<PendingUploadItem> LoadPendingUploadsNoThrow()
        {
            try
            {
                if (!File.Exists(PendingUploadFilePath))
                    return new List<PendingUploadItem>();

                string json = File.ReadAllText(PendingUploadFilePath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<PendingUploadItem>();

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<PendingUploadItem>>(json) ?? new List<PendingUploadItem>();
            }
            catch (Exception ex)
            {
                log.InsertLog("读取待补传队列失败: " + ex.Message);
                return new List<PendingUploadItem>();
            }
        }

        static void SavePendingUploadsNoThrow(List<PendingUploadItem> list)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list ?? new List<PendingUploadItem>(), Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(PendingUploadFilePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                log.InsertLog("保存待补传队列失败: " + ex.Message);
            }
        }

        static void EnqueuePendingUpload(UploadTestData data, string error)
        {
            if (data == null)
                return;

            lock (pendingUploadLock)
            {
                var list = LoadPendingUploadsNoThrow();
                list.Add(new PendingUploadItem
                {
                    Data = data,
                    LastError = error ?? ""
                });
                SavePendingUploadsNoThrow(list);
            }
        }

        internal static int GetPendingUploadCount()
        {
            lock (pendingUploadLock)
            {
                return LoadPendingUploadsNoThrow().Count;
            }
        }

        internal static string ManualRetryPendingUploads()
        {
            List<PendingUploadItem> snapshot;
            lock (pendingUploadLock)
            {
                snapshot = LoadPendingUploadsNoThrow();
                SavePendingUploadsNoThrow(new List<PendingUploadItem>());
            }

            if (snapshot.Count == 0)
                return "没有待补传的追溯数据。";

            int success = 0;
            int failed = 0;
            var remain = new List<PendingUploadItem>();
            foreach (var item in snapshot)
            {
                item.LastTryTime = DateTime.Now;
                if (TryUploadTestData(item.Data, out string error))
                {
                    success++;
                }
                else
                {
                    failed++;
                    item.RetryCount++;
                    item.LastError = error;
                    remain.Add(item);
                }
            }

            if (remain.Count > 0)
            {
                lock (pendingUploadLock)
                {
                    var latest = LoadPendingUploadsNoThrow();
                    latest.AddRange(remain);
                    SavePendingUploadsNoThrow(latest);
                }
            }

            return $"补传完成：成功 {success} 条，失败 {failed} 条，剩余待补传 {GetPendingUploadCount()} 条。";
        }
        //internal static bool CheckPreprocess(string sn, string routingId, string sapNum="123")
        //{
            
        //    var url = $"http://{serverAddr}/publicapi/testbench/checkpreprocess?secret={secret}&sn={System.Web.HttpUtility.UrlEncode(sn)}&orderNum={sapNum}&routingId={routingId}";
             
        //    var request = WebRequest.Create(url);

        //    request.Method = "GET";
        //    request.ContentType = "application/json";

        //    using (var response = request.GetResponse())
        //    using (var stream = response.GetResponseStream())
        //    using (var sr = new StreamReader(stream))
        //    {
        //        var content = sr.ReadToEnd();
        //        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckPreprocessInfo>(content);
        //        //if (p.resultCode == 460)
        //        //    return true;
        //        if (p.resultCode != 0)
        //            log.InsertLog($"前工位查询错误{p.resultCode}：{p.resultText}");
        //        else 
        //            return true;
        //        return false;
        //      //  Program.MsgBox(content);
        //      //  return (p.content?.sn) != null;
        //    }
        //}
        internal static GetSnContent[] GetTestResult(string sn)
        {
            var url = $"http://{serverAddr}/publicapi/CheckData/GetListFromSn?sn={sn}";
            var request = WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var sr = new StreamReader(stream))
            {
                var content = sr.ReadToEnd();
                var p = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSnInfo>(content);
                if (p.resultCode != 0)
                    log.InsertLog($"查询错误{p.resultCode}：{p.resultText}");
                //Program.MsgBox(content);
                return p.content;
            }
            //return null;
        }
        internal static GetSnContent[] GetTestResultFromEcuid(string ecuid)
        {
            var url = $"http://{serverAddr}/publicapi/CheckData/GetListFromEcuid?ecuid={ecuid}";
            var request = WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var sr = new StreamReader(stream))
            {
                var content = sr.ReadToEnd();
                var p = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSnInfo>(content);
                if (p.resultCode != 0)
                    log.InsertLog($"查询错误{p.resultCode}：{p.resultText}");
                //Program.MsgBox(content);
                return p.content;
            }
            //return null;
        }
        //internal static TraceInfoBase[] GetTestResult(string sn)
        //{

        //    //if (!Settings.Default.WEB服务器KEY.IsNullOrEmpty())
        //    //    secret = Settings.Default.WEB服务器KEY;

        //    StringBuilder sb = new StringBuilder();
        //    if (sn.IsNullOrEmpty() == false)
        //    {
        //        for (int i = 0; i < sn.Length; i++)
        //        {
        //            if (sn[i] >= 0x2400 && sn[i] <= 0x2420)
        //                sb.Append((char)(sn[i] - 0x2400));
        //            else if (sn[i] == '␡')
        //                sb.Append((char)127);
        //            else
        //                sb.Append(sn[i]);
        //        }
        //    }
        //    var request = WebRequest.Create($"http://{serverAddr}/publicapi/testbench/GetCheckDataListBySn?secret={secret}&sn={sb}");

        //    request.Method = "GET";
        //    request.ContentType = "application/json";

        //    using (var response = request.GetResponse())
        //    using (var stream = response.GetResponseStream())
        //    using (var sr = new StreamReader(stream))
        //    {
        //        var content = sr.ReadToEnd();
        //        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<GetTestData>(content);
        //        if (p.ResultCode != 0)
        //            log.InsertLog($"查询错误{p.ResultCode}：{p.ResultText}");
        //        return p.GetResult();
        //    }
        //}
        //internal static GetTestContentList[] GetTestResultList(string sn)
        //{
          
        //    //if (!Settings.Default.WEB服务器KEY.IsNullOrEmpty())
        //    //    secret = Settings.Default.WEB服务器KEY;
        //    var request = WebRequest.Create($"http://{serverAddr}/publicapi/TestBench/GetCheckDataListBySn?secret={secret}&sn={sn}");

        //    request.Method = "GET";

        //    using (var response = request.GetResponse())
        //    using (var stream = response.GetResponseStream())
        //    using (var sr = new StreamReader(stream))
        //    {
        //        var content = sr.ReadToEnd();
        //        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<GetTestDataList>(content);
        //        //Program.MsgBox(content);
        //        if (p.resultCode != 0)
        //            throw new Exception($"获取失败{p.resultCode}：{p.resultText}");
        //        return p.content;
        //        //textBox1.Text = content;
        //    }
        //}
        /// <summary>
        /// 上传检测数据 [0]sn [1]检测数据 [2]ecuid [3]零件号 [4]结果文本
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static string SaveTestResult(string[] result)
        {
            var data = BuildUploadTestData(result);
            if (data == null || data.sn.IsNullOrEmpty())
            {
                WinForm.追溯结果 = 3;
                return "追溯上传失败：SN为空。";
            }

            if (TryUploadTestData(data, out string error))
            {
                WinForm.追溯结果 = 2;
                return "数据保存合格";
            }

            // 2026-05-20: 上传失败自动落盘缓存，待网络恢复后手动补传。
            EnqueuePendingUpload(data, error);
            WinForm.追溯结果 = 3;
            log.InsertLog($"追溯上传失败，已缓存待补传。原因: {error}，当前待补传数量: {GetPendingUploadCount()}");
            return "上传失败，已缓存待补传。";
        }

        static UploadTestData BuildUploadTestData(string[] result)
        {
            if (result == null || result.Length < 5)
                return null;

            string sncode = result[0];
            if (sncode.IsNullOrEmpty())
                return null;

            return new UploadTestData()
            {
                sn = sncode,
                data = result[1],
                mtc = WinForm.追溯码,
                ecuid = result[2],
                partNum = result[3],
                ResultCode = Convert.ToInt32(WinForm.本轮测试结果),
                productModel = WinForm.产品型号,
                ResultText = result[4],
                stationNum = LocalSetting.localSetting.CurrentGw,
            };
        }

        static bool TryUploadTestData(UploadTestData data, out string error)
        {
            error = string.Empty;
            if (data == null)
            {
                error = "上传数据为空。";
                return false;
            }

            string s = string.Empty;
            try
            {
                var request = WebRequest.Create($"http://{serverAddr}/publicapi/CheckData/UploadCheckData?");
                request.Method = "POST";
                request.ContentType = "application/json";

                s = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        error = "服务器返回状态码错误: " + response.StatusCode;
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var re = reader.ReadToEnd();
                        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultData>(re);
                        if (p == null)
                        {
                            error = "服务器返回空响应。";
                            return false;
                        }
                        if (p.resultCode != 0)
                        {
                            error = $"保存数据失败：{p.resultText}, code: {p.resultCode}";
                            return false;
                        }
                        log.InsertLog("保存成功：" + re);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                log.InsertLog("上传异常：" + ex.Message);
                return false;
            }
        }
        internal static GetFileListData[] GetFIleList()
        {

            var request = WebRequest.Create($"http://{serverAddr}/publicapi/ConfigFile/GetList?groupCode={groupCode}");

            request.Method = "GET";

            try
            {
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var content = sr.ReadToEnd();
                    var p = Newtonsoft.Json.JsonConvert.DeserializeObject<GetFileList>(content);
                    //Program.MsgBox(content);
                    if (p.resultCode != 0)
                        log.InsertLog($"获取失败{p.resultCode}：{p.resultText}");
                    return p.content.data;
                    //textBox1.Text = content;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
           
        }
        internal static void UploadFile(string filenm, byte[] dat, string mark = "通用发样上位机")
        {
            var request = WebRequest.Create($"http://{serverAddr}/publicapi/configFile/UploadFileInfo?secret={secret}");

            request.Method = "POST";
            request.ContentType = "application/json";

            
            var d = new UploadFileInfo()
            {
                fileName = filenm,
                data = dat,
                remark = mark
            };

            byte[] bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(d));
            request.ContentLength = bytes.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var re = reader.ReadToEnd();
                    var p = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultData>(re);
                    if (p.resultCode != 0)
                        throw new Exception($"上传文件失败{p.resultCode}：{p.resultText}");
                }
            }
         
        }
        internal static MemoryStream DownLoadFile(string key)
        {
            var request = WebRequest.Create($"http://{serverAddr}/publicapi/ConfigFile/GetFileBinary?id={key}");

            request.Method = "GET";

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                MemoryStream ms = new MemoryStream();
                var buf = new byte[4096];
                while (true)
                {
                    var len = stream.Read(buf, 0, 4000);
                    if (len == 0)
                        break;
                    ms.Write(buf, 0, len);
                }

                //textBox1.Text = content;
                //var bt = Encoding.ASCII.GetBytes(content);
                //MessageBox.Show("ms:" + ms.Length);
                //File.WriteAllBytes(textBox6.Text, ms.ToArray());
                return ms;
            }
        }
        public class GetSnInfo
        {
            public int resultCode;
            public string resultText;
            public string taskId;
            public GetSnContent[] content;
            public class GetSnContent
            {
                public string id;
                public string sn;//产品条码;
                public string productModel;//产品型号;
                public string partNum;//零件号
                public string ecuid;
                public string mtc;
                public string stationNum;
                public int resultCode;
                public string resultText;
                public string data;
                public string ip;
                public string operationTime;
                public string insertTime;
            }
           //public GetSnContent[] GetResult( )
           //{
           //     if (content == null )
           //     {
           //         return null;
           //     }
           //     GetSnContent[] getSnContents = new GetSnContent[content.Length];

           //}
        }
        public class Ipdev
        {
            public string 设备名称 { get; set; }
            public string 设备编码 { get; set; }
            public string 设备备注 { get; set; }
        }
        public class ProductModelResponse
        {
            public int resultCode { get; set; }
            public string resultText { get; set; }
            [Browsable(false)]
            public string taskId { get; set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public modelPage content { get; set; }
        }
        public class modelPage
        {
            public int total { get; set; }
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public List<modelOne> data { get; set; }
        }
        public class modelOne
        {
            public string id { get; set; }
            public string productName { get; set; }
            public string productModel { get; set; }
            public string sapMaterialCode { get; set; }
            public string pnCode { get; set; }
            public string customerName { get; set; }
            public string customerCode { get; set; }
            public int checkSapMaterialComplete { get; set; }
            public string createUserId { get; set; }
            public string createTime { get; set; }
            public string updateUserId { get; set; }
            public string updateTime { get; set; }
            public int state { get; set; }
        }

        public class RoutinglistResponse
        {
            public int resultCode { get; set; }
            public string resultText { get; set; }
            public string taskId { get; set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public RoutingPage content { get; set; }
        }
        public class RoutingPage
        {
            public int tatal { get; set; }
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public List<RoutingOne> data { get; set; }
        }

        public class RoutingOne
        {
            [DisplayName("工艺路线id")]
            public string id { get; set; }
            [DisplayName("产品id")]
            public string productModelId { get; set; }

            /// <summary>
            /// 工艺名称
            /// </summary>
            [DisplayName("工艺名称")]
            public string processName { get; set; }
            /// <summary>
            /// 工位号
            /// </summary>
            [DisplayName("工位号")]
            public string stationNum { get; set; }

            /// <summary>
            /// 工艺顺序
            /// </summary>
            [DisplayName("工艺顺序")]
            public int prosessOder { get; set; }
            /// <summary>
            /// 操作权限验证 0不验证 1操作代码 2工号 
            /// </summary>
            [Browsable(false)]
            public int operationType { get; set; }
            /// <summary>
            /// 操作权限内容
            /// </summary>
            [Browsable(false)]
            public string operationCode { get; set; }
            /// <summary>
            /// 是否判断前工序
            /// </summary>
            [DisplayName("是否判断前工序")]
            public int checkPreprocess { get; set; }
            /// <summary>
            /// 工位校验方式（0：不校验；1：条码正则；2：列表逗号隔开）
            /// </summary>
            [DisplayName("工位校验方式")]
            public int stationVerificationType { get; set; }
            [Browsable(false)]
            public string stationVerificationContent { get; set; }
            [DisplayName("创建用户id")]
            public string createUserId { get; set; }
            [DisplayName("创建时间")]
            public string createTime { get; set; }
            [DisplayName("最后更新人id")]
            public string updateUserId { get; set; }
            [DisplayName("最后更新时间")]
            public string updateTime { get; set; }
            [DisplayName("状态")]
            public int state { get; set; }
        }
        public class GetMtcInfo
        {
            public int resultCode;
            public string resultText;
            public string taskId;
            public GetMtcContent content;
            public class GetMtcContent
            {
                public int sn;//序号;
                public string partType;//零件类型
                public string productModel;//产品型号;
                public string prefix;//年月日;
            }
        }
     

        //internal static ProductModelResponse GetProductModelResponse()
        //{
           
        //    //if (!Settings.Default.WEB服务器KEY.IsNullOrEmpty())
        //    //    secret = Settings.Default.WEB服务器KEY;
        //    int index = Settings.Default.型号名称.IndexOf('+');
        //    string temp = index > 0 ? Settings.Default.型号名称.Substring(0, index) : Settings.Default.型号名称;
        //    var request = WebRequest.Create($"http://{serverAddr}/publicapi/processrouting/getproductlist?secret={secret}&productModel={temp}" +
        //        $"&pageindex=0&pagesize={pageMaxsize}");

        //    request.Method = "GET";
        //    request.ContentType = "application/json";

        //    using (var response = request.GetResponse())
        //    using (var stream = response.GetResponseStream())
        //    using (var sr = new StreamReader(stream))
        //    {
        //        var content = sr.ReadToEnd();
        //        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductModelResponse>(content);
        //        if (p.resultCode != 0)
        //            Program.ErrHdl($"查询错误{p.resultCode}：{p.resultText}");
        //        return p;
        //    }
        //}
    
        internal static string ConvertString(string s)
        {
            if (s.IsNullOrEmpty())
                return s;
            StringBuilder sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < 32)
                    sb.Append((char)(0x2400 + s[i]));
                else if (s[i] == 127)
                    sb.Append('␡');
                else
                    sb.Append(s[i]);
            }
            return sb.ToString();
        }
    }
    public class CheckPreprocessInfo
    {
        public int resultCode;
        public string resultText;
        public string taskId;
        public CheckPreprocessContent content;
        public class CheckPreprocessContent
        {
            public string sn;
            public string sap订单号;
            public string 产品型号;
        }
    }

    public class GetTestContent
    {
        public class GetTestData
        {
            public string id;
            public string sn;
            public string dataName;
            public string createTime;
            public string insertTime;
            public string receiveTime;
            public int infoType;
            public Dictionary<string, string> data = new Dictionary<string, string>();
           // public OperationInfo operationInfo = new OperationInfo();
            public int resultCode;
            public string resultText;
            public string ip;
            public string productModel;
            public string productSn;
            public string sap工单号;
            public string routingId;
            public ProductionInfo productionInfo;

            public EnvirdfonmentalData envirdfonmentalData = new EnvirdfonmentalData();
        }
        public int pageSize;
        public int pageIndex;
        public GetTestData[] data;
    }
  
    //public class GetTestDataList
    //{
    //    public int resultCode;
    //    public string resultText;
    //    public string taskId;
    //    public GetTestContentList[] content;
    //    public class GetTestContentList
    //    {
    //        public int pageSize;
    //        public int pageIndex;

    //        public GetTestContent data;
    //    }
    //    public class GetTestContent
    //    {
    //        public string id;
    //        public string sn;
    //        public string dataName;
    //        public string createTime;
    //        public string insertTime;
    //        public string receiveTime;
    //        public int infoType;
    //        public Dictionary<string, string> data = new Dictionary<string, string>();
    //        public OperationInfo operationInfo = new OperationInfo();
    //        public int resultCode;
    //        public string resultText;
    //        public string ip;
    //        public string productModel;
    //        public string productSn;
    //        public string sap工单号;
    //        public string routingId;
    //        public ProductionInfo productionInfo;

    //        public EnvirdfonmentalData envirdfonmentalData = new EnvirdfonmentalData();
    //    }
    //}
   /// <summary>
   /// 上传检测数据
   /// </summary>
    public class UploadTestData
    {
        public string sn;
        public string productModel = "";
        public string partNum = "";
        public string ecuid = "";
        public string mtc = "";
        public string stationNum = "";
        public int ResultCode;
        public string ResultText;
        public string data;
        public string operationTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    public class PartsData
    {
        public string partid;
        public string sn;
    }
    public class EnvirdfonmentalData
    {
        public string 温度;
        public string 湿度;
        //public string PM25;
    }
    public class ProductionInfo
    {
        public string 产品型号;
        public string sap工单号 = "检测工位";
        public string 零件号;
        //public string 物料号;
    }
    //public class OperationInfo
    //{
    //    public string 操作时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    //    public string 操作人 = LogIn.CurrentUser.name=="未登录"?"员工1": LogIn.CurrentUser.name;
    //    public string 工序名称 = Settings.Default.设备名称;
    //    public string 工位 = Settings.Default.追溯工位号;
    //    public string 工号;
    //}
    public class UploadFileInfo
    {
        [DisplayName("打印序列号")]
        public string fileName { get; set; }
        public int fileType { get; set; } = 0;//0二进制  1文本
        public string tag { get; set; }
        public string describe { get; set; }
        public string remark { get; set; } = "通用上位机";
        public byte[] data { get; set; }
        public string dataFormat { get; set; } = "base64";
        public string groupCode { get; set; } = "test_bench_config";
    }
    public class ResultData
    {
        public int resultCode { get; set; }
        public string resultText { get; set; }
        public string taskId { get; set; }
    }
    public class GetFileList
    {
        public int resultCode { get; set; }
        public string resultText { get; set; }
        public string taskId { get; set; }
        public GetFileListContent content { get; set; }
        public class GetFileListContent
        {
            public int? total { get; set; }
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            public GetFileListData[] data { get; set; }
        }
    }
    public class GetFileListData : IComparable
    {
        [Browsable(false)]
        public string id { get; set; }
        public string fileName { get; set; }
        [Browsable(false)]
        public string tag { get; set; }
        [Browsable(false)]
        public string describe { get; set; }
        [Browsable(false)]
        public string remark { get; set; }
        public string createTime { get; set; }
        public string 
            
            Time { get; set; }

        public int CompareTo(object obj)
        {
            var cmp = (GetFileListData)obj;
            if (fileName.IsNullOrEmpty())
                return -1;
            if (cmp.fileName.IsNullOrEmpty())
                return 1;
            for (int i = 0; i < fileName.Length; i++)
            {
                if (i < cmp.fileName.Length)
                {
                    if (fileName[i] != cmp.fileName[i])
                        return fileName[i] - cmp.fileName[i];
                }
                else if (fileName.Length == cmp.fileName.Length)
                    return 0;
            }
            return 1;
        }

        public override string ToString()
        {
            return fileName;
        }
    }
    public class GetFileBinary
    {
        public byte[] content { get; set; }
    }

}
