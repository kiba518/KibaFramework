using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ViewModel 
{
    /// <summary>
    /// 获取主线程帮助类
    /// </summary>
    public class DispatcherHelper
    {
        private static Dispatcher _UI_Dispatcher = null;
        public static Dispatcher UI_Dispatcher
        {
            get
            {
                if (_UI_Dispatcher == null)
                {
                    _UI_Dispatcher = GetUIDispatcher();
                }
                return _UI_Dispatcher;
            }
            set { _UI_Dispatcher = value; }
        }

        public static Dispatcher GetUIDispatcher()
        {
            try
            {
                return Application.Current.MainWindow.Dispatcher;
            }
            catch
            {
                return null;
            }
        }

        public static void DoEvents()
        {
            try
            {
                DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
                DispatcherFrame nestedFrame = new DispatcherFrame();
                DispatcherOperation exitOperation = DispatcherHelper.UI_Dispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);
                Dispatcher.PushFrame(nestedFrame);
                if (exitOperation.Status != DispatcherOperationStatus.Completed)
                {
                    exitOperation.Abort();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }

        public static void ExcuteDelegete(Action act)
        {
            try
            {
                DispatcherHelper.UI_Dispatcher.Invoke(act);
            }
            catch
            { }
        }
        public static void DoMenthodByDispatcher<T>(Action<T> action, T obj)
        {
            UI_Dispatcher.BeginInvoke(new Action(() =>
            {
                action(obj);
            }), DispatcherPriority.Normal);
        }
        public static void DoMenthodByDispatcher(Action action)
        {
            UI_Dispatcher.BeginInvoke(new Action(() =>
            {
                action();
            }), DispatcherPriority.Normal);
        }
    }
}
