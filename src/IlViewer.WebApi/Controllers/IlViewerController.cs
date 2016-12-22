using Microsoft.AspNetCore.Mvc;

namespace IlViewer.WebApi.Controllers
{
	[Route("api/il")]
	public class IlViewerController : Controller
	{
		[HttpGet("{path}")]
		public string Get(string path)
		{
			return path;
		}
	}
}