using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.ProjectModel.Workspaces;

namespace IlViewer.Core
{
    public static class WorkspaceManager
    {
        public static Compilation LoadWorkspace(string filePath)
        {
            var projectWorkspace = new ProjectJsonWorkspace(filePath);

            var project = projectWorkspace.CurrentSolution.Projects.FirstOrDefault();
            var compilation = project.GetCompilationAsync().Result;

            /*foreach (var tree in compilation.SyntaxTrees)
            {
                var source = tree.GetRoot().DescendantNodes();
                var classDeclarations = source.OfType<ClassDeclarationSyntax>().Where(x => x.Identifier.Text.Contains("MessagingConfig")).ToList();
                if (classDeclarations.Any())
                {

                }
            }*/

            return compilation;
        }
    }
}