namespace Client
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtLogin = new TextBox();
            txtPassword = new TextBox();
            btnLoginSubmit = new Button();
            button1 = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // txtLogin
            // 
            txtLogin.Font = new Font("Segoe UI", 12F);
            txtLogin.Location = new Point(77, 119);
            txtLogin.Name = "txtLogin";
            txtLogin.Size = new Size(668, 39);
            txtLogin.TabIndex = 4;
            txtLogin.Text = "Введіть логін...";
            txtLogin.Enter += txtLogin_Enter;
            txtLogin.Leave += txtLogin_Leave;
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.Location = new Point(77, 195);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(668, 39);
            txtPassword.TabIndex = 5;
            txtPassword.Text = "Введіть пароль...";
            txtPassword.Enter += txtPassword_Enter;
            txtPassword.Leave += txtPassword_Leave;
            // 
            // btnLoginSubmit
            // 
            btnLoginSubmit.BackColor = Color.CornflowerBlue;
            btnLoginSubmit.FlatStyle = FlatStyle.Flat;
            btnLoginSubmit.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoginSubmit.ForeColor = Color.White;
            btnLoginSubmit.Location = new Point(77, 291);
            btnLoginSubmit.Name = "btnLoginSubmit";
            btnLoginSubmit.Size = new Size(250, 50);
            btnLoginSubmit.TabIndex = 7;
            btnLoginSubmit.Text = "Увійти в акаунт";
            btnLoginSubmit.UseVisualStyleBackColor = false;
            btnLoginSubmit.Click += btnLoginSubmit_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.DarkBlue;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(363, 291);
            button1.Name = "button1";
            button1.Size = new Size(382, 50);
            button1.TabIndex = 8;
            button1.Text = "Перейти на форму реєстрації";
            button1.UseVisualStyleBackColor = false;
            button1.Click += btnRegisterRedirect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.Location = new Point(267, 42);
            label1.Name = "label1";
            label1.Size = new Size(279, 32);
            label1.TabIndex = 9;
            label1.Text = "Форма входу в акаунт";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(btnLoginSubmit);
            Controls.Add(txtPassword);
            Controls.Add(txtLogin);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLoginSubmit;
        private Button button1;
        private Label label1;
    }
}