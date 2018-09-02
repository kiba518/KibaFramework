using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ViewModel;

namespace KibaFramework
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            VM_WindowMain vm = new VM_WindowMain();
            Application.Current.MainWindow = vm.UIElement as Window;
            vm.Show();
            base.OnStartup(e);
        } 

        protected override void OnLoadCompleted(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {

                }
            }
            catch (Exception ex)
            {


            }
        }
    }
}
