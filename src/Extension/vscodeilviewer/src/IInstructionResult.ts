interface IInstructionResult {
    value : string;
    ilOpCode : string;
    ilOperand : string;
}

interface ICompilationError {
    message : string;
}

interface IInspectionResult {
     ilResults : IInstructionResult[]
     compilationErrors : ICompilationError[]
     hasErrors : boolean;
}