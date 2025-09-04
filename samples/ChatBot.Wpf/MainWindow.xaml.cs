using DeepSeek.Client;
using System.Windows;

namespace ChatBot.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private DeepSeekClient _client;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize DeepSeek client with default options
            UpdateClientOptions();

            AppendToOutput("DeepSeek Chat Library Demo\n");
            AppendToOutput("==========================\n\n");

            // Subscribe to checkbox events
            DebugInfoCheckBox.Checked += (s, e) => UpdateClientOptions();
            DebugInfoCheckBox.Unchecked += (s, e) => UpdateClientOptions();
            TokenUsageCheckBox.Checked += (s, e) => UpdateClientOptions();
            TokenUsageCheckBox.Unchecked += (s, e) => UpdateClientOptions();
        }

        private void UpdateClientOptions()
        {
            var options = new DeepSeekClientOptions
            {
                Model = "deepseek-chat",
                Temperature = 0.7,
                MaxTokens = 1000,
                ShowDebugInfo = DebugInfoCheckBox.IsChecked ?? false,
                ShowTokenUsage = TokenUsageCheckBox.IsChecked ?? false,
                MaxHistorySize = 50
            };

            // Dispose old client if exists
            _client?.Dispose();

            // TODO: Replace with your actual API key
            _client = new DeepSeekClient("sk-e1cc8e86b4ba48f6bfb2083c894ceeeb", options);

            // Subscribe to events
            _client.OnStreamChunkReceived += Client_OnStreamChunkReceived;
            _client.OnError += Client_OnError;
            _client.OnDebugInfo += Client_OnDebugInfo;
            _client.OnTokenUsage += Client_OnTokenUsage;
            _client.OnMessageAdded += Client_OnMessageAdded;
        }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        string userMessage = InputTextBox.Text.Trim();
        if (string.IsNullOrEmpty(userMessage))
            return;

        // Clear input
        InputTextBox.Text = string.Empty;

        // Display user message
        AppendToOutput($"You: {userMessage}\n\n");

        // Disable button during request
        SendButton.IsEnabled = false;
        SendButton.Content = "Sending...";

        try
        {
            // Send message with automatic history management
            AppendToOutput("Assistant: ");
            var result = await _client.SendMessageAsync(userMessage, stream: true);

            // Add spacing after response
            AppendToOutput("\n");
        }
        catch (Exception ex)
        {
            AppendToOutput($"Error: {ex.Message}\n");
        }
        finally
        {
            SendButton.IsEnabled = true;
            SendButton.Content = "Send";
        }
    }

    private void Client_OnStreamChunkReceived(object? sender, string chunk)
    {
        Dispatcher.Invoke(() => AppendToOutput(chunk));
    }

    private void Client_OnError(object? sender, Exception error)
    {
        Dispatcher.Invoke(() => AppendToOutput($"[ERROR] {error.Message}\n"));
    }

    private void Client_OnDebugInfo(object? sender, string debugInfo)
    {
        if (_client.Options.ShowDebugInfo)
        {
            Dispatcher.Invoke(() => AppendToOutput($"[DEBUG] {debugInfo}\n"));
        }
    }

    private void Client_OnTokenUsage(object? sender, DeepSeek.Client.Usage usage)
    {
        if (_client.Options.ShowTokenUsage)
        {
            Dispatcher.Invoke(() => AppendToOutput($"[USAGE] Tokens: {usage.TotalTokens} (Prompt: {usage.PromptTokens}, Completion: {usage.CompletionTokens})\n"));
        }
    }

    private void Client_OnMessageAdded(object? sender, DeepSeek.Client.ChatMessage message)
    {
        // Optional: Log when messages are added to history
        if (_client.Options.ShowDebugInfo)
        {
            Dispatcher.Invoke(() => AppendToOutput($"[HISTORY] Added {message.Role}: {message.Content.Substring(0, Math.Min(50, message.Content.Length))}...\n"));
        }
    }

    private void AppendToOutput(string text)
    {
        OutputTextBox.AppendText(text);
        OutputTextBox.ScrollToEnd();
    }
}
