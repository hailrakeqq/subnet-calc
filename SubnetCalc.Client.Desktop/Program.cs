using System;
using Gtk;

namespace SubnetCalc.Client.Desktop
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.SubnetCalc.Client.Desktop.SubnetCalc.Client.Desktop", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
