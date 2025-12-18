using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using ChatApp.Client.Models;

namespace ChatApp.Client
{
    public partial class MainWindow : Window
    {
        HubConnection connection;
        private readonly HttpClient _httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();

            // Port numarasını Server projenin çalıştığı port ile (örn: 7123) değiştirdiğinden emin ol
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5105/chatHub") 
                .Build();

            connection.On<string, string, string>("ReceiveMessage", (user, message, time) =>
            {
                Dispatcher.Invoke(() => {
                    MessagesList.Items.Add($"[{time}] {user}: {message}");
                });
            });

            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadMessageHistory();
            
            try 
            { 
                await connection.StartAsync(); 
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Canlı bağlantı kurulamadı: " + ex.Message); 
            }
        }

        private async Task LoadMessageHistory()
        {
            try
            {
                // API URL'sini portuna göre kontrol et
                var history = await _httpClient.GetFromJsonAsync<List<Message>>("http://localhost:5105/api/messages");

                if (history != null)
                {
                    foreach (var msg in history)
                    {
                        MessagesList.Items.Add($"[{msg.Timestamp.ToShortTimeString()}] {msg.Sender}: {msg.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessagesList.Items.Add("Geçmiş yüklenemedi: " + ex.Message);
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection.State == HubConnectionState.Connected)
                {
                    await connection.InvokeAsync("SendMessage", UserNameInput.Text, MessageInput.Text);
                    MessageInput.Text = "";
                }
                else
                {
                    MessageBox.Show("Bağlantı henüz hazır değil!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesaj gönderilemedi: " + ex.Message);
            }
        }
    }
}