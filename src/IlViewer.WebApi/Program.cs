using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IlViewer.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
	        //Console.WriteLine("Arg" + args[1]);
            var config = new ConfigurationBuilder()
                //.AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray())
                .Build();
            
	        var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://localhost:65530")
	            .Build();

            host.Run();
        }
    }
}
