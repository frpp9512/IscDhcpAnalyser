using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IscDhcpAnalyser
{
    public class DchpConfigAnalyser
    {
        #region Private members

        private readonly string _configPath = "/etc/dhcp/dhcpd.conf";

        #endregion

        #region Constructor

        public DchpConfigAnalyser() { }

        public DchpConfigAnalyser(string configPath) => _configPath = configPath;

        #endregion

        public async Task<DhcpConfigAnalysis> AnalyseAsync()
        {
            if (!File.Exists(_configPath))
            {
                throw new Exception("The config file doesn't exist in the given path.");
            }
            else
            {
                var reader = new StreamReader(_configPath);
                var config = await reader.ReadToEndAsync();
                reader.Close();
                if (string.IsNullOrEmpty(config))
                {
                    throw new Exception("Error in the config file reading.");
                }
                else
                {
                    var analysis = new DhcpConfigAnalysis();
                    ExtractDomainName(config, analysis);
                    ExtractDomainServers(config, analysis);
                    ExtractDefaultLeaseTime(config, analysis);
                    ExtractMaxLeaseTime(config, analysis);
                    ExtractAuthoritativeStatus(config, analysis);
                    ExtractSubnetDeclarations(config, analysis);
                    ExtractHostDeclarations(config, analysis);
                    return analysis;
                }
            }
        }

        private void ExtractDomainName(string config, DhcpConfigAnalysis analysis)
        {
            var domainName = Regex.Match(config, "(?<commented>\\s*#\\s*)*\\s*option\\s+domain-name\\s+\"(?<domain>.*?)\";");
            if (!string.IsNullOrEmpty(domainName.Groups["domain"].Value) && !domainName.Groups["commented"].Success)
            {
                analysis.DomainName = domainName.Groups["domain"].Value;
            }
        }

        private void ExtractDomainServers(string config, DhcpConfigAnalysis analysis)
        {
            var domainNameServers = Regex.Match(config, @"(?<commented>\\s*#\\s*)*option\s+domain-name-servers\s+(?<domainservers>.*?);");
            if (!string.IsNullOrEmpty(domainNameServers.Groups["domainservers"].Value) && 
                !domainNameServers.Groups["commented"].Success)
            {
                var domainServersLine = domainNameServers.Groups["domainservers"].Value;
                if (domainServersLine.Contains(" "))
                {
                    var domainServers = domainServersLine.Split(' ');
                    domainServers = domainServers.Where(ds => !string.IsNullOrWhiteSpace(ds)).ToArray();
                    analysis.DomainNameServers = domainServers;
                }
                else
                {
                    analysis.DomainNameServers = new string[] { domainServersLine };
                }
            }
        }

        private void ExtractDefaultLeaseTime(string config, DhcpConfigAnalysis analysis)
        {
            var defLeaseTimeText = Regex.Match(config, @"(?<commented>\\s*#\\s*)*default-lease-time\s+(?<def_lease_time>.*?);");
            if (!string.IsNullOrEmpty(defLeaseTimeText.Groups["def_lease_time"].Value) &&
                int.TryParse(defLeaseTimeText.Groups["def_lease_time"].Value, out var defLeaseTime) &&
                !defLeaseTimeText.Groups["commented"].Success)
            {
                analysis.DefaultLeaseTime = defLeaseTime;
            }
        }

        private void ExtractMaxLeaseTime(string config, DhcpConfigAnalysis analysis)
        {
            var maxLeaseTimeText = Regex.Match(config, @"(?<commented>\\s*#\\s*)*max-lease-time\s+(?<max_lease_time>.*?);");
            if (!string.IsNullOrEmpty(maxLeaseTimeText.Groups["max_lease_time"].Value) &&
                int.TryParse(maxLeaseTimeText.Groups["max_lease_time"].Value, out var maxLeaseTime) &&
                !maxLeaseTimeText.Groups["commented"].Success)
            {
                analysis.MaxLeaseTime = maxLeaseTime;
            }
        }

        private void ExtractAuthoritativeStatus(string config, DhcpConfigAnalysis analysis)
        {
            var authoritative = Regex.Match(config, @"(?<commented>\\s*#\\s*)*authoritative;\s*");
            if (!authoritative.Groups["commented"].Success)
            {
                analysis.Authoritative = authoritative.Success;
            }
        }

        private void ExtractSubnetDeclarations(string config, DhcpConfigAnalysis analysis)
        {
            var subnets = Regex.Matches(config, @"(?<commented>\\s*#\\s*)*subnet\s+(?<identifier>.*?)\s+netmask\s+(?<netmask>.*?)\s*\{(?<options>(\s*.+\s*)+?)}");
            if (subnets.Count > 0)
            {
                foreach (Match subnet in subnets)
                {
                    if (!subnet.Groups["commented"].Success)
                    {
                        var declaration = new SubnetDeclaration
                        {
                            NetworkIdentifier = subnet.Groups["identifier"].Value,
                            NetworkMask = subnet.Groups["netmask"].Value,
                        };
                        var options = subnet.Groups["options"].Value;
                        ExtractSubnetDeclarationOptions(declaration, options);
                        analysis.SubnetDeclarations.Add(declaration); 
                    }
                }
            }
        }

        private void ExtractSubnetDeclarationOptions(SubnetDeclaration declaration, string options)
        {
            var range = Regex.Match(options, @"(?<commented>\\s*#\\s*)*range\s+(?<ipFrom>.*?)\s+(?<ipTo>.*?)\s*;");
            if (range.Success && !range.Groups["commented"].Success)
            {
                declaration.IpRangeFrom = range.Groups["ipFrom"].Value;
                declaration.IpRangeTo = range.Groups["ipTo"].Value;
            }
            var broadcast = Regex.Match(options, @"(?<commented>\\s*#\\s*)*option\s+broadcast-address\s+(?<broadcast>.*?)\s*;");
            if (broadcast.Success && !range.Groups["commented"].Success)
            {
                declaration.Broadcast = broadcast.Groups["broadcast"].Value;
            }
            var gateway = Regex.Match(options, @"(?<commented>\\s*#\\s*)*option\s+routers\s+(?<gateway>.*?)\s*;");
            if (gateway.Success && !range.Groups["commented"].Success)
            {
                declaration.Gateway = gateway.Groups["gateway"].Value;
            }
        }

        private void ExtractHostDeclarations(string config, DhcpConfigAnalysis analysis)
        {
            var hosts = Regex.Matches(config, @"(?<commented>\\s*#\\s*)*host\s+(?<host>.*?)\s*\{\s+(?<options>(\s*.+\s*)+?)}");
            if (hosts.Count > 0)
            {
                foreach (Match host in hosts)
                {
                    if (!host.Groups["commented"].Success)
                    {
                        var declaration = new HostDeclaration
                        {
                            Host = host.Groups["host"].Value
                        };
                        var options = host.Groups["options"].Value;
                        ExtractHostDeclarationOptions(declaration, options);
                        analysis.HostDeclarations.Add(declaration); 
                    }
                }
            }
        }

        private void ExtractHostDeclarationOptions(HostDeclaration declaration, string options)
        {
            var macaddr = Regex.Match(options, @"(?<commented>\s*#\s*)*hardware\s+ethernet\s+(?<macaddr>.*?)\s*;");
            if (macaddr.Success && !macaddr.Groups["commented"].Success)
            {
                declaration.MacAddress = macaddr.Groups["macaddr"].Value;
            }
            var ipaddr = Regex.Match(options, @"(?<commented>\s*#\s*)*fixed-address\s+(?<ipaddr>.*?)\s*;");
            if (ipaddr.Success && !ipaddr.Groups["commented"].Success)
            {
                declaration.IpAddress = ipaddr.Groups["ipaddr"].Value;
            }
            var opthostname = Regex.Match(options, "(?<commented>\\s*#\\s*)*option\\s+host-name\\s+\"(?<opthostname>.*?)\"\\s*;");
            if (opthostname.Success && !opthostname.Groups["commented"].Success)
            {
                declaration.Hostname = opthostname.Groups["opthostname"].Value;
            }
        }
    }
}