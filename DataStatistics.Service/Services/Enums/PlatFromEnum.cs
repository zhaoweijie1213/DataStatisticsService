using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStatistics.Service.Enums
{
    /// <summary>
    /// 平台来源
    /// </summary>
    public enum PlatFromEnum
    {
        All,
        IOS,
        Android,
        Windows
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataType
    {
        登录=1,
        按钮=2,
        自定义=3

    }
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserState
    {
        /// <summary>
        /// 未注册
        /// </summary>
        Register,
        /// <summary>
        /// 活跃
        /// </summary>
        Active
    }
    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class PlatFromEnumExt
    {

        public static string GetName(this PlatFromEnum platFromEnum)
        {
            return Enum.Parse(typeof(PlatFromEnum), platFromEnum.ToString()).ToString();
        }

        /// <summary>
        /// 获取枚举中的 所有 name 和 value
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="dic">存储数据的 Dictionary:int,string</param>
        public static Dictionary<int, string> GetEnumAllNameAndValue<T>()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                dic.Add(Convert.ToInt32(value), value.ToString());
            }
            return dic;
        }

        /// <summary>
        /// 获取枚举中的 所有 name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetEnumAllName<T>()
        {
            List<string> dic = new List<string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                dic.Add(value.ToString());
            }
            return dic;
        }

        /// <summary>
        /// 获取枚举中的 所有 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<int> GetEnumAllValue<T>()
        {
            List<int> dic = new List<int>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                dic.Add(Convert.ToInt32(value));
            }
            return dic;
        }
    }
}
