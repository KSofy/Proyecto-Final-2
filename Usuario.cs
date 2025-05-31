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
            string contraseña = txtContraseña.Text;
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            if (txtUsuario.Text == "Andy" && txtContraseña.Text == "1234")
            {
                MessageBox.Show("Ingreso exitoso");
                Servicios servicios = new Servicios();
                this.Hide();
                servicios.FormClosed += (s, args) => this.Close();
                servicios.Show();
            }
            else if (txtUsuario.Text == "Sofia" && txtContraseña.Text == "5678")
            {
                MessageBox.Show("Ingreso exitoso");
                Servicios servicios = new Servicios();
                this.Hide();
                servicios.FormClosed += (s, args) => this.Close();
                servicios.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.","Error de Incio de Seciòn", MessageBoxButtons.OK,MessageBoxIcon.Error);
                txtUsuario.Clear();
                txtContraseña.Clear();
                txtUsuario.Focus();
            }

        }
    }
}

