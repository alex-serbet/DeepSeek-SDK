# DeepSeek.Client

A modern C# client library for DeepSeek AI API with streaming support.

## Features

- ✅ **Streaming Support**: Real-time text streaming from DeepSeek API
- ✅ **Async/Await**: Fully asynchronous API
- ✅ **Event-Driven**: Events for stream chunks and errors
- ✅ **Conversation History**: Built-in support for multi-turn conversations
- ✅ **Configurable**: Customizable model, temperature, max tokens, and timeout
- ✅ **Error Handling**: Comprehensive error handling and logging
- ✅ **WPF Support**: Ready-to-use WPF components

## Installation

### From Source

1. Clone the repository
2. Build the solution: `dotnet build DeepSeek.sln`
3. Reference the `src/DeepSeek.Client/DeepSeek.Client.csproj` in your project

### NuGet (Future)

```bash
# Not yet published to NuGet
dotnet add package DeepSeek.Client
```

## Quick Start

### Basic Usage

```csharp
using DeepSeek.Client;

// Initialize client
var client = new DeepSeekClient("your-api-key-here");

// Send a simple message
var result = await client.SendMessageAsync("Hello, how are you?");
Console.WriteLine(result.Content);
```

### Streaming Response

```csharp
// Handle streaming response
client.OnStreamChunkReceived += (sender, chunk) =>
{
    Console.Write(chunk); // Print each chunk as it arrives
};

var result = await client.SendMessageAsync("Tell me a story", stream: true);
```

### Advanced Configuration

```csharp
var options = new DeepSeekClientOptions
{
    Model = "deepseek-chat",
    Temperature = 0.7,
    MaxTokens = 1000,
    Timeout = TimeSpan.FromMinutes(2),
    ShowDebugInfo = false,    // Hide debug messages
    ShowTokenUsage = true     // Show token statistics
};

var client = new DeepSeekClient("your-api-key", options);
```

### Conversation History

```csharp
var messages = new List<ChatMessage>
{
    new ChatMessage { Role = "system", Content = "You are a helpful assistant." },
    new ChatMessage { Role = "user", Content = "What's the weather like?" }
};

var result = await client.SendMessagesAsync(messages, stream: true);
```

## WPF Integration

### Basic WPF Chat Control

```csharp
using DeepSeek.Client;

public partial class ChatWindow : Window
{
    private readonly DeepSeekClient _client;
    private readonly List<ChatMessage> _history = new();

    public ChatWindow()
    {
        InitializeComponent();

        _client = new DeepSeekClient("your-api-key");
        _client.OnStreamChunkReceived += (s, chunk) => Dispatcher.Invoke(() =>
        {
            ChatTextBox.AppendText(chunk);
        });
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var message = InputTextBox.Text;
        _history.Add(new ChatMessage { Role = "user", Content = message });

        var result = await _client.SendMessagesAsync(_history, stream: true);
        _history.Add(new ChatMessage { Role = result.Role, Content = result.Content });
    }
}
```

## API Reference

### DeepSeekClient

#### Constructor
```csharp
DeepSeekClient(string apiKey, DeepSeekClientOptions? options = null)
```

#### Methods
- `Task<ChatResult> SendMessageAsync(string message, bool stream = true, CancellationToken cancellationToken = default)` - Send message with automatic history management
- `Task<ChatResult> SendMessageOnceAsync(string message, bool stream = true, CancellationToken cancellationToken = default)` - Send message without history management
- `Task<ChatResult> SendMessagesAsync(IEnumerable<ChatMessage> messages, bool stream = true, CancellationToken cancellationToken = default)` - Send custom message list
- `List<ChatMessage> GetHistory()` - Get copy of conversation history
- `void ClearHistory()` - Clear conversation history
- `void AddMessage(string role, string content)` - Manually add message to history
- `int HistoryCount` - Get number of messages in history

#### Events
- `EventHandler<string> OnStreamChunkReceived` - Fired when a chunk of streamed content is received
- `EventHandler<Exception> OnError` - Fired when an error occurs
- `EventHandler<string> OnDebugInfo` - Fired when debug information should be displayed
- `EventHandler<Usage> OnTokenUsage` - Fired when token usage should be displayed
- `EventHandler<ChatMessage> OnMessageAdded` - Fired when a message is added to history
- `EventHandler OnHistoryCleared` - Fired when history is cleared

### DeepSeekClientOptions

- `string Model` - The model to use (default: "deepseek-chat")
- `int? MaxTokens` - Maximum tokens to generate
- `double? Temperature` - Controls randomness (0.0-2.0, default: 0.7)
- `TimeSpan Timeout` - Request timeout (default: 5 minutes)
- `bool ShowDebugInfo` - Whether to show debug information (default: false)
- `bool ShowTokenUsage` - Whether to show token usage statistics (default: false)
- `int MaxHistorySize` - Maximum messages in conversation history (default: 50, 0 = unlimited)

### ChatResult

- `string Content` - The generated content
- `string Role` - The role of the response
- `Usage? Usage` - Token usage statistics
- `bool WasStreamed` - Whether the response was streamed
- `string? FinishReason` - Why the generation finished

## Error Handling

```csharp
client.OnError += (sender, error) =>
{
    Console.WriteLine($"Error: {error.Message}");
};

try
{
    var result = await client.SendMessageAsync("Hello");
}
catch (HttpRequestException ex)
{
    // Handle HTTP errors
}
catch (TaskCanceledException ex)
{
    // Handle timeout
}
```

## Examples

See the `samples/` directory for complete examples:

- **ConsoleDemo**: Console application showing basic usage with streaming
- **ChatBot.Wpf**: WPF chat application demonstrating the library usage

## Building from Source

```bash
# Build the entire solution
dotnet build DeepSeek.sln

# Run tests (when available)
dotnet test

# Run the WPF sample
cd samples/ChatBot.Wpf
dotnet run
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

MIT License - see LICENSE file for details.

## Support

For issues and questions:
- Create an issue on GitHub
- Check the examples in the `samples/` directory
- Review the DeepSeek API documentation

## Changelog

### v1.1.0
- Added configurable debug information display
- Added configurable token usage statistics
- Added new events: OnDebugInfo, OnTokenUsage, OnMessageAdded, OnHistoryCleared
- **Added automatic conversation history management inside DeepSeekClient**
- Enhanced WPF demo with checkboxes for settings
- Enhanced console demo with configurable output
- Improved error handling and logging
- Fixed hardcoded token display messages
- Removed duplicate token usage messages
- Added MaxHistorySize configuration
- Added GetHistory(), ClearHistory(), AddMessage() methods

### v1.0.0
- Initial release
- Streaming support
- WPF integration
- Comprehensive error handling
- Conversation history support
