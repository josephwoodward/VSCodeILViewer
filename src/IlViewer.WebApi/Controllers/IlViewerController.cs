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
			//string assemblyPath = "/Users/josephwoodward/Dev/VsCodeIlViewer/src/IlViewer.WebApi/bin/Debug/netcoreapp1.0/IlViewer.Core.dll";
			//AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

			IList<InstructionResult> result = IlGeneration.ExtractIl(request.ProjectFilePath, request.Filename);

			return result;
		}
	}
}