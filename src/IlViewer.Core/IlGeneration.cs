using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.DotNet.ProjectModel;
using Microsoft.DotNet.ProjectModel.Compilation;
using Microsoft.DotNet.ProjectModel.Graph;
using Microsoft.DotNet.ProjectModel.Workspaces;
using NuGet.Frameworks;
using Project = Microsoft.CodeAnalysis.Project;

namespace IlViewer.Core
{
    public static class IlGeneration
    {
        public static IList<InstructionResult> ExtractIl(string projectJsonPath, string classFilename)
        {
            if (string.IsNullOrEmpty(projectJsonPath))
                throw new ArgumentNullException(nameof(projectJsonPath));           

            if (string.IsNullOrEmpty(classFilename))
               throw new ArgumentNullException(nameof(classFilename));

            var workspace = LoadWorkspace(projectJsonPath);

	        var totals = workspace.CurrentSolution.Projects.ToList();
	        Project project = workspace.CurrentSolution.Projects.FirstOrDefault();

	        var refer = project.AllProjectReferences.ToList();

	        var references = new List<MetadataReference>();
	        foreach (var reference in project.MetadataReferences)
	        {
		        references.Add(reference);
	        }

	        var metadataReferences = project.MetadataReferences;

	        Compilation compilation = project.GetCompilationAsync().Result;

	        INamedTypeSymbol symbol = compilation.GetTypeByMetadataName(classFilename);

	        SyntaxTree result = compilation.SyntaxTrees.FirstOrDefault(x => x.FilePath.Contains(classFilename));

	        SourceText tempRes = result.GetText();
	        string sourceCode = result.GetText().ToString();

	        var res = CompileInMemory(sourceCode, references);
	        //var symbol = compilation.GetTypeByMetadataName(request.TypeName);

	        return res;
        }

        public static void UpdateFileReferences(IEnumerable<string> sourceFiles)
        {
            foreach(var file in sourceFiles){
                using (var stream = File.OpenRead(file))
                {
                    var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
                    var metadata = AssemblyMetadata.Create(moduleMetadata);
                    var metadataRef = metadata.GetReference();
                }
            }
            
        }

        public static void UpdateSourceFiles(IEnumerable<string> sourceFiles)
        {
            sourceFiles = sourceFiles.Where(filename => Path.GetExtension(filename) == ".cs");
            foreach(var file in sourceFiles){
                using (var stream = File.OpenRead(file))
                {
                    var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);

                    var metadata = AssemblyMetadata.Create(moduleMetadata);
                    var metadataRef = metadata.GetReference();
                }
            }
            
        }
        public static Microsoft.CodeAnalysis.Workspace LoadWorkspace(string filePath)
        {
	        ProjectContext res = ProjectContext.Create(filePath, NuGetFramework.AgnosticFramework);

	        var res2 = GetProjectReferences(res);

	        ProjectContextLens lens = new ProjectContextLens(res, "Debug");

	        var fileRef = lens.FileReferences;
	        var projRef = lens.ProjectReferences;
	        var allClasses = lens.SourceFiles;
            
            UpdateFileReferences(lens.FileReferences);
            //UpdateSourceFiles(lens.SourceFiles);

	        var projectFile = ProjectReader.GetProject(filePath);

            ProjectContext context = new ProjectContextBuilder()
                .WithProject(projectFile)
                .WithTargetFramework(FrameworkConstants.CommonFrameworks.NetCoreApp10)
                .Build();

            Microsoft.CodeAnalysis.Workspace workspace = context.CreateRoslynWorkspace();

            return workspace;
        }

        private static IList<InstructionResult> CompileInMemory(string sourceCode, IList<MetadataReference> metadataReferences)
        {
	        MetadataReference[] references = {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DynamicAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(AssemblyMetadata).GetTypeInfo().Assembly.Location),
            };

            var sourceLanguage = new CSharpLanguage();

            var syntaxTree = sourceLanguage.ParseText(sourceCode, SourceCodeKind.Regular);

	        var stream = new MemoryStream();
	        Compilation compilation = sourceLanguage
	            .CreateLibraryCompilation("ExampleAssembly", false)
	            .AddReferences(references)
	            .AddSyntaxTrees(syntaxTree);

            var emitResult = compilation.Emit(stream);

if (!emitResult.Success)
{
	foreach (var diagnostic in emitResult.Diagnostics)
	{
		Debug.WriteLine(diagnostic.ToString());
	}
}
            
            stream.Seek(0, SeekOrigin.Begin);

            //var resultWriter = new StringWriter();
            //var decompiler = new ILDecompiler();
            //decompiler.Decompile(stream, resultWriter);

            return GenerateIlFromStream(stream, "ConsoleApplication.Program");
        }

        private static IList<InstructionResult> GenerateIlFromStream(Stream stream, string typeFullName)
        {  


            // var thisDocument = workspace.CurrentSolution.GetDocumentIdsWithFilePath(Path.Combine(path, "ExampleClass.cs")).First();
            // var project = workspace.CurrentSolution.GetProject(thisDocument.ProjectId);

            // var program = project.GetCompilationAsync().Result.Assembly.GetTypeByMetadataName("ExampleClass");

            var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(stream);
            var result = RoslynClass.GetiLInstructionsFromAssembly(assembly, typeFullName);

            var output = new List<InstructionResult>();
            foreach(var item in result)
            {
                foreach(var i in item.Value)
                {
                    Console.WriteLine(i.ToString());
                    output.Add(new InstructionResult {
                        // IlOpCode = i.OpCode.ToString(),
                        // IlOperand = i.Operand.ToString(),
                        Value = i.ToString()
                    });
                }
            }

            return output;
        }

	    private static IEnumerable<ProjectDescription> GetProjectReferences(ProjectContext context)
	    {
		    var projectDescriptions = context.LibraryManager
			    .GetLibraries()
			    .Where(lib => lib.Identity.Type == LibraryType.Project)
			    .OfType<ProjectDescription>();

		    foreach (var description in projectDescriptions)
		    {
			    if (description.Identity.Name == context.ProjectFile.Name)
			    {
				    continue;
			    }

			    // if this is an assembly reference then don't treat it as project reference
			    if (!string.IsNullOrEmpty(description.TargetFrameworkInfo?.AssemblyPath))
			    {
				    continue;
			    }

			    yield return description;
		    }
	    }
    }
}