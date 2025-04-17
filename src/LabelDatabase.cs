using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLabelApp
{
    public static class LabelDatabase
    {
        // Labels (LabelName)
        // ImageLabels (ImagePath, LabelName)

        private static readonly string dbFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ImageLabelApp"
        );
        private static readonly string dbPath = Path.Combine(dbFolder, "labels.db");

        static LabelDatabase()
        {
            EnsureDatabaseInitialized();
        }

        private static void EnsureDatabaseInitialized()
        {
            try
            {
                if (!Directory.Exists(dbFolder))
                {
                    Directory.CreateDirectory(dbFolder);
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
                        "CREATE TABLE IF NOT EXISTS Labels (LabelName TEXT PRIMARY KEY )", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Create a table for image-label associations
                    using (var cmd = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS ImageLabels (ImagePath TEXT, LabelName TEXT, " +
                        "PRIMARY KEY (ImagePath, LabelName), " +
                        "FOREIGN KEY (LabelName) REFERENCES Labels(LabelName))", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            }
        }

        public static void AddLabelToImage(string imagePath, string labelName)
        {
            // Add entry into ImageLabels
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO ImageLabels (ImagePath, LabelName) VALUES (@p, @l)", conn))
                {
                    cmd.Parameters.AddWithValue("@p", imagePath);
                    cmd.Parameters.AddWithValue("@l", labelName);
                    cmd.ExecuteNonQuery();
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
                using (var cmd = new SQLiteCommand("DELETE FROM ImageLabels WHERE Path = @p AND Label = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@p", imagePath);
                    cmd.Parameters.AddWithValue("@l", labelName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool LabelExists(string labelName)
        {
            List<string> labels = new List<string>();
            EnsureDatabaseInitialized();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT LabelName FROM LABELS WHERE LabelName = @l", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labels.Add(reader.GetString(0));
                    }
                }
            }
            return (labels.Count > 0);
        }

        public static void DeleteDatabase()
        {
            if (Directory.Exists(dbFolder))
            {
                Directory.Delete(dbFolder, true);
            }
        }

    }
}
