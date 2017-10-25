using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace IlViewer.Core
{
    public static class WorkspaceManager
    {
        public static Compilation LoadWorkspace(string filePath)
        {
            var manager = new AnalyzerManager();
            ProjectAnalyzer analyzer = manager.GetProject(filePath);
            var references = analyzer.GetProjectReferences();
            foreach (var p in references)
            {
                analyzer.Manager.GetProject(p);    
            }
            
            Workspace workspace = analyzer.GetWorkspace(true);
            var project = workspace.CurrentSolution.Projects.ToList();
            
            var compilation = project.FirstOrDefault().GetCompilationAsync().Result;

            return compilation;
        }
    }
}