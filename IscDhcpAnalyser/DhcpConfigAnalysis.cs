using System;
using System.Collections.Generic;
using System.Text;

namespace IscDhcpAnalyser
{
    public class DhcpConfigAnalysis
    {
        /// <summary>
        /// The name of the domain which the DHCP server will assign IP addresses.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// The name of the Domain Name Servers.
        /// </summary>
        public string[] DomainNameServers { get; set; }

        /// <summary>
        /// The default configured lease time for the ip address assignament (in milliseconds).
        /// </summary>
        public int DefaultLeaseTime { get; set; }

        /// <summary>
        /// The maximun time for the lease if the ip address.
        /// </summary>
        public int MaxLeaseTime { get; set; }

        /// <summary>
        /// <see langword="true"/> if the DHCP server is authoritative.
        /// </summary>
        public bool Authoritative { get; set; }

        /// <summary>
        /// All the subnets declared 
        /// </summary>
        public List<SubnetDeclaration> SubnetDeclarations { get; set; } = new List<SubnetDeclaration>();

        /// <summary>
        /// The hosts ip address assignament.
        /// </summary>
        public List<HostDeclaration> HostDeclarations { get; set; } = new List<HostDeclaration>();
    }
}
