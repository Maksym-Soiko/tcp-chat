using Microsoft.EntityFrameworkCore;
using Server.Database.Models;
using System.Net.Sockets;
using System.Text;
using Message = Server.Database.Models.Message;

namespace Client
{
    public partial class MainForm : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _clientName;
        private CancellationTokenSource _cancellationTokenSource;


        public MainForm(string login)
        {
            InitializeComponent();
            _clientName = login;
            _cancellationTokenSource = new CancellationTokenSource();
            LoadContacts();
            LoadMessageHistoryAsync();
        }


        private async void LoadContacts()
        {
            try
            {
                using (var context = new ChatContext())
                {
                    var user = await context.Users
                        .Include(u => u.Contacts)
                        .SingleOrDefaultAsync(u => u.Username == _clientName);

                    if (user != null && user.Contacts.Any())
                    {
                        listViewContacts.Items.Clear();

                        foreach (var contact in user.Contacts)
                        {
                            var item = new ListViewItem(contact.ContactName)
                            {
                                Tag = contact.Id
                            };

                            listViewContacts.Items.Add(item);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ваш список контактів порожній.", "Інформація",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні контактів: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task LoadMessageHistoryAsync()
        {
            try
            {
                using (var context = new ChatContext())
                {
                    var user = await context.Users
                        .SingleOrDefaultAsync(u => u.Username == _clientName);

                    if (user == null)
                    {
                        MessageBox.Show("Користувача не знайдено.", "Помилка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    var messages = await context.Messages
                        .Where(m => m.SenderId == user.Id || m.RecipientId == user.Id)
                        .OrderBy(m => m.Timestamp)
                        .Include(m => m.Sender)
                        .Include(m => m.Recipient)
                        .ToListAsync();

                    var files = await context.FileRecords
                        .Where(f => f.SenderId == user.Id || f.RecipientId == user.Id)
                        .OrderBy(f => f.Timestamp)
                        .Include(f => f.Sender)
                        .Include(f => f.Recipient)
                        .ToListAsync();

                    var history = messages
                        .Select(m => new
                        {
                            Timestamp = m.Timestamp,
                            Type = "Message",
                            Content = m.Text,
                            Sender = m.SenderId == user.Id ? "Ви" : m.Sender.Username
                        })
                        .Union(files.Select(f => new
                        {
                            Timestamp = f.Timestamp,
                            Type = "File",
                            Content = $"Файл: {f.FileName}",
                            Sender = f.SenderId == user.Id ? "Ви" : f.Sender.Username
                        }))
                        .OrderBy(h => h.Timestamp);

                    foreach (var item in history)
                    {
                        string entry = $"{item.Timestamp:dd.MM.yyyy HH:mm} {item.Sender}: {item.Content}";
                        txtMessageHistory.AppendText(entry + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження історії: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _client = new TcpClient();

                await _client.ConnectAsync("127.0.0.1", 5000);

                if (!_client.Connected)
                {
                    AppendToMessageHistory("Не вдалося підключитись до сервера.");
                    return;
                }

                _stream = _client.GetStream();

                byte[] buffer = Encoding.UTF8.GetBytes(_clientName);
                await _stream.WriteAsync(buffer, 0, buffer.Length);

                buffer = new byte[1024];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string serverName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                this.Text = $"Client - {serverName}";

                _ = Task.Run(() => ReceiveMessagesAsync(_cancellationTokenSource.Token));
                AppendToMessageHistory("З'єднано з сервером.");
            }
            catch (Exception ex)
            {
                AppendToMessageHistory($"Помилка: {ex.Message}");
            }
        }


        private async void btnSendToAll_Click(object sender, EventArgs e)
        {
            await SendMessageAsync(txtMessage.Text, string.Empty);
        }


        private async void btnSendToUser_Click(object sender, EventArgs e)
        {
            if (listViewContacts.SelectedItems.Count == 0)
            {
                MessageBox.Show("Оберіть контакт із вашого списку.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            string recipientName = listViewContacts.SelectedItems[0].Text;
            string message = txtMessage.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Введіть повідомлення перед відправленням.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            try
            {
                using (var context = new ChatContext())
                {
                    var _sender = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientName);
                    var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == recipientName);

                    if (recipient == null)
                    {
                        MessageBox.Show("Контакт більше не існує.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var existingContact = await context.Contacts
                        .SingleOrDefaultAsync(c => c.UserId == _sender.Id && c.ContactName == recipientName);

                    if (existingContact == null)
                    {
                        MessageBox.Show("Ви не можете відправляти повідомлення цьому користувачу, " +
                            "оскільки він не у вашому списку контактів.",
                            "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes($"/private {recipientName} {message}");
                    await _stream.WriteAsync(buffer, 0, buffer.Length);

                    var newMessage = new Message
                    {
                        Text = message,
                        Timestamp = DateTime.Now,
                        SenderId = _sender.Id,
                        RecipientId = recipient.Id
                    };

                    context.Messages.Add(newMessage);
                    await context.SaveChangesAsync();

                    AppendToMessageHistory($"Ви: {message}");
                    txtMessage.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при відправленні повідомлення: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task SendMessageAsync(string message, string recipient)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (!string.IsNullOrWhiteSpace(recipient))
            {
                message = $"/private{recipient} {message}";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(message);

            try
            {
                await _stream.WriteAsync(buffer, 0, buffer.Length);
                AppendToMessageHistory($"Ви: {message}");


                using (var context = new ChatContext())
                {
                    var sender = context.Users.SingleOrDefault(u => u.Username == _clientName);
                    var receiver = context.Users.SingleOrDefault(u => u.Username == recipient);

                    if (sender != null && receiver != null)
                    {
                        var msg = new Message
                        {
                            Text = message,
                            Timestamp = DateTime.Now,
                            SenderId = sender.Id,
                            RecipientId = receiver.Id
                        };
                        context.Messages.Add(msg);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToMessageHistory($"Помилка: {ex.Message}");
            }
        }


        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            if (listViewContacts.SelectedItems.Count == 0)
            {
                MessageBox.Show("Оберіть контакт із вашого списку.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string recipientName = listViewContacts.SelectedItems[0].Text;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    FileInfo fileInfo = new FileInfo(filePath);

                    try
                    {
                        byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileInfo.Name);
                        byte[] fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);
                        byte[] fileSize = BitConverter.GetBytes(fileInfo.Length);

                        byte[] command = Encoding.UTF8.GetBytes($"/sendfile {recipientName}");
                        await _stream.WriteAsync(command, 0, command.Length);

                        await _stream.WriteAsync(fileSize, 0, fileSize.Length);
                        await _stream.WriteAsync(fileNameLength, 0, fileNameLength.Length);
                        await _stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead;

                            while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await _stream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }

                        using (var context = new ChatContext())
                        {
                            var _sender = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientName);
                            var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == recipientName);

                            if (_sender == null || recipient == null)
                            {
                                MessageBox.Show("Помилка: відправник або отримувач не знайдені у базі даних.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            var fileRecord = new FileRecord
                            {
                                FileName = fileInfo.Name,
                                FileContent = File.ReadAllBytes(fileInfo.FullName),
                                SenderId = _sender.Id,
                                RecipientId = recipient.Id,
                                Timestamp = DateTime.Now
                            };

                            context.FileRecords.Add(fileRecord);
                            await context.SaveChangesAsync();
                        }

                        MessageBox.Show($"Файл '{fileInfo.Name}' успішно надіслано.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при відправленні файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.StartsWith("/sendfile"))
                    {
                        AppendToMessageHistory("Файл отримано.");
                    }
                    else
                    {
                        AppendToMessageHistory($"Сервер: {message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AppendToMessageHistory("Прийом повідомлень зупинено.");
            }
            catch (Exception ex)
            {
                AppendToMessageHistory($"Помилка: {ex.Message}");
            }
        }

        private async Task ReceiveFileFromDatabase(int fileId)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    var fileRecord = await context.FileRecords.FindAsync(fileId);
                    if (fileRecord != null)
                    {
                        string fileExtension = Path.GetExtension(fileRecord.FileName);
                        string savePath = Path.Combine(Environment.GetFolderPath
                            (Environment.SpecialFolder.MyDocuments), $"{fileRecord.FileName}");

                        await File.WriteAllBytesAsync(savePath, fileRecord.FileContent);

                        AppendToMessageHistory
                            ($"Файл '{fileRecord.FileName}' успішно отримано та збережено за адресою {savePath}.");
                    }
                    else
                    {
                        AppendToMessageHistory("Файл не знайдено в базі даних.");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToMessageHistory($"Помилка при завантаженні файлу: {ex.Message}");
            }
        }

        private async void btnViewReceivedFiles_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    var user = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientName);

                    if (user != null)
                    {
                        var files = await context.FileRecords
                            .Where(fr => fr.RecipientId == user.Id)
                            .ToListAsync();

                        if (files.Any())
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Отримані файли:");
                            foreach (var file in files)
                            {
                                sb.AppendLine(file.FileName);
                            }

                            MessageBox.Show(sb.ToString(), "Отримані файли");

                            using (OpenFileDialog openFileDialog = new OpenFileDialog())
                            {
                                openFileDialog.Filter = "All files (*.*)|*.*";

                                string receivedFilesDirectory = Path.Combine
                                    (@"D:\IT STEP\МЕРЕЖЕВЕ ПРОГРАМУВАННЯ\TcpChat\Server\Server\
                                    bin\Debug\net9.0\TransferredFiles",
                                    $"{_clientName}_ReceivedFiles");

                                openFileDialog.InitialDirectory = receivedFilesDirectory;

                                if (openFileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    string selectedFile = openFileDialog.FileName;
                                    await ReceiveFileFromDatabase(int.Parse(selectedFile));
                                }
                            }
                        }
                        else
                        {

                            MessageBox.Show("Немає отриманих файлів.", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Користувача не знайдено в базі даних.", "Помилка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToMessageHistory($"Помилка при отриманні файлів: {ex.Message}");
            }
        }


        private async void btnAddContact_Click(object sender, EventArgs e)
        {
            if (listViewSearchResult.SelectedItems.Count > 0)
            {
                string selectedUserName = listViewSearchResult.SelectedItems[0].Text;
                int selectedUserId = (int)listViewSearchResult.SelectedItems[0].Tag;

                if (selectedUserName == _clientName)
                {
                    MessageBox.Show("Не можна додати себе в контакти.", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    using (var context = new ChatContext())
                    {
                        var user = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientName);
                        var contactUser = await context.Users.SingleOrDefaultAsync(u => u.Id == selectedUserId);

                        if (contactUser != null)
                        {
                            var existingContact = await context.Contacts
                                .SingleOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == contactUser.Username);

                            if (existingContact == null)
                            {
                                var contact = new Contact
                                {
                                    UserId = user.Id,
                                    ContactName = contactUser.Username
                                };

                                context.Contacts.Add(contact);
                                await context.SaveChangesAsync();

                                ListViewItem item = new ListViewItem(contactUser.Username)
                                {
                                    Tag = contactUser.Id
                                };
                                listViewContacts.Items.Add(item);

                                MessageBox.Show("Контакт успішно додано.", "Інформація",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Такий контакт вже існує.", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Користувача не знайдено.", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при додаванні контакту: {ex.Message}", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Виберіть користувача для додавання в контакти.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private async void btnDeleteContact_Click(object sender, EventArgs e)
        {
            if (listViewContacts.SelectedItems.Count > 0)
            {
                string contactName = listViewContacts.SelectedItems[0].Text;

                DialogResult result = MessageBox.Show($"Ви дійсно хочете видалити контакт {contactName}?",
                    "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new ChatContext())
                        {
                            var user = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientName);

                            var contact = await context.Contacts
                                .SingleOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == contactName);

                            if (contact != null)
                            {
                                context.Contacts.Remove(contact);
                                await context.SaveChangesAsync();

                                listViewContacts.Items.Remove(listViewContacts.SelectedItems[0]);

                                MessageBox.Show("Контакт успішно видалено.", "Інформація",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Контакт не знайдено.", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при видаленні контакту: {ex.Message}", "Помилка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберіть контакт для видалення.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private async void btnRenameContact_Click(object sender, EventArgs e)
        {
            if (listViewContacts.SelectedItems.Count > 0)
            {
                string oldContactName = listViewContacts.SelectedItems[0].Text;
                string newContactName = Microsoft.VisualBasic.Interaction
                    .InputBox("Введіть нове ім'я контакту:", "Перейменувати контакт");

                if (!string.IsNullOrWhiteSpace(newContactName) && oldContactName != newContactName)
                {
                    try
                    {
                        using (var context = new ChatContext())
                        {
                            var user = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientName);

                            if (newContactName == _clientName)
                            {
                                MessageBox.Show("Не можна перейменувати контакт на себе.", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            var existingContact = await context.Contacts
                                .SingleOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == oldContactName);

                            if (existingContact != null)
                            {
                                var duplicateContact = await context.Contacts
                                    .SingleOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == newContactName);

                                if (duplicateContact != null)
                                {
                                    MessageBox.Show("Контакт з таким ім'ям вже існує.", "Помилка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                existingContact.ContactName = newContactName;
                                existingContact.Timestamp = DateTime.Now;

                                context.Contacts.Update(existingContact);
                                await context.SaveChangesAsync();

                                listViewContacts.SelectedItems[0].Text = newContactName;

                                UpdateMessageHistory(oldContactName, newContactName);

                                MessageBox.Show("Контакт успішно перейменовано.", "Інформація",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Контакт не знайдено.", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при перейменуванні контакту: {ex.Message}", "Помилка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберіть контакт для перейменування.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void UpdateMessageHistory(string oldContactName, string newContactName)
        {
            string historyText = txtMessageHistory.Text;

            string updatedHistoryText = historyText.Replace(
                $"{oldContactName}:",
                $"{newContactName}:");

            txtMessageHistory.Text = updatedHistoryText;

            txtMessageHistory.SelectionStart = txtMessageHistory.Text.Length;
            txtMessageHistory.ScrollToCaret();
        }


        private async void btnSearchUser_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearchUser.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                MessageBox.Show("Введіть ім'я користувача для пошуку.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            listViewSearchResult.Items.Clear();

            try
            {
                using (var context = new ChatContext())
                {
                    var users = await context.Users
                        .Where(u => u.Username.Contains(searchQuery))
                        .ToListAsync();

                    if (users.Any())
                    {
                        foreach (var user in users)
                        {
                            ListViewItem item = new ListViewItem(user.Username);
                            item.Tag = user.Id;
                            listViewSearchResult.Items.Add(item);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Користувачів не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при пошуку користувача: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void btnBlockUser_Click(object sender, EventArgs e)
        {
            if (listViewContacts.SelectedItems.Count > 0)
            {
                var selectedItem = listViewContacts.SelectedItems[0];

                if (selectedItem.Tag == null || !int.TryParse(selectedItem.Tag.ToString(), out int selectedUserId))
                {
                    MessageBox.Show("Не вдалося отримати ідентифікатор контакту. Спробуйте перезавантажити список контактів.",
                        "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await SendCommandAsync($"/block {selectedUserId}");
                MessageBox.Show("Користувач успішно заблокований.", "Інформація",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Виберіть контакт для блокування.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private async void btnUnblockUser_Click(object sender, EventArgs e)
        {
            if (listViewContacts.SelectedItems.Count > 0)
            {
                var selectedItem = listViewContacts.SelectedItems[0];

                if (selectedItem.Tag == null || !int.TryParse(selectedItem.Tag.ToString(), out int selectedUserId))
                {
                    MessageBox.Show("Не вдалося отримати ідентифікатор контакту. Спробуйте перезавантажити список контактів.",
                        "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await SendCommandAsync($"/unblock {selectedUserId}");
                MessageBox.Show("Користувач успішно розблокований.", "Інформація",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Виберіть контакт для розблокування.", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private async Task SendCommandAsync(string command)
        {
            try
            {
                if (_stream != null && _client.Connected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(command + Environment.NewLine);
                    await _stream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    MessageBox.Show("Відсутнє з'єднання з сервером.", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при відправленні команди: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtMessage_Enter(object sender, EventArgs e)
        {
            if (txtMessage.Text == "Введіть текст повідомлення...")
            {
                txtMessage.Text = string.Empty;
                txtMessage.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtMessage_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtMessage.Text = "Введіть текст повідомлення...";
                txtMessage.ForeColor = SystemColors.GrayText;
            }
        }


        private void txtSearchUser_Enter(object sender, EventArgs e)
        {
            if (txtSearchUser.Text == "Пошук користувача...")
            {
                txtSearchUser.Text = string.Empty;
                txtSearchUser.ForeColor = SystemColors.WindowText;
            }
        }


        private void txtSearchUser_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchUser.Text))
            {
                txtSearchUser.Text = "Пошук користувача...";
                txtSearchUser.ForeColor = SystemColors.GrayText;
            }
        }


        private void AppendToMessageHistory(string message)
        {
            txtMessageHistory.AppendText(message + Environment.NewLine);
            txtMessageHistory.ScrollToCaret();
        }


        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                if (_stream != null)
                {
                    await _stream.FlushAsync();
                    _stream.Close();
                    _stream = null;
                }

                if (_client != null)
                {
                    _client.Close();
                    _client = null;
                }
            }
            catch (Exception ex)
            {
                AppendToMessageHistory($"Помилка закриття: {ex.Message}");
            }
            finally
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                base.OnFormClosing(e);
            }
        }
    }
}
