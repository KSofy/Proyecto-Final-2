namespace ServicioAgro_Jutiapa
{
    public partial class Usuario : Form
    {
        public Usuario()
        {
            InitializeComponent();
        }
    
    private void btnIngreso_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contrase�a = txtContrase�a.Text;
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrase�a))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            if (txtUsuario.Text == "Andy" && txtContrase�a.Text == "1234")
            {
                MessageBox.Show("Ingreso exitoso");
                Servicios servicios = new Servicios();
                this.Hide();
                servicios.FormClosed += (s, args) => this.Close();
                servicios.Show();
            }
            else if (txtUsuario.Text == "Sofia" && txtContrase�a.Text == "5678")
            {
                MessageBox.Show("Ingreso exitoso");
                Servicios servicios = new Servicios();
                this.Hide();
                servicios.FormClosed += (s, args) => this.Close();
                servicios.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contrase�a incorrectos.","Error de Incio de Seci�n", MessageBoxButtons.OK,MessageBoxIcon.Error);
                txtUsuario.Clear();
                txtContrase�a.Clear();
                txtUsuario.Focus();
            }

        }
    }
}

