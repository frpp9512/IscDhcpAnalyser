using System;
using System.Collections.Generic;
using System.Text;

namespace IscDhcpAnalyser
{
    /// <summary>
    /// A host declared for specific Ip assignament.
    /// </summary>
    public class HostDeclaration
    {
        /// <summary>
        /// The name of the host for the ip address assignament.
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// The MAC address of the host interface to connect to the subnet.
        /// </summary>
        public string MacAddress { get; set; } = "";

        /// <summary>
        /// The ip address to assign to the host.
        /// </summary>
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// The optional name of the host.
        /// </summary>
        public string Hostname { get; set; } = "";
    }
}