using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Server.Database.Models;

namespace ChatServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Сервер запускається...");
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 5000);
            await server.StartAsync();
        }
    }

    public class Server
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly ConcurrentDictionary<TcpClient, NetworkStream> _clients = new();
        private readonly ConcurrentDictionary<TcpClient, string> _clientNames = new();
        private readonly string _fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TransferredFiles");


        public Server(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            if (!Directory.Exists(_fileDirectory))
            {
                Directory.CreateDirectory(_fileDirectory);
            }
        }


        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(_ipAddress, _port);
            listener.Start();
            Console.WriteLine($"Сервер запущено за адресою {_ipAddress}:{_port}");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }


        private async Task HandleClientAsync(TcpClient client)
        {
            Console.WriteLine($"Клієнт підключений: {client.Client.RemoteEndPoint}");
            NetworkStream stream = client.GetStream();
            _clients[client] = stream;

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                _clientNames[client] = clientName;

                Console.WriteLine($"Клієнт зареєстрований: {clientName}");
                await SendMessageAsync(stream, $"Ласкаво просимо, {clientName}!");
                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine($"{clientName}: {clientMessage}");

                    if (clientMessage.StartsWith("/private"))
                    {
                        await HandlePrivateMessage(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/sendfile"))
                    {
                        await HandleFileTransfer(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/viewreceivedfiles"))
                    {
                        await HandleViewReceivedFiles(client);
                    }

                    else if (clientMessage.StartsWith("/friendrequest"))
                    {
                        await HandleSendFriendRequestAsync(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/friendresponse"))
                    {
                        await HandleFriendRequestResponseAsync(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/contact"))
                    {
                        await HandleContactManagementAsync(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/viewcontacts"))
                    {
                        await HandleViewContactsAsync(client);
                    }

                    else if (clientMessage.StartsWith("/block"))
                    {
                        await HandleBlockUserAsync(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/unblock"))
                    {
                        await HandleUnblockUserAsync(client, clientMessage);
                    }

                    else if (clientMessage.StartsWith("/getmessagehistory"))
                    {
                        await HandleGetMessageHistoryAsync(clientName, stream);
                    }

                    else
                    {
                        string broadcastMessage = $"{DateTime.Now:dd.MM.yyyy HH:mm} " +
                            $"{clientName}: {clientMessage.ToUpper()}";
                        BroadcastMessage(broadcastMessage, client);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка з клієнтом {client.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                _clients.TryRemove(client, out _);
                _clientNames.TryRemove(client, out _);
                client.Close();
                Console.WriteLine($"Клієнта від'єднано.");
            }
        }


        private async Task HandlePrivateMessage(TcpClient client, string clientMessage)
        {
            var parts = clientMessage.Split(' ', 3);

            using var context = new ChatContext();

            var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == parts[1]);
            var sender = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientNames[client]);

            if (await IsBlockedAsync(sender.Id, recipient.Id))
            {
                await SendMessageAsync(client.GetStream(), "/error|Вас заблоковано.");
                return;
            }

            if (parts.Length >= 3)
            {
                string recipientName = parts[1];
                string message = parts[2];

                var recipientClient = _clientNames.FirstOrDefault(x => x.Value == recipientName).Key;
                if (recipientClient != null)
                {
                    byte[] privateMessageBuffer = Encoding.UTF8
                        .GetBytes($"Приватне від {_clientNames[client]}: {message}");
                    var recipientStream = _clients[recipientClient];
                    await recipientStream.WriteAsync(privateMessageBuffer, 0, privateMessageBuffer.Length);
                }
                else
                {
                    string errorMessage = $"Користувача {recipientName} не знайдено.";
                    await SendMessageAsync(_clients[client], errorMessage);
                }
            }
        }


        private async Task HandleFileTransfer(TcpClient client, string clientMessage)
        {
            var parts = clientMessage.Split(' ', 2);
            if (parts.Length != 2)
            {
                await SendMessageAsync(_clients[client], "Некоректна команда. Використовуйте /sendfile <користувач>.");
                return;
            }

            string recipientName = parts[1];

            using (var context = new ChatContext())
            {
                var sender = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientNames[client]);
                var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == recipientName);

                if (sender == null || recipient == null)
                {
                    await SendMessageAsync(_clients[client], "Користувача не знайдено.");
                    return;
                }

                bool isBlocked = await context.BlockedUsers.AnyAsync(b =>
                    b.BlockerId == recipient.Id && b.BlockedId == sender.Id);

                if (isBlocked)
                {
                    await SendMessageAsync(_clients[client], "Ви не можете надіслати файл, оскільки вас заблоковано.");
                    return;
                }
            }

            await SendMessageAsync(_clients[client], "Готуйтеся до передачі файлу.");
            await ReceiveFileAsync(client, recipientName);
        }



        private async Task HandleViewReceivedFiles(TcpClient client)
        {
            string clientName = _clientNames[client];

            string userDirectory = Path.Combine(_fileDirectory, $"{clientName}_ReceivedFiles");

            if (Directory.Exists(userDirectory))
            {
                var files = Directory.GetFiles(userDirectory);
                if (files.Length == 0)
                {
                    await SendMessageAsync(client.GetStream(), "Немає отриманих файлів.");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Отримані файли:");
                    foreach (var file in files)
                    {
                        sb.AppendLine(Path.GetFileName(file));
                    }
                    await SendMessageAsync(client.GetStream(), sb.ToString());
                }
            }
            else
            {
                await SendMessageAsync(client.GetStream(), "Директорія не існує.");
            }
        }


        private async Task HandleSendFriendRequestAsync(TcpClient client, string clientMessage)
        {
            var parts = clientMessage.Split(' ', 2);
            if (parts.Length == 2)
            {
                string recipientName = parts[1];

                using (var context = new ChatContext())
                {
                    var sender = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientNames[client]);
                    var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == recipientName);

                    if (sender != null && recipient != null)
                    {
                        var existingContact = await context.Contacts
                            .FirstOrDefaultAsync(c => c.UserId == recipient.Id && c.ContactName == sender.Username);

                        if (existingContact == null)
                        {
                            var recipientClient = _clientNames.FirstOrDefault(x => x.Value == recipientName).Key;
                            if (recipientClient != null)
                            {
                                string message = $"{sender.Username} хоче додати вас у контакти!";
                                await SendMessageAsync(_clients[recipientClient], message);
                            }

                            await SendMessageAsync(client.GetStream(),
                                "Запит на додавання в контакти успішно відправлений.");
                        }
                        else
                        {
                            await SendMessageAsync(client.GetStream(), "Цей контакт вже є у вас у списку.");
                        }
                    }
                    else
                    {
                        await SendMessageAsync(client.GetStream(), "Користувача не знайдено.");
                    }
                }
            }
        }


        private async Task HandleFriendRequestResponseAsync(TcpClient client, string clientMessage)
        {
            var parts = clientMessage.Split(' ', 3);
            if (parts.Length == 3 && parts[0].ToLower() == "/friendresponse")
            {
                string response = parts[1].ToLower();
                string senderUsername = parts[2];

                using (var context = new ChatContext())
                {
                    var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientNames[client]);
                    var sender = await context.Users.SingleOrDefaultAsync(u => u.Username == senderUsername);

                    if (recipient != null && sender != null)
                    {
                        if (response == "accept")
                        {
                            var contact = new Contact
                            {
                                UserId = recipient.Id,
                                ContactName = sender.Username
                            };
                            context.Contacts.Add(contact);

                            var reverseContact = new Contact
                            {
                                UserId = sender.Id,
                                ContactName = recipient.Username
                            };
                            context.Contacts.Add(reverseContact);
                        }

                        await context.SaveChangesAsync();

                        string statusMessage = response == "accept" ? "Контакт додано." : "Запит відхилено.";
                        await SendMessageAsync(client.GetStream(), statusMessage);

                        var senderClient = _clientNames.FirstOrDefault(x => x.Value == senderUsername).Key;
                        if (senderClient != null)
                        {
                            string message = $"Ваш запит на додавання в контакти до {recipient.Username} " +
                                $"був {statusMessage}.";

                            await SendMessageAsync(_clients[senderClient], message);
                        }
                    }
                    else
                    {
                        await SendMessageAsync(client.GetStream(), "Користувача не знайдено.");
                    }
                }
            }
        }


        private async Task HandleContactManagementAsync(TcpClient client, string contactMessage)
        {
            var parts = contactMessage.Split(' ', 3);
            if (parts.Length >= 2)
            {
                string action = parts[0];
                string contactName = parts[1];

                using (var context = new ChatContext())
                {
                    var user = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientNames[client]);

                    if (user != null)
                    {
                        if (action == "/addcontact")
                        {
                            if (contactName == user.Username)
                            {
                                await SendMessageAsync(client.GetStream(), "Не можна додати себе в контакти.");
                                return;
                            }

                            var existingContact = await context.Contacts
                                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == contactName);

                            if (existingContact != null)
                            {
                                await SendMessageAsync(client.GetStream(), "Такий контакт вже існує.");
                            }
                            else
                            {
                                var contact = new Contact
                                {
                                    UserId = user.Id,
                                    ContactName = contactName,
                                    Timestamp = DateTime.Now
                                };

                                context.Contacts.Add(contact);
                                await context.SaveChangesAsync();

                                var contactForOtherUser = new Contact
                                {
                                    UserId = existingContact.UserId,
                                    ContactName = user.Username,
                                    Timestamp = DateTime.Now
                                };
                                context.Contacts.Add(contactForOtherUser);
                                await context.SaveChangesAsync();

                                await SendMessageAsync(client.GetStream(), $"Контакт {contactName} додано.");
                            }
                        }
                        else if (action == "/removecontact")
                        {
                            var contact = await context.Contacts.SingleOrDefaultAsync(c => c.UserId == user.Id &&
                            c.ContactName == contactName);

                            if (contact != null)
                            {
                                context.Contacts.Remove(contact);
                                await context.SaveChangesAsync();
                                await SendMessageAsync(client.GetStream(), $"Контакт {contactName} видалено.");
                            }
                            else
                            {
                                await SendMessageAsync(client.GetStream(), "Контакт не знайдено.");
                            }
                        }
                        else if (action == "/renamecontact" && parts.Length == 3)
                        {
                            string newName = parts[2];

                            if (newName == user.Username)
                            {
                                await SendMessageAsync(client.GetStream(), "Не можна перейменувати контакт на себе.");
                                return;
                            }

                            var existingContact = await context.Contacts
                                .SingleOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == contactName);

                            if (existingContact != null)
                            {
                                var duplicateContact = await context.Contacts
                                    .SingleOrDefaultAsync(c => c.UserId == user.Id && c.ContactName == newName);

                                if (duplicateContact != null)
                                {
                                    await SendMessageAsync(client.GetStream(), "Контакт з таким ім'ям вже існує.");
                                    return;
                                }

                                existingContact.ContactName = newName;
                                existingContact.Timestamp = DateTime.Now;
                                await context.SaveChangesAsync();

                                await SendMessageAsync(client.GetStream(), $"Контакт {contactName}" +
                                    $" перейменовано на {newName}.");
                            }
                            else
                            {
                                await SendMessageAsync(client.GetStream(), "Контакт не знайдено.");
                            }
                        }
                    }
                }
            }
        }


        private async Task HandleViewContactsAsync(TcpClient client)
        {
            string username = _clientNames[client];

            using (var context = new ChatContext())
            {
                var user = await context.Users.Include(u => u.Contacts)
                                              .SingleOrDefaultAsync(u => u.Username == username);

                if (user != null && user.Contacts.Any())
                {
                    StringBuilder contactsList = new StringBuilder();
                    contactsList.AppendLine("Ваш список контактів:");
                    foreach (var contact in user.Contacts)
                    {
                        contactsList.AppendLine(contact.ContactName);
                    }

                    await SendMessageAsync(client.GetStream(), contactsList.ToString());
                }
                else
                {
                    await SendMessageAsync(client.GetStream(), "Ваш список контактів порожній.");
                }
            }
        }


        private async Task HandleGetMessageHistoryAsync(string username, NetworkStream stream)
        {
            using (var context = new ChatContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    await SendMessageAsync(stream, "/messages|");
                    return;
                }

                var messages = await context.Messages
                    .Where(m => (m.SenderId == user.Id || m.RecipientId == user.Id) &&
                                !context.BlockedUsers.Any(b => b.BlockerId == user.Id && b.BlockedId == m.SenderId))
                    .OrderBy(m => m.Timestamp)
                    .Include(m => m.Sender)
                    .Include(m => m.Recipient)
                    .ToListAsync();

                var files = await context.FileRecords
                    .Where(f => (f.SenderId == user.Id || f.RecipientId == user.Id) &&
                                !context.BlockedUsers.Any(b => b.BlockerId == user.Id && b.BlockedId == f.SenderId))
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

                string historyText = string.Join('|', history.Select(h =>
                    $"{h.Timestamp:dd.MM.yyyy HH:mm} {h.Sender}: {h.Content}"));

                await SendMessageAsync(stream, $"/messages|{historyText}");
            }
        }



        private async Task<bool> IsBlockedAsync(int senderId, int recipientId)
        {
            using (var context = new ChatContext())
            {
                return await context.BlockedUsers
                    .AnyAsync(b => b.BlockerId == recipientId && b.BlockedId == senderId);
            }
        }


        private async Task HandleBlockUserAsync(TcpClient client, string clientMessage)
        {
            var parts = clientMessage.Split(' ', 2);
            if (parts.Length < 2 || !int.TryParse(parts[1], out int blockedUserId))
            {
                await SendMessageAsync(client.GetStream(), "/error|Некоректна команда.");
                return;
            }

            string blockerUserName = _clientNames[client];

            using (var context = new ChatContext())
            {
                var blocker = await context.Users.SingleOrDefaultAsync(u => u.Username == blockerUserName);
                var blocked = await context.Users.SingleOrDefaultAsync(u => u.Id == blockedUserId);

                if (blocker == null || blocked == null)
                {
                    await SendMessageAsync(client.GetStream(), "/error|Користувача не знайдено.");
                    return;
                }

                if (await context.BlockedUsers.AnyAsync(b => b.BlockerId == blocker.Id && b.BlockedId == blocked.Id))
                {
                    await SendMessageAsync(client.GetStream(), "/error|Користувач вже заблокований.");
                    return;
                }

                var blockedUser = new BlockedUser
                {
                    BlockerId = blocker.Id,
                    BlockedId = blocked.Id,
                    Timestamp = DateTime.Now
                };

                context.BlockedUsers.Add(blockedUser);
                await context.SaveChangesAsync();
                await SendMessageAsync(client.GetStream(), "/info|Користувач успішно заблокований.");
            }
        }


        private async Task HandleUnblockUserAsync(TcpClient client, string clientMessage)
        {
            var parts = clientMessage.Split(' ', 2);
            if (parts.Length < 2 || !int.TryParse(parts[1], out int blockedUserId))
            {
                await SendMessageAsync(client.GetStream(), "/error|Некоректна команда.");
                return;
            }

            string blockerUserName = _clientNames[client];

            using (var context = new ChatContext())
            {
                var blocker = await context.Users.SingleOrDefaultAsync(u => u.Username == blockerUserName);
                var blocked = await context.Users.SingleOrDefaultAsync(u => u.Id == blockedUserId);

                if (blocker == null || blocked == null)
                {
                    await SendMessageAsync(client.GetStream(), "/error|Користувача не знайдено.");
                    return;
                }

                var blockedUser = await context.BlockedUsers
                    .SingleOrDefaultAsync(b => b.BlockerId == blocker.Id && b.BlockedId == blocked.Id);

                if (blockedUser == null)
                {
                    await SendMessageAsync(client.GetStream(), "/error|Користувач не заблокований.");
                    return;
                }

                context.BlockedUsers.Remove(blockedUser);
                await context.SaveChangesAsync();
                await SendMessageAsync(client.GetStream(), "/info|Користувач успішно розблокований.");
            }
        }


        public static async Task<string> ReadFileHeaderAsync(NetworkStream stream, int headerSize = 1024)
        {
            try
            {
                byte[] buffer = new byte[headerSize];

                int bytesRead = await stream.ReadAsync(buffer, 0, headerSize);

                Array.Resize(ref buffer, bytesRead);

                return BitConverter.ToString(buffer).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні заголовку файлу: {ex.Message}");
                throw;
            }
        }


        private async Task ReceiveFileAsync(TcpClient client, string recipientName)
        {
            NetworkStream stream = _clients[client];
            string userDirectory = Path.Combine(_fileDirectory, $"{recipientName}_ReceivedFiles");

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            try
            {
                using (var context = new ChatContext())
                {
                    var sender = await context.Users.SingleOrDefaultAsync(u => u.Username == _clientNames[client]);
                    var recipient = await context.Users.SingleOrDefaultAsync(u => u.Username == recipientName);

                    if (sender != null && recipient != null)
                    {
                        var isBlocked = await context.BlockedUsers
                            .AnyAsync(b => b.BlockerId == recipient.Id && b.BlockedId == sender.Id);

                        if (isBlocked)
                        {
                            await SendMessageAsync(stream, "Ви не можете надіслати файл цьому користувачу, оскільки він вас заблокував.");
                            return;
                        }
                    }

                    byte[] sizeBuffer = new byte[sizeof(long)];
                    await stream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length);
                    long fileSize = BitConverter.ToInt64(sizeBuffer, 0);

                    byte[] nameLengthBuffer = new byte[sizeof(int)];
                    await stream.ReadAsync(nameLengthBuffer, 0, nameLengthBuffer.Length);
                    int nameLength = BitConverter.ToInt32(nameLengthBuffer, 0);

                    byte[] nameBuffer = new byte[nameLength];
                    await stream.ReadAsync(nameBuffer, 0, nameBuffer.Length);
                    string fileName = Encoding.UTF8.GetString(nameBuffer);

                    string filePath = Path.Combine(userDirectory, fileName);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[1024];
                        long remainingBytes = fileSize;

                        while (remainingBytes > 0)
                        {
                            int bytesToRead = (int)Math.Min(buffer.Length, remainingBytes);
                            int bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);

                            if (bytesRead == 0)
                                throw new Exception("З'єднання перервано під час передачі файлу.");

                            await fs.WriteAsync(buffer, 0, bytesRead);
                            remainingBytes -= bytesRead;
                        }
                    }

                    Console.WriteLine($"Файл отримано та збережено: {filePath}");

                    var recipientClient = _clientNames.FirstOrDefault(x => x.Value == recipientName).Key;
                    if (recipientClient != null)
                    {
                        string fileReceivedMessage = $"Ви отримали файл від {_clientNames[client]}: {fileName}";
                        await SendMessageAsync(_clients[recipientClient], fileReceivedMessage);
                    }

                    string confirmationMessage = $"Файл успішно надіслано {recipientName}.";
                    await SendMessageAsync(stream, confirmationMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні файлу: {ex.Message}");
                await SendMessageAsync(stream, "Виникла помилка під час надсилання файлу.");
            }
        }



        private async Task SendMessageAsync(NetworkStream stream, string message)
        {
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageBuffer, 0, messageBuffer.Length);
        }


        private void BroadcastMessage(string message, TcpClient senderClient)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(message);

            foreach (var kvp in _clients)
            {
                var client = kvp.Key;
                var stream = kvp.Value;

                if (client == senderClient) continue;

                try
                {
                    stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не вдалося відправити повідомлення клієнту: {ex.Message}");
                }
            }

            Console.WriteLine($"Загальне: {message}");
        }
    }
}
