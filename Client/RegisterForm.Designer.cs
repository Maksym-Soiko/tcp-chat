namespace Client
{
    partial class RegisterForm
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
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtConfirmPassword = new TextBox();
            btnRegister = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Font = new Font("Segoe UI", 12F);
            txtUsername.Location = new Point(68, 101);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(668, 39);
            txtUsername.TabIndex = 3;
            txtUsername.Text = "Введіть логін...";
            txtUsername.Enter += txtUsername_Enter;
            txtUsername.Leave += txtUsername_Leave;
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.Location = new Point(68, 174);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(668, 39);
            txtPassword.TabIndex = 4;
            txtPassword.Text = "Введіть пароль...";
            txtPassword.Enter += txtPassword_Enter;
            txtPassword.Leave += txtPassword_Leave;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Font = new Font("Segoe UI", 12F);
            txtConfirmPassword.Location = new Point(68, 246);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(668, 39);
            txtConfirmPassword.TabIndex = 5;
            txtConfirmPassword.Text = "Підтвердіть пароль...";
            txtConfirmPassword.Enter += txtConfirmPassword_Enter;
            txtConfirmPassword.Leave += txtConfirmPassword_Leave;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.CornflowerBlue;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(265, 334);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(250, 50);
            btnRegister.TabIndex = 6;
            btnRegister.Text = "Зареєструватись";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegisterSubmit_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.Location = new Point(280, 39);
            label1.Name = "label1";
            label1.Size = new Size(224, 32);
            label1.TabIndex = 7;
            label1.Text = "Форма реєстрації";
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(btnRegister);
            Controls.Add(txtConfirmPassword);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Name = "RegisterForm";
            Text = "RegisterForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Label label1;
    }
}