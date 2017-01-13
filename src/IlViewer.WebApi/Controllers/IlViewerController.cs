using System;
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
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			var result = IlGeneration.ExtractIl(request.ProjectFilePath, request.Filename);

			return result;
		}
	}
}