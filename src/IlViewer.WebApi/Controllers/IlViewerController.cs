using Microsoft.AspNetCore.Mvc;
using IlViewer.Core;
using System.Collections.Generic;
using Mono.Cecil;

namespace IlViewer.WebApi.Controllers
{
	[Route("api/il")]
	public class IlViewerController : Controller
	{
		[HttpGet("{path}")]
		public IList<InstructionResult> Get(string path)
		{
			string assemblyPath = "/Users/josephwoodward/Dev/VsCodeIlViewer/src/IlViewer.WebApi/bin/Debug/netcoreapp1.1/IlViewer.Core.dll";
			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

			IList<InstructionResult> result = IlGeneration.GenerateILFromDll(assembly, "IlViewer.Core.ExampleClass");

			return result;
		}
	}
}