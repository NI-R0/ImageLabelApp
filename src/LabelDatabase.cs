using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLabelApp
{
    public static class LabelDatabase
    {
        private static readonly string dbFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "imageLabelApp"
        );
        private static readonly string dbPath = Path.Combine(dbFolder, "labels.db");

        static LabelDatabase()
        {
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("CREATE TABLE Labels (Path TEXT PRIMARY KEY, Label TEXT)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void AddLabel(string imagePath, string label)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Labels (Path, Label) VALUES (@p, @l)", conn))
                {
                    cmd.Parameters.AddWithValue("@p", imagePath);
                    cmd.Parameters.AddWithValue("@l", label);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveLabel(string imagePath, string label)
        {
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Labels WHERE Path = @p AND Label = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@p", imagePath);
                    cmd.Parameters.AddWithValue("@l", label);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static List<string> GetImagesForLabel(string label)
        {
            var result = new List<string>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Path FROM Labels WHERE Label = @l", conn))
                {
                    cmd.Parameters.AddWithValue("@l", label);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return result;
        }
    }
}
