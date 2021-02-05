using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ScanApp.Areas.Identity.IdentityHostingStartup))]

namespace ScanApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}