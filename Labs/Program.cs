using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Labs
{
    class Program
    {
        /* Change m_CurrentTask to the current task you are working on.
         * Valid tasks are L1, L2_1, L2_2, L3, Challenge, L4 and ACW
         */
        private static Lab m_CurrentLab = Lab.ACW;

        #region No Changes Required Here Ever!
        /* 
         * By all means look at this code, but you will not be required to change
         * any of this code to complete any lab work
         */
        private enum Lab { ACW };
        static void Main(string[] args)
        {
            GameWindow window = null;
            switch(m_CurrentLab)
            {
                case Lab.ACW:
                    window = new ACW.ACWWindow();
                    break;
                default:
                    throw new Exception("Unrecognised lab number");
            }

            if (window != null)
            {
                window.Run();
            }
        }
        #endregion
    }
}
