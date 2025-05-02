using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            if (projectNo == null || projectNo == string.Empty)
            {
                return new Dictionary<int, string>();
            }
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

            await CloseConnectionAsync();
            projectIds = projectIds.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            return projectIds;
        }
        public async Task<ObservableCollection<Lighting>> GetLighting(
            string projectId
        )
        {
            // Get the lighting data from the database
            ObservableCollection<Lighting> lightings =
                new ObservableCollection<Lighting>();
            string query = @"SELECT 
                            el.id AS lighting_id,
                            el.project_id,
                            el.tag,
                            el.description,
                            el.wattage,
                            el.qty,
                            ell.outdoor,
                            ellum.id AS luminaire_id,
                            ellum.fixture_id,
                            ellum.type_id,
                            ellum.is_decorative,
                            ellum.wattage_source_id,
                            ellum.occupancy_type_id,
                            ellum.conditioned_type_id,
                            ellum.compliance_method_id,
                            ellum.is_excluded
                            FROM 
                                electrical_lighting el
                            LEFT JOIN 
                                electrical_lighting_locations ell 
                            ON 
                                ell.id = el.location_id
                            LEFT JOIN 
                                electrical_lighting_lti_luminaires ellum 
                            ON 
                                ellum.fixture_id = el.id
                            WHERE 
                                el.project_id = @projectId 
                                AND ell.outdoor = 0;";
            await OpenConnectionAsync();
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@projectId", projectId);
            MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();

            List<string> newLuminaireIds = new List<string>();

            while (await reader.ReadAsync())
            {
                lightings.Add(
                    new Lighting(
                        reader.GetString("lighting_id"),
                        reader.GetString("project_id"),
                        reader.GetString("tag"),
                        reader.GetString("description"),
                        !reader.IsDBNull(reader.GetOrdinal("is_decorative")) ? reader.GetBoolean("is_decorative") : false,
                        reader.GetFloat("wattage"),
                        !reader.IsDBNull(reader.GetOrdinal("wattage_source_id")) ? reader.GetInt32("wattage_source_id") : 1,
                        !reader.IsDBNull(reader.GetOrdinal("type_id")) ? reader.GetInt32("type_id") : 1,
                        reader.GetInt32("qty"),
                        !reader.IsDBNull(reader.GetOrdinal("is_excluded")) && reader.GetBoolean("is_excluded"),
                        !reader.IsDBNull(reader.GetOrdinal("compliance_method_id")) ? reader.GetInt32("compliance_method_id") : 1,
                        !reader.IsDBNull(reader.GetOrdinal("occupancy_type_id")) ? reader.GetInt32("occupancy_type_id") : 1,
                        !reader.IsDBNull(reader.GetOrdinal("conditioned_type_id")) ? reader.GetInt32("conditioned_type_id") : 1
                    )
                );
                if (reader.IsDBNull(reader.GetOrdinal("luminaire_id")))
                {
                    newLuminaireIds.Add(reader.GetString("lighting_id"));
                }
            }
            await reader.CloseAsync();

            //Adding Luminaires for lighting that doesnt have one
            query = @"INSERT INTO electrical_lighting_lti_luminaires 
                     (id, fixture_id, type_id, is_decorative, wattage_source_id, is_excluded, project_id, compliance_method_id, occupancy_type_id, conditioned_type_id) 
                     VALUES 
                     (@id, @fixtureId, @typeId, @isDecorative, @wattageSourceId, @isExcluded, @projectId, @complianceMethodId, @occupancyTypeId, @conditionedTypeId)";


            foreach (var luminaireId in newLuminaireIds)
            {
                command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@fixtureId", luminaireId);
                command.Parameters.AddWithValue("@typeId", 1); // Default or placeholder value
                command.Parameters.AddWithValue("@isDecorative", false); // Default or placeholder value
                command.Parameters.AddWithValue("@wattageSourceId", 1); // Default or placeholder value
                command.Parameters.AddWithValue("@isExcluded", false); // Default or placeholder value
                command.Parameters.AddWithValue("@projectId", projectId); // Default or placeholder value
                command.Parameters.AddWithValue("@complianceMethodId", 1); // Default or placeholder value
                command.Parameters.AddWithValue("@occupancyTypeId", 1); // Default or placeholder value
                command.Parameters.AddWithValue("@conditionedTypeId", 1); // Default or placeholder value
                await command.ExecuteNonQueryAsync();
            }

            //Deleting Luminaires whose lightingfixture ids are not in the lighting table
            query = @" DELETE FROM electrical_lighting_lti_luminaires
                        WHERE project_id = @projectId
                        AND fixture_id NOT IN (
                            SELECT id FROM electrical_lighting WHERE project_id = @projectId
                        )";
            command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@projectId", projectId);
            await command.ExecuteNonQueryAsync();


            await CloseConnectionAsync();
            return lightings;
        }
        public async Task UpdateLuminaires(
            ObservableCollection<Lighting> lightings
        )
        {
            await OpenConnectionAsync();
            foreach (var lighting in lightings)
            {
                string query = "UPDATE electrical_lighting_lti_luminaires SET is_decorative = @isDecorative, type_id = @typeId, wattage_source_id = @wattageSourceId, is_excluded = @isExcluded, compliance_method_id = @complianceMethodId, occupancy_type_id = @occupancyTypeId, conditioned_type_id = @conditionedTypeId WHERE fixture_id = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@isDecorative", lighting.IsDecorative);
                command.Parameters.AddWithValue("@typeId", lighting.TypeId);
                command.Parameters.AddWithValue("@wattageSourceId", lighting.WattageSourceId);
                command.Parameters.AddWithValue("@isExcluded", lighting.IsExcluded);
                command.Parameters.AddWithValue("@id", lighting.Id);
                command.Parameters.AddWithValue("@complianceMethodId", lighting.ComplianceMethodId); // Default or placeholder value
                command.Parameters.AddWithValue("@occupancyTypeId", lighting.OccupancyTypeId); // Default or placeholder value
                command.Parameters.AddWithValue("@conditionedTypeId", lighting.ConditionedTypeId); // Default or placeholder value
                await command.ExecuteNonQueryAsync();
            }
            await CloseConnectionAsync();
        }
    }
}
