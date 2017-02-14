using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IlViewer.Core.ResultOutput;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace IlViewer.Core
{
    public static class IlGeneration
    {
        public static InspectionResult ExtractIl(string projectJsonPath, string classFilename)
        {
            if (string.IsNullOrEmpty(projectJsonPath))
                throw new ArgumentNullException(nameof(projectJsonPath));

            if (string.IsNullOrEmpty(classFilename))
               throw new ArgumentNullException(nameof(classFilename));

            Compilation workspaceCompilation = WorkspaceManager.LoadWorkspace(projectJsonPath);

            var inspectionResult = new InspectionResult();
            using (var stream = new MemoryStream())
            {
                var compilationResult = workspaceCompilation.Emit(stream);
                if (!compilationResult.Success)
                {
                    var errors = compilationResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).ToList();
                    foreach (var error in errors)
                    {
                        inspectionResult.AddError(error.GetMessage());
                        Console.WriteLine(error.ToString());
                    }

                    if (inspectionResult.HasErrors)
                        return inspectionResult;
                }

                stream.Seek(0, SeekOrigin.Begin);

                //AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stream);
                PortableExecutableReference ref2 = MetadataReference.CreateFromStream(stream);

                var syntaxTree2 = workspaceCompilation.SyntaxTrees.Where(x => x.FilePath.Contains(classFilename));
                var trees = workspaceCompilation.SyntaxTrees.ToList();

                var allTrees = trees.Where(x => x.FilePath.Contains(classFilename + ".cs")).ToList();
                //TODO: optimise this
                
                var syntaxTree = allTrees.FirstOrDefault(x => x.FilePath.Contains("\\" + classFilename + ".cs"));
                if (syntaxTree == null)
                    syntaxTree = allTrees.FirstOrDefault(x => x.FilePath.Contains("/" + classFilename + ".cs"));

                var sourceCode = syntaxTree.GetText().ToString();

                var metadataReferences = workspaceCompilation.References.ToList();
                var res3 = ref2.GetMetadata();
                metadataReferences.Add(ref2);

                InspectionResult finalResult = CompileInMemory(sourceCode, metadataReferences, classFilename, inspectionResult);
                return finalResult;
            }
        }

        private static InspectionResult CompileInMemory(string sourceCode, IEnumerable<MetadataReference> metadataReferences, string classFilename, InspectionResult inspectionResult)
        {
            var sourceLanguage = new CSharpLanguage();
            var syntaxTree = sourceLanguage.ParseText(sourceCode, SourceCodeKind.Regular);

	        var stream = new MemoryStream();
	        Compilation compilation = sourceLanguage
	            .CreateLibraryCompilation("ExampleAssembly", false)
	            .AddReferences(metadataReferences)
                .AddReferences()
	            .AddSyntaxTrees(syntaxTree);

            var emitResult = compilation.Emit(stream);
			if (!emitResult.Success)
			{
			    var errors = emitResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error);
			    foreach (var error in errors)
				{
				    inspectionResult.AddError(error.GetMessage());
				    Console.WriteLine(error.ToString());
				}

			    if (inspectionResult.HasErrors)
			        return inspectionResult;
			}
            
            stream.Seek(0, SeekOrigin.Begin);

            //var resultWriter = new StringWriter();
            //var decompiler = new ILDecompiler();
            //decompiler.Decompile(stream, resultWriter);

            var ilResult = GenerateIlFromStream(stream, classFilename);
            inspectionResult.IlResults = ilResult;

            return inspectionResult;
        }

        private static IList<InstructionResult> GenerateIlFromStream(Stream stream, string typeFullName)
        {
            var assembly = AssemblyDefinition.ReadAssembly(stream);

            Dictionary<string, Collection<Instruction>> result;
            try
            {
                 result = RoslynClass.GetiLInstructionsFromAssembly(assembly, typeFullName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            var output = new List<InstructionResult>();
            foreach(var item in result)
            {
                output.Add(new InstructionResult {Value = item.Key});
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