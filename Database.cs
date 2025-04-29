using System;
using System.Collections.Generic;
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

        public Database(string sqlConnectionString)
        {
            ConnectionString = sqlConnectionString;
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
    }
}
