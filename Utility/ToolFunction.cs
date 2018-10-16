using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility
{
    public class ToolFunction
    {
        public static object GetPropertyValue(object obj, string name)
        {
            var p = obj.GetType().GetProperty(name);
            if (p != null)
            {
                object drv1 = p.GetValue(obj);
                return drv1;
            }
            else
            {
                return null;
            }
        }

        public static void SetPropertyValue(object obj, string name, Object value)
        {
            var p = obj.GetType().GetProperty(name);
            if (p != null)
            {
                p.SetValue(obj, value); 
            }
            else
            {
                return;
            }
        }

        public static object GetPropertyValue(object obj, PropertyInfo info)
        {
            return info.GetValue(obj);
        }





        #region 转换
        public static bool IsNumeric(string value)
        { 
            return Regex.IsMatch(value, @"^(\-)?\d+([.][0-9]+){0,1}$");
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]*$");
        }

        public static decimal ToDec(object source)
        {
            if (source.ToString().Length > 0 && source.ToString() == ".")
            {
                source = "0";
            }
            return ToDec(source, -1);
        }
        public static decimal ToDec(object source, decimal def)
        {
            if (source != null && source != System.DBNull.Value && source.ToString() != "")
            {
                if (IsNumeric(source.ToString()))
                {
                    try
                    {
                        def = decimal.Parse(source.ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return def;
        } 
     

        public static DateTime ToDateTime(object source)
        {
            return ToDateTime(source, "1900-1-1");
        } 
        public static DateTime ToDateTime(object source, string def)
        {
            if (source != System.DBNull.Value && source != null && source.ToString() != "")
            {
                if (source is DateTime)
                    return (DateTime)source;

                try
                {
                    DateTime de = DateTime.Parse(source.ToString().Trim());
                    return de;
                }
                catch
                {
                }
            }
            return DateTime.Parse(def);
        }

        public static int ToInt(object source)
        {
            return ToInt(source, -1);
        }
        public static int ToInt(object source, int def)
        {
            if (source != null && source != System.DBNull.Value)
            {
                string txt = source.ToString().ToLower();

                if (txt == "false")
                {
                    return 0;
                }
                else if (txt == "true")
                {
                    return 1;
                }
                else if (txt != "")
                {
                    try
                    {
                        def = int.Parse(source.ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return def;
        }
    
        public static double ToDouble(object source)
        {
            return ToDouble(source, -1);
        }
        public static double ToDouble(object source, double def)
        {
            if (source != null && source != System.DBNull.Value && source.ToString() != "")
            {
                try
                {
                    def = double.Parse(source.ToString());
                }
                catch (Exception ex)
                {
                }
            }

            return def;
        }

        public static double ToFloat(object source)
        {
            return ToFloat(source, -1);
        }
        public static double ToFloat(object source, double def)
        {
            if (source != null && source != System.DBNull.Value && source.ToString() != "")
            {
                try
                {
                    def = float.Parse(source.ToString());
                }
                catch (Exception ex)
                {
                }
            }

            return def;
        }
        #endregion
    }
}
