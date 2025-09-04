# DeepSeek.Client - Complete Documentation

## 📖 Table of Contents

1. [Introduction](#introduction)
2. [Installation and Setup](#installation-and-setup)
3. [Quick Start](#quick-start)
4. [Core Concepts](#core-concepts)
5. [API Reference](#api-reference)
6. [Usage Examples](#usage-examples)
7. [Advanced Features](#advanced-features)
8. [Error Handling](#error-handling)
9. [Best Practices](#best-practices)
10. [Troubleshooting](#troubleshooting)

---

## 🎯 Introduction

**DeepSeek.Client** is a modern C# library for working with the DeepSeek AI API. The library provides a simple and intuitive interface for integrating AI capabilities into your .NET applications.

### ✨ Key Features

- 🚀 **Full asynchronous support** - all operations are async/await
- 📡 **Streaming support** - real-time response reception
- 💬 **Automatic history management** - seamless work with conversation context
- 🎛️ **Flexible configuration** - customization of all API parameters
- 📊 **Usage monitoring** - token statistics and debug information
- 🎨 **WPF integration** - ready-to-use WPF components
- 🔧 **Extensible architecture** - events and overrides for customization

### 🏗️ Architecture

The library is built on the principles of:
- **Ease of use** - minimal boilerplate code
- **Security** - proper error handling and resource management
- **Performance** - efficient use of HTTP and memory
- **Extensibility** - ability to add new features

---

## 📦 Installation and Setup

### Requirements

- .NET 8.0 or higher
- DeepSeek API key (obtain from [platform.deepseek.com](https://platform.deepseek.com))

### Installation from Source

1. **Clone the repository:**
```bash
git clone https://github.com/your-repo/deepseek-client.git
cd deepseek-client
```

2. **Build the solution:**
```bash
dotnet build DeepSeek.sln
```

3. **Add project reference:**
```xml
<!-- In your .csproj file -->
<ItemGroup>
  <ProjectReference Include="path/to/src/DeepSeek.Client/DeepSeek.Client.csproj" />
</ItemGroup>
```

### Alternative Installation Methods

#### Via NuGet (Future Release)
```bash
dotnet add package DeepSeek.Client
```

#### Manual Copying
Copy files from `src/DeepSeek.Client/` to your project.

---

## 🚀 Quick Start

### Minimal Example

```csharp
using DeepSeek.Client;

class Program
{
    static async Task Main()
    {
        // Create client with API key
        var client = new DeepSeekClient("your-api-key-here");

        // Send message
        var result = await client.SendMessageAsync("Hello! How are you?");

        // Display response
        Console.WriteLine(result.Content);
    }
}
```

### With Streaming Output

```csharp
using DeepSeek.Client;

class Program
{
    static async Task Main()
    {
        var client = new DeepSeekClient("your-api-key");

        // Subscribe to real-time chunk reception
        client.OnStreamChunkReceived += (sender, chunk) =>
        {
            Console.Write(chunk); // Display as received
        };

        // Send message with streaming output
        var result = await client.SendMessageAsync("Tell me a story", stream: true);

        Console.WriteLine($"\n\nTotal tokens: {result.Usage?.TotalTokens ?? 0}");
    }
}
```

### WPF Integration

```csharp
using DeepSeek.Client;
using System.Windows;

public partial class ChatWindow : Window
{
    private DeepSeekClient _client;

    public ChatWindow()
    {
        InitializeComponent();

        // Initialize client
        _client = new DeepSeekClient("your-api-key");

        // Subscribe to events
        _client.OnStreamChunkReceived += (s, chunk) =>
            Dispatcher.Invoke(() => ChatTextBox.AppendText(chunk));
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var message = InputTextBox.Text;

        // Just send message - history is managed automatically
        await _client.SendMessageAsync(message, stream: true);
    }
}
```

---

## 🧠 Core Concepts

### 1. Client (DeepSeekClient)

The main class for interacting with the DeepSeek API. Manages connections, message history, and all operations.

**Key features:**
- Automatic HTTP connection management
- Built-in conversation history
- Configurable options
- Event-driven monitoring model

### 2. Messages (ChatMessage)

Represent individual messages in a conversation.

```csharp
public class ChatMessage
{
    public string Role { get; set; }     // "user", "assistant", "system"
    public string Content { get; set; }  // Message text
}
```

**Message roles:**
- `user` - messages from the user
- `assistant` - responses from AI
- `system` - system instructions

### 3. Results (ChatResult)

The result of executing a request to the API.

```csharp
public class ChatResult
{
    public string Content { get; set; }      // Generated text
    public string Role { get; set; }         // Response role
    public Usage? Usage { get; set; }        // Usage statistics
    public bool WasStreamed { get; set; }    // Whether response was streamed
    public string? FinishReason { get; set; } // Completion reason
}
```

### 4. Configuration (DeepSeekClientOptions)

Client behavior settings.

```csharp
public class DeepSeekClientOptions
{
    public string Model { get; set; } = "deepseek-chat";
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; } = 0.7;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
    public bool ShowDebugInfo { get; set; } = false;
    public bool ShowTokenUsage { get; set; } = false;
    public int MaxHistorySize { get; set; } = 50;
}
```

### 5. Automatic History Management

The library automatically:
- Saves sent user messages
- Saves received assistant responses
- Limits history size (MaxHistorySize)
- Provides access to history via GetHistory()

---

## 📚 API Reference

### DeepSeekClient

#### Constructors

```csharp
// Basic constructor
public DeepSeekClient(string apiKey)

// With extended options
public DeepSeekClient(string apiKey, DeepSeekClientOptions? options)
```

#### Main Methods

##### SendMessageAsync
Sends a message with automatic history management.

```csharp
Task<ChatResult> SendMessageAsync(
    string message,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `message` - message text
- `stream` - whether to use streaming output (default true)
- `cancellationToken` - cancellation token

**Returns:** `ChatResult` with execution result

##### SendMessageOnceAsync
Sends a message without saving to history.

```csharp
Task<ChatResult> SendMessageOnceAsync(
    string message,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Usage:** For one-time requests or custom history management.

##### SendMessagesAsync
Sends a list of messages.

```csharp
Task<ChatResult> SendMessagesAsync(
    IEnumerable<ChatMessage> messages,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Usage:** For sending full history or system messages.

##### SendMessageWithDebugAsync
Sends a message with debug enabled.

```csharp
Task<ChatResult> SendMessageWithDebugAsync(
    string message,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Features:** Temporarily enables all debug options.

#### History Management

##### GetHistory
Gets a copy of the current conversation history.

```csharp
List<ChatMessage> GetHistory()
```

**Returns:** New list with message copies.

##### ClearHistory
Clears the conversation history.

```csharp
void ClearHistory()
```

**Features:** Raises `OnHistoryCleared` event.

##### AddMessage
Adds a message to history manually.

```csharp
void AddMessage(string role, string content)
```

**Parameters:**
- `role` - message role ("user", "assistant", "system")
- `content` - message text

##### HistoryCount
Gets the number of messages in history.

```csharp
int HistoryCount { get; }
```

#### Properties

##### Options
Current client settings.

```csharp
DeepSeekClientOptions Options { get; }
```

**Features:** Read-only, changes via client recreation.

#### Events

##### OnStreamChunkReceived
Raised when receiving the next chunk in streaming mode.

```csharp
event EventHandler<string>? OnStreamChunkReceived
```

**Event parameters:**
- `sender` - DeepSeekClient instance
- `chunk` - received text fragment

##### OnError
Raised when an error occurs.

```csharp
event EventHandler<Exception>? OnError
```

**Event parameters:**
- `sender` - DeepSeekClient instance
- `error` - exception with error details

##### OnDebugInfo
Raised for debug information.

```csharp
event EventHandler<string>? OnDebugInfo
```

**Call conditions:** Only if `Options.ShowDebugInfo = true`

##### OnTokenUsage
Raised when receiving token usage statistics.

```csharp
event EventHandler<Usage>? OnTokenUsage
```

**Call conditions:** Only if `Options.ShowTokenUsage = true`

##### OnMessageAdded
Raised when a message is added to history.

```csharp
event EventHandler<ChatMessage>? OnMessageAdded
```

##### OnHistoryCleared
Raised when history is cleared.

```csharp
event EventHandler? OnHistoryCleared
```

### DeepSeekClientOptions

#### Properties

##### Model
Model for generating responses.

```csharp
string Model { get; set; } = "deepseek-chat"
```

**Possible values:**
- `"deepseek-chat"` - main chat model
- `"deepseek-coder"` - programming model

##### MaxTokens
Maximum number of tokens in response.

```csharp
int? MaxTokens { get; set; }
```

**Default:** `null` (uses API limit)

##### Temperature
Degree of randomness in responses (0.0 - 2.0).

```csharp
double? Temperature { get; set; } = 0.7
```

**Recommendations:**
- `0.0` - deterministic responses
- `0.7` - balanced randomness
- `1.5+` - high creativity

##### Timeout
Timeout for HTTP requests.

```csharp
TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5)
```

##### ShowDebugInfo
Whether to show debug information.

```csharp
bool ShowDebugInfo { get; set; } = false
```

**What is shown:**
- Response Content-Type
- First lines of stream
- Parsing error details

##### ShowTokenUsage
Whether to show token usage statistics.

```csharp
bool ShowTokenUsage { get; set; } = false
```

**Output format:**
```
[USAGE] Tokens: 150 (Prompt: 100, Completion: 50)
```

##### MaxHistorySize
Maximum number of messages in history.

```csharp
int MaxHistorySize { get; set; } = 50
```

**Features:**
- `0` = unlimited
- When exceeded, oldest messages are removed

### ChatMessage

#### Properties

##### Role
Message author's role.

```csharp
string Role { get; set; }
```

**Allowed values:**
- `"user"` - message from user
- `"assistant"` - response from AI
- `"system"` - system instruction

##### Content
Message text.

```csharp
string Content { get; set; }
```

### ChatResult

#### Properties

##### Content
Generated response text.

```csharp
string Content { get; set; }
```

##### Role
Response role (usually "assistant").

```csharp
string Role { get; set; }
```

##### Usage
Token usage statistics.

```csharp
Usage? Usage { get; set; }
```

##### WasStreamed
Whether response was received in streaming mode.

```csharp
bool WasStreamed { get; set; }
```

##### FinishReason
Reason for generation completion.

```csharp
string? FinishReason { get; set; }
```

**Possible values:**
- `"stop"` - normal completion
- `"length"` - token limit reached
- `"content_filter"` - content filter triggered

### Usage

#### Properties

##### PromptTokens
Number of tokens in request (including history).

```csharp
int PromptTokens { get; set; }
```

##### CompletionTokens
Number of tokens in response.

```csharp
int CompletionTokens { get; set; }
```

##### TotalTokens
Total number of tokens used.

```csharp
int TotalTokens { get; set; }
```

---

## 💡 Usage Examples

### 1. Simple Chat Bot

```csharp
using DeepSeek.Client;

class SimpleChatBot
{
    private readonly DeepSeekClient _client;

    public SimpleChatBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey, new DeepSeekClientOptions
        {
            Temperature = 0.8,
            MaxHistorySize = 20
        });
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        var result = await _client.SendMessageAsync(userMessage);
        return result.Content;
    }
}
```

### 2. Streaming Chat Bot

```csharp
class StreamingChatBot
{
    private readonly DeepSeekClient _client;

    public StreamingChatBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        // Configure streaming output
        _client.OnStreamChunkReceived += (sender, chunk) =>
        {
            Console.Write(chunk);
        };
    }

    public async Task ChatAsync()
    {
        while (true)
        {
            Console.Write("\nYou: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
                break;

            Console.Write("Assistant: ");
            await _client.SendMessageAsync(input, stream: true);
        }
    }
}
```

### 3. WPF Chat Application

```csharp
using DeepSeek.Client;
using System.Windows;

public partial class ChatWindow : Window
{
    private DeepSeekClient _client;

    public ChatWindow()
    {
        InitializeComponent();

        InitializeChatClient();
    }

    private void InitializeChatClient()
    {
        var options = new DeepSeekClientOptions
        {
            Model = "deepseek-chat",
            Temperature = 0.7,
            MaxTokens = 1000,
            ShowDebugInfo = DebugCheckBox.IsChecked ?? false,
            ShowTokenUsage = TokenCheckBox.IsChecked ?? false,
            MaxHistorySize = 50
        };

        _client = new DeepSeekClient("your-api-key", options);

        // Configure event handlers
        _client.OnStreamChunkReceived += Client_OnStreamChunkReceived;
        _client.OnError += Client_OnError;
        _client.OnTokenUsage += Client_OnTokenUsage;
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var message = InputTextBox.Text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        InputTextBox.Clear();
        SendButton.IsEnabled = false;

        try
        {
            AppendToChat($"You: {message}\n");
            AppendToChat("Assistant: ");

            await _client.SendMessageAsync(message, stream: true);

            AppendToChat("\n\n");
        }
        catch (Exception ex)
        {
            AppendToChat($"Error: {ex.Message}\n\n");
        }
        finally
        {
            SendButton.IsEnabled = true;
        }
    }

    private void Client_OnStreamChunkReceived(object? sender, string chunk)
    {
        Dispatcher.Invoke(() => AppendToChat(chunk));
    }

    private void Client_OnError(object? sender, Exception error)
    {
        Dispatcher.Invoke(() => AppendToChat($"[ERROR] {error.Message}\n"));
    }

    private void Client_OnTokenUsage(object? sender, Usage usage)
    {
        Dispatcher.Invoke(() =>
        {
            StatusTextBlock.Text = $"Tokens: {usage.TotalTokens} (P: {usage.PromptTokens}, C: {usage.CompletionTokens})";
        });
    }

    private void AppendToChat(string text)
    {
        ChatTextBox.AppendText(text);
        ChatTextBox.ScrollToEnd();
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _client.ClearHistory();
        ChatTextBox.Clear();
        StatusTextBlock.Text = "History cleared";
    }
}
```

### 4. System Instructions

```csharp
class SpecializedAssistant
{
    private readonly DeepSeekClient _client;

    public SpecializedAssistant(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        // Add system instruction
        _client.AddMessage("system", "You are a helpful programming assistant. Always provide code examples.");
    }

    public async Task<string> GetProgrammingHelpAsync(string question)
    {
        var result = await _client.SendMessageAsync(question);
        return result.Content;
    }
}
```

### 5. Multi-step Conversation

```csharp
class ConversationManager
{
    private readonly DeepSeekClient _client;

    public ConversationManager(string apiKey)
    {
        _client = new DeepSeekClient(apiKey, new DeepSeekClientOptions
        {
            MaxHistorySize = 100, // Long history for complex conversations
            Temperature = 0.3     // More deterministic responses
        });
    }

    public async Task RunConversationAsync()
    {
        // Step 1: Greeting
        var greeting = await _client.SendMessageAsync("Hello! Let's talk about programming.");
        Console.WriteLine($"Assistant: {greeting.Content}");

        // Step 2: Topic discussion
        var discussion = await _client.SendMessageAsync("What programming languages do you know?");
        Console.WriteLine($"Assistant: {discussion.Content}");

        // Step 3: Deep analysis
        var analysis = await _client.SendMessageAsync("Tell me more about C#.");
        Console.WriteLine($"Assistant: {analysis.Content}");

        // View history
        var history = _client.GetHistory();
        Console.WriteLine($"\nTotal messages in history: {history.Count}");
    }
}
```

### 6. Error Handling

```csharp
class RobustChatBot
{
    private readonly DeepSeekClient _client;

    public RobustChatBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey, new DeepSeekClientOptions
        {
            Timeout = TimeSpan.FromSeconds(30)
        });

        // Configure error handling
        _client.OnError += HandleError;
    }

    public async Task<string> SendMessageSafeAsync(string message)
    {
        try
        {
            var result = await _client.SendMessageAsync(message);
            return result.Content;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new InvalidOperationException("Invalid API key", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new TimeoutException("Request timed out", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Chat error: {ex.Message}", ex);
        }
    }

    private void HandleError(object? sender, Exception error)
    {
        if (error is HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Error: {httpEx.StatusCode}");
        }
        else
        {
            Console.WriteLine($"Error: {error.Message}");
        }
    }
}
```

---

## ⚡ Advanced Features

### Streaming and Real-time Processing

```csharp
class RealTimeChat
{
    private readonly DeepSeekClient _client;

    public RealTimeChat(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        // Process each word individually
        _client.OnStreamChunkReceived += ProcessWord;
    }

    private void ProcessWord(object? sender, string chunk)
    {
        // You can analyze each word
        if (chunk.Contains("error") || chunk.Contains("Error"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.Write(chunk);
        Console.ResetColor();
    }

    public async Task DemonstrateStreamingAsync()
    {
        Console.WriteLine("Starting streaming transmission...\n");

        var result = await _client.SendMessageAsync(
            "Write a long story about programming",
            stream: true);

        Console.WriteLine($"\n\nCompleted. Tokens: {result.Usage?.TotalTokens ?? 0}");
    }
}
```

### History Management

```csharp
class HistoryManager
{
    private readonly DeepSeekClient _client;

    public HistoryManager(string apiKey)
    {
        _client = new DeepSeekClient(apiKey, new DeepSeekClientOptions
        {
            MaxHistorySize = 10
        });

        // Track history changes
        _client.OnMessageAdded += (sender, message) =>
            Console.WriteLine($"Added: {message.Role}");

        _client.OnHistoryCleared += (sender, args) =>
            Console.WriteLine("History cleared");
    }

    public async Task DemonstrateHistoryAsync()
    {
        // Send several messages
        for (int i = 1; i <= 5; i++)
        {
            await _client.SendMessageAsync($"Message #{i}");
            Console.WriteLine($"History contains {_client.HistoryCount} messages");
        }

        // View history
        var history = _client.GetHistory();
        Console.WriteLine("\nConversation history:");
        foreach (var msg in history)
        {
            Console.WriteLine($"{msg.Role}: {msg.Content}");
        }

        // Clear history
        _client.ClearHistory();
        Console.WriteLine($"After clearing: {_client.HistoryCount} messages");
    }
}
```

### Debugging and Monitoring

```csharp
class DebugChatBot
{
    private readonly DeepSeekClient _client;

    public DebugChatBot(string apiKey)
    {
        var options = new DeepSeekClientOptions
        {
            ShowDebugInfo = true,
            ShowTokenUsage = true
        };

        _client = new DeepSeekClient(apiKey, options);

        // Subscribe to all events
        _client.OnDebugInfo += (sender, info) =>
            Log($"DEBUG: {info}");

        _client.OnTokenUsage += (sender, usage) =>
            Log($"USAGE: Total={usage.TotalTokens}, Prompt={usage.PromptTokens}, Completion={usage.CompletionTokens}");

        _client.OnError += (sender, error) =>
            Log($"ERROR: {error.Message}");

        _client.OnStreamChunkReceived += (sender, chunk) =>
            Log($"CHUNK: '{chunk}'");
    }

    public async Task DebugSessionAsync()
    {
        Log("Starting debug session...");

        var result = await _client.SendMessageWithDebugAsync(
            "Tell me about C# in 3 sentences");

        Log($"Result: {result.Content}");
        Log($"Streaming mode: {result.WasStreamed}");
        Log($"Finish reason: {result.FinishReason}");
    }

    private void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine($"[{timestamp}] {message}");
    }
}
```

### Custom Models and Options

```csharp
class AdvancedConfiguration
{
    public static DeepSeekClient CreateOptimizedClient(string apiKey)
    {
        var options = new DeepSeekClientOptions
        {
            Model = "deepseek-coder",  // For programming
            Temperature = 0.1,         // Deterministic responses
            MaxTokens = 2000,          // Long responses
            Timeout = TimeSpan.FromMinutes(10),
            ShowDebugInfo = false,
            ShowTokenUsage = true,
            MaxHistorySize = 25
        };

        return new DeepSeekClient(apiKey, options);
    }

    public static DeepSeekClient CreateCreativeClient(string apiKey)
    {
        var options = new DeepSeekClientOptions
        {
            Model = "deepseek-chat",
            Temperature = 1.2,         // Creative responses
            MaxTokens = 500,
            Timeout = TimeSpan.FromSeconds(30),
            ShowDebugInfo = false,
            ShowTokenUsage = false,
            MaxHistorySize = 15
        };

        return new DeepSeekClient(apiKey, options);
    }
}
```

---

## 🚨 Error Handling

### Error Types

#### 1. HttpRequestException
HTTP connection errors.

```csharp
try
{
    var result = await client.SendMessageAsync("Hello");
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
{
    // Invalid API key
    Console.WriteLine("Check your API key");
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
{
    // Rate limit exceeded
    await Task.Delay(TimeSpan.FromSeconds(60));
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
{
    // DeepSeek server error
    Console.WriteLine("Server temporarily unavailable");
}
```

#### 2. TaskCanceledException
Timeout or operation cancellation.

```csharp
try
{
    var result = await client.SendMessageAsync("Hello");
}
catch (TaskCanceledException ex)
{
    Console.WriteLine("Request canceled or timed out");
    // Can retry with larger timeout
}
```

#### 3. JsonException
JSON parsing error in response.

```csharp
client.OnError += (sender, error) =>
{
    if (error is JsonException jsonEx)
    {
        Console.WriteLine($"Response parsing error: {jsonEx.Message}");
        // API might have returned unexpected format
    }
};
```

#### 4. ArgumentException
Invalid parameters.

```csharp
// Validation before sending
if (string.IsNullOrWhiteSpace(message))
{
    throw new ArgumentException("Message cannot be empty");
}

if (message.Length > 10000)
{
    throw new ArgumentException("Message is too long");
}
```

### Error Handling Strategies

#### 1. Retry Attempts

```csharp
class ResilientChatBot
{
    private readonly DeepSeekClient _client;

    public async Task<string> SendWithRetryAsync(string message, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var result = await _client.SendMessageAsync(message);
                return result.Content;
            }
            catch (HttpRequestException ex) when (ex.StatusCode >= 500)
            {
                if (attempt == maxRetries) throw;

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                await Task.Delay(delay);
            }
        }

        throw new InvalidOperationException("All attempts exhausted");
    }
}
```

#### 2. Graceful Degradation

```csharp
class GracefulChatBot
{
    private readonly DeepSeekClient _client;
    private bool _isOnline = true;

    public GracefulChatBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        _client.OnError += (sender, error) =>
        {
            if (error is HttpRequestException httpEx &&
                httpEx.StatusCode >= 500)
            {
                _isOnline = false;
                StartReconnectionTimer();
            }
        };
    }

    public async Task<string> SendMessageAsync(string message)
    {
        if (!_isOnline)
        {
            return "Service temporarily unavailable. Please try again later.";
        }

        try
        {
            var result = await _client.SendMessageAsync(message);
            return result.Content;
        }
        catch
        {
            _isOnline = false;
            return "An error occurred. Please try again.";
        }
    }

    private void StartReconnectionTimer()
    {
        Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(_ =>
        {
            _isOnline = true;
        });
    }
}
```

#### 3. Input Validation

```csharp
class ValidatingChatBot
{
    private readonly DeepSeekClient _client;

    public async Task<string> SendValidatedMessageAsync(string message)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty");

        if (message.Length > 8000)
            throw new ArgumentException("Message is too long");

        // Check for forbidden content
        if (ContainsForbiddenContent(message))
            throw new ArgumentException("Message contains forbidden content");

        try
        {
            var result = await _client.SendMessageAsync(message);
            return result.Content;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Message sending error: {ex.Message}", ex);
        }
    }

    private bool ContainsForbiddenContent(string message)
    {
        var forbiddenWords = new[] { "forbidden", "word" };
        return forbiddenWords.Any(word => message.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
}
```

---

## 🎯 Best Practices

### 1. Resource Management

```csharp
class ResourceManagedBot : IDisposable
{
    private readonly DeepSeekClient _client;
    private bool _disposed;

    public ResourceManagedBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    public async Task<string> ProcessMessageAsync(string message)
    {
        // Always use using for cancellation tokens
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            var result = await _client.SendMessageAsync(message, cancellationToken: cts.Token);
            return result.Content;
        }
        catch (TaskCanceledException)
        {
            return "Request canceled by timeout";
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _client?.Dispose();
            _disposed = true;
        }
    }
}
```

### 2. Performance Optimization

```csharp
class OptimizedBot
{
    private readonly DeepSeekClient _client;

    public OptimizedBot(string apiKey)
    {
        // Optimized settings
        var options = new DeepSeekClientOptions
        {
            Temperature = 0.7,
            MaxTokens = 1000,
            Timeout = TimeSpan.FromSeconds(30),
            MaxHistorySize = 20,  // Limited history for performance
            ShowDebugInfo = false // Disable debug in production
        };

        _client = new DeepSeekClient(apiKey, options);
    }

    public async Task<string> GetQuickResponseAsync(string question)
    {
        // Use SendMessageOnceAsync for quick responses
        var result = await _client.SendMessageOnceAsync(question, stream: false);
        return result.Content;
    }

    public async Task<string> GetDetailedResponseAsync(string question)
    {
        // Use full history for detailed responses
        var result = await _client.SendMessageAsync(question, stream: true);
        return result.Content;
    }
}
```

### 3. Logging and Monitoring

```csharp
class MonitoredBot
{
    private readonly DeepSeekClient _client;
    private readonly ILogger _logger;

    public MonitoredBot(string apiKey, ILogger logger)
    {
        _logger = logger;

        var options = new DeepSeekClientOptions
        {
            ShowTokenUsage = true
        };

        _client = new DeepSeekClient(apiKey, options);

        // Log all events
        _client.OnError += (sender, error) =>
            _logger.LogError(error, "DeepSeek API error");

        _client.OnTokenUsage += (sender, usage) =>
            _logger.LogInformation("Tokens used: {Total}", usage.TotalTokens);
    }

    public async Task<string> SendLoggedMessageAsync(string message)
    {
        using (_logger.BeginScope("DeepSeek Request: {Message}", message))
        {
            _logger.LogInformation("Sending message to DeepSeek");

            var result = await _client.SendMessageAsync(message);

            _logger.LogInformation("Received response: {Length} chars", result.Content.Length);

            return result.Content;
        }
    }
}
```

### 4. Security

```csharp
class SecureBot
{
    private readonly DeepSeekClient _client;
    private readonly string[] _forbiddenTopics = { "passwords", "keys", "secrets" };

    public SecureBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    public async Task<string> SendSecureMessageAsync(string message)
    {
        // Security check
        if (ContainsSensitiveInfo(message))
        {
            throw new SecurityException("Message contains sensitive information");
        }

        // Clean potentially dangerous content
        var sanitizedMessage = SanitizeMessage(message);

        var result = await _client.SendMessageAsync(sanitizedMessage);
        return result.Content;
    }

    private bool ContainsSensitiveInfo(string message)
    {
        return _forbiddenTopics.Any(topic =>
            message.Contains(topic, StringComparison.OrdinalIgnoreCase));
    }

    private string SanitizeMessage(string message)
    {
        // Remove potentially dangerous patterns
        return message.Replace("password", "[FILTERED]")
                     .Replace("api_key", "[FILTERED]");
    }
}
```

### 5. Testing

```csharp
class TestableBot
{
    private readonly DeepSeekClient _client;

    public TestableBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    // Method for testing with mock data
    public async Task<string> SendTestMessageAsync(string message)
    {
        // In tests you can replace _client with mock
        var result = await _client.SendMessageAsync(message);
        return result.Content;
    }

    // Method for integration tests
    public async Task<ChatResult> SendMessageWithResultAsync(string message)
    {
        return await _client.SendMessageAsync(message);
    }

    // Method for checking history
    public List<ChatMessage> GetConversationHistory()
    {
        return _client.GetHistory();
    }

    // Method for resetting state between tests
    public void Reset()
    {
        _client.ClearHistory();
    }
}
```

---

## 🔧 Troubleshooting

### Common Issues

#### 1. "Invalid API key"

**Symptoms:**
- `HttpRequestException` with code 401
- Message "Unauthorized"

**Solutions:**
```csharp
// Check your API key
var client = new DeepSeekClient("sk-correct-api-key-here");

// Make sure the key is active on platform.deepseek.com
```

#### 2. "Request timeout"

**Symptoms:**
- `TaskCanceledException`
- Long wait for response

**Solutions:**
```csharp
var options = new DeepSeekClientOptions
{
    Timeout = TimeSpan.FromMinutes(10)  // Increase timeout
};

var client = new DeepSeekClient("api-key", options);
```

#### 3. "Rate limit exceeded"

**Symptoms:**
- `HttpRequestException` with code 429
- Message "Too Many Requests"

**Solutions:**
```csharp
// Add delay between requests
await Task.Delay(TimeSpan.FromSeconds(60));

// Or use retry with exponential backoff
```

#### 4. "Maximum context length exceeded"

**Symptoms:**
- Error with long history
- `FinishReason = "length"`

**Solutions:**
```csharp
// Limit history size
var options = new DeepSeekClientOptions
{
    MaxHistorySize = 10
};

// Or clear history periodically
if (client.HistoryCount > 20)
{
    client.ClearHistory();
}
```

#### 5. "Stream parsing errors"

**Symptoms:**
- `JsonException` during streaming output
- Incomplete or corrupted responses

**Solutions:**
```csharp
// Disable streaming mode for problematic requests
var result = await client.SendMessageAsync(message, stream: false);

// Or add error handling
client.OnError += (sender, error) =>
{
    if (error is JsonException)
    {
        Console.WriteLine("Stream parsing error, switching to normal mode");
    }
};
```

### Debugging Issues

#### Enabling Detailed Logging

```csharp
var options = new DeepSeekClientOptions
{
    ShowDebugInfo = true,
    ShowTokenUsage = true
};

var client = new DeepSeekClient("api-key", options);

client.OnDebugInfo += (sender, info) => Console.WriteLine($"[DEBUG] {info}");
client.OnError += (sender, error) => Console.WriteLine($"[ERROR] {error.Message}");
```

#### Network Connection Check

```csharp
class NetworkChecker
{
    public static async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://api.deepseek.com/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
```

#### Configuration Validation

```csharp
class ConfigurationValidator
{
    public static void ValidateOptions(DeepSeekClientOptions options)
    {
        if (string.IsNullOrEmpty(options.Model))
            throw new ArgumentException("Model cannot be empty");

        if (options.Temperature < 0 || options.Temperature > 2)
            throw new ArgumentException("Temperature must be in range 0-2");

        if (options.MaxHistorySize < 0)
            throw new ArgumentException("MaxHistorySize cannot be negative");
    }
}
```

### Performance

#### High Load Optimization

```csharp
class HighPerformanceBot
{
    private readonly DeepSeekClient _client;

    public HighPerformanceBot(string apiKey)
    {
        var options = new DeepSeekClientOptions
        {
            Timeout = TimeSpan.FromSeconds(15),  // Short timeout
            MaxHistorySize = 5,                  // Minimal history
            ShowDebugInfo = false,               // Disable debug
            ShowTokenUsage = false               // Disable statistics
        };

        _client = new DeepSeekClient(apiKey, options);
    }

    public async Task<string> GetFastResponseAsync(string question)
    {
        // Use SendMessageOnceAsync to avoid history overhead
        var result = await _client.SendMessageOnceAsync(question, stream: false);
        return result.Content;
    }
}
```

#### Resource Monitoring

```csharp
class ResourceMonitor
{
    private readonly DeepSeekClient _client;
    private int _totalTokensUsed = 0;
    private int _totalRequests = 0;

    public ResourceMonitor(string apiKey)
    {
        _client = new DeepSeekClient(apiKey, new DeepSeekClientOptions
        {
            ShowTokenUsage = true
        });

        _client.OnTokenUsage += (sender, usage) =>
        {
            _totalTokensUsed += usage.TotalTokens;
            _totalRequests++;

            Console.WriteLine($"Stats: {_totalRequests} requests, {_totalTokensUsed} total tokens");
        };
    }
}
```

---

## 📝 Conclusion

DeepSeek.Client is a powerful and flexible library for integrating with the DeepSeek AI API. It provides:

- ✅ **Simple API** for quick start
- ✅ **Advanced features** for complex scenarios
- ✅ **Reliable error handling** for production use
- ✅ **Performance optimization** for high loads
- ✅ **Complete documentation** for easy learning

### Next Steps

1. **Explore examples** in the `samples/` folder
2. **Try the basic example** for first acquaintance
3. **Review the API Reference** for deep understanding
4. **Study best practices** for production use
5. **Check troubleshooting** when encountering issues

### Support

- 📖 **Documentation**: This file contains complete information
- 💡 **Examples**: Ready-made examples in the `samples/` folder
- 🐛 **Issues**: Create issues on GitHub for bugs and features
- 💬 **Discussions**: Discuss ideas and ask questions

---

*Documentation created for DeepSeek.Client v1.1.0*
*Last updated: September 4, 2025*
