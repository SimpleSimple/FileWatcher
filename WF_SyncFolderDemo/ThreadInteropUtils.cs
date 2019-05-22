using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_SyncFolderDemo
{
    public class ThreadInteropUtils
    {
        public static void OpeMainFormControl(Action action, Control control = null)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action); //返回主线程（创建控件的线程）
            }
            else
            {
                action();
            }
        }
    }
}
