using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProcessFormsApp.测试类
{
     class CaseConfig
    {

        static List<TestNameWithSpan> testNameWithSpans = new List<TestNameWithSpan>();

        public static List<ICaseInterface> caseList = new List<ICaseInterface>()
        {
         //   CaseFactory.CreateCase("电压读取","电压读取","5"),
        };
        public static void CreateParseItem()
        {
            caseList.Clear();
            int fallbackStep = 1;
            foreach (var item in WinForm.ParseTestItemsSetting) 
            {
                // 2026-05-11: 兜底顺序赋值，避免配置缺失/导入异常时全部Step=0。
                int step = item.测试顺序 > 0 ? item.测试顺序 : fallbackStep;
                caseList.Add(CaseFactory.CreateCase(item.测试方法, item.测试项名称, item.判断数据1, item.判断数据2, item.判断数据3, step,
               /* item.测试项报文开关 ,*//* item.测试数据来源,*/ item.判断数据类型, item.测试结果对比规则,item.被测数据/*,  item.负载开启报文开关*/
               )
                    );
                fallbackStep++;
            }
        }
        public static void 测试项排序()
        {
            testNameWithSpans.Clear();
            foreach (var temp in WinForm.ParseTestItemsSetting)
            {
                testNameWithSpans.Add(new TestNameWithSpan { 测试项名称 = temp.测试项名称 });
                temp.测试顺序 = testNameWithSpans.Count;
            }
        }
    }
}
