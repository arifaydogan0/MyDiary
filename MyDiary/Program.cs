using System;
using System.Windows.Forms;

namespace MyDiary
{
    internal class Program
    {
        public static Form1 formMain;
        public static FormSettings formSett;
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            formMain = new Form1();
            formSett = new FormSettings();
            Application.Run(formMain);
        }
    }
}
