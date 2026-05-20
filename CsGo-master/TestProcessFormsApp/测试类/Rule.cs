using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace TestProcessFormsApp.测试类
{
    public enum 对比规则
    {
        全部,
        范围,
        等于,
        不等于,
        大于,
        大于或等于,
        小于,
        小于或等于
    }
    [Serializable]
    public class Rule
    {
        
        public bool IsMatch(对比规则 rule, int 实际值,int value1,int value2 = 0)
        {
            try
            {
                switch (rule)
                {
                    case 对比规则.全部:
                        return true;
                    case 对比规则.范围:
                        if (实际值 >= value1 && 实际值 <= value2)
                            return true;
                        break;
                    case 对比规则.等于:
                        if (实际值 == value1)
                            return true;
                        break;
                    case 对比规则.不等于:
                        if (实际值 != value1)
                            return true;
                        break;
                    case 对比规则.大于:
                        if (实际值 > value1)
                            return true;
                        break;
                    case 对比规则.大于或等于:
                        if (实际值 >= value1)
                            return true;
                        break;
                    case 对比规则.小于:
                        if (实际值 < value1)
                            return true;
                        break;
                    case 对比规则.小于或等于:
                        if (实际值 <= value1)
                            return true;
                        break;
                }
            }
            catch(Exception ex) { }
            return false;
        }
        public bool IsMatch(对比规则 rule, float 实际值, float value1, float value2 = 0)
        {
            try
            {
                switch (rule)
                {
                    case 对比规则.全部:
                        return true;
                    case 对比规则.范围:
                        if (实际值 >= value1 && 实际值 <= value2)
                            return true;
                        break;
                    case 对比规则.等于:
                        if (实际值 == value1)
                            return true;
                        break;
                    case 对比规则.不等于:
                        if (实际值 != value1)
                            return true;
                        break;
                    case 对比规则.大于:
                        if (实际值 > value1)
                            return true;
                        break;
                    case 对比规则.大于或等于:
                        if (实际值 >= value1)
                            return true;
                        break;
                    case 对比规则.小于:
                        if (实际值 < value1)
                            return true;
                        break;
                    case 对比规则.小于或等于:
                        if (实际值 <= value1)
                            return true;
                        break;
                }
            }
            catch (Exception ex) { }
            return false;
        }
        public bool IsMatch(对比规则 rule, double 实际值, double value1, double value2 = 0)
        {
            try
            {
                switch (rule)
                {
                    case 对比规则.全部:
                        return true;
                    case 对比规则.范围:
                        if (实际值 >= value1 && 实际值 <= value2)
                            return true;
                        break;
                    case 对比规则.等于:
                        if (实际值 == value1)
                            return true;
                        break;
                    case 对比规则.不等于:
                        if (实际值 != value1)
                            return true;
                        break;
                    case 对比规则.大于:
                        if (实际值 > value1)
                            return true;
                        break;
                    case 对比规则.大于或等于:
                        if (实际值 >= value1)
                            return true;
                        break;
                    case 对比规则.小于:
                        if (实际值 < value1)
                            return true;
                        break;
                    case 对比规则.小于或等于:
                        if (实际值 <= value1)
                            return true;
                        break;
                }
            }
            catch (Exception ex) { }
            return false;
        }

    }


}
