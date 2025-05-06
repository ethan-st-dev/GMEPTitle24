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
                            ellum.is_excluded,
                            ellum.conditioned_qty,
                            ellum.unconditioned_qty,
                            ellum.luminaire_qty,
                            ellum.volt_ampere_rating,
                            ellum.linear_feet,
                            ellum.branch_circuit_voltage,
                            ellum.combined_breaker_amps,
                            ellum.max_input_wattage
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
                        !reader.IsDBNull(reader.GetOrdinal("conditioned_type_id")) ? reader.GetInt32("conditioned_type_id") : 1,
                        !reader.IsDBNull(reader.GetOrdinal("conditioned_qty")) ? reader.GetInt32("conditioned_qty") : 0,
                         !reader.IsDBNull(reader.GetOrdinal("unconditioned_qty")) ? reader.GetInt32("unconditioned_qty") : 0,
                        !reader.IsDBNull(reader.GetOrdinal("luminaire_qty")) ? reader.GetInt32("luminaire_qty") : 0,
                        !reader.IsDBNull(reader.GetOrdinal("volt_ampere_rating")) ? reader.GetFloat("volt_ampere_rating") : 0,
                        !reader.IsDBNull(reader.GetOrdinal("linear_feet")) ? reader.GetFloat("linear_feet") : 0,
                        !reader.IsDBNull(reader.GetOrdinal("branch_circuit_voltage")) ? reader.GetFloat("branch_circuit_voltage") : 0,
                        !reader.IsDBNull(reader.GetOrdinal("combined_breaker_amps")) ? reader.GetFloat("combined_breaker_amps") : 0,
                        !reader.IsDBNull(reader.GetOrdinal("max_input_wattage")) ? reader.GetFloat("max_input_wattage") : 0

                    )
                );
                if (reader.IsDBNull(reader.GetOrdinal("luminaire_id")))
                {
                    newLuminaireIds.Add(reader.GetString("lighting_id"));
                }
            }
            await reader.CloseAsync();

            //Adding Luminaires for lighting that doesn't have one
            query = @"INSERT INTO electrical_lighting_lti_luminaires 
                     (id, fixture_id, type_id, is_decorative, wattage_source_id, is_excluded, project_id, compliance_method_id, occupancy_type_id, conditioned_type_id, conditioned_qty, unconditioned_qty, luminaire_qty, volt_ampere_rating, linear_feet, branch_circuit_voltage, combined_breaker_amps, max_input_wattage) 
                     VALUES 
                     (@id, @fixtureId, @typeId, @isDecorative, @wattageSourceId, @isExcluded, @projectId, @complianceMethodId, @occupancyTypeId, @conditionedTypeId, @conditionedQty, @unconditionedQty, @luminaireQty, @voltAmpereRating, @linearFeet, @branchCircuitVoltage, @combinedBreakerAmps, @maxInputWattage)";


            foreach (var luminaireId in newLuminaireIds)
            {
                command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@fixtureId", luminaireId);
                command.Parameters.AddWithValue("@typeId", 1);
                command.Parameters.AddWithValue("@isDecorative", false);
                command.Parameters.AddWithValue("@wattageSourceId", 1);
                command.Parameters.AddWithValue("@isExcluded", false);
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@complianceMethodId", 1);
                command.Parameters.AddWithValue("@occupancyTypeId", 1);
                command.Parameters.AddWithValue("@conditionedTypeId", 1);
                command.Parameters.AddWithValue("@conditionedQty", 0);
                command.Parameters.AddWithValue("@unconditionedQty", 0);
                command.Parameters.AddWithValue("@luminaireQty", 1);
                command.Parameters.AddWithValue("@voltAmpereRating", 0);
                command.Parameters.AddWithValue("@linearFeet", 0);
                command.Parameters.AddWithValue("@branchCircuitVoltage", 0);
                command.Parameters.AddWithValue("@combinedBreakerAmps", 0);
                command.Parameters.AddWithValue("@maxInputWattage", 0);
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
                string query = "UPDATE electrical_lighting_lti_luminaires SET is_decorative = @isDecorative, type_id = @typeId, wattage_source_id = @wattageSourceId, is_excluded = @isExcluded, compliance_method_id = @complianceMethodId, occupancy_type_id = @occupancyTypeId, conditioned_type_id = @conditionedTypeId, conditioned_qty = @conditionedQty, unconditioned_qty = @unconditionedQty, luminaire_qty = @luminaireQty, volt_ampere_rating = @voltAmpereRating, linear_feet = @linearFeet, branch_circuit_voltage = @branchCircuitVoltage, combined_breaker_amps = @combinedBreakerAmps, max_input_wattage = @maxInputWattage WHERE fixture_id = @id";
                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@isDecorative", lighting.IsDecorative);
                command.Parameters.AddWithValue("@typeId", lighting.TypeId);
                command.Parameters.AddWithValue("@wattageSourceId", lighting.WattageSourceId);
                command.Parameters.AddWithValue("@isExcluded", lighting.IsExcluded);
                command.Parameters.AddWithValue("@id", lighting.Id);
                command.Parameters.AddWithValue("@complianceMethodId", lighting.ComplianceMethodId);
                command.Parameters.AddWithValue("@occupancyTypeId", lighting.OccupancyTypeId);
                command.Parameters.AddWithValue("@conditionedTypeId", lighting.ConditionedTypeId);
                command.Parameters.AddWithValue("@conditionedQty", lighting.ConditionedQty);
                command.Parameters.AddWithValue("@unconditionedQty", lighting.UnconditionedQty);
                command.Parameters.AddWithValue("@luminaireQty", lighting.LuminaireQty);
                command.Parameters.AddWithValue("@voltAmpereRating", lighting.VoltAmpRating);
                command.Parameters.AddWithValue("@linearFeet", lighting.LinearFeet);
                command.Parameters.AddWithValue("@branchCircuitVoltage", lighting.BranchCircuitVoltage);
                command.Parameters.AddWithValue("@combinedBreakerAmps", lighting.CombinedBreakerAmps);
                command.Parameters.AddWithValue("@maxInputWattage", lighting.MaxInputWattage);
                await command.ExecuteNonQueryAsync();
            }
            await CloseConnectionAsync();
        }
        public async Task<ObservableCollection<ControlArea>> GetControlAreas(
            string projectId
        )
        {
            // Get the lighting data from the database
            ObservableCollection<ControlArea> areas =
                new ObservableCollection<ControlArea>();
            string query = @"SELECT 
                            *
                            FROM 
                                electrical_lighting_lti_control_areas
                            WHERE 
                                project_id = @projectId";
            await OpenConnectionAsync();
            MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@projectId", projectId);
            MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();

            List<string> newLuminaireIds = new List<string>();

            while (await reader.ReadAsync())
            {
                areas.Add(
                    new ControlArea(
                        reader.GetString("id"),
                        reader.GetString("project_id"),
                        reader.GetString("description"),
                        reader.GetInt32("primary_function_id"),
                        reader.GetInt32("area_control_type_id"),
                        reader.GetInt32("multilevel_control_type_id"),
                        reader.GetInt32("shutoff_control_type_id"),
                        reader.GetInt32("primary_daylight_control_type_id"),
                        reader.GetInt32("secondary_daylight_control_type_id"),
                        reader.GetBoolean("interlocked_systems"),
                        reader.GetFloat("square_footage")
                    )
                );
            }
            await reader.CloseAsync();
            return areas;
        }
        public async Task UpdateControlAreas(
            ObservableCollection<ControlArea> areas,
            string projectId
        )
        {
            await OpenConnectionAsync();


            string deleteQuery = @"
                DELETE FROM electrical_lighting_lti_control_areas
                WHERE project_id = @projectId
                AND id NOT IN (@ids)";

            var ids = string.Join(",", areas.Select(a => $"'{a.Id}'"));

            if (!string.IsNullOrEmpty(ids))
            {
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, Connection);
                deleteCommand.Parameters.AddWithValue("@projectId", projectId);
                deleteCommand.Parameters.AddWithValue("@ids", ids);
                await deleteCommand.ExecuteNonQueryAsync();
            }
            foreach (var area in areas)
            {
                string query = @"
                    INSERT INTO electrical_lighting_lti_control_areas 
                    (id, project_id, description, primary_function_id, area_control_type_id, 
                     multilevel_control_type_id, shutoff_control_type_id, 
                     primary_daylight_control_type_id, secondary_daylight_control_type_id, 
                     interlocked_systems, square_footage) 
                    VALUES 
                    (@id, @projectId, @description, @primaryFunctionId, @areaControlTypeId, 
                     @multilevelControlTypeId, @shutoffControlTypeId, 
                     @primaryDaylightControlTypeId, @secondaryDaylightControlTypeId, 
                     @interlockedSystems, @squareFootage)
                    ON DUPLICATE KEY UPDATE 
                    description = @description, 
                    primary_function_id = @primaryFunctionId, 
                    area_control_type_id = @areaControlTypeId, 
                    multilevel_control_type_id = @multilevelControlTypeId, 
                    shutoff_control_type_id = @shutoffControlTypeId, 
                    primary_daylight_control_type_id = @primaryDaylightControlTypeId, 
                    secondary_daylight_control_type_id = @secondaryDaylightControlTypeId, 
                    interlocked_systems = @interlockedSystems, 
                    square_footage = @squareFootage";

                MySqlCommand command = new MySqlCommand(query, Connection);
                command.Parameters.AddWithValue("@id", area.Id);
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@description", area.Description);
                command.Parameters.AddWithValue("@primaryFunctionId", area.PrimaryFunctionId);
                command.Parameters.AddWithValue("@areaControlTypeId", area.AreaControlTypeId);
                command.Parameters.AddWithValue("@multilevelControlTypeId", area.MultilevelControlTypeId);
                command.Parameters.AddWithValue("@shutoffControlTypeId", area.ShutoffControlTypeId);
                command.Parameters.AddWithValue("@primaryDaylightControlTypeId", area.PrimaryDaylightControlTypeId);
                command.Parameters.AddWithValue("@secondaryDaylightControlTypeId", area.SecondaryDaylightControlTypeId);
                command.Parameters.AddWithValue("@interlockedSystems", area.InterlockedSystems);
                command.Parameters.AddWithValue("@squareFootage", area.SquareFootage);

                await command.ExecuteNonQueryAsync();
            }
            await CloseConnectionAsync();
        }
    }
}
