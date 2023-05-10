using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ideassoccer.BaseStation.UI.Utilities
{
    public class Networking
    {
        static public string? GetWiFiIP()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var intf in interfaces)
            {
                if (intf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    IPInterfaceProperties ipProperties = intf.GetIPProperties();
                    UnicastIPAddressInformationCollection ipAddresses =
                        ipProperties.UnicastAddresses;

                    foreach (UnicastIPAddressInformation ipAddress in ipAddresses)
                    {
                        if (
                            ipAddress.Address.AddressFamily == AddressFamily.InterNetwork
                            && (
                                ipAddress.PrefixOrigin == PrefixOrigin.Manual
                                || ipAddress.PrefixOrigin == PrefixOrigin.Dhcp
                            )
                        )
                        {
                            return ipAddress.Address.ToString();
                        }
                    }
                }
            }

            return null;
        }
    }
}
