using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AzureAdWebX.Startup))]

namespace AzureAdWebX
{
    public class Startup
    {
        //public Iconfi MyProperty { get; set; }
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
