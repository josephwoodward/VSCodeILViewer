using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.ProjectModel;
using Microsoft.DotNet.ProjectModel.Compilation;
using Microsoft.DotNet.ProjectModel.Graph;
using Microsoft.DotNet.ProjectModel.Resolution;

namespace IlViewer.Core
{
	public class ProjectContextLens
	{
		private readonly string _configuration;
		private readonly ProjectContext _context;
		private readonly List<string> _sourceFiles = new List<string>();
		private readonly List<string> _fileReferences = new List<string>();
		private readonly List<ProjectDescription> _projectReferenes = new List<ProjectDescription>();

		public ProjectContextLens(ProjectContext context, string configuration)
		{
			_context = context;
			_configuration = configuration;
			Resolve();
		}

		public IEnumerable<string> SourceFiles => _sourceFiles;

		public IEnumerable<string> FileReferences => _fileReferences;

		public IEnumerable<ProjectDescription> ProjectReferences => _projectReferenes;

		private void Resolve()
		{
			_sourceFiles.AddRange(_context.ProjectFile.Files.SourceFiles);
			var exporter = _context.CreateExporter(_configuration);

			var library = exporter.LibraryManager;
			ResolveLibraries(library);

			foreach (var export in exporter.GetAllExports())
			{

				ResolveFileReferences(export);
				ResolveProjectReference(export);
				ResolveSourceFiles(export);
			}
		}

		private void ResolveLibraries(LibraryManager libraryManager)
		{
			var allDiagnostics = libraryManager.GetAllDiagnostics();
			var unresolved = libraryManager.GetLibraries().Where(dep => !dep.Resolved);
			var needRestore = allDiagnostics.Any(diag => diag.ErrorCode == ErrorCodes.NU1006) || unresolved.Any();
		}

		private void ResolveSourceFiles(LibraryExport export)
		{
			foreach (var file in export.SourceReferences)
			{
				_sourceFiles.Add(file.ResolvedPath);
			}
		}

		private void ResolveFileReferences(LibraryExport export)
		{
			if (export.Library.Identity.Type != LibraryType.Project)
			{
				_fileReferences.AddRange(export.CompilationAssemblies.Select(asset => asset.ResolvedPath));
			}
		}

		private void ResolveProjectReference(LibraryExport export)
		{
			var desc = export.Library as ProjectDescription;
			if (desc == null || export.Library.Identity.Type != LibraryType.Project)
			{
				return;
			}

			if (export.Library.Identity.Name == _context.ProjectFile.Name)
			{
				return;
			}

			if (!string.IsNullOrEmpty(desc?.TargetFrameworkInfo?.AssemblyPath))
			{
				return;
			}

			_sourceFiles.AddRange(export.SourceReferences.Select(source => source.ResolvedPath));
			_projectReferenes.Add(desc);
		}
	}
}