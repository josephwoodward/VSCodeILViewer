using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.ProjectModel.Workspaces;

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

            Compilation compilation = LoadWorkspace(projectJsonPath);

	        SyntaxTree systenTree = compilation.SyntaxTrees.FirstOrDefault(x => x.FilePath.Contains(classFilename));
	        string sourceCode = systenTree.GetText().ToString();

	        IList<InstructionResult> instructionResults = CompileInMemory(sourceCode, compilation.References.ToList());
	        return instructionResults;
        }

        private static Compilation LoadWorkspace(string filePath)
        {
	        var projectWorkspace = new ProjectJsonWorkspace(filePath);

	        var compilation = projectWorkspace.CurrentSolution.Projects.FirstOrDefault().GetCompilationAsync().Result;

            return compilation;
        }

        private static IList<InstructionResult> CompileInMemory(string sourceCode, IList<MetadataReference> metadataReferences)
        {
            var sourceLanguage = new CSharpLanguage();

            var syntaxTree = sourceLanguage.ParseText(sourceCode, SourceCodeKind.Regular);

	        var stream = new MemoryStream();
	        Compilation compilation = sourceLanguage
	            .CreateLibraryCompilation("ExampleAssembly", false)
	            .AddReferences(metadataReferences)
	            .AddSyntaxTrees(syntaxTree);

            var emitResult = compilation.Emit(stream);
			if (!emitResult.Success)
			{

				foreach (var diagnostic in emitResult.Diagnostics)
				{
					Debug.WriteLine(diagnostic.ToString());
				}

				throw new ArgumentException("Something broke - check output for errors");
			}
            
            stream.Seek(0, SeekOrigin.Begin);

            //var resultWriter = new StringWriter();
            //var decompiler = new ILDecompiler();
            //decompiler.Decompile(stream, resultWriter);

	        return GenerateIlFromStream(stream, "ConsoleApplication.Program", metadataReferences);
        }

        private static IList<InstructionResult> GenerateIlFromStream(Stream stream, string typeFullName, IList<MetadataReference> metadataReferences)
        {
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
    }
}