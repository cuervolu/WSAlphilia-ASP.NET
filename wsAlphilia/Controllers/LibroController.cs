using ConexionDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using wsAlphilia.Services;

namespace wsAlphilia.Controllers
{
    public class LibroController : ApiController
    {
        private readonly GoogleBooksApiService _googleBooksApiService;
        private readonly LibroContext _libroContext;

        public LibroController()
        {
            _googleBooksApiService = new GoogleBooksApiService();
            _libroContext = new LibroContext();
        }

        [HttpGet]
        [Route("api/Libro/GetLibros")]
        public async Task<IHttpActionResult> GetLibrosFromApi()
        {
            // Obtener la clave de la API de Google Books desde la variable de entorno
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_BOOKS_API_KEY");

            // Obtener los libros de la API de Google Books
            var response = await _googleBooksApiService.GetBooks(0, 40);

            // Si la respuesta no es exitosa, devolver un mensaje de error con el código de estado correspondiente
            if (!response.IsSuccessStatusCode)
            {
                return Content(response.StatusCode, "No se pudo obtener la respuesta de la API de Google Books.");
            }

            // Leer la respuesta de la API de Google Books
            var content = await response.Content.ReadAsStringAsync();

            // Convertir la respuesta de la API de Google Books en un objeto dynamic de C#
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

            // Crear una lista para almacenar los libros
            var libros = new List<LIBRO>();

            // Iterar a través de los resultados y crear objetos Libro
            foreach (var item in result.items)
            {
                // Obtener la información del volumen y la información de venta
                var volume_info = (JObject)item["volumeInfo"];
                var sale_info = (JObject)item["saleInfo"];

                // Obtener la categoría del libro, si está disponible
                var categorias = volume_info.Value<JArray>("categories");
                string categoria = "General";
                if (categorias != null && categorias.Count > 0)
                {
                    categoria = categorias[0].ToString();

                }

                // Obtener la fecha de publicación del libro
                var fecha_publicacion_str = volume_info.Value<string>("publishedDate");
                DateTime fecha_publicacion;
                if (!DateTime.TryParse(fecha_publicacion_str, out fecha_publicacion))
                {
                    continue;
                }

                // Crear un objeto Libro con valores predeterminados
                var default_values = new LIBRO
                {
                    NOMBRE_LIBRO = volume_info.Value<string>("title") ?? "",
                    AUTOR = volume_info.Value<JArray>("authors")?[0]?.ToString() ?? "a/d",
                    DESCRIPCION = volume_info.Value<string>("description") ?? "Sin descripción",
                    EDITORIAL = volume_info.Value<string>("publisher") ?? "s/e",
                    PRECIO_UNITARIO = Convert.ToInt64(sale_info.Value<JObject>("retailPrice")?.Value<double?>("amount") ?? new Random().Next(8000, 45000)),
                    PORTADA = volume_info.Value<JObject>("imageLinks")?.Value<string>("thumbnail") ?? "https://islandpress.org/sites/default/files/default_book_cover_2015.jpg",
                    CANTIDAD_DISPONIBLE = new Random().Next(1, 150),
                    FECHA_PUBLICACION = fecha_publicacion,
                    CATEGORIA = categoria
                };

                // Agregar el objeto Libro a la lista
                libros.Add(default_values);
            }

            // Guardar los libros en la base de datos de Oracle
            _libroContext.Libro.AddRange(libros);
            _libroContext.SaveChanges();

            /* 
                Devolver una respuesta exitosa con la lista de libros en formato JSON
                Se utiliza el método Json que serializa la lista de libros a formato JSON y la devuelve en la respuesta HTTP.
                Además, se establecen algunas opciones de serialización, como el formateo y la resolución de nombres de propiedades en formato CamelCase.
                */
            return Json(libros, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}