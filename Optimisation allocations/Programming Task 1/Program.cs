using System;
using System.Windows.Forms;

namespace AllocationsApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region Library
            PT1.Log.Append("Main()");
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AllocationsViewerForm());
        }
    }
}