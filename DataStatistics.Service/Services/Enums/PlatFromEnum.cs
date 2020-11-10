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
        public static void GetEnumAllNameAndValue<T>(ref Dictionary<int, string> dic)
        {
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                dic.Add(Convert.ToInt32(value), value.ToString());
            }
        }
    }
}
