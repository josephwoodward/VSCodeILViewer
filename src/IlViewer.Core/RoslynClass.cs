using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace IlViewer.Core
{
    public class RoslynClass
    {
        public static Dictionary<string, Mono.Collections.Generic.Collection<Instruction>> GetiLInstructionsFromAssembly(AssemblyDefinition assembly, string typeName)
        {
            var ilInstructions = new Dictionary<string, Mono.Collections.Generic.Collection<Instruction>>();
            var typeDefinitions = assembly.MainModule.GetTypes().ToList();
            //var typeDefinition = typeDefinitions.FirstOrDefault(x => x.Name == typeName);

            /*if (typeDefinition == null)
            {
                // Could be generic type?
                typeDefinition = typeDefinitions.FirstOrDefault(x => x.Name.Contains(typeName) && x.HasGenericParameters);
            }*/

            foreach (TypeDefinition typeDefinition in typeDefinitions)
            {
                if (typeDefinition != null)
                {
                    foreach (var method in typeDefinition.Methods)
                    {
                        ilInstructions.Add(method.FullName, method.Body.Instructions);
                    }

                    foreach (var nestedType in typeDefinition.NestedTypes)
                    {
                        foreach (var method in nestedType.Methods)
                        {
                            ilInstructions.Add(method.FullName, method.Body.Instructions);
                        }

                        foreach (var nestedNestedType in nestedType.NestedTypes)
                        {
                            foreach (var method in nestedNestedType.Methods)
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
            }

            return ilInstructions;
        }

    }
}