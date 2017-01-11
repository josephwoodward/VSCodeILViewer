using System.IO;
using Mono.Cecil;

namespace IlViewer.Core
{
    public class IlDecompiler {
        public void Decompile(Stream assemblyStream, TextWriter codeWriter) {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyStream);

            // var output = new CustomizableIndentPlainTextOutput(codeWriter) {
            //     IndentationString = "    "
            // };
            // var disassembler = new ReflectionDisassembler(new ILCommentingTextOutput(output, 30), false, new CancellationToken());
            // disassembler.WriteModuleContents(assembly.MainModule);
        }
    }
}