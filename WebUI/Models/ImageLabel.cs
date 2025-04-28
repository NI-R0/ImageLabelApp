namespace WebUI.Models
{
    public class ImageLabel
    {
        public required string ImageHash { get; set; }
        public required string PathHash { get; set; }
        public required string OriginalFileName { get; set; }
        public required string OriginalFullPath { get; set; }
        public List<string> Labels { get; set; } = new List<string>();

        public string Extension => Path.GetExtension(OriginalFullPath); // e.g., ".jpg"
    }

}
