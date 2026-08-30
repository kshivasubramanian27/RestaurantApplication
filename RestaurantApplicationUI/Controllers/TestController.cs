using Microsoft.AspNetCore.Mvc;

namespace RestaurantApplicationUI.Controllers
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RestaurantAPI");

            var response = await client.GetAsync("api/Dashboard/test");

            return Content(
                $"Status Code: {(int)response.StatusCode}");
        }
    }
}