using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var code = @"
            using System;
            public class C {
                public void M() {
                }
            }";

            GenerateILFromText(code);
        }

        public static void GenerateILFromText(string code)
        {
            MetadataReference[] _references = {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DynamicAttribute).GetTypeInfo().Assembly.Location)
            };

            var sourceLanguage = new CSharpLanguage();

            var syntaxTree = sourceLanguage.ParseText(code, SourceCodeKind.Regular);

            var stream = new MemoryStream();
            var compilation = sourceLanguage
                .CreateLibraryCompilation("Test", false)
                .AddReferences(_references)
                .AddSyntaxTrees(syntaxTree);

            var emitResult = compilation.Emit(stream);

            
            stream.Seek(0, SeekOrigin.Begin);

            var resultWriter = new StringWriter();

            var decompiler = new ILDecompiler();
            //decompiler.Decompile(stream, resultWriter);

            GenerateILFromDll(stream);
        }
        public static void GenerateILFromDll(Stream assemblyPath)
        {
            //string assemblyPath = "bin/Debug/netcoreapp1.1/RoslynDemo.dll";
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

            var result = RoslynClass.GetILInstructionsFromAssembly(assembly);

            foreach(var item in result)
            {
                foreach(var i in item.Value)
                {
                    Console.WriteLine(i.ToString());
                }
            }
        }
    }
}