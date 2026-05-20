using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestProcessFormsApp.测试类
{
    public class CsvHelper
    {
        public static BindingList<ParseTestItems> parseTestItems = new BindingList<ParseTestItems>();
        // 导出到CSV
        public static void ExportToCsv(IEnumerable<ParseTestItems> items, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // 写入表头
                writer.WriteLine("测试方法,测试项名称,被测数据,判断数据类型,测试结果对比规则,判断数据1,判断数据2,判断数据3");
                foreach (var item in items)
                {
                    var fields = new List<string>
                    {
                        EscapeCsv(item.测试方法),
                        EscapeCsv(item.测试项名称),
                      //  EscapeCsv(item.负载开启报文开关),
                 
                        EscapeCsv(item.被测数据),
                        EscapeCsv(item.判断数据类型.ToString()),
                        EscapeCsv(item.测试结果对比规则.ToString())
                    };

                    // 处理判断数据1-12
                    for (int i = 1; i <= 3; i++)
                    {
                        var value = GetPrivateField(item, $"_判断数据{i}");
                        fields.Add(EscapeCsv(Format判断数据(value, item.判断数据类型.ToString())));
                    }
                   // fields.Add(EscapeCsv(item.测试顺序.ToString()));
             
                    writer.WriteLine(string.Join(",", fields));
                }
            }
        }

        // 从CSV导入
        public static BindingList<ParseTestItems> ImportFromCsv(string filePath)
        {
            var list = new BindingList<ParseTestItems>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            if (lines.Length < 1) return list;

            for (int i = 1; i < lines.Length; i++)
            {
                var fields = SplitCsvLine(lines[i]);
                //if (fields.Length < 10) continue;

                var item = new ParseTestItems
                {
                    测试方法 = UnescapeCsv(fields[0]),
                    测试项名称 = UnescapeCsv(fields[1]),
              //      负载开启报文开关 = UnescapeCsv(fields[2]),               
                    被测数据 = UnescapeCsv(fields[2]),
                    判断数据类型 = Parse数据类型(UnescapeCsv(fields[3])),
                    测试结果对比规则 = Parse对比规则(UnescapeCsv(fields[4])),
                 //   测试顺序 = int.Parse(UnescapeCsv(fields[8])),
                   
                };

                // 设置判断数据私有字段
                for (int j = 1; j <= 3; j++)
                {
                    var value = UnescapeCsv(fields[4 + j]);
                    var field = item.GetType().GetField($"_判断数据{j}",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    field?.SetValue(item, value);
                }

                list.Add(item);
            }
            return list;
        }

        // 辅助方法
        private static string EscapeCsv(string value) =>
            $"\"{value?.Replace("\"", "\"\"") ?? string.Empty}\"";

        private static string UnescapeCsv(string value) =>
            value.Trim('"').Replace("\"\"", "\"");

        private static object GetPrivateField(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(obj) ?? "0";
        }

        private static string Format判断数据(object value,string type)
        {
            if (value == null) return "0";
            if(type == "_int")
            {
                if (int.TryParse(value.ToString(), out var i))
                    return i.ToString();
            }
            if (type == "_float")
            {
                if (float.TryParse(value.ToString(), out var num))
                    return num.ToString();
            }
            else
            {
                return value.ToString();
            }
            return "0";
        }

        private static 时序 Parse时序(string value)
        {
            switch (value)
            {
                case "常开": return 时序.常开;
                case "Case": return 时序.Case;
                default: return 时序.常开;
            }
        }
        private static string OutPut时序(时序 value)
        {
            switch (value)
            {
                case 时序.常开: return "常开";
                case 时序.Case: return "Case";
                default: return "常开";
            }
        }
        private static string OutPutCase(OpenorClose value)
        {
            switch (value)
            {
                case OpenorClose.OFF: return "OFF";
                case OpenorClose.ON: return "ON";
                case OpenorClose.正转: return "正转";
                case OpenorClose.反转: return "反转";
                case OpenorClose.空: return "空";
                default: return "OFF";
            }
        }
        private static OpenorClose ParseCase(string value)
        {
            switch (value)
            {
                case "OFF": return OpenorClose.OFF;
                case "ON": return OpenorClose.ON;
                case "正转": return OpenorClose.正转;
                case "反转": return OpenorClose.反转;
                case "空": return OpenorClose.空;
                default: return OpenorClose.OFF;
            }
        }
        private static 数据类型 Parse数据类型(string value)
        {
              switch (value)
            {
                case "_int":
                    return 数据类型._int;
                case "_float":
                    return 数据类型._float;
                default:
                    return 数据类型._String;
            }
        }

        private static 对比规则 Parse对比规则(string value)
        {
            return Enum.TryParse(value, out 对比规则 result) ? result : 对比规则.范围;
        }

        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var current = new StringBuilder();

            foreach (var c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
