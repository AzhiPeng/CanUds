using CCWin.SkinClass;
using JLRScan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TestProcessFormsApp.Boards;

namespace TestProcessFormsApp.测试类
{
    public class ParseTestItems
    {
        public string 测试方法 { get; set; }
        public string 测试项名称 { get; set; }
        public string 被测数据 { get; set; }
        //public 数据来源类型 测试数据来源 { get; set; }//
        public 数据类型 判断数据类型 { get; set; }//
        public 对比规则 测试结果对比规则 { get; set; }//

        private object _判断数据1;
        public object 判断数据1
        {
            get
            {
                try
                {
                    if (_判断数据1 is null)
                        _判断数据1 = 0;
                    switch (判断数据类型)
                    {
                        case 数据类型._String:
                            return _判断数据1.ToString();
                        case 数据类型._int:
                            return _判断数据1.ToInt32();
                        case 数据类型._float:
                            return Convert.ToSingle(_判断数据1);
                        default:
                            return null;
                    }
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
            set
            {
                _判断数据1 = value;
            }
           
        }
        private object _判断数据2;
        public object 判断数据2
        {
            get
            {
                if (_判断数据2 is null)
                    _判断数据2 = 0;
                switch (判断数据类型)
                {
                    case 数据类型._String:
                        return _判断数据2.ToString();
                    case 数据类型._int:
                        return _判断数据2.ToInt32();
                    case 数据类型._float:
                        return Convert.ToSingle(_判断数据2);
                    default:
                        return null;
                }
            }
            set
            {
                _判断数据2 = value;
            }

        }
        private object _判断数据3;
        public object 判断数据3
        {
            get
            {
                try
                {
                    if (_判断数据3 is null)
                        _判断数据3 = 0;
                    switch (判断数据类型)
                    {
                        case 数据类型._String:
                            return _判断数据3.ToString();
                        case 数据类型._int:
                            return _判断数据3.ToInt32();
                        case 数据类型._float:
                            return Convert.ToSingle(_判断数据3);
                        default:
                            return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            set
            {
                _判断数据3 = value;
            }

        }
        public int 测试顺序
        {
            get;
            set;
        }
    }
    public class TestNameWithSpan
    {
        public string 测试项名称 { get; set; }
        public 时序 测试时序 { get; set; }
    }
    public enum 时序
    {
        常开 = 0,
        Case,
    }
    public enum 数据类型
    {
        _String = 0,
        _int = 1,
        _float = 2,
    }
    public enum 数据来源类型
    {
        CAEA008 = 0,
        CAEAAT03 = 1,
    }
    public enum OpenorClose
    {
        OFF = 0,
        ON = 1,
        正转 = 2,
        反转 = 3,
        空 = 4,
    }
}
