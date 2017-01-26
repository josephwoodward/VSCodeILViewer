using Mono.Cecil.Cil;

namespace IlViewer.Core
{
    public class InstructionWrapper
    {
        public Instruction Instruction { get; set; }

        public bool HasInstruction => Instruction != null;
    }
}