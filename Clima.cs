using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ServicioAgro_Jutiapa
{
    public partial class Clima : Form
    {
        public Clima()
        {
            InitializeComponent();

        }



        private async void btnInvestigar_Click(object sender, EventArgs e)
        {
            lblError.Text = ""; // Limpia errores previos

            string ciudad = txtLugar.Text.Trim();
            string cultivo = txtPrompt.Text.Trim();

            if (string.IsNullOrEmpty(ciudad))
            {
                lblError.Text = "Por favor, ingresa la ciudad.";
                LimpiarDatosClima();
                return;
            }

            string apiKey = System.Configuration.ConfigurationManager.AppSettings["OpenWeather_ApiKey"];
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={ciudad}&appid={apiKey}&units=metric&lang=es";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string jsonResult = await response.Content.ReadAsStringAsync();
                    WeatherData climaData = JsonConvert.DeserializeObject<WeatherData>(jsonResult);

                    if (climaData != null)
                    {
                        lblLugar.Text = $"{climaData.Name}, {climaData.Sys?.Country}";
                        lblTemperatura.Text = $" {climaData.Main?.temp}°C";
                        lblHumedad.Text = $" {climaData.Main?.humidity}%";
                        lblViento.Text = $" {climaData.Wind?.speed} m/s";

                        string descripcionClima = climaData.Weather?[0]?.Description ?? "";
                        string prompt;

                        if (string.IsNullOrEmpty(cultivo))
                        {
                            // Sugerir cultivos según el clima y lugar
                            prompt = $"¿Qué cultivos son recomendables para sembrar en {ciudad} considerando que el clima actual es '{descripcionClima}'? Explica por qué.";
                        }
                        else
                        {
                            // Dar consejos de cuidado para el cultivo y clima
                            prompt = $"Dame consejos prácticos para el cuidado del cultivo de '{cultivo}' en {ciudad}, considerando que el clima actual es '{descripcionClima}'.";
                        }

                        var openAi = new Conexiones.OpenAi();
                        string recomendacion = await openAi.ConsultarAsync(prompt);
                        txtRecomendacion.Text = recomendacion;
                    }
                    else
                    {
                        lblError.Text = "No se encontraron datos para la ciudad ingresada.";
                        LimpiarDatosClima();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                lblError.Text = $"Error al conectar con la API: {ex.Message}";
                LimpiarDatosClima();
            }
            catch (JsonException ex)
            {
                lblError.Text = $"Error al procesar la respuesta de la API: {ex.Message}";
                LimpiarDatosClima();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ocurrió un error inesperado: {ex.Message}";
                LimpiarDatosClima();
            }
        }


        private void LimpiarDatosClima()
        {
            lblLugar.Text = "";
            lblTemperatura.Text = "";
            lblHumedad.Text = "";
            lblViento.Text = "";
            txtRecomendacion.Text = "";
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            // Oculta la ventana actual
            this.Hide();

            // Muestra la ventana de servicios
            var serviciosForm = new Servicios();
            serviciosForm.Show();
        }

        private void Clima_Load(object sender, EventArgs e)
        {

        }
    }

    // Clases para deserializar la respuesta de OpenWeatherMap
    public class WeatherData
    {
        public string Name { get; set; }
        public Sys Sys { get; set; }
        public Main Main { get; set; }
        public Wind Wind { get; set; }
        public Weather[] Weather { get; set; }
    }
    public class Sys { public string Country { get; set; } }
    public class Main { public double temp { get; set; } public int humidity { get; set; } }
    public class Wind { public double speed { get; set; } }
    public class Weather { public string Description { get; set; } }
}
