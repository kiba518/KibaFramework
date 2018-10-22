using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public  class InnerExcepiton
    {  
        //
        // 摘要: 
        //     获取或设置指向此异常所关联帮助文件的链接。
        //
        // 返回结果: 
        //     统一资源名称 (URN) 或统一资源定位器 (URL)。
        public virtual string HelpLink { get; set; }
        //
        // 摘要: 
        //     获取或设置 HRESULT（一个分配给特定异常的编码数字值）。
        //
        // 返回结果: 
        //     HRESULT 值。
        public int HResult { get; set; } 
      
        //
        // 摘要: 
        //     获取描述当前异常的消息。
        //
        // 返回结果: 
        //     解释异常原因的错误消息或空字符串 ("")。
        public string Message { get; set; }
        //
        // 摘要: 
        //     获取或设置导致错误的应用程序或对象的名称。
        //
        // 返回结果: 
        //     导致错误的应用程序或对象的名称。
        //
        // 异常: 
        //   System.ArgumentException:
        //     该对象必须为运行时 System.Reflection 对象
        public virtual string Source { get; set; }
        //
        // 摘要: 
        //     获取调用堆栈上直接帧的字符串表示形式。
        //
        // 返回结果: 
        //     用于描述调用堆栈的直接帧的字符串。
        public virtual string StackTrace { get; set; }
        //
        // 摘要: 
        //     获取引发当前异常的方法。
        //
        // 返回结果: 
        //     引发当前异常的 System.Reflection.MethodBase。
        public MethodBase TargetSite { get; set; }
    }
}
