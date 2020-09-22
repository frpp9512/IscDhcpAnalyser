using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IscDhcpAnalyser
{
    /// <summary>
    /// A subnet declared in for Ip leasing.
    /// </summary>
    public class SubnetDeclaration
    {
        /// <summary>
        /// The identifier of the subnet (Ex. 192.168.0.0)
        /// </summary>
        public string NetworkIdentifier { get; set; }

        /// <summary>
        /// The network mask (Ex. 255.255.255.0)
        /// </summary>
        public string NetworkMask { get; set; }

        /// <summary>
        /// The network broadcast address (Ex. 192.168.0.255)
        /// </summary>
        public string Broadcast { get; set; }

        /// <summary>
        /// The network gateway (Ex. 192.168.0.1)
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// <see langword="true"/> if the address range are defined.
        /// </summary>
        public bool RangeDefined => !string.IsNullOrEmpty(IpRangeFrom) && !string.IsNullOrEmpty(IpRangeTo);

        /// <summary>
        /// The starting ip address to lease to clients.
        /// </summary>
        public string IpRangeFrom { get; set; }

        /// <summary>
        /// The starting ip address to lease to clients.
        /// </summary>
        public string IpRangeTo { get; set; }
    }
}