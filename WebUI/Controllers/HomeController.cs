using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using WebUI.Models;

public class HomeController : Controller
{
    private static readonly string dbPath = Path.Combine(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ImageLabelApp"), "labels.db");

    public IActionResult Index(List<string> selectedLabels)
    {
        var images = GetImages(selectedLabels);
        return View(images);
    }

    [HttpPost]
    public IActionResult CopyImages(List<string> selectedLabels)
    {
        var images = GetImages(selectedLabels);

        var targetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LabelApp_Copy");
        Directory.CreateDirectory(targetFolder);

        foreach (var img in images)
        {
            var originalPath = img.OriginalFullPath;
            var extension = Path.GetExtension(originalPath);

            var newFileName = img.PathHash + extension; // Use PathHash for filename
            var destinationPath = Path.Combine(targetFolder, newFileName);

            System.IO.File.Copy(originalPath, destinationPath, overwrite: true);
        }

        return RedirectToAction("Index");
    }

    public IActionResult GetImage(string imageHash)
    {
        var (filePath, extension) = GetOriginalFilePath(imageHash);

        if (System.IO.File.Exists(filePath))
        {
            var bytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = GetContentType(extension);
            return File(bytes, contentType);
        }

        return NotFound();
    }

    // Helper Methods

    private List<ImageLabel> GetImages(List<string> selectedLabels)
    {
        var images = new List<ImageLabel>();

        using (var conn = new SqliteConnection($"Data Source={dbPath}"))
        {
            conn.Open();
            var cmd = conn.CreateCommand();

            if (selectedLabels != null && selectedLabels.Any())
            {
                // build where clauses for labels
                string labelsCondition = string.Join(" INTERSECT ", selectedLabels.Select(label => $@"
                    SELECT ImageHash FROM ImageLabels WHERE LabelName = '{label}'
                "));

                cmd.CommandText = $@"
                    SELECT ImageHash, PathHash, OriginalFileName, OriginalFullPath
                    FROM Images
                    WHERE ImageHash IN ({labelsCondition})
                ";
            }
            else
            {
                cmd.CommandText = "SELECT ImageHash, PathHash, OriginalFileName, OriginalFullPath FROM Images";
            }

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    images.Add(new ImageLabel
                    {
                        ImageHash = reader.GetString(0),
                        PathHash = reader.GetString(1),
                        OriginalFileName = reader.GetString(2),
                        OriginalFullPath = reader.GetString(3)
                    });
                }
            }
        }

        return images;
    }

    private (string filePath, string extension) GetOriginalFilePath(string imageHash)
    {
        using (var conn = new SqliteConnection($"Data Source={dbPath}"))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT OriginalFullPath FROM Images WHERE ImageHash = @hash";
            cmd.Parameters.AddWithValue("@hash", imageHash);

            var result = cmd.ExecuteScalar() as string;
            if (result != null)
            {
                var extension = Path.GetExtension(result);
                return (result, extension);
            }
            else
            {
                (string filePath, string extension) value = (null, null);
                return value;
            }
        }
    }

    private string GetContentType(string extension)
    {
        switch (extension.ToLowerInvariant())
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".gif":
                return "image/gif";
            case ".bmp":
                return "image/bmp";
            default:
                return "application/octet-stream";
        }
    }
}


