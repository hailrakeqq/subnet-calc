using System;
using System.Net;
using System.Text;
namespace SubnetCalc;

class IpResult
{
    public string Network { get; set; }
    public string NetworkClass { get; set; }
    public string FirstIPAddress { get; set; }
    public string LastIPAddress { get; set; }
    public string BroadcastIPAddress { get; set; }
    public string Range { get; set; }
    public int HostCount { get; set; }
    public int IPAddressCount { get; set; }

    public IpResult(string network, string networkClass, string firstIPAddress, string lastIPAddress, string broadcastIPAddress, string range, int hostCount, int IPAddressCount)
    {
        Network = network;
        NetworkClass = networkClass;
        FirstIPAddress = firstIPAddress;
        LastIPAddress = lastIPAddress;
        BroadcastIPAddress = broadcastIPAddress;
        Range = range;
        HostCount = hostCount;
        this.IPAddressCount = IPAddressCount;
    }
}

class IPCalculator
{
    private string ip;
    private string mask;

    public IPCalculator(string ip, string mask)
    {
        this.ip = ip;
        //Example mask: "3 - 224.0.0.0"
        this.mask = mask.Split("- ")[1];
    }

    public IpResult Calculate()
    {
        var subnet = BinaryToIP(GetSubnet());
        string binaryMask = IPToBinary(mask);
        int mask_bit = 0;

        var networkClass = DetectSubnetClass(subnet);

        for (int i = 0; i < binaryMask.Length; i++)
            if (binaryMask[i] == '1')
                mask_bit++;

        int IPAddressCount = Convert.ToInt32(Math.Pow(2, (32 - mask_bit)));
        int HostCount = IPAddressCount - 2;
        var (firstIP, lastIP, broadcastIP) = CalculateIPRange(subnet, HostCount);
        string range = $"{subnet} - {broadcastIP}";

        return new IpResult(subnet, networkClass, firstIP, lastIP, broadcastIP, range, HostCount, IPAddressCount);
    }

    public static (string firstIPAddress, string lastIPAddress, string broadcastIPAddress) CalculateIPRange(string ipAddress, int numberOfHosts)
    {
        string[] octets = ipAddress.Split('.');

        if (octets.Length == 4 && int.TryParse(octets[3], out var lastOctet))
        {
            int firstOctet = int.Parse(octets[0]);
            int secondOctet = int.Parse(octets[1]);
            int thirdOctet = int.Parse(octets[2]);

            if (lastOctet + numberOfHosts <= 255)
            {
                string firstIPAddress = $"{firstOctet}.{secondOctet}.{thirdOctet}.{lastOctet + 1}";
                string lastIPAddress = $"{firstOctet}.{secondOctet}.{thirdOctet}.{lastOctet + numberOfHosts}";
                string broadcastIPAddress = $"{firstOctet}.{secondOctet}.{thirdOctet}.{lastOctet + numberOfHosts + 1}";
                return (firstIPAddress, lastIPAddress, broadcastIPAddress);
            }
        }

        return (null, null, null);
    }

    public string GetSubnet()
    {
        string ip_bin = IPToBinary(ip);
        string mask_bin = IPToBinary(mask);
        var subnet = new StringBuilder();

        for (int i = 0; i < ip_bin.Length; i++)
        {
            if (ip_bin[i] == '.')
            {
                subnet.Append('.');
                continue;
            }

            if (ip_bin[i] == '1' && mask_bin[i] == '1')
                subnet.Append('1');
            else
                subnet.Append('0');
        }

        return subnet.ToString();
    }

    public static string DetectSubnetClass(string ipAddress)
    {
        if (IPAddress.TryParse(ipAddress, out var ip))
        {
            byte[] octets = ip.GetAddressBytes();
            int firstOctet = octets[0];

            return (firstOctet >= 1 && firstOctet <= 126) ? "Class A" :
                   (firstOctet >= 128 && firstOctet <= 191) ? "Class B" :
                   (firstOctet >= 192 && firstOctet <= 223) ? "Class C" :
                   (firstOctet >= 224 && firstOctet <= 239) ? "Class D (Multicast)" :
                   (firstOctet >= 240 && firstOctet <= 255) ? "Class E (Reserved)" : "Unknown";
        }

        return "Invalid IP address";
    }

    public string IPToBinary(string ipAddress)
    {
        IPAddress ip;
        if (IPAddress.TryParse(ipAddress, out ip))
        {
            byte[] octets = ip.GetAddressBytes();
            string binaryIP = "";

            foreach (byte octet in octets)
            {
                binaryIP += Convert.ToString(octet, 2).PadLeft(8, '0');
                binaryIP += ".";
            }

            binaryIP = binaryIP.TrimEnd('.');

            return binaryIP;
        }
        else
            return "Invalid IP address";
    }

    public string BinaryToIP(string ip_bin)
    {
        string[] binaryOctets = ip_bin.Split('.');

        if (binaryOctets.Length == 4)
        {
            byte[] octets = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                octets[i] = Convert.ToByte(binaryOctets[i], 2);
            }

            IPAddress ipAddress = new IPAddress(octets);
            return ipAddress.ToString();
        }
        else
        {
            return "Invalid binary IP address";
        }
    }
}
