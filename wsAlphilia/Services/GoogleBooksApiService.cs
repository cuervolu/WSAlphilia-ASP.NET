using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace wsAlphilia.Services
{
    public class GoogleBooksApiService
    {
        // Se crea una instancia de HttpClient que se utilizará para hacer solicitudes HTTP
        private readonly HttpClient _httpClient;

        // Se guarda la clave de API de Google Books en una variable de instancia
        private readonly string _apiKey;


        public GoogleBooksApiService()
        {
            // Se inicializa la instancia de HttpClient
            _httpClient = new HttpClient();

            // Se obtiene la clave de API de Google Books desde una variable de entorno
            _apiKey = $"{Environment.GetEnvironmentVariable("GOOGLE_BOOKS_API_KEY")}";
        }

        public async Task<HttpResponseMessage> GetBooks(int startIndex, int maxResults)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q=*&key={_apiKey}&startIndex={startIndex}&maxResults={maxResults}&orderBy=relevance&random={Guid.NewGuid()}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Si la respuesta no es exitosa, devolver un mensaje de error con el código de estado correspondiente
                return new HttpResponseMessage(response.StatusCode)
                {
                    Content = new StringContent("No se pudo obtener la respuesta de la API de Google Books.")
                };
            }

            string content = await response.Content.ReadAsStringAsync();
            return new HttpResponseMessage()
            {
                Content = new StringContent(content),
                StatusCode = HttpStatusCode.OK
            };
        }

    }
}