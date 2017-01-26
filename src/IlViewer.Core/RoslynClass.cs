using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace IlViewer.Core
{
    public class RoslynClass
    {
        public static Dictionary<string, Collection<Instruction>> GetiLInstructionsFromAssembly(AssemblyDefinition assembly, string typeName)
        {
            var ilInstructions = new Dictionary<string, Collection<Instruction>>();
            var typeDefinitions = assembly.MainModule.GetTypes().ToList();

            TypeDefinition typeDefinition = typeDefinitions.FirstOrDefault(x => x.Name == typeName) ?? typeDefinitions.FirstOrDefault(x => x.Name.Contains(typeName) && x.HasGenericParameters);
            if (typeDefinition != null)
            {
                foreach (var method in typeDefinition.Methods)
                {
                    ilInstructions.Add(method.FullName, method.Body?.Instructions ?? new Collection<Instruction>());
                }

                foreach (var nestedType in typeDefinition.NestedTypes)
                {
                    foreach (var method in nestedType.Methods)
                    {
                        ilInstructions.Add(method.FullName, method.Body?.Instructions ?? new Collection<Instruction>());
                    }

                    foreach (var nestedNestedType in nestedType.NestedTypes)
                    {
                        foreach (var method in nestedNestedType.Methods)
                        {
                            ilInstructions.Add(method.FullName, method.Body?.Instructions ?? new Collection<Instruction>());
                        }
                    }
                }
            }
            else
            {
                ModuleDefinition module = assembly.MainModule;

                var ilProcessor = module?.EntryPoint?.Body?.GetILProcessor();
                if (ilProcessor != null)
                {
                    ilInstructions.Add(assembly.MainModule.EntryPoint.FullName, ilProcessor?.Body?.Instructions ?? new Collection<Instruction>());
                }
            }

            return ilInstructions;
        }

    }
}