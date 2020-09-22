using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IscDhcpAnalyser;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IscDhcpAnalyserWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DchpConfigAnalyser _analyser;

        public IndexModel(ILogger<IndexModel> logger, DchpConfigAnalyser analyser)
        {
            _logger = logger;
            _analyser = analyser;
        }

        public DhcpConfigAnalysis Analysis { get; set; }

        public async Task<IActionResult> OnGetAsync(string hostSearch = "")
        {
            Analysis = await _analyser.AnalyseAsync();
            if (!string.IsNullOrEmpty(hostSearch))
            {
                var hosts = from h in Analysis.HostDeclarations
                            where h.Host.ToLower().Contains(hostSearch.ToLower()) ||
                                  h.MacAddress.ToLower().Contains(hostSearch.ToLower()) ||
                                  h.Hostname.ToLower().Contains(hostSearch.ToLower()) ||
                                  h.IpAddress.ToLower().Contains(hostSearch.ToLower())
                            select h;
                Analysis.HostDeclarations = hosts.ToList(); 
            }
            return Page();
        }
    }
}