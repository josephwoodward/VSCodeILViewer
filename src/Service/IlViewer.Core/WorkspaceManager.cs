using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Project = Microsoft.Build.Evaluation.Project;

namespace IlViewer.Core
{
    public static class WorkspaceManager
    {
        public static Compilation LoadWorkspace(string filePath)
        {
            var manager = new AnalyzerManager();
            ProjectAnalyzer analyzer = manager.GetProject(filePath);

            Project project = analyzer.Project;

            var projectPath = project.DirectoryPath;
            
            var projects = analyzer.Project.Items.Where(x => x.ItemType == "ProjectReference");            
            
            /*var references = analyzer.GetProjectReferences();*/
            foreach (ProjectItem projItem in projects)
            {
                var p = Path.Combine(projectPath, projItem.EvaluatedInclude);
                analyzer.Manager.GetProject(p);    
            }
            
            Workspace workspace = analyzer.GetWorkspace(true);
            var compilation = workspace.CurrentSolution.Projects.FirstOrDefault().GetCompilationAsync().Result;
            return compilation;
        }
    }
}