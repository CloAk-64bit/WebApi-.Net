using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Polly;
using System.Net.Http;
using PropertyApi.Models;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PropertyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<PropertyController> _logger;
        public PropertyController( IHttpClientFactory httpClient, ILogger<PropertyController> logger ) 
        { 
            _httpClient= httpClient;
            _logger = logger;
        }

        
        private async Task<List<PropertyModel>> GetDataFromWebsiteAsync(int pageNumber)
        {
            var client = _httpClient.CreateClient();
            var response = await client.GetAsync($"https://www.remax.co.za/property/for-sale/south-africa/?page={pageNumber}");


                response = await Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(20, _ => TimeSpan.FromSeconds(4))
                    .ExecuteAsync(() => client.GetAsync($"https://www.remax.co.za/property/for-sale/south-africa/?page={pageNumber}"));
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);


                //parse the html to extract the data
                var propertiesHtml = htmlDoc.DocumentNode.Descendants("div")
                                                         .Where(node => node.GetAttributeValue("id", "").Equals("search-results-list"))
                                                         .ToList();

                var dataList = propertiesHtml[0].Descendants("div")
                                                .Where(node => node.GetAttributeValue("id", "").Contains("property-"))
                                                .ToList();

                var result = new List<PropertyModel>();

                foreach (var data in dataList)
                {
                    var propertyId = data.GetAttributeValue("id", "").ToString();

                    var propertyTitle = data.Descendants("div")
                        .Where(node => node.GetAttributeValue("itemprop", "")
                        .Equals("name"))
                        .FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                    var propertyPrice =
                        Regex.Match(
                        data.Descendants("span")
                        .Where(node => node.GetAttributeValue("itemprop", "")
                        .Equals("price"))
                        .FirstOrDefault().InnerText
                        .Trim('\r', '\n', '\t')
                        , @"\d+.\d+").Value;

                    var propertyDesc = data.Descendants("p")
                        .Where(node => node.GetAttributeValue("itemprop", "")
                        .Equals("description"))
                        .FirstOrDefault().InnerText;

                    var totalBedRooms = data.Descendants("span")
                        .Where(node => node.GetAttributeValue("itemprop", "")
                        .Equals("numberOfBedRooms"))
                        .FirstOrDefault().InnerText;

                    var totalBathRooms = data.Descendants("span")
                        .Where(node => node.GetAttributeValue("itemprop", "")[0]
                        .Equals("amenityFeature"))
                        .FirstOrDefault().InnerText;

                    var propertyLink = data.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");

                    var totalParkingSpace = data.Descendants("span")
                        .Where(node => node.GetAttributeValue("itemprop", "")[1]
                        .Equals("amenityFeature"))
                        .FirstOrDefault().InnerText;

                    var erfSize = data.Descendants("span")
                        .Where(node => node.GetAttributeValue("itemprop", "")
                        .Equals("floorSize"))
                        .FirstOrDefault().InnerText;


                    result.Add(new PropertyModel(propertyId, propertyTitle, propertyDesc,
                        totalParkingSpace, erfSize, propertyPrice, totalBedRooms,
                        totalBathRooms, propertyLink));
                }
            return result;
        }


        private async Task<List<PropertyModel>> ScrapeWebsiteAsync(int pageNumber)
        {
            
            var allDataList = new List<PropertyModel>();
            for (int i = 1; i >= pageNumber; i++)
            {
                var dataList = await GetDataFromWebsiteAsync(i);
                allDataList.AddRange(dataList);
            }
            return allDataList;
        }

        [HttpGet(Name = "Properties")]
        public async Task<ActionResult<List<PropertyModel>>> GetData()
        {
            var dataList = await ScrapeWebsiteAsync(1);
            return Ok(dataList);
        }
    }
}
