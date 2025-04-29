using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebUI.Models;

public class HomeController : Controller
{
    private static readonly string dbPath = Path.Combine(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ImageLabelApp"), "labels.db");

    public IActionResult Index(List<string> selectedLabels)
    {
        ViewBag.AvailableLabels = GetAllLabels();
        var images = GetImages(selectedLabels);
        return View(images);
    }

    [HttpPost]
    public void CopyImages(List<string> selectedLabels)
    {
        var images = GetImages(selectedLabels);

        var folderName = string.Join("-", selectedLabels);

        var targetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LabelApp");
        targetFolder = Path.Combine(targetFolder, folderName);
        Directory.CreateDirectory(targetFolder);

        foreach (var img in images)
        {
            var origin = GetCopyFolder();
            var originPath = Path.Combine(origin, img.PathHash, img.Extension);

            var newFileName = img.PathHash + img.Extension; // Use PathHash for filename
            var destinationPath = Path.Combine(targetFolder, newFileName);

            System.IO.File.Copy(originPath, destinationPath, overwrite: true);
        }
    }

    public IActionResult GetImage(string imageHash)
    {
        var (filePath, extension) = GetFilePath(imageHash);
        var fullFilePath = Path.Combine(filePath + extension);

        if (System.IO.File.Exists(fullFilePath))
        {
            var bytes = System.IO.File.ReadAllBytes(fullFilePath);
            var contentType = GetContentType(extension);
            return File(bytes, contentType);
        }

        return NotFound();
    }

    // Helper Methods

    private List<string> GetAllLabels()
    {
        var labels = new List<string>();
        using (var conn = new SqliteConnection($"Data Source={dbPath}"))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT LabelName FROM Labels";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    labels.Add(reader.GetString(0));
                }
            }
        }
        return labels;
    }


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

    private (string filePath, string extension) GetFilePath(string imageHash)
    {
        using (var conn = new SqliteConnection($"Data Source={dbPath}"))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PathHash, OriginalFullPath FROM Images WHERE ImageHash = @hash";
            cmd.Parameters.AddWithValue("@hash", imageHash);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string result = reader.GetString(0);
                    string extension = Path.GetExtension(reader.GetString(1));
                    if (result != null)
                    {
                        return (Path.Combine(GetCopyFolder(), result), extension);
                    }
                }
            }
        }
        (string filePath, string extension) value = (null, null);
        return value;
    }

    private string GetContentType(string extension)
    {
        switch (extension.ToLowerInvariant())
        {
            case ".jpg":
                return "image/jpg";
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

    private static string GetCopyFolder()
    {
        string appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ImageLabelApp"
        );
        return Path.Combine(appFolder, "Images");
    }

}


