using Model;
using Proxy;
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
    /// 代理帮助类
    /// </summary>
    public class ProxyHelper
    {
        public TestDataProxy _TestDataProxy = new TestDataProxy();
        public TestDataProxy TestDataProxy { get { return _TestDataProxy; } }

        public void Save(DataGridConfig dataGridConfig)
        {
            
        }
    }
}
