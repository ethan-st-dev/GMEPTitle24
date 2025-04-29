using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GMEPTitle24
{
    public class Database
    {
        public string ConnectionString { get; set; }
        public MySqlConnection Connection { get; set; }

        public Database()
        {
            ConnectionString = Properties.Settings.Default.ConnectionString;
            Connection = new MySqlConnection(ConnectionString);
        }

        public async Task OpenConnectionAsync()
        {
            if (Connection.State == System.Data.ConnectionState.Closed)
            {
                await Connection.OpenAsync();
            }
        }

        public async Task CloseConnectionAsync()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                await Connection.CloseAsync();
            }
        }

        public async Task<Dictionary<int, string>> GetProjectIds(string projectNo)
        {
            string query = "SELECT id, version FROM projects WHERE gmep_project_no = @projectNo";
            await OpenConnectionAsync();
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@projectNo", projectNo);
            MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();

            Dictionary<int, string> projectIds = new Dictionary<int, string>();
            while (reader.Read())
            {
                projectIds.Add(reader.GetInt32("version"), reader.GetString("id"));
            }
            await reader.CloseAsync();

            if (!projectIds.Any())
            {
                // Project name does not exist, insert a new entry with a generated ID
                var id = Guid.NewGuid().ToString();
                string insertQuery =
                    "INSERT INTO projects (id, gmep_project_no) VALUES (@id, @projectNo)";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, Connection);
                insertCommand.Parameters.AddWithValue("@id", id);
                insertCommand.Parameters.AddWithValue("@projectNo", projectNo);
                await insertCommand.ExecuteNonQueryAsync();
                projectIds.Add(1, id);
            }

            await CloseConnectionAsync();
            projectIds = projectIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            return projectIds;
        }
        public async Task<ObservableCollection<Lighting>> GetLighting(
            string projectId
        )
        {
            ObservableCollection<Lighting> lightings =
                new ObservableCollection<Lighting>();
            string query = "SELECT * FROM `electrical_lighting` LEFT JOIN electrical_lighting_locations on electrical_lighting_locations.id = electrical_lighting.location_id where electrical_lighting.project_id = 'b3b6260b-416e-4449-bfb3-6e0e98f774d2' and electrical_lighting_locations.outdoor = 0";
            await OpenConnectionAsync();
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@projectId", projectId);
            MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lightings.Add(
                    new Lighting(
                        reader.GetString("id"),
                        reader.GetString("project_id"),
                        reader.GetString("tag"),
                        reader.GetString("description"),
                        false,
                        reader.GetFloat("wattage"),
                        1,
                        1,
                        reader.GetInt32("qty"),
                        false
                    )
                );
            }

            await reader.CloseAsync();
            await CloseConnectionAsync();
            return lightings;
        }
    }
}
