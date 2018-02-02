namespace ConfigurationService.Data
{
    public class FolderStructure
    {
        public static FolderStructure Default = new FolderStructure();

        public FolderStructure(string containerName = "default", string relativePath = "")
        {
            ContainerName = containerName;
            RelativePath = relativePath;
        }

        public string ContainerName { get; }
        public string RelativePath { get; }
    }
}
