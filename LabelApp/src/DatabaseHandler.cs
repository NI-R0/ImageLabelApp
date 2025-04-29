using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace LabelApp
{
    public static class DatabaseHandler
    {
        // Labels (LabelName)
        // ImageLabels (ImagePath, LabelName)

        private static readonly string appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ImageLabelApp"
        );
        private static readonly string dbPath = Path.Combine(appFolder, "labels.db");

        static DatabaseHandler()
        {
            EnsureDatabaseInitialized();
        }

        public static void CreateDatabase()
        {
            EnsureDatabaseInitialized();
        }

        private static void EnsureDatabaseInitialized()
        {
            try
            {
                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }
                using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
                {
                    conn.Open();

                    // Create a table for unique labels
                    using (var cmd = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS Labels (LabelName TEXT PRIMARY KEY)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create a table to store labeled images
                    using (var cmd = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS Images(" +
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "ImageHash TEXT UNIQUE, " +
                        "PathHash TEXT UNIQUE, " +
                        "OriginalFileName TEXT, " +
                        "OriginalFullPath TEXT, " +
                        "AddedDate DATETIME)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create a table for image-label associations
                    using (var cmd = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS ImageLabels (ImageHash TEXT, LabelName TEXT, " +
                        "PRIMARY KEY (ImageHash, LabelName), " +
                        "FOREIGN KEY (ImageHash) REFERENCES Images(ImageHash), " +
                        "FOREIGN KEY (LabelName) REFERENCES Labels(LabelName))", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create a table for (future) app settings
                    using (var cmd = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS AppSettings (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, Value TEXT)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("EnsureDatabaseInitialized - " + ex.Message);
                Console.WriteLine("Failed to initialize label database: " + ex.Message);
                throw;
            }
        }

        public static List<string> GetAllLabels()
        {
            List<string> labels = new List<string>();
            EnsureDatabaseInitialized();

            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(
                    "SELECT LabelName FROM Labels ORDER BY LabelName", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        labels.Add(reader.GetString(0));
                }
            }
            return labels;
        }

        public static void CreateNewLabel(string labelName)
        {
            // Create new label in database
            // Form -> Create new context menu entry
            EnsureDatabaseInitialized();
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Labels (LabelName) VALUES (@l)", conn))
                    {
                        cmd.Parameters.AddWithValue("@l", labelName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DeleteExistingLabel(string labelName)
        {
            // Form -> Delete label folder
            // Delete all entries from ImageLabels where label=labelName
            // Delete label from labels
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM ImageLabels WHERE LabelName = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@l", labelName);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SQLiteCommand("DELETE FROM Labels WHERE LabelName = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@l", labelName);
                    cmd.ExecuteNonQuery();
                }

                DeleteOrphanedImages(conn);
            }
        }

        private static void DeleteOrphanedImages(SQLiteConnection conn)
        {
            using (conn)
            {
                List<string> orphanedImages = new List<string>();
                List<string> orphanedPaths = new List<string>();
                using (var cmd = new SQLiteCommand(@"
                                    SELECT ImageHash, OriginalFullPath
                                    FROM Images 
                                    WHERE ImageHash NOT IN (SELECT DISTINCT ImageHash FROM ImageLabels)", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orphanedImages.Add(reader.GetString(0));
                        orphanedPaths.Add(reader.GetString(1));
                    }
                }

                foreach (var hash in orphanedImages)
                {
                    using (var cmd = new SQLiteCommand("DELETE FROM Images WHERE ImageHash = @hash", conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", hash);
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (var path in orphanedPaths)
                {
                    CopyManager.RemoveCopy(path);
                }
            }
            
        }

        public static void AddImage(string imagePath)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Images (ImageHash, PathHash, OriginalFileName, OriginalFullPath, AddedDate) VALUES (@image, @hash, @name, @path, @date)", conn))
                {
                    cmd.Parameters.AddWithValue("@image", ConvertImageToHash(imagePath));
                    cmd.Parameters.AddWithValue("@hash", ConvertPathToHash(imagePath));
                    cmd.Parameters.AddWithValue("@name", Path.GetFileName(imagePath));
                    cmd.Parameters.AddWithValue("@path", imagePath);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddLabelToImage(string imagePath, string labelName)
        {
            // Add entry into ImageLabels
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO ImageLabels (ImageHash, LabelName) VALUES (@h, @l)", conn))
                {
                    cmd.Parameters.AddWithValue("@h", ConvertImageToHash(imagePath));
                    cmd.Parameters.AddWithValue("@l", labelName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveImage(string imagePath)
        {
            EnsureDatabaseInitialized();
            string hash = ConvertImageToHash(imagePath);

            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                int count = 0;
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM ImageLabels WHERE ImageHash = @hash", conn))
                {
                    cmd.Parameters.AddWithValue("@hash", hash);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = reader.GetInt32(0);
                        }
                    }
                }

                if (count <= 0)
                {
                    using (var cmd = new SQLiteCommand("DELETE FROM Images WHERE ImageHash = @hash", conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", hash);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void RemoveLabelFromImage(string imagePath, string labelName)
        {
            // Remove entry from ImageLabels
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM ImageLabels WHERE ImageHash = @h AND LabelName = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@h", ConvertImageToHash(imagePath));
                    cmd.Parameters.AddWithValue("@l", labelName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool ImageExists(string imagePath)
        {
            List<int> ids = new List<int>();
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Id FROM Images WHERE ImageHash = @h", conn))
                {
                    cmd.Parameters.AddWithValue("@h", ConvertImageToHash(imagePath));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return (ids.Count > 0);
        }

        public static bool LabelExists(string labelName)
        {
            List<string> labels = new List<string>();
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT LabelName FROM LABELS WHERE LabelName = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@l", labelName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            labels.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return (labels.Count > 0);
        }


        public static string GetAppFolder()
        {
            return appFolder;
        }

        public static string GetCopyFolder()
        {
            return Path.Combine(appFolder, "Images");
        }

        public static void DeleteDatabase()
        {
            if (Directory.Exists(appFolder))
            {
                Directory.Delete(appFolder, true);
            }
        }

        public static string ConvertImageToHash(string imagePath)
        {
            byte[] fileBytes = File.ReadAllBytes(imagePath);
            return CalculateSHA256Hash(fileBytes);
        }

        public static string ConvertPathToHash(string imagePath)
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(imagePath);
            return CalculateMD5Hash(stringBytes);
        }

        private static string CalculateSHA256Hash(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private static string CalculateMD5Hash(byte[] bytes)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
