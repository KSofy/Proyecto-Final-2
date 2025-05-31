using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicioAgro_Jutiapa
{
    public partial class Servicios : Form
    {
        public Servicios()
        {
            InitializeComponent();

        }
        private void BtnCamara_Click(object sender, EventArgs e)
        {

            this.Hide();

            Camara camaraForm = new Camara();
            camaraForm.ShowDialog();

        }

        private void BtnIA_Click(object sender, EventArgs e)
        {

            this.Hide();

            PreguntasEHistorial iAForm = new PreguntasEHistorial();
            iAForm.ShowDialog();

        }

        private void BtnClima_Click(object sender, EventArgs e)
        {

            this.Hide();

            var climaForm = new Clima();
            climaForm.ShowDialog();

        }

        private void Servicios_Load(object sender, EventArgs e)
        {

        }

        private void btnCalendario_Click(object sender, EventArgs e)
        {
           
        }
    }

}
