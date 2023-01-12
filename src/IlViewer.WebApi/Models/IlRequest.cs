namespace IlViewer.WebApi.Models
{
    public class IlRequest
    {
        // Project.csproj file location
        public string ProjectFilePath { get; set; }
        // Filename of file user is wishing to inspect
        public string Filename { get; set; }
    }
}