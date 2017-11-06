using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IlViewer.Core.ResultOutput;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace IlViewer.Core
{
    public static class IlGeneration
    {
        public static InspectionResult ExtractIl(string projectPath, string classFilename)
        {
            if (string.IsNullOrEmpty(projectPath))
                throw new ArgumentNullException(nameof(projectPath));

            if (string.IsNullOrEmpty(classFilename))
               throw new ArgumentNullException(nameof(classFilename));

            
            var compilation = WorkspaceManager.LoadWorkspace(projectPath);
            
            var inspectionResult = new InspectionResult();
            using (var stream = new MemoryStream())
            {
                /*compilation = compilation.WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication));*/
                var compilationResult = compilation.Emit(stream);
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
                var ilResult = GenerateIlFromStream(stream, classFilename);

                return new InspectionResult
                {
                    IlResults = ilResult
                };
            }
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