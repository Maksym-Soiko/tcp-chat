namespace Client
{
    partial class MainForm
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
            txtMessage = new TextBox();
            btnSendToAll = new Button();
            btnSendToUser = new Button();
            btnSendFile = new Button();
            txtMessageHistory = new TextBox();
            label3 = new Label();
            btnViewReceivedFiles = new Button();
            listViewContacts = new ListView();
            contactNameHeader = new ColumnHeader();
            btnAddContact = new Button();
            btnDeleteContact = new Button();
            btnRenameContact = new Button();
            label4 = new Label();
            txtSearchUser = new TextBox();
            btnSearchUser = new Button();
            listViewSearchResult = new ListView();
            columnHeader1 = new ColumnHeader();
            btnBlockUser = new Button();
            btnUnblockUser = new Button();
            SuspendLayout();
            // 
            // txtMessage
            // 
            txtMessage.Font = new Font("Segoe UI", 12F);
            txtMessage.Location = new Point(601, 411);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(682, 39);
            txtMessage.TabIndex = 1;
            txtMessage.Text = "Введіть текст повідомлення...";
            txtMessage.Enter += txtMessage_Enter;
            txtMessage.Leave += txtMessage_Leave;
            // 
            // btnSendToAll
            // 
            btnSendToAll.BackColor = Color.MediumSlateBlue;
            btnSendToAll.FlatStyle = FlatStyle.Flat;
            btnSendToAll.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSendToAll.ForeColor = Color.White;
            btnSendToAll.Location = new Point(601, 484);
            btnSendToAll.Name = "btnSendToAll";
            btnSendToAll.Size = new Size(233, 50);
            btnSendToAll.TabIndex = 3;
            btnSendToAll.Text = "Відправити всім";
            btnSendToAll.UseVisualStyleBackColor = false;
            btnSendToAll.Click += btnSendToAll_Click;
            // 
            // btnSendToUser
            // 
            btnSendToUser.BackColor = Color.MediumSlateBlue;
            btnSendToUser.FlatStyle = FlatStyle.Flat;
            btnSendToUser.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSendToUser.ForeColor = Color.White;
            btnSendToUser.Location = new Point(898, 484);
            btnSendToUser.Name = "btnSendToUser";
            btnSendToUser.Size = new Size(385, 50);
            btnSendToUser.TabIndex = 4;
            btnSendToUser.Text = "Відправити користувачу";
            btnSendToUser.UseVisualStyleBackColor = false;
            btnSendToUser.Click += btnSendToUser_Click;
            // 
            // btnSendFile
            // 
            btnSendFile.BackColor = Color.MediumSlateBlue;
            btnSendFile.FlatStyle = FlatStyle.Flat;
            btnSendFile.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSendFile.ForeColor = Color.White;
            btnSendFile.Location = new Point(601, 554);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(233, 50);
            btnSendFile.TabIndex = 3;
            btnSendFile.Text = "Відправити файл";
            btnSendFile.UseVisualStyleBackColor = false;
            btnSendFile.Click += btnSendFile_Click;
            // 
            // txtMessageHistory
            // 
            txtMessageHistory.Font = new Font("Segoe UI", 12F);
            txtMessageHistory.Location = new Point(601, 68);
            txtMessageHistory.Multiline = true;
            txtMessageHistory.Name = "txtMessageHistory";
            txtMessageHistory.ReadOnly = true;
            txtMessageHistory.ScrollBars = ScrollBars.Vertical;
            txtMessageHistory.Size = new Size(682, 320);
            txtMessageHistory.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(818, 14);
            label3.Name = "label3";
            label3.Size = new Size(258, 32);
            label3.TabIndex = 10;
            label3.Text = "Історія повідомлень";
            // 
            // btnViewReceivedFiles
            // 
            btnViewReceivedFiles.BackColor = Color.MediumSlateBlue;
            btnViewReceivedFiles.FlatStyle = FlatStyle.Flat;
            btnViewReceivedFiles.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnViewReceivedFiles.ForeColor = Color.White;
            btnViewReceivedFiles.Location = new Point(898, 554);
            btnViewReceivedFiles.Name = "btnViewReceivedFiles";
            btnViewReceivedFiles.Size = new Size(385, 50);
            btnViewReceivedFiles.TabIndex = 11;
            btnViewReceivedFiles.Text = "Переглянути отримані файли";
            btnViewReceivedFiles.UseVisualStyleBackColor = false;
            btnViewReceivedFiles.Click += btnViewReceivedFiles_Click;
            // 
            // listViewContacts
            // 
            listViewContacts.Columns.AddRange(new ColumnHeader[] { contactNameHeader });
            listViewContacts.Font = new Font("Segoe UI", 12F);
            listViewContacts.FullRowSelect = true;
            listViewContacts.GridLines = true;
            listViewContacts.Location = new Point(30, 68);
            listViewContacts.Name = "listViewContacts";
            listViewContacts.Size = new Size(377, 320);
            listViewContacts.Sorting = SortOrder.Descending;
            listViewContacts.TabIndex = 12;
            listViewContacts.UseCompatibleStateImageBehavior = false;
            listViewContacts.View = View.Details;
            // 
            // contactNameHeader
            // 
            contactNameHeader.Text = "Contact Name";
            contactNameHeader.Width = 200;
            // 
            // btnAddContact
            // 
            btnAddContact.BackColor = Color.CornflowerBlue;
            btnAddContact.FlatStyle = FlatStyle.Flat;
            btnAddContact.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddContact.ForeColor = Color.White;
            btnAddContact.Location = new Point(1487, 469);
            btnAddContact.Name = "btnAddContact";
            btnAddContact.Size = new Size(377, 49);
            btnAddContact.TabIndex = 13;
            btnAddContact.Text = "Додати контакт";
            btnAddContact.UseVisualStyleBackColor = false;
            btnAddContact.Click += btnAddContact_Click;
            // 
            // btnDeleteContact
            // 
            btnDeleteContact.BackColor = Color.DarkBlue;
            btnDeleteContact.FlatStyle = FlatStyle.Flat;
            btnDeleteContact.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteContact.ForeColor = Color.White;
            btnDeleteContact.Location = new Point(1487, 539);
            btnDeleteContact.Name = "btnDeleteContact";
            btnDeleteContact.Size = new Size(377, 49);
            btnDeleteContact.TabIndex = 14;
            btnDeleteContact.Text = "Видалити контакт";
            btnDeleteContact.UseVisualStyleBackColor = false;
            btnDeleteContact.Click += btnDeleteContact_Click;
            // 
            // btnRenameContact
            // 
            btnRenameContact.BackColor = Color.MediumSlateBlue;
            btnRenameContact.FlatStyle = FlatStyle.Flat;
            btnRenameContact.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRenameContact.ForeColor = Color.White;
            btnRenameContact.Location = new Point(1487, 609);
            btnRenameContact.Name = "btnRenameContact";
            btnRenameContact.Size = new Size(377, 49);
            btnRenameContact.TabIndex = 15;
            btnRenameContact.Text = "Перейменувати контакт";
            btnRenameContact.UseVisualStyleBackColor = false;
            btnRenameContact.Click += btnRenameContact_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(103, 14);
            label4.Name = "label4";
            label4.Size = new Size(218, 32);
            label4.TabIndex = 16;
            label4.Text = "Список контактів";
            // 
            // txtSearchUser
            // 
            txtSearchUser.Font = new Font("Segoe UI", 12F);
            txtSearchUser.Location = new Point(1487, 31);
            txtSearchUser.Name = "txtSearchUser";
            txtSearchUser.Size = new Size(377, 39);
            txtSearchUser.TabIndex = 17;
            txtSearchUser.Text = "Пошук користувача...";
            txtSearchUser.Enter += txtSearchUser_Enter;
            txtSearchUser.Leave += txtSearchUser_Leave;
            // 
            // btnSearchUser
            // 
            btnSearchUser.BackColor = Color.MediumSlateBlue;
            btnSearchUser.FlatStyle = FlatStyle.Flat;
            btnSearchUser.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSearchUser.ForeColor = Color.White;
            btnSearchUser.Location = new Point(1487, 90);
            btnSearchUser.Name = "btnSearchUser";
            btnSearchUser.Size = new Size(377, 50);
            btnSearchUser.TabIndex = 18;
            btnSearchUser.Text = "Пошук";
            btnSearchUser.UseVisualStyleBackColor = false;
            btnSearchUser.Click += btnSearchUser_Click;
            // 
            // listViewSearchResult
            // 
            listViewSearchResult.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            listViewSearchResult.Font = new Font("Segoe UI", 12F);
            listViewSearchResult.Location = new Point(1487, 192);
            listViewSearchResult.Name = "listViewSearchResult";
            listViewSearchResult.Size = new Size(377, 243);
            listViewSearchResult.TabIndex = 19;
            listViewSearchResult.UseCompatibleStateImageBehavior = false;
            listViewSearchResult.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Знайдені користувачі";
            columnHeader1.Width = 250;
            // 
            // btnBlockUser
            // 
            btnBlockUser.BackColor = Color.DarkBlue;
            btnBlockUser.FlatStyle = FlatStyle.Flat;
            btnBlockUser.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBlockUser.ForeColor = Color.White;
            btnBlockUser.Location = new Point(30, 433);
            btnBlockUser.Name = "btnBlockUser";
            btnBlockUser.Size = new Size(377, 50);
            btnBlockUser.TabIndex = 20;
            btnBlockUser.Text = "Заблокувати користувача";
            btnBlockUser.UseVisualStyleBackColor = false;
            btnBlockUser.Click += btnBlockUser_Click;
            // 
            // btnUnblockUser
            // 
            btnUnblockUser.BackColor = Color.CornflowerBlue;
            btnUnblockUser.FlatStyle = FlatStyle.Flat;
            btnUnblockUser.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnUnblockUser.ForeColor = Color.White;
            btnUnblockUser.Location = new Point(30, 506);
            btnUnblockUser.Name = "btnUnblockUser";
            btnUnblockUser.Size = new Size(377, 50);
            btnUnblockUser.TabIndex = 21;
            btnUnblockUser.Text = "Розблокувати користувача";
            btnUnblockUser.UseVisualStyleBackColor = false;
            btnUnblockUser.Click += btnUnblockUser_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(1914, 686);
            Controls.Add(btnUnblockUser);
            Controls.Add(btnBlockUser);
            Controls.Add(listViewSearchResult);
            Controls.Add(btnSearchUser);
            Controls.Add(txtSearchUser);
            Controls.Add(label4);
            Controls.Add(btnRenameContact);
            Controls.Add(btnDeleteContact);
            Controls.Add(btnAddContact);
            Controls.Add(listViewContacts);
            Controls.Add(btnViewReceivedFiles);
            Controls.Add(label3);
            Controls.Add(txtMessageHistory);
            Controls.Add(btnSendFile);
            Controls.Add(btnSendToUser);
            Controls.Add(btnSendToAll);
            Controls.Add(txtMessage);
            Name = "MainForm";
            Text = "Чат - Головна";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtMessage;
        private Button btnSendToAll;
        private Button btnSendToUser;
        private Button btnSendFile;
        private TextBox txtMessageHistory;
        private Label label3;
        private Button btnViewReceivedFiles;
        private ListView listViewContacts;
        private ColumnHeader contactNameHeader;
        private Button btnAddContact;
        private Button btnDeleteContact;
        private Button btnRenameContact;
        private Label label4;
        private TextBox txtSearchUser;
        private Button btnSearchUser;
        private ListView listViewSearchResult;
        private Button btnBlockUser;
        private Button btnUnblockUser;
        private ColumnHeader columnHeader1;
    }
}