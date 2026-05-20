using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProcessFormsApp.测试类
{
    public class CaseFactory
    {
        public static ICaseInterface CreateCase(string caseType, string caseName, object value1, object value2, object value3,int 测试顺序,
          /* string 测试项报文开关/*, 数据来源类型 测试数据来源*/ 数据类型 判断数据类型, 对比规则 测试结果对比规则,string 被测数据
          /* string 负载开启报文开关*/)
        {
            ICaseInterface temp = null;
            switch (caseType)
            {
                case "电压读取":
                    temp = new 电压读取();
                    break; 
                case "CANID读取":
                    temp = new CANID读取();
                    break;
                case "Can读取":
                    temp = new Can读取();
                    break;
                case "前工位判断":
                    temp = new 前工位判断();
                    break; 
                case "追溯码读取":
                    temp = new 追溯码读取();
                    break;
                case "ECUID判断":
                    temp = new ECUID判断();
                    break;
                case "扫码对比":
                    temp = new 扫码对比();
                    break; 
                case "密钥解锁验证":
                    temp = new 写MEC校验();
                    break; 
                case "零件号读取":
                    temp = new 零件号读取();
                    break;
                case "下载ECUID以及密钥":
                    temp = new 下载ECUID以及密钥();
                    break;
                case "获取以及写入ECUID":
                    temp = new 获取以及写入ECUID();
                    break;
                case "CAN初始化":
                    temp = new CAN初始化();
                    break;
                case "写密钥":
                    temp = new WriteMASTER_UnlockKey();
                    break;
                case "解锁":
                    temp = new UDS_0D解锁();  
                    break;
                // 2026-05-20: 新增可配置UDS 2F写入测试项。
                // 支持测试方法名称: 写MEC / UDS写2F / Can写2F
                case "写MEC":
                case "UDS写2F":
                case "Can写2F":
                    temp = new UDS_2F可配置写入();
                    break;
                case "周期发送":
                    temp = new 周期发送();
                    break;
                case "上传通用服务器":
                    temp = new 上传GM();
                    break;
                default:
                    temp = null;
                    break;                                                                                                                                                                                                                                                                                                                                                                                                               
            }
            if (temp != null)
            {
                temp.SetCaseName(caseName);
                temp.SetCondition(value1, value2, value3);
                // 2026-05-11: 必须将CSV/重排序后的测试顺序下发到Case实例，
                // 否则TaskMainWorker无法按顺序调度（会出现不按顺序或卡住）。
                temp.Set测试顺序(测试顺序);
                temp.Set被测数据(被测数据);
                temp.Set判断数据类型(判断数据类型);
                temp.Set测试结果对比规则(测试结果对比规则);
            
            //    temp.Set负载开启报文开关(负载开启报文开关);
              
                
                //temp.SetCondition(caseCondition);
            }
            return temp;
        }
         
    }
   
}
