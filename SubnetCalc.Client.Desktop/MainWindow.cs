using Gtk;
using System;
using UI = Gtk.Builder.ObjectAttribute;
namespace SubnetCalc.Client.Desktop;

class MainWindow : Window
{
    [UI] private Button quit_btn = null;
    [UI] private Button calculate_btn = null;
    [UI] private Entry ip_input = null;
    [UI] private ComboBoxText subnet_masks = null;
    [UI] private Label network_str = null;
    [UI] private Label firstIP_str = null;
    [UI] private Label lastIP_str = null;
    [UI] private Label broadcast_str = null;
    [UI] private Label range_str = null;
    [UI] private Label host_count_str = null;
    [UI] private Label ip_count_str = null;

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
        network_str.Text = $"Subnet: {result.Network} ({result.NetworkClass})";
        firstIP_str.Text = $"First IP: {result.FirstIPAddress}";
        lastIP_str.Text = $"Last IP: {result.LastIPAddress}";
        broadcast_str.Text = $"Broadcast: {result.BroadcastIPAddress}";
        range_str.Text = $"Range: {result.Range}";
        host_count_str.Text = $"Host Count: {result.HostCount.ToString()}";
        ip_count_str.Text = $"IP Address Count: {result.IPAddressCount.ToString()}";
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
