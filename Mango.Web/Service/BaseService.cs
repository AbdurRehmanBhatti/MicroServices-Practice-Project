using Mango.Web.Modals;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            // Create an HttpClient instance using an HTTP client factory with the named client "MangoApi"
            HttpClient client = _httpClientFactory.CreateClient("MangoApi");

            // Initialize a new HttpRequestMessage to send an HTTP request
            HttpRequestMessage message = new();

            // Add an "Accept" header to indicate the client expects JSON responses
            message.Headers.Add("Accept", "application/json");

            // If the request requires an authorization token, add a bearer token to the request headers
            if (withBearer)
            {
                // Get the access token from the session
                var accessToken = _tokenProvider.GetToken();

                message.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            // Set the request URI from the provided RequestDto object
            message.RequestUri = new Uri(requestDto.Url);

            // If the request has a data payload, serialize it as JSON and attach it as the request content
            if (requestDto.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage? apiResponse = null;

            // Determine the HTTP method based on the ApiType in the requestDto
            switch (requestDto.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            // Send the request asynchronously and store the response
            apiResponse = await client.SendAsync(message);

            try
            {
                // Handle different HTTP status codes and return corresponding error messages
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, ErrorMessage = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, ErrorMessage = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, ErrorMessage = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, ErrorMessage = "Internal Server Error" };
                    default:
                        // If the response status is OK, read the content and deserialize it into a ResponseDto
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResoinbseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResoinbseDto;
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs, return a ResponseDto with the exception message
                var dto = new ResponseDto
                {
                    ErrorMessage = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
