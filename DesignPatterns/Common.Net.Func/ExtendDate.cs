﻿/***************************************************************************** 
*        filename :ExtendDate 
*        描述 : 

*        创建者 TangXiaoJun
*        CLR版本:            4.0.30319.42000 
*        新建项输入的名称:   ExtendDate 
*        机器名称:           LD 
*        注册组织名:          
*        命名空间名称:       Common.Net.Helper 
*        文件名:             ExtendDate 
*        创建系统时间:       2016/2/1 16:07:54 
*        创建年份:           2016 
/*****************************************************************************/
using System;

namespace Common.Net.Func
{
    /// <summary>
    /// 时间的扩展
    /// </summary>
    public class ExtendDate
    {
        /// <summary>
        /// 获取当前时间一天的起始时间2016-01-01 00:00:00.000
        /// </summary>
        /// <returns> 返回时间字符串</returns>
        public static string GetBeginDayStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000";
        }
        /// <summary>
        /// 获取 当前的时间一天的结束时间2016-01-01 23:59:59.999
        /// </summary>
        /// <returns> 返回时间字符串</returns>
        public static string GetEndDayStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59.999";
        }
        /// <summary>
        /// 获取指定时间到当前时间的时间差字符串
        /// </summary>
        /// <param name="time">指定的时间</param>
        /// <param name="time2">目标时间</param>
        /// <param name="outformat">默认输出格式</param>
        /// <returns></returns>
        public static string GetTimeSpanString(DateTime time2, DateTime time, string outformat)
        {
            TimeSpan ts = time2.Subtract(time);
            if (ts.Days > 30)
            {
                return time.ToString(outformat);
            }
            else if (ts.Days > 0)
            {
                return ts.Days + "天前";
            }
            else if (ts.Hours > 0)
            {

                return ts.Hours + "小时前";
            }
            else if (ts.Minutes > 0)
            {
                return ts.Minutes + "分钟前";
            }
            else
            {
                return "刚刚";
            }
        }

        /// <summary>获取指定时间到当前时间的日期差字符串
        /// </summary>
        /// <param name="time">指定的时间</param>
        /// <param name="time2">目标时间</param>
        /// <param name="outformat">默认输出格式</param>
        /// <returns>今天，昨天，前天，几天前（小于30天），日期（大于30天）</returns>
        public static string GetDateSpanString(DateTime time, DateTime time2, string outformat)
        {
            TimeSpan ts = time2.Date.Subtract(time.Date);
            if (ts.Days > 30)
            {
                return time.ToString(outformat);
            }
            else if (ts.Days > 2)
            {
                return ts.Days + "天前";
            }
            else if (ts.Days > 1)
            {

                return "前天";
            }
            else if (ts.Days > 0)
            {
                return "昨天";
            }
            else
            {
                return "今天";
            }
        }
        /// <summary>获取指定时间到当前时间的日期差字符串
        /// </summary>
        /// <param name="time">指定的时间</param>
        /// <param name="time2">目标时间</param>
        /// <param name="hourstyle">小时[-分钟][-秒]输出格式</param>
        /// <param name="outformat">默认输出格式</param>
        /// <returns>今天6：10，昨天6：10，前天6：10，几天前（小于30天），日期（大于30天）</returns>
        public static string GetDateSpanStringWithTime(DateTime time2, DateTime time, string hourstyle, string outformat)
        {
            TimeSpan ts = time2.Date.Subtract(time.Date);
            if (ts.Days > 30)
            {
                return time.ToString(outformat);
            }
            else if (ts.Days > 2)
            {
                return ts.Days + "天前";
            }
            else if (ts.Days > 1)
            {

                return "前天" + time.ToString(hourstyle);
            }
            else if (ts.Days > 0)
            {
                return "昨天" + time.ToString(hourstyle);
            }
            else
            {
                return "今天" + time.ToString(hourstyle);
            }
        }
        /// <summary>
        /// 返回bool类型 判断是否小于当前时间 小于则返回true,不小于返回false
        /// </summary>
        /// <returns>小于当前时间则返回false,不小于当前时间返回true</returns>
        public static bool IsMinnerTime(DateTime time)
        {
            DateTime currentTime = System.DateTime.Now;//获取当前系统时间
            if (currentTime > time)
            {
                return true;
            }
            return false;
        }

        ///// <summary>得到时间的字符串
        ///// 得到时间的字符串
        ///// </summary>
        ///// <param name="td">要解析的时间</param>
        ///// <param name="spliter">日期分隔符（大写X代表汉字分割符）</param>
        ///// <param name="len">返回的时间长度1-6分别为年 月 日 时 分 秒（如果为空，则返回1-14的纯时间数字）</param>
        ///// <returns>解析完成的字符串</returns>
        public static string getNow(DateTime td, string spliter, int len)
        {
            string rtStr = string.Empty;
            if (spliter == string.Empty)
            {

                rtStr = td.ToString("yyyyMMddhhmmssfff");
                return rtStr.Substring(0, len > 17 ? 17 : len);
            }
            else
            {
                string[] tmpDt = new string[7];
                if (spliter == "X")
                {
                    tmpDt[0] = td.Year.ToString() + "年";
                    tmpDt[1] = td.Month.ToString() + "月";
                    tmpDt[2] = td.Day.ToString() + "日";
                    tmpDt[3] = td.Hour.ToString() + "时";
                    tmpDt[4] = td.Minute.ToString() + "分";
                    tmpDt[5] = td.Second.ToString() + "秒";
                    tmpDt[6] = td.Millisecond.ToString() + "毫秒";
                }
                else
                {
                    tmpDt[0] = td.Year.ToString();
                    tmpDt[1] = spliter + td.Month.ToString();
                    tmpDt[2] = spliter + td.Day.ToString();
                    tmpDt[3] = " " + td.Hour.ToString();
                    tmpDt[4] = ":" + td.Minute.ToString();
                    tmpDt[5] = ":" + td.Second.ToString();
                    tmpDt[6] = " " + td.Millisecond.ToString();
                }
                for (int i = 0; i < len; i++)
                {
                    if (i > 6) break;
                    rtStr += tmpDt[i];
                }
                return rtStr;

            }
        }

        /// <summary>得到当前时间的字符串
        /// 得到当前时间的字符串
        /// </summary>
        /// <param name="spliter">日期分隔符（大写X代表汉字分割符）</param>
        /// <param name="len">返回的时间长度1-6分别为年 月 日 时 分 秒（如果为空，则返回1-14的纯时间数字）</param>
        /// <returns>解析完成的字符串</returns>
        public static string getNow(string spliter, int len)
        {
            return getNow(DateTime.Now, spliter, len);
        }



    }
}
