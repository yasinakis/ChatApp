using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using ChatApp.Client.Models;

namespace ChatApp.Client
{
    public partial class MainWindow : Window
    {
        // SignalR bağlantısı ve HTTP istemcisi tanımları
        private HubConnection connection;
        private readonly HttpClient _httpClient = new HttpClient();
        
        // Sunucu adresi (Terminalindeki portla eşleşmeli)
        private readonly string _baseUrl = "http://localhost:5105";

        public MainWindow()
        {
            InitializeComponent();

            // 1. SignalR Hub bağlantısını yapılandır
            connection = new HubConnectionBuilder()
                .WithUrl($"{_baseUrl}/chatHub")
                .WithAutomaticReconnect() // Bağlantı koparsa otomatik denemesi için
                .Build();

            // 2. Sunucudan gelen canlı mesajları dinle
            connection.On<string, string, string>("ReceiveMessage", (user, message, time) =>
            {
                Dispatcher.Invoke(() => {
                    // Modern ListBox tasarımı nesne beklediği için anonim tip ekliyoruz
                    MessagesList.Items.Add(new { Sender = user, Content = message, Timestamp = time });
                    
                    // Otomatik aşağı kaydır
                    if (MessagesList.Items.Count > 0)
                    {
                        MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
                    }
                });
            });

            // Pencere yüklendiğinde çalışacak olay
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Önce eski mesajları yükle, sonra canlı bağlantıyı aç
            await LoadMessageHistory();
            
            try 
            { 
                await connection.StartAsync(); 
                this.Title = "SkyChat - Bağlandı ✅";
            }
            catch (Exception ex)
            { 
                this.Title = "SkyChat - Bağlantı Hatası! ❌";
                MessageBox.Show("Sunucuya bağlanılamadı: " + ex.Message); 
            }
        }

        private async Task LoadMessageHistory()
        {
            try
            {
                // API'den geçmiş mesajları çek
                var history = await _httpClient.GetFromJsonAsync<List<Message>>($"{_baseUrl}/api/messages");

                if (history != null)
                {
                    foreach (var msg in history)
                    {
                        // Geçmiş mesajları baloncuk tasarımına uygun ekle
                        MessagesList.Items.Add(new { 
                            Sender = msg.Sender, 
                            Content = msg.Content, 
                            Timestamp = msg.Timestamp.ToShortTimeString() 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessagesList.Items.Add(new { Sender = "Sistem", Content = "Geçmiş yüklenemedi: " + ex.Message, Timestamp = "--:--" });
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageInput.Text)) return;

            try
            {
                if (connection.State == HubConnectionState.Connected)
                {
                    // Hub üzerindeki SendMessage metodunu tetikle
                    await connection.InvokeAsync("SendMessage", UserNameInput.Text, MessageInput.Text);
                    MessageInput.Text = "";
                    MessageInput.Focus(); // Gönderdikten sonra odağı tekrar yazı alanına al
                }
                else
                {
                    MessageBox.Show("Bağlantı kesik, lütfen bekleyin...");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesaj gönderilemedi: " + ex.Message);
            }
        }

        // Enter tuşu ile mesaj gönderme desteği
        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}