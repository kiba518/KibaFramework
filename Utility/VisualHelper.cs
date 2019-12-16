using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Utility
{
    public static class VisualHelper
    {
        /// <summary>  
        /// 获取父可视对象中第一个指定类型的子可视对象  
        /// </summary>  
        /// <typeparam name="T">可视对象类型</typeparam>  
        /// <param name="parent">父可视对象</param>  
        /// <returns>第一个指定类型的子可视对象</returns>  
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        /// <summary>
        /// 调用方式   Button btn = sender as Button; Image chb = VisualHelper.GetVisualChild<Image>(btn, c => c.Name == "imgBg");
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T GetVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : Visual
        {
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                T child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v, predicate);
                    if (child != null)
                    {
                        return child;
                    }
                }
                else
                {
                    if (predicate(child))
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        public static T FindVisualChildByName<T>(Visual parent, string name) where T : Visual
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as Visual;
                    string controlName = child.GetValue(System.Windows.Controls.Control.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }
    }
}
