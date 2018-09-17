using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel 
{
    public class ProgressBar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; 

        #region  进度条绑定属性
        private int _ProgressBarMinimum = 0;
        /// <summary>
        /// 进度条绑定最小值
        /// </summary>
        public int ProgressBarMinimum
        {
            get
            {
                return this._ProgressBarMinimum;
            }
            set
            {
                this._ProgressBarMinimum = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarMinimum"));
                }
            }
        }
        private int _ProgressBarMaximum = 100;
        /// <summary>
        /// 进度条绑定最大值
        /// </summary>
        public int ProgressBarMaximum
        {
            get
            {
                return this._ProgressBarMaximum;
            }
            set
            {
                this._ProgressBarMaximum = value; 
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarMaximum"));
                }
            }
        }
        private volatile int _ProgressBarValue = 0;
        /// <summary>
        /// 进度条绑定当前值
        /// </summary>
        public int ProgressBarValue
        {
            get
            {
                return this._ProgressBarValue;
            }
            set
            {
                this._ProgressBarValue = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarValue"));
                }
            }
        }
        private System.Windows.Visibility _Visibility;
        /// <summary>
        /// 进度条是否显示绑定
        /// </summary>
        public System.Windows.Visibility Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Visibility"));
                }
            }
        }
        #endregion
    }
}
