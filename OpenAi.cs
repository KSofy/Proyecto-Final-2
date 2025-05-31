using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Protocols;

namespace ServicioAgro_Jutiapa.Conexiones
{
    public class OpenAi
    {
        private readonly string apiKey;

        public OpenAi()
        {
            var key = ConfigurationManager.AppSettings["OpenAI_ApiKey"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("La clave de API de OpenAI no está configurada en App.config");
            apiKey = key!;
        }
        /// Realiza una consulta asíncrona a la API de OpenAI usando el prompt proporcionado.
        public async Task<string> ConsultarAsync(string prompt)
        {
            try
            {
                // Crea un cliente HTTP y agrega la cabecera de autorización con la API Key.
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                // Construye el cuerpo de la solicitud con el modelo y el mensaje del usuario.
               
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new 
                        {
                            role = "system",
                            content = "Eres un experto en agricultura y agroindustria, capaz de responder preguntas técnicas y brindar recomendaciones sobre la enfermedades en cultivos."

                        },
                        new
                        {
                            role = "user",
                            content = prompt 
                        }
                    }
                };
                // Serializa el cuerpo a JSON y lo prepara para enviar.
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                // Envía la solicitud POST a la API de OpenAI.
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();// Lanza excepción si la respuesta no es exitosa.
                // Lee y deserializa la respuesta JSON.
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);
                // Devuelve el contenido generado por la IA.
                return result.choices[0].message.content;
            }
            catch (Exception ex)
            {
                return $"Error consultando IA: {ex.Message}";
            }
        }
    }
}
    
