using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace IlViewer.Core
{
    public class RoslynClass
    {
        public static Dictionary<string, Mono.Collections.Generic.Collection<Instruction>> GetiLInstructionsFromAssembly(AssemblyDefinition assembly, string typeFullName)
        {
            // Probably need to do this properly, i.e.
            // For each Type inside assembly, in those look for methods, etc
            // Also look for the methods at the top level, etc!!
            var ilInstructions = new Dictionary<string, Mono.Collections.Generic.Collection<Instruction>>();
	        var script = assembly.MainModule.GetTypes().FirstOrDefault(x => x.Name == typeFullName.Replace(".cs", string.Empty));

            if (script != null)
            {
                foreach (var method in script.Methods)
                {
                    ilInstructions.Add(method.FullName, method.Body.Instructions);
                }

                foreach (var type in script.NestedTypes)
                {
                    foreach (var method in type.Methods)
                    {
                        ilInstructions.Add(method.FullName, method.Body.Instructions);
                    }

                    foreach (var nestedType in type.NestedTypes)
                    {
                        foreach (var method in nestedType.Methods)
                        {
                            ilInstructions.Add(method.FullName, method.Body.Instructions);
                        }
                    }
                }
            }
            else
            {
                var ilProcessor = assembly.MainModule.EntryPoint.Body.GetILProcessor();
                var res5 = ilProcessor.Body.Instructions;
                ilInstructions.Add(assembly.MainModule.EntryPoint.FullName, ilProcessor.Body.Instructions);
            }

            return ilInstructions;
        }

    }
}