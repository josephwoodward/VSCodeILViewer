using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IlViewer.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
	        var builder = WebApplication.CreateBuilder(args);
           http://localhost:65530builder.Services.AddControllers();

            var app = builder.Build();
            app.UseHsts();
            app.Urls.Add("http://localhost:65530");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
}
