namespace Client
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLoginSubmit_Click(object sender, EventArgs e)
        {
            string username = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Всі поля мають бути заповнені.", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            using (var context = new ChatContext())
            {
                var user = context.Users.SingleOrDefault(u => u.Username == username);

                if (user != null)
                {
                    if (user.Password == password)
                    {
                        MessageBox.Show("Вхід успішний!", "Успіх", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        MainForm mainForm = new MainForm(username);
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Невірний пароль.", "Помилка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Користувача з таким логіном не знайдено.", "Помилка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRegisterRedirect_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Hide();
        }


        private void txtLogin_Enter(object sender, EventArgs e)
        {
            if (txtLogin.Text == "Введіть логін...")
            {
                txtLogin.Text = string.Empty;
                txtLogin.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtLogin_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                txtLogin.Text = "Введіть логін...";
                txtLogin.ForeColor = SystemColors.GrayText;
            }
        }


        private void txtPassword_Enter(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;

            if (txtPassword.Text == "Введіть пароль...")
            {
                txtPassword.Text = string.Empty;
                txtPassword.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.Text = "Введіть пароль...";
                txtPassword.ForeColor = SystemColors.GrayText;
            }
        }


        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
