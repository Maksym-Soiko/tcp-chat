namespace Client
{
    partial class ChooseForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnRegister = new Button();
            btnLogin = new Button();
            lblChoose = new Label();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // btnRegister
            // 
            btnRegister.Font = new Font("Arial", 10F, FontStyle.Bold);
            btnRegister.Location = new Point(82, 214);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(257, 48);
            btnRegister.TabIndex = 0;
            btnRegister.Text = "Перейти до реєстрації";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.Font = new Font("Arial", 10F, FontStyle.Bold);
            btnLogin.Location = new Point(423, 214);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(257, 48);
            btnLogin.TabIndex = 1;
            btnLogin.Text = "Увійти в акаунт";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblChoose
            // 
            lblChoose.AutoSize = true;
            lblChoose.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblChoose.Location = new Point(313, 115);
            lblChoose.Name = "lblChoose";
            lblChoose.Size = new Size(156, 32);
            lblChoose.TabIndex = 2;
            lblChoose.Text = "Оберіть дію";
            // 
            // button1
            // 
            button1.BackColor = Color.DarkBlue;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button1.ForeColor = Color.White;
            button1.Location = new Point(47, 214);
            button1.Name = "button1";
            button1.Size = new Size(310, 50);
            button1.TabIndex = 0;
            button1.Text = "Перейти до реєстрації";
            button1.UseVisualStyleBackColor = false;
            button1.Click += btnRegister_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.CornflowerBlue;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button2.ForeColor = Color.White;
            button2.Location = new Point(423, 214);
            button2.Name = "button2";
            button2.Size = new Size(310, 50);
            button2.TabIndex = 1;
            button2.Text = "Увійти в акаунт";
            button2.UseVisualStyleBackColor = false;
            button2.Click += btnLogin_Click;
            // 
            // ChooseForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(lblChoose);
            Controls.Add(button2);
            Controls.Add(btnLogin);
            Controls.Add(button1);
            Controls.Add(btnRegister);
            Name = "ChooseForm";
            Text = "ChooseForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRegister;
        private Button btnLogin;
        private Label lblChoose;
        private Button button1;
        private Button button2;
    }
}
