using CarInfoTask.Models;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;

namespace CarInfoTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public CarsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet("{modelyea}/{make}")]
        public async Task<string> GetModelsForMakeIdYear(string modelyea, string make)
        {
            var client = _httpClientFactory.CreateClient();

            //Read car make ID from .csv file
            using var reader = new StreamReader("CarMakeCsv/CarMake.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var carMake = csv.GetRecords<CarMakeModel>().Where(x => x.make_name.ToUpper() == make.Trim().ToUpper()).FirstOrDefault();

            //Get car Models
            using var httpResponse = await client.GetAsync($"https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMakeIdYear/makeId/" +
                $"{carMake?.make_id}/modelyear/" +
                $"{modelyea}?format=json",
                HttpCompletionOption.ResponseHeadersRead);


            if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();

                try
                {
                    var carInfo = await System.Text.Json.JsonSerializer.DeserializeAsync<CarInfoModel>(contentStream, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });

                    if (carInfo is not null)
                        return JsonConvert.SerializeObject(new ResponseModel
                        {
                            Models = carInfo.Results.Select(x => x.Model_Name).Distinct().ToList()
                        });
                }
                catch (JsonReaderException)
                {
                    return "Invalid JSON.";
                }
            }
            else
            {
                return "invalid input, There is no data found.";
            }

            return "There is no data found.";
        }
    }
}
