using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feather32u4Programmer {
    public static class ControlExtensions {
        public static void InvokeIfRequired(this Control control, MethodInvoker action) {
            while (!control.Visible) {
                System.Threading.Thread.Sleep(50);
            }
            if (control.InvokeRequired) {
                control.Invoke(action);
            } else {
                action();
            }
        }
    }
}
