using Server.Database.Models;

namespace Client
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }


        private void btnRegisterSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password) || 
                string.IsNullOrEmpty(confirmPassword))

            {
                MessageBox.Show("Всі поля мають бути заповнені.", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            using (var context = new ChatContext())
            {
                if (context.Users.Any(u => u.Username == username))
                {
                    MessageBox.Show("Користувач із таким логіном вже існує.", "Помилка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Паролі не співпадають.", "Помилка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                var user = new User
                {
                    Username = username,
                    Password = password
                };

                context.Users.Add(user);
                context.SaveChanges();

                MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MainForm mainForm = new MainForm(username);
                mainForm.Show();
                this.Hide();
            }
        }


        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Введіть логін...")
            {
                txtUsername.Text = string.Empty;
                txtUsername.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Введіть логін...";
                txtUsername.ForeColor = SystemColors.GrayText;
            }
        }


        private void txtPassword_Enter(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;

            if (txtPassword.Text == "Введіть пароль...")
            {
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.Text = string.Empty;
                txtPassword.ForeColor = SystemColors.WindowText;
            }
        }


        private void txtPassword_Leave(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.Text = "Введіть пароль...";
                txtPassword.ForeColor = SystemColors.GrayText;
            }
        }

        private void txtConfirmPassword_Enter(object sender, EventArgs e)
        {
            txtConfirmPassword.UseSystemPasswordChar = true;

            if (txtConfirmPassword.Text == "Підтвердіть пароль...")
            {
                txtConfirmPassword.Text = string.Empty;
                txtConfirmPassword.ForeColor = SystemColors.WindowText;
            }
        }


        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtConfirmPassword.UseSystemPasswordChar = false;
                txtConfirmPassword.Text = "Підтвердіть пароль...";
                txtConfirmPassword.ForeColor = SystemColors.GrayText;
            }
        }


        private void RegisterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
