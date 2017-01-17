using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IlViewer.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
	        var builder = new ConfigurationBuilder()
		        .SetBasePath(env.ContentRootPath);
		        /*.AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());*/
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        /*Console.WriteLine("Port: " + Configuration.GetSection("server:port"));
	        Console.WriteLine("Port2: " + Configuration["server:urls"]);
	        Console.WriteLine("Port3: " + Configuration["server__urls"]);
	        Console.WriteLine("Port3: " + Configuration["server.urls"]);*/
	        // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
