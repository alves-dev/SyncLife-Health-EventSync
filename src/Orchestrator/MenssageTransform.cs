using System.Diagnostics.Tracing;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Orchestrator
{
    public class Device
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("identifiers")]
        public string[] Identifiers { get; set; }
    }

    public class MenssageTransform
    {
        public static String GetLiquidSummaryHealthyPayload(string eventRabbit)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(eventRabbit);
                int healthy = jsonDoc.RootElement.GetProperty("total_liquid").GetProperty("healthy").GetInt16();
                return healthy.ToString();
            }
            catch (Exception)
            {
                Console.WriteLine("[MenssageTransform] 'healthy' não encontrado.");
                return "-1";
            }
        }

        public static String GetLiquidSummaryAcceptablePayload(string eventRabbit)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(eventRabbit);
                JsonElement liquidsArray = jsonDoc.RootElement.GetProperty("accepted_liquids");
                string acceptedLiquids = string.Join(",", liquidsArray.EnumerateArray().Select(liquid => liquid.GetString()));
                return acceptedLiquids;
            }
            catch (Exception)
            {
                Console.WriteLine("[MenssageTransform] 'accepted_liquids' não encontrado.");
                return "-1";
            }
        }

        public static String? GetEntityLiquidSummaryHealthyPayload()
        {

            var json = new Dictionary<string, object>
            {
                { "name", "Liquid Summary Healthy" },
                { "object_id", "health.nutri.track.liquid.summary.healthy.state" },
                { "unique_id", "health.nutri.track.liquid.summary.healthy.state" },
                { "device_class", "water"},
                { "unit_of_measurement", "mL"},
                { "expire_after", GetSecondsUntilEndOfDay()},
                { "state_topic", "health/nutri/track/liquid/summary/healthy/state"},
                { "device", GetHealthDevice() }
            };

            try
            {
                return JsonSerializer.Serialize(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deu ruim né!");
            }
            return null;
        }

        public static String? GetEntityLiquidAcceptablePayload()
        {

            var json = new Dictionary<string, object>
            {
                { "name", "Liquid Acceptable" },
                { "object_id", "health.nutri.track.liquid.acceptable.state" },
                { "unique_id", "health.nutri.track.liquid.acceptable.state" },
                { "state_topic", "health/nutri/track/liquid/acceptable/state"},
                { "device", GetHealthDevice() }
            };

            try
            {
                return JsonSerializer.Serialize(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deu ruim né!");
            }
            return null;
        }

        private static Device GetHealthDevice()
        {
            var device = new Device
            {
                Name = "Health Data",
                Manufacturer = "Health",
                Model = "Health V1",
                Identifiers = new[] { "health_data" }
            };

            return device;
        }

        private static int GetSecondsUntilEndOfDay()
        {
            DateTime now = DateTime.Now;
            DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            TimeSpan difference = endOfDay - now;

            return (int)difference.TotalSeconds;
        }
    }
}