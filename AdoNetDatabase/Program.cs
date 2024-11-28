using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AdoNetDatabase
{
    class Program
    {
        // Verbindung zur Datenbank über den Connection-String aus appsettings.json
        static string connectionString = GetConnectionString();

        /// <summary>
        /// Hauptmethode der Konsole-Anwendung. Wählt die gewünschte Option für Datenbankoperationen.
        /// </summary>
        static void Main(string[] args)
        {
            // Auswahl des zu verwendenden Befehls
            Console.WriteLine("Wählen Sie eine Option:");
            Console.WriteLine("1. Alle Blog-Einträge anzeigen");
            Console.WriteLine("2. Neuen Blog einfügen");
            Console.WriteLine("3. Blog löschen");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Select();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                default:
                    Console.WriteLine("Ungültige Option.");
                    break;
            }
        }

        /// <summary>
        /// Holt den Connection-String aus der appsettings.json-Datei.
        /// </summary>
        /// <returns>Connection-String als String</returns>
        static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            return configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Listet alle Blog-Einträge auf der Konsole auf.
        /// </summary>
        static void Select()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();  // Verbindungsversuch
                    Console.WriteLine("Verbindung erfolgreich hergestellt.");

                    string query = "SELECT BlogId, Url FROM Blog";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Keine Blog-Einträge gefunden.");
                    }
                    else
                    {
                        Console.WriteLine("Alle Blog-Einträge:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"BlogId: {reader["BlogId"]}, Url: {reader["Url"]}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Herstellen der Verbindung: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Fügt einen neuen Blog-Eintrag mit einer URL in die Datenbank ein.
        /// </summary>
        static void Insert()
        {
            Console.Write("Geben Sie die URL des Blogs ein: ");
            string url = Console.ReadLine();

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Blog (Url) VALUES (@Url)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Url", url);
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} Blog hinzugefügt.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Einfügen des Blogs: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Löscht einen Blog-Eintrag basierend auf der angegebenen BlogId.
        /// </summary>
        static void Delete()
        {
            Console.Write("Geben Sie die BlogId des zu löschenden Blogs ein: ");
            int blogId = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM Blog WHERE BlogId = @BlogId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BlogId", blogId);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Blog mit BlogId {blogId} wurde gelöscht.");
                    }
                    else
                    {
                        Console.WriteLine("BlogId nicht gefunden.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Löschen des Blogs: " + ex.Message);
                }
            }
        }
    }
}
