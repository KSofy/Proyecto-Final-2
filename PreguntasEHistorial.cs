using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServicioAgro_Jutiapa.Conexiones;
using System.Data.SqlClient;

namespace ServicioAgro_Jutiapa
{
    public partial class PreguntasEHistorial : Form
    {
        public PreguntasEHistorial()
        {
            InitializeComponent();
        }
        private async void btnInvestigar_Click(object sender, EventArgs e)
        {
            string prompt = txtPrompt.Text.Trim();
            if (string.IsNullOrEmpty(prompt))
            {
                richRespuesta.Text = "Por favor, escribe una pregunta.";
                return;
            }

            // Llama a OpenAI
            var openAi = new OpenAi();
            string resultado = await openAi.ConsultarAsync(prompt);
            richRespuesta.Text = resultado;


            var db = new BaseDeDatos();
            db.Guardar(prompt, resultado);
        }



        private void btnBaseDatos_Click(object sender, EventArgs e)
        {
            var db = new BaseDeDatos();
            var historial = db.ObtenerHistorial();

            var sb = new StringBuilder();
            foreach (var item in historial)
            {
                sb.AppendLine($"Fecha: {item.Fecha}");
                sb.AppendLine($"Pregunta: {item.Prompt}");
                sb.AppendLine($"Respuesta: {item.Resultado}");
                sb.AppendLine(new string('-', 40));
            }
            richRespuesta.Text = sb.ToString();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            // Oculta la ventana actual
            this.Hide();

            // Muestra la ventana de servicios
            var serviciosForm = new Servicios();
            serviciosForm.Show();
        }

        private void PreguntasEHistorial_Load(object sender, EventArgs e)
        {

        }
    }
}
