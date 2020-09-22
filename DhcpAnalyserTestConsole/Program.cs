using IscDhcpAnalyser;
using System;
using System.Linq;

namespace DhcpAnalyserTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var analyser = new DchpConfigAnalyser(@"C:\Users\fperez\source\repos\IscDhcpAnalyser\IscDhcpAnalyser\testconfig.conf");
            var analysis = analyser.AnalyseAsync().GetAwaiter().GetResult();
            Console.WriteLine($"Domain name: {analysis.DomainName}");
            Console.WriteLine($"Domain servers: {string.Join(", ", analysis.DomainNameServers)}");
            Console.WriteLine($"Default lease time: {analysis.DefaultLeaseTime}");
            Console.WriteLine($"Max lease time: {analysis.MaxLeaseTime}");
            Console.WriteLine($"Is authoritative: {analysis.Authoritative}");
            Console.WriteLine("\r\nSubnet declarations\r\n==================");
            foreach (var declaration in analysis.SubnetDeclarations)
            {
                Console.WriteLine($"Subnet identifier: {declaration.NetworkIdentifier}");
                Console.WriteLine($"Subnet mask: {declaration.NetworkMask}");
                if (declaration.RangeDefined)
                {
                    Console.WriteLine("IP range: ");
                    Console.WriteLine($"From: {declaration.IpRangeFrom}");
                    Console.WriteLine($"To: {declaration.IpRangeTo}");
                }
                Console.WriteLine($"Broadcast: {declaration.Broadcast}");
                Console.WriteLine($"Gateway: {declaration.Gateway}");
                Console.WriteLine();
            }
            Console.WriteLine("\r\nHost declarations\r\n==================");
            foreach (var declaration in analysis.HostDeclarations)
            {
                Console.WriteLine($"Host: {declaration.Host}");
                Console.WriteLine($"MAC address: {declaration.MacAddress}");
                Console.WriteLine($"Fixed IP address: {declaration.IpAddress}");
                Console.WriteLine($"Option host name: {declaration.Hostname}");
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
