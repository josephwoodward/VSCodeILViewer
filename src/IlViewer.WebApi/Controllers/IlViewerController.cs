using Microsoft.AspNetCore.Mvc;
using IlViewer.Core;
using System.Collections.Generic;
using IlViewer.WebApi.Models;

namespace IlViewer.WebApi.Controllers
{
	[Route("api/il")]
	public class IlViewerController : Controller
	{
		[HttpPost]
		public IList<InstructionResult> Post([FromBody] IlRequest request)
		{
			if (string.IsNullOrEmpty(request.Filename))
				return new List<InstructionResult>();

			if (string.IsNullOrEmpty(request.ProjectFilePath))
				return new List<InstructionResult>();

 			var result = IlGeneration.ExtractIl(request.ProjectFilePath, request.Filename);

			return result;
		}
	}
}