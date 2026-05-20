using JLRScan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestProcessFormsApp.Public;

namespace TestProcessFormsApp.Log
{
    public class LogTestDatacsv
    {
        public string FileNamePath = @"D:\Android\writeCSVTest.csv"; //文件名路径
        public static void SelectPath()
        {
            // 创建FolderBrowserDialog实例
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            // 设置对话框属性
            folderBrowserDialog.Description = "请选择一个文件夹保存文件";

            // 显示对话框
            DialogResult result = folderBrowserDialog.ShowDialog();

            // 检查用户是否点击了“保存”按钮
            if (result == DialogResult.OK)
            {
                // 获取用户选择的文件路径
                LocalSetting.localSetting.FileNamePath = folderBrowserDialog.SelectedPath;
                var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                try
                {
                    var json = JsonConvert.SerializeObject(LocalSetting.localSetting, Formatting.None, jsonSetting);
                    string path = System.Environment.CurrentDirectory;
                    File.WriteAllText(path + "\\LocalSetting.json", json, Encoding.UTF8);
                }
                catch (Exception ex) { }
                // 获取用户选择的文件夹路径
                //      string folderPath = folderBrowserDialog.SelectedPath;

                // 在选择的文件夹中创建或保存文件
                //  string filePath = Path.Combine(folderPath, "example.csv");

            }
        }
        public static ScanLog log = new ScanLog();
        public static void WriteData(string path, List<string[]> ls)
        {
            using (StreamWriter writer = new StreamWriter(/*LocalSetting.localSetting.FileNamePath */path, false, System.Text.Encoding.UTF8))
            {
                // 写入表头
            //     writer.WriteLine(string.Join(",", headers));
                foreach (string[] strArr in ls)
                {
                    writer.WriteLine(string.Join(",", strArr));
                }
                log.InsertLog("数据保存成功！" + path);
            }
        }
        public static List<string[]> ReadCSV(string filePathName)
        {
            List<string[]> ls = new List<string[]>();
            StreamReader sr = new StreamReader(filePathName);
            string strLine = "";
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    ls.Add(strLine.Split(','));
                }
            }
            sr.Close();
            return ls;
        }
    }
}
