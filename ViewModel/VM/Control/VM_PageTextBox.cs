using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class VM_PageTextBox : BaseViewModel
    {
        public TextBox<string> TestTextBox { get; set; }
        public TextBox<string> ChangeTextBox { get; set; }
        public TextBox<string> ChangeTextBox2 { get; set; }
        public VM_PageTextBox()
        {
            TestTextBox = new TextBox<string>();
            ChangeTextBox = new TextBox<string>();
            ChangeTextBox2 = new TextBox<string>();
            ChangeTextBox.TextChangeCallBack = (text) => { MessageBox(text); };//声明TextChange
            ChangeTextBox2.TextChangeCallBack = (text) => { MessageBox(text); };//声明TextChange
        }

        #region 切换设置Text
        //普通命令模式
        public BaseCommand TabKibaCommand
        {
            get
            {
                return new BaseCommand(TabKibaCommand_Executed);
            }
        } 
        private void TabKibaCommand_Executed(object obj)
        {
            TestTextBox.Text = "Kiba";
        }
        //简易命令模式
        public BaseCommand Tab518Command 
        {
            get
            {
                return new BaseCommand((obj)=> { TestTextBox.Text = "518"; });
            }
        }
        #endregion

    }
}
