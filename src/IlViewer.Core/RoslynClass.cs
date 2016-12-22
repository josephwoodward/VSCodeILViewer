using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ConsoleApplication {
    
    public class RoslynClass {

        public static Dictionary<string, Mono.Collections.Generic.Collection<Instruction>> GetILInstructionsFromAssembly(AssemblyDefinition assembly)
        {
            // Probably need to do this properly, i.e.
            // For each Type inside assembly, in those look for methods, etc
            // Also look for the methods at the top level, etc!!
            var ilInstructions = new Dictionary<string, Mono.Collections.Generic.Collection<Instruction>>();

            // var res8 = assembly.MainModule.GetTypes();
            // var item = res8.Where(x => x.Name == "ExampleClass").ToList();

            var script = assembly.MainModule.GetType("C"); // This is the class that Roslyn puts everything in
            if (script != null)
            {
                foreach (var method in script.Methods)
                {
                    var res2 = method.Body.Instructions;

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