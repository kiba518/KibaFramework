using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Diagnostics;
using System.Reflection;
using log4net;


namespace Utility
{ 
    /// <summary>
    /// 日志类
    /// </summary>
    public class Logger
    { 
        /// <summary>
        /// 方法开始
        /// </summary>
        /// <param name="method"></param>
        public static void LogStart()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("====================" +
                sf.GetMethod().Name + ":" + " Start====================");
        }
        /// <summary>
        /// 方法结束
        /// </summary>
        /// <param name="method"></param>
        public static void LogEnd()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("====================" +
                sf.GetMethod().Name + ":" + " End====================");
        } 
        /// <summary>
        /// 写错误信息
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(Exception ex)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
         
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Environment.NewLine); 
            if (ex != null)
            {
                sb.AppendLine("Exception Info:" + ex.ToString());
                sb.AppendLine("Source:" + ex.Source);
                sb.AppendLine("Message:" + ex.Message);
                sb.AppendLine("StackTrace:" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    sb.AppendLine("InnerException.Message:" + ex.InnerException.Message);
                }
            } 
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info Begin ===");
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Error(sb.ToString());
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info End ===");
        }
        public static void Error(InnerExcepiton ex)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Environment.NewLine);
            if (ex != null)
            {
                sb.AppendLine("Exception Info:" + ex.ToString());
                sb.AppendLine("Source:" + ex.Source);
                sb.AppendLine("Message:" + ex.Message);
                sb.AppendLine("StackTrace:" + ex.StackTrace); 
            }
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info Begin ===");
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Error(sb.ToString());
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info End ===");
        }
        /// <summary>
        /// 写错误信息
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(string errMessage, Exception ex)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Environment.NewLine);
            sb.AppendLine("=== Error Info Begin ===");
            if (ex != null)
            {
                sb.AppendLine("Exception Info:" + ex.ToString());
                sb.AppendLine("CustomMessage:" + errMessage);
                sb.AppendLine("Source:" + ex.Source);
                sb.AppendLine("Message:" + ex.Message);
                sb.AppendLine("StackTrace:" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    sb.AppendLine("InnerException.Message:" + ex.InnerException.Message);
                }
            }
            sb.AppendLine("=== Error Info End ===");

            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info Begin ===");
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Error(sb.ToString());
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info End ===");
        }

        public static void Error(string errMessage, InnerExcepiton ex)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Environment.NewLine);
            sb.AppendLine("=== Error Info Begin ===");
            if (ex != null)
            {
                sb.AppendLine("Exception Info:" + ex.ToString());
                sb.AppendLine("CustomMessage:" + errMessage);
                sb.AppendLine("Source:" + ex.Source);
                sb.AppendLine("Message:" + ex.Message);
                sb.AppendLine("StackTrace:" + ex.StackTrace); 
            }
            sb.AppendLine("=== Error Info End ===");

            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info Begin ===");
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Error(sb.ToString());
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug("=== Error Info End ===");
        }
        /// <summary>
        /// 写INFO内容
        /// </summary>
        /// <param name="strMessage"></param>
        public static void Info(string strMessage)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Info(strMessage);
        }
        /// <summary>
        /// 写Debug内容
        /// </summary>
        /// <param name="strMessage"></param>
        public static void Debug(string strMessage)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            log4net.LogManager.GetLogger(sf.GetMethod().DeclaringType.FullName).Debug(strMessage);
        }
    }
}
