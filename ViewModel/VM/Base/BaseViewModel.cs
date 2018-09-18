using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows; 
using DTO;
using Utility; 
using System.Drawing; 
using System.Threading; 
using System.Data;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public const string UINameSapce = "KibaFramework";
        public string UIElementName = "";
        public FrameworkElement UIElement { get; set; }
        public Window WindowMain { get; set; } //主窗体  
       
        public EventHandler CloseCallBack = null; //窗体/页面/控件 关闭委托 
        public BaseViewModel()
        {
            WindowMain = Application.Current.MainWindow; 
            SetUIElement();
            UIElement.DataContext = this;
        }
      
        
        #region 通过反射创建对应的UI元素
        public void SetUIElement()
        {
            Type childType = this.GetType();//获取子类的类型   
            string name = this.GetType().Name;
            UIElementName = name.Replace("VM_", "");
            UIElementName = UIElementName.Replace("`1", "");//应对泛型实体

            if (name.Contains("Window"))
            {
                UIElement = GetElement<Window>();
                (UIElement as Window).Closing += (s, e) =>
                {
                    if (CloseCallBack != null)
                    {
                        CloseCallBack(s, e);
                    }
                };
            }
            else if (name.Contains("Page"))
            {
                UIElement = GetElement<Page>();
                (UIElement as Page).Unloaded += (s, e) =>
                {
                    if (CloseCallBack != null)
                    {
                        CloseCallBack(s, e);
                    }
                };
            }
            else if (name.Contains("UC"))
            {
                UIElement = GetElement<UserControl>();
                (UIElement as UserControl).Unloaded += (s, e) =>
                {
                    if (CloseCallBack != null)
                    {
                        CloseCallBack(s, e);
                    }
                };
            }
            else
            {
                throw new Exception("元素名不规范");
            }
        }

        public E GetElement<E>()
        {
            Type type = GetFormType(UINameSapce + "." + UIElementName);
            E element = (E)Activator.CreateInstance(type);
            return element;
        }

        public static Type GetFormType(string fullName)
        {
            Assembly assembly = Assembly.Load(UINameSapce);
            Type type = assembly.GetType(fullName, true, false);
            return type;
        }
        #endregion

        #region 窗体操作
        public void Show()
        {
            if (UIElement is Window)
            {
                (UIElement as Window).Show();
            }
            else
            {
                throw new Exception("元素类型不正确");
            }
        }

        public void ShowDialog()
        {
            if (UIElement is Window)
            {
                (UIElement as Window).ShowDialog();
            }
            else
            {
                throw new Exception("元素类型不正确");
            }
        }

        public void Close()
        {
            if (UIElement is Window)
            {
                (UIElement as Window).Close();
            }
            else
            {
                throw new Exception("元素类型不正确");
            }
        }

        public void Hide()
        {
            if (UIElement is Window)
            {
                (UIElement as Window).Hide();
            }
            else
            {
                throw new Exception("元素类型不正确");
            }
        }
        #endregion

        #region Message
        public void MessageBox(Window owner, string msg)
        {
            DispatcherHelper.GetUIDispatcher().Invoke(new Action(() =>
            {
                if (owner != null)
                {
                    System.Windows.MessageBox.Show(owner, msg, "提示信息");
                }
                else
                {
                    System.Windows.MessageBox.Show(WindowMain, msg, "提示信息");
                }
            }));
        }

        public void MessageBox(string msg)
        {
            DispatcherHelper.GetUIDispatcher().Invoke(new Action(() =>
            {
                System.Windows.MessageBox.Show(WindowMain, msg, "提示信息");
            }));
        }

        public void MessageBox(string msg, string strTitle)
        {
            DispatcherHelper.GetUIDispatcher().Invoke(new Action(() =>
            {
                System.Windows.MessageBox.Show(WindowMain, msg, "提示信息");
            }));
        }

        public void MessageBox(string msg, Action<bool> callback)
        {
            MessageBox("系统提示", msg, callback);
        }

        public void MessageBox(string title, string msg, Action<bool> callback)
        {
            DispatcherHelper.GetUIDispatcher().Invoke(new Action(() =>
            {
                if (System.Windows.MessageBox.Show(WindowMain, msg, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    callback(true);
                }
                else
                {
                    callback(false);
                }
            }));
        }
        #endregion

        #region   异步线程
        public void AsyncLoad(Action action)
        {
            IAsyncResult result = action.BeginInvoke((iar) =>
            {
            }, null);
        }

        public void AsyncLoad(Action action, Action callback)
        {
            IAsyncResult result = action.BeginInvoke((iar) =>
            {
                this.DoMenthodByDispatcher(callback);
            }, null);
        }

        public void AsyncLoad<T>(Action<T> action, T para, Action callback)
        {
            IAsyncResult result = action.BeginInvoke(para, (iar) =>
            {
                this.DoMenthodByDispatcher(callback);
            }, null);
        }

        public void AsyncLoad<T, R>(Func<T, R> action, T para, Action<R> callback)
        {
            IAsyncResult result = action.BeginInvoke(para, (iar) =>
            {
                var res = action.EndInvoke(iar);
                this.DoMenthodByDispatcher<R>(callback, res);
            }, null);
        }

        public void AsyncLoad<R>(Func<R> action, Action<R> callback)
        {
            IAsyncResult result = action.BeginInvoke((iar) =>
            {
                var res = action.EndInvoke(iar);
                this.DoMenthodByDispatcher<R>(callback, res);
            }, null);
        }

        public void DoMenthodByDispatcher<T>(Action<T> action, T obj)
        {
            DispatcherHelper.GetUIDispatcher().BeginInvoke(new Action(() =>
            {
                action(obj);
            }), DispatcherPriority.Normal);
        }

        public void DoMenthodByDispatcher(Action action)
        {
            DispatcherHelper.GetUIDispatcher().BeginInvoke(new Action(() =>
            {
                action();
            }), DispatcherPriority.Normal);
        }
        #endregion  

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}