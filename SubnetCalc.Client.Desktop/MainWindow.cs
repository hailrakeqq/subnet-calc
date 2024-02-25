using Gtk;
using System;
using UI = Gtk.Builder.ObjectAttribute;
using SubnetCalc;
namespace SubnetCalc.Client.Desktop;

class MainWindow : Window
{
    [UI] private Button quit_btn = null;
    [UI] private Button calculate_btn = null;
    [UI] private Entry ip_input = null;
    [UI] private ComboBoxText subnet_masks = null;

    public MainWindow() : this(new Builder("MainWindow.glade")) { }

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        builder.Autoconnect(this);
        quit_btn.Clicked += QuitButton_Clicked;

        DeleteEvent += Window_DeleteEvent;

        calculate_btn.Clicked += Calculate;
    }

    public void Calculate(object sender, EventArgs e)
    {
        var ip = ip_input.Text;
        var mask = subnet_masks.ActiveText;
        var calculator = new IPCalculator(ip, mask);

        var result = calculator.Calculate();
        Console.WriteLine(result.Network);
        Console.WriteLine(result.NetworkClass);
        Console.WriteLine(result.FirstIPAddress);
        Console.WriteLine(result.LastIPAddress);
        Console.WriteLine(result.BroadcastIPAddress);
        Console.WriteLine(result.Range);
        Console.WriteLine(result.HostCount);
        Console.WriteLine(result.IPAddressCount);

        //TODO: render this info in GUI (label)

    }

    private void QuitButton_Clicked(object sender, EventArgs e)
    {
        Application.Quit();
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }
}
