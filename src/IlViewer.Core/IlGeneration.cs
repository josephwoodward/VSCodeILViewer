using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IlViewer.Core.ResultOutput;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace IlViewer.Core
{
    public static class IlGeneration
    {
        public static InspectionResult ExtractIl(string projectPath, string classFileName)
        {
            if (string.IsNullOrEmpty(projectPath)) 
            {
                throw new ArgumentNullException(nameof(projectPath));
            }
            
            if (string.IsNullOrEmpty(classFileName))
            {
                throw new ArgumentNullException(nameof(classFileName));
            }

            var d = new DirectoryInfo(projectPath); 
            var fileToCompile = d.EnumerateFiles("*.cs", SearchOption.AllDirectories)
                                 .Select(a => a.FullName)
                                 .FirstOrDefault(f => f.Contains(classFileName));                
            var dllFiles = d.EnumerateFiles("*.dll", SearchOption.AllDirectories)
                            .Select(f => f.FullName).ToArray();
            var references = new List<PortableExecutableReference>();
            foreach (var dllFile in dllFiles)
            {
                var newRef = MetadataReference.CreateFromFile(dllFile);
                references.Add(newRef);
            }

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")));
            var code = File.ReadAllText(fileToCompile);
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("new").AddSyntaxTrees(tree);
            var usings = compilation.SyntaxTrees.Select(tree => tree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>()).SelectMany(s => s).ToArray();
            foreach (var u in usings)
            {
                try
                {

                    references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, u.Name.ToString() + ".dll")));
                }
                catch
                {
                    // ToDo log not absent .dll for usings
                }
            }
            
            //ToDo support global usings
            var sysRef = MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll"));
            references.Add(sysRef);
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.IO.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Http.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.dll")));
            var finalCompilation = compilation.AddReferences(references);

            var inspectionResult = new InspectionResult();
            using (var assemblyStream = new MemoryStream())
            {
                var compilationResult = finalCompilation.Emit(assemblyStream);
                if (!compilationResult.Success)
                {
                    var errors = compilationResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).ToList();
                    foreach (var error in errors)
                    {
                        inspectionResult.AddError(error.GetMessage());
                        Console.WriteLine(error.ToString());
                    }

                    if (inspectionResult.HasErrors) return inspectionResult;
                }

                assemblyStream.Seek(0, SeekOrigin.Begin);
                InspectionResult finalResult = new InspectionResult { IlResults = GenerateIlFromStream (assemblyStream, classFileName) };
                return finalResult;
            }
        }

        private static IList<InstructionResult> GenerateIlFromStream(Stream stream, string typeFullName)
        {
            var assembly = AssemblyDefinition.ReadAssembly(stream);
            var result = new Dictionary<string, Collection<Instruction>>();
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