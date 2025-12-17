using System;
using System.Windows.Forms;

namespace FileConverterExtension
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                var form = new ConversionForm(args[0]);
                Application.Run(form);
            }
            else
            {
                MessageBox.Show("This application is designed to be called from the Windows Explorer context menu.",
                    "File Converter Extension", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
