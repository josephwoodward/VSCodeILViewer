using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Mono.Cecil;

namespace IlViewer.Core
{
    public static class IlGeneration
    {
        // public static IList<string> GenerateILFromText(string code)
        // {
        //     MetadataReference[] _references = {
        //         MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
        //         MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location),
        //         MetadataReference.CreateFromFile(typeof(DynamicAttribute).GetTypeInfo().Assembly.Location)
        //     };

        //     var sourceLanguage = new CSharpLanguage();

        //     var syntaxTree = sourceLanguage.ParseText(code, SourceCodeKind.Regular);

        //     var stream = new MemoryStream();
        //     var compilation = sourceLanguage
        //         .CreateLibraryCompilation("Test", false)
        //         .AddReferences(_references)
        //         .AddSyntaxTrees(syntaxTree);

        //     var emitResult = compilation.Emit(stream);
            
        //     stream.Seek(0, SeekOrigin.Begin);

        //     var resultWriter = new StringWriter();

        //     var decompiler = new ILDecompiler();
        //     //decompiler.Decompile(stream, resultWriter);

        //     //return GenerateILFromDll(stream);

        //     return new List<string>();
        // }

        public static IList<InstructionResult> GenerateILFromDll(AssemblyDefinition assembly, string typeFullName)
        {
            var result = RoslynClass.GetILInstructionsFromAssembly(assembly, typeFullName);

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