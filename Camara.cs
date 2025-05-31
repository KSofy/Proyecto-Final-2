using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ServicioAgro_Jutiapa
{
    public partial class Camara : Form
    {
        // Configuración de OpenAI
        private const string ApiKey = "";
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string Model = "gpt-4.1";

        // Variables para la cámara
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isCameraActive = false;
        private Bitmap capturedImage;

        public Camara()
        {
            InitializeComponent();
            ConfigureControls();
        }

        private void ConfigureControls()
        {
            btnUpload.Text = "Cargar Imagen";
            btnCapture.Text = "Activar Cámara";
            picImage.SizeMode = PictureBoxSizeMode.Zoom;
            picImage.BorderStyle = BorderStyle.FixedSingle;
            txtResult.Multiline = true;
            txtResult.ScrollBars = ScrollBars.Vertical;

            btnUpload.Click += BtnUpload_Click;
            btnCapture.Click += BtnCapture_Click;
        }

        private async void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picImage.Image = Image.FromFile(openFileDialog.FileName);
                        txtResult.Text = "Analizando imagen...";
                        string analysis = await AnalyzeCropImage(openFileDialog.FileName);
                        txtResult.Text = analysis;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnCapture_Click(object sender, EventArgs e)
        {
            if (!isCameraActive)
            {
                StartCamera();
            }
            else
            {
                CaptureImage();
            }
        }

        private void StartCamera()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No se encontraron cámaras disponibles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                isCameraActive = true;
                btnCapture.Text = "Capturar Foto";
                btnUpload.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar cámara: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Usamos una variable temporal para evitar problemas de concurrencia
                var newFrame = (Bitmap)eventArgs.Frame.Clone();

                // Actualizamos la UI de forma segura
                if (picImage.InvokeRequired)
                {
                    picImage.Invoke(new Action(() =>
                    {
                        if (picImage.Image != null)
                            picImage.Image.Dispose();
                        picImage.Image = newFrame;
                    }));
                }
                else
                {
                    if (picImage.Image != null)
                        picImage.Image.Dispose();
                    picImage.Image = newFrame;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en NewFrame: {ex.Message}");
            }
        }

        private void CaptureImage()
        {
            try
            {
                if (picImage.Image != null)
                {
                    // Detenemos la cámara primero
                    StopCamera();

                    // Guardamos la imagen capturada
                    capturedImage = (Bitmap)picImage.Image.Clone();

                    // Procesamos la imagen en segundo plano
                    Task.Run(async () =>
                    {
                        string tempFile = Path.Combine(Path.GetTempPath(), $"crop_analysis_{DateTime.Now:yyyyMMddHHmmss}.jpg");
                        capturedImage.Save(tempFile, System.Drawing.Imaging.ImageFormat.Jpeg);

                        // Actualizamos la UI desde el hilo principal
                        this.Invoke(new Action(() =>
                        {
                            txtResult.Text = "Analizando imagen...";
                        }));

                        string analysis = await AnalyzeCropImage(tempFile);

                        this.Invoke(new Action(() =>
                        {
                            txtResult.Text = analysis;
                        }));
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al capturar imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopCamera()
        {
            try
            {
                if (videoSource != null && videoSource.IsRunning)
                {
                    // Eliminamos el manejador de eventos primero
                    videoSource.NewFrame -= VideoSource_NewFrame;

                    // Detenemos la cámara
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                    videoSource = null;
                }

                isCameraActive = false;
                btnCapture.Text = "Activar Cámara";
                btnUpload.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al detener cámara: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> AnalyzeCropImage(string imagePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                    var requestBody = new
                    {
                        model = Model,
                        messages = new[]
                        {
                            new
                            {
                                role = "user",
                                content = new object[]
                                {
                                    new
                                    {
                                        type = "text",
                                        text = @"Analiza esta imagen de cultivo como experto agronómico. Proporciona:
1. Nombre de enfermedades detectadas
2. Problemas nutricionales
3. Recomendaciones de tratamiento
4. Medidas preventivas
si no tiene enfermedades dilo"
                                    },
                                    new
                                    {
                                        type = "image_url",
                                        image_url = new { url = $"data:image/jpeg;base64,{base64Image}" }
                                    }
                                }
                            }
                        },
                        max_tokens = 2000
                    };

                    string json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return $"Error en la API: {response.StatusCode}\n{responseContent}";
                    }

                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                    return jsonResponse.choices[0].message.content.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Error al analizar: {ex.Message}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopCamera();

            // Liberamos recursos de imagen
            if (capturedImage != null)
            {
                capturedImage.Dispose();
            }

            if (picImage.Image != null)
            {
                picImage.Image.Dispose();
            }

            base.OnFormClosing(e);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            StopCamera();
            
            this.Hide();

            var serviciosForm = new Servicios();
            serviciosForm.Show();

        }

        private void Camara_Load(object sender, EventArgs e)
        {
     
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
