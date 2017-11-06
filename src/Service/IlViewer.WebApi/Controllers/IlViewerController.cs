using System;
using System.Diagnostics;
using IlViewer.Core;
using IlViewer.Core.ResultOutput;
using IlViewer.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IlViewer.WebApi.Controllers
{
    [Route("api/il")]
	public class IlViewerController : Controller
	{
	    private readonly LoggerFactory _loggerFactory;
	    private ILogger<IlViewerController> _logger;

		[HttpPost]
		public InspectionResult Post([FromBody] IlRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));
			
		    /*_logger = _loggerFactory.CreateLogger<IlViewerController>();*/
		    Console.WriteLine($"Request Params: ProjectFilePath {request.ProjectFilePath}");
		    Console.WriteLine($"Request Params: Filename: {request.Filename}");
		    /*using (_logger.BeginScope("Request received"))
		    {
		        _logger.LogInformation("ProjectFilePath: " + request.ProjectFilePath);
		        _logger.LogInformation("Filename: " + request.Filename);
		    }*/

		    try
		    {
			    var timer = new Stopwatch();
			    timer.Start();
		        InspectionResult result = IlGeneration.ExtractIl(request.ProjectFilePath, request.Filename);
			    timer.Stop();
			    Console.WriteLine("Total time: " + timer.ElapsedMilliseconds);
		        return result;
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine(e);
		        throw;
		    }
		}

		[HttpGet]
		public string Ping()
		{
			return "pong";
		}
	}
}