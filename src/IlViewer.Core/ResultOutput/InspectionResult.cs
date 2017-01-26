using System.Collections.Generic;
using System.Linq;

namespace IlViewer.Core.ResultOutput
{
    public class InspectionResult
    {
        public InspectionResult()
        {
            IlResults = new List<InstructionResult>();
            CompilationErrors = new List<InspectionError>();
        }

        public IList<InstructionResult> IlResults { get; set; }

        public IList<InspectionError> CompilationErrors { get; }

        public bool HasErrors => CompilationErrors.Any();

        public void AddError(string errorMessage)
        {
            CompilationErrors.Add(new InspectionError
            {
                Message = errorMessage
            });
        }
    }
}