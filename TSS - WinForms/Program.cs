using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TSS.WinForms
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            try
            {
                TSS.WorkHelpers.UniverseParser.LoadFromFile(); 
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                DialogResult dialogResult = MessageBox.Show("Want to load default configs?", "Reading configs exception.",
                    MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    TSS.WorkHelpers.UniverseParser.SaveToFile();
            }
            
            Application.Run(new Form1());
            
        }
    }
}
