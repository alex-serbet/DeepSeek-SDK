# DeepSeek.Client - Полная документация

## 📖 Оглавление

1. [Введение](#введение)
2. [Установка и настройка](#установка-и-настройка)
3. [Быстрый старт](#быстрый-старт)
4. [Основные концепции](#основные-концепции)
5. [API Reference](#api-reference)
6. [Примеры использования](#примеры-использования)
7. [Расширенные возможности](#расширенные-возможности)
8. [Обработка ошибок](#обработка-ошибок)
9. [Лучшие практики](#лучшие-практики)
10. [Troubleshooting](#troubleshooting)

---

## 🎯 Введение

**DeepSeek.Client** - это современная C# библиотека для работы с DeepSeek AI API. Библиотека предоставляет простой и интуитивный интерфейс для интеграции возможностей ИИ в ваши .NET приложения.

### ✨ Ключевые возможности

- 🚀 **Полная асинхронная поддержка** - все операции async/await
- 📡 **Streaming поддержка** - получение ответов в реальном времени
- 💬 **Автоматическое управление историей** - бесшовная работа с контекстом разговора
- 🎛️ **Гибкая конфигурация** - настройка всех параметров API
- 📊 **Мониторинг использования** - статистика токенов и отладочная информация
- 🎨 **WPF интеграция** - готовые компоненты для WPF приложений
- 🔧 **Расширяемая архитектура** - события и переопределения для кастомизации

### 🏗️ Архитектура

Библиотека построена на принципах:
- **Простота использования** - минимум boilerplate кода
- **Безопасность** - правильная обработка ошибок и ресурсов
- **Производительность** - эффективное использование HTTP и памяти
- **Расширяемость** - возможность добавления новых функций

---

## 📦 Установка и настройка

### Требования

- .NET 8.0 или выше
- DeepSeek API ключ (получить на [platform.deepseek.com](https://platform.deepseek.com))

### Установка из исходников

1. **Клонируйте репозиторий:**
```bash
git clone https://github.com/your-repo/deepseek-client.git
cd deepseek-client
```

2. **Соберите решение:**
```bash
dotnet build DeepSeek.sln
```

3. **Добавьте ссылку на проект:**
```xml
<!-- В вашем .csproj файле -->
<ItemGroup>
  <ProjectReference Include="path/to/src/DeepSeek.Client/DeepSeek.Client.csproj" />
</ItemGroup>
```

### Альтернативные способы установки

#### Через NuGet (будущий релиз)
```bash
dotnet add package DeepSeek.Client
```

#### Ручное копирование
Скопируйте файлы из `src/DeepSeek.Client/` в ваш проект.

---

## 🚀 Быстрый старт

### Минимальный пример

```csharp
using DeepSeek.Client;

class Program
{
    static async Task Main()
    {
        // Создаем клиент с API ключом
        var client = new DeepSeekClient("your-api-key-here");

        // Отправляем сообщение
        var result = await client.SendMessageAsync("Привет! Как дела?");

        // Выводим ответ
        Console.WriteLine(result.Content);
    }
}
```

### С потоковым выводом

```csharp
using DeepSeek.Client;

class Program
{
    static async Task Main()
    {
        var client = new DeepSeekClient("your-api-key");

        // Подписываемся на получение чанков в реальном времени
        client.OnStreamChunkReceived += (sender, chunk) =>
        {
            Console.Write(chunk); // Выводим по мере получения
        };

        // Отправляем сообщение с потоковым выводом
        var result = await client.SendMessageAsync("Расскажи сказку", stream: true);

        Console.WriteLine($"\n\nВсего токенов: {result.Usage?.TotalTokens ?? 0}");
    }
}
```

### WPF интеграция

```csharp
using DeepSeek.Client;
using System.Windows;

public partial class ChatWindow : Window
{
    private DeepSeekClient _client;

    public ChatWindow()
    {
        InitializeComponent();

        // Инициализируем клиент
        _client = new DeepSeekClient("your-api-key");

        // Подписываемся на события
        _client.OnStreamChunkReceived += (s, chunk) =>
            Dispatcher.Invoke(() => ChatTextBox.AppendText(chunk));
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var message = InputTextBox.Text;

        // Просто отправляем сообщение - история управляется автоматически
        await _client.SendMessageAsync(message, stream: true);
    }
}
```

---

## 🧠 Основные концепции

### 1. Клиент (DeepSeekClient)

Основной класс для взаимодействия с DeepSeek API. Управляет соединением, историей сообщений и всеми операциями.

**Ключевые особенности:**
- Автоматическое управление HTTP соединением
- Встроенная история разговора
- Конфигурируемые опции
- Событийная модель для мониторинга

### 2. Сообщения (ChatMessage)

Представляют отдельные сообщения в разговоре.

```csharp
public class ChatMessage
{
    public string Role { get; set; }     // "user", "assistant", "system"
    public string Content { get; set; }  // Текст сообщения
}
```

**Роли сообщений:**
- `user` - сообщения от пользователя
- `assistant` - ответы от ИИ
- `system` - системные инструкции

### 3. Результаты (ChatResult)

Результат выполнения запроса к API.

```csharp
public class ChatResult
{
    public string Content { get; set; }      // Сгенерированный текст
    public string Role { get; set; }         // Роль ответа
    public Usage? Usage { get; set; }        // Статистика использования
    public bool WasStreamed { get; set; }    // Был ли потоковый вывод
    public string? FinishReason { get; set; } // Причина завершения
}
```

### 4. Конфигурация (DeepSeekClientOptions)

Настройки поведения клиента.

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

### 5. Автоматическое управление историей

Библиотека автоматически:
- Сохраняет отправленные сообщения пользователя
- Сохраняет полученные ответы ассистента
- Ограничивает размер истории (MaxHistorySize)
- Предоставляет доступ к истории через GetHistory()

---

## 📚 API Reference

### DeepSeekClient

#### Конструкторы

```csharp
// Базовый конструктор
public DeepSeekClient(string apiKey)

// С расширенными опциями
public DeepSeekClient(string apiKey, DeepSeekClientOptions? options)
```

#### Основные методы

##### SendMessageAsync
Отправляет сообщение с автоматическим управлением историей.

```csharp
Task<ChatResult> SendMessageAsync(
    string message,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Параметры:**
- `message` - текст сообщения
- `stream` - использовать ли потоковый вывод (по умолчанию true)
- `cancellationToken` - токен отмены операции

**Возвращает:** `ChatResult` с результатом выполнения

##### SendMessageOnceAsync
Отправляет сообщение без сохранения в истории.

```csharp
Task<ChatResult> SendMessageOnceAsync(
    string message,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Использование:** Для одноразовых запросов или кастомного управления историей.

##### SendMessagesAsync
Отправляет список сообщений.

```csharp
Task<ChatResult> SendMessagesAsync(
    IEnumerable<ChatMessage> messages,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Использование:** Для отправки полной истории или системных сообщений.

##### SendMessageWithDebugAsync
Отправляет сообщение с включенной отладкой.

```csharp
Task<ChatResult> SendMessageWithDebugAsync(
    string message,
    bool stream = true,
    CancellationToken cancellationToken = default)
```

**Особенности:** Временно включает все отладочные опции.

#### Управление историей

##### GetHistory
Получает копию текущей истории разговора.

```csharp
List<ChatMessage> GetHistory()
```

**Возвращает:** Новый список с копиями сообщений.

##### ClearHistory
Очищает историю разговора.

```csharp
void ClearHistory()
```

**Особенности:** Вызывает событие `OnHistoryCleared`.

##### AddMessage
Добавляет сообщение в историю вручную.

```csharp
void AddMessage(string role, string content)
```

**Параметры:**
- `role` - роль сообщения ("user", "assistant", "system")
- `content` - текст сообщения

##### HistoryCount
Получает количество сообщений в истории.

```csharp
int HistoryCount { get; }
```

#### Свойства

##### Options
Текущие настройки клиента.

```csharp
DeepSeekClientOptions Options { get; }
```

**Особенности:** Только для чтения, изменения через пересоздание клиента.

#### События

##### OnStreamChunkReceived
Вызывается при получении очередного чанка в потоковом режиме.

```csharp
event EventHandler<string>? OnStreamChunkReceived
```

**Параметры события:**
- `sender` - экземпляр DeepSeekClient
- `chunk` - полученный текстовый фрагмент

##### OnError
Вызывается при возникновении ошибки.

```csharp
event EventHandler<Exception>? OnError
```

**Параметры события:**
- `sender` - экземпляр DeepSeekClient
- `error` - исключение с деталями ошибки

##### OnDebugInfo
Вызывается для отладочной информации.

```csharp
event EventHandler<string>? OnDebugInfo
```

**Условия вызова:** Только если `Options.ShowDebugInfo = true`

##### OnTokenUsage
Вызывается при получении статистики использования токенов.

```csharp
event EventHandler<Usage>? OnTokenUsage
```

**Условия вызова:** Только если `Options.ShowTokenUsage = true`

##### OnMessageAdded
Вызывается при добавлении сообщения в историю.

```csharp
event EventHandler<ChatMessage>? OnMessageAdded
```

##### OnHistoryCleared
Вызывается при очистке истории.

```csharp
event EventHandler? OnHistoryCleared
```

### DeepSeekClientOptions

#### Свойства

##### Model
Модель для генерации ответов.

```csharp
string Model { get; set; } = "deepseek-chat"
```

**Возможные значения:**
- `"deepseek-chat"` - основная модель для чата
- `"deepseek-coder"` - модель для программирования

##### MaxTokens
Максимальное количество токенов в ответе.

```csharp
int? MaxTokens { get; set; }
```

**По умолчанию:** `null` (используется лимит API)

##### Temperature
Степень случайности в ответах (0.0 - 2.0).

```csharp
double? Temperature { get; set; } = 0.7
```

**Рекомендации:**
- `0.0` - детерминированные ответы
- `0.7` - сбалансированная случайность
- `1.5+` - высокая креативность

##### Timeout
Таймаут для HTTP запросов.

```csharp
TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5)
```

##### ShowDebugInfo
Показывать ли отладочную информацию.

```csharp
bool ShowDebugInfo { get; set; } = false
```

**Что показывается:**
- Content-Type ответов
- Первые строки стрима
- Детали ошибок парсинга

##### ShowTokenUsage
Показывать ли статистику использования токенов.

```csharp
bool ShowTokenUsage { get; set; } = false
```

**Формат вывода:**
```
[USAGE] Tokens: 150 (Prompt: 100, Completion: 50)
```

##### MaxHistorySize
Максимальное количество сообщений в истории.

```csharp
int MaxHistorySize { get; set; } = 50
```

**Особенности:**
- `0` = без ограничений
- При превышении удаляются самые старые сообщения

### ChatMessage

#### Свойства

##### Role
Роль автора сообщения.

```csharp
string Role { get; set; }
```

**Допустимые значения:**
- `"user"` - сообщение от пользователя
- `"assistant"` - ответ от ИИ
- `"system"` - системная инструкция

##### Content
Текст сообщения.

```csharp
string Content { get; set; }
```

### ChatResult

#### Свойства

##### Content
Сгенерированный текст ответа.

```csharp
string Content { get; set; }
```

##### Role
Роль ответа (обычно "assistant").

```csharp
string Role { get; set; }
```

##### Usage
Статистика использования токенов.

```csharp
Usage? Usage { get; set; }
```

##### WasStreamed
Был ли ответ получен в потоковом режиме.

```csharp
bool WasStreamed { get; set; }
```

##### FinishReason
Причина завершения генерации.

```csharp
string? FinishReason { get; set; }
```

**Возможные значения:**
- `"stop"` - нормальное завершение
- `"length"` - достигнут лимит токенов
- `"content_filter"` - сработал фильтр контента

### Usage

#### Свойства

##### PromptTokens
Количество токенов в запросе (включая историю).

```csharp
int PromptTokens { get; set; }
```

##### CompletionTokens
Количество токенов в ответе.

```csharp
int CompletionTokens { get; set; }
```

##### TotalTokens
Общее количество использованных токенов.

```csharp
int TotalTokens { get; set; }
```

---

## 💡 Примеры использования

### 1. Простой чат-бот

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

### 2. Чат-бот с потоковым выводом

```csharp
class StreamingChatBot
{
    private readonly DeepSeekClient _client;

    public StreamingChatBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        // Настраиваем потоковый вывод
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

### 3. WPF чат приложение

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

        // Настраиваем обработчики событий
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

### 4. Системные инструкции

```csharp
class SpecializedAssistant
{
    private readonly DeepSeekClient _client;

    public SpecializedAssistant(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        // Добавляем системную инструкцию
        _client.AddMessage("system", "You are a helpful programming assistant. Always provide code examples.");
    }

    public async Task<string> GetProgrammingHelpAsync(string question)
    {
        var result = await _client.SendMessageAsync(question);
        return result.Content;
    }
}
```

### 5. Многоступенчатый разговор

```csharp
class ConversationManager
{
    private readonly DeepSeekClient _client;

    public ConversationManager(string apiKey)
    {
        _client = new DeepSeekClient(apiKey, new DeepSeekClientOptions
        {
            MaxHistorySize = 100, // Длинная история для сложных разговоров
            Temperature = 0.3     // Более детерминированные ответы
        });
    }

    public async Task RunConversationAsync()
    {
        // Этап 1: Приветствие
        var greeting = await _client.SendMessageAsync("Привет! Давай поговорим о программировании.");
        Console.WriteLine($"Assistant: {greeting.Content}");

        // Этап 2: Обсуждение темы
        var discussion = await _client.SendMessageAsync("Какие языки программирования ты знаешь?");
        Console.WriteLine($"Assistant: {discussion.Content}");

        // Этап 3: Глубокий анализ
        var analysis = await _client.SendMessageAsync("Расскажи подробнее о C#.");
        Console.WriteLine($"Assistant: {analysis.Content}");

        // Просмотр истории
        var history = _client.GetHistory();
        Console.WriteLine($"\nВсего сообщений в истории: {history.Count}");
    }
}
```

### 6. Обработка ошибок

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

        // Настраиваем обработку ошибок
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

## ⚡ Расширенные возможности

### Streaming и реальное время

```csharp
class RealTimeChat
{
    private readonly DeepSeekClient _client;

    public RealTimeChat(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);

        // Обработка каждого слова отдельно
        _client.OnStreamChunkReceived += ProcessWord;
    }

    private void ProcessWord(object? sender, string chunk)
    {
        // Можно анализировать каждое слово
        if (chunk.Contains("error") || chunk.Contains("Error"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.Write(chunk);
        Console.ResetColor();
    }

    public async Task DemonstrateStreamingAsync()
    {
        Console.WriteLine("Начинаем потоковую передачу...\n");

        var result = await _client.SendMessageAsync(
            "Напиши длинный рассказ о программировании",
            stream: true);

        Console.WriteLine($"\n\nЗавершено. Токенов: {result.Usage?.TotalTokens ?? 0}");
    }
}
```

### Управление историей

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

        // Отслеживаем изменения истории
        _client.OnMessageAdded += (sender, message) =>
            Console.WriteLine($"Добавлено: {message.Role}");

        _client.OnHistoryCleared += (sender, args) =>
            Console.WriteLine("История очищена");
    }

    public async Task DemonstrateHistoryAsync()
    {
        // Отправляем несколько сообщений
        for (int i = 1; i <= 5; i++)
        {
            await _client.SendMessageAsync($"Сообщение #{i}");
            Console.WriteLine($"История содержит {_client.HistoryCount} сообщений");
        }

        // Просматриваем историю
        var history = _client.GetHistory();
        Console.WriteLine("\nИстория разговора:");
        foreach (var msg in history)
        {
            Console.WriteLine($"{msg.Role}: {msg.Content}");
        }

        // Очищаем историю
        _client.ClearHistory();
        Console.WriteLine($"После очистки: {_client.HistoryCount} сообщений");
    }
}
```

### Отладка и мониторинг

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

        // Подписываемся на все события
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
        Log("Начинаем отладочную сессию...");

        var result = await _client.SendMessageWithDebugAsync(
            "Расскажи о C# за 3 предложения");

        Log($"Результат: {result.Content}");
        Log($"Потоковый режим: {result.WasStreamed}");
        Log($"Причина завершения: {result.FinishReason}");
    }

    private void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine($"[{timestamp}] {message}");
    }
}
```

### Кастомные модели и опции

```csharp
class AdvancedConfiguration
{
    public static DeepSeekClient CreateOptimizedClient(string apiKey)
    {
        var options = new DeepSeekClientOptions
        {
            Model = "deepseek-coder",  // Для программирования
            Temperature = 0.1,         // Детерминированные ответы
            MaxTokens = 2000,          // Длинные ответы
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
            Temperature = 1.2,         // Креативные ответы
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

## 🚨 Обработка ошибок

### Типы ошибок

#### 1. HttpRequestException
Ошибки HTTP соединения.

```csharp
try
{
    var result = await client.SendMessageAsync("Hello");
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
{
    // Неверный API ключ
    Console.WriteLine("Проверьте API ключ");
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
{
    // Превышен лимит запросов
    await Task.Delay(TimeSpan.FromSeconds(60));
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
{
    // Ошибка сервера DeepSeek
    Console.WriteLine("Сервер временно недоступен");
}
```

#### 2. TaskCanceledException
Таймаут или отмена операции.

```csharp
try
{
    var result = await client.SendMessageAsync("Hello");
}
catch (TaskCanceledException ex)
{
    Console.WriteLine("Запрос отменен или превышен таймаут");
    // Можно повторить с большим таймаутом
}
```

#### 3. JsonException
Ошибка парсинга JSON ответа.

```csharp
client.OnError += (sender, error) =>
{
    if (error is JsonException jsonEx)
    {
        Console.WriteLine($"Ошибка парсинга ответа: {jsonEx.Message}");
        // Возможно, API вернул неожиданный формат
    }
};
```

#### 4. ArgumentException
Неверные параметры.

```csharp
// Проверка перед отправкой
if (string.IsNullOrWhiteSpace(message))
{
    throw new ArgumentException("Сообщение не может быть пустым");
}

if (message.Length > 10000)
{
    throw new ArgumentException("Сообщение слишком длинное");
}
```

### Стратегии обработки ошибок

#### 1. Повторные попытки

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

        throw new InvalidOperationException("Все попытки исчерпаны");
    }
}
```

#### 2. Graceful degradation

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
            return "Сервис временно недоступен. Попробуйте позже.";
        }

        try
        {
            var result = await _client.SendMessageAsync(message);
            return result.Content;
        }
        catch
        {
            _isOnline = false;
            return "Произошла ошибка. Попробуйте еще раз.";
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

#### 3. Валидация входных данных

```csharp
class ValidatingChatBot
{
    private readonly DeepSeekClient _client;

    public async Task<string> SendValidatedMessageAsync(string message)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Сообщение не может быть пустым");

        if (message.Length > 8000)
            throw new ArgumentException("Сообщение слишком длинное");

        // Проверка на запрещенный контент
        if (ContainsForbiddenContent(message))
            throw new ArgumentException("Сообщение содержит запрещенный контент");

        try
        {
            var result = await _client.SendMessageAsync(message);
            return result.Content;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ошибка отправки сообщения: {ex.Message}", ex);
        }
    }

    private bool ContainsForbiddenContent(string message)
    {
        var forbiddenWords = new[] { "запрещенное", "слово" };
        return forbiddenWords.Any(word => message.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
}
```

---

## 🎯 Лучшие практики

### 1. Управление ресурсами

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
        // Всегда используем using для cancellation tokens
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            var result = await _client.SendMessageAsync(message, cancellationToken: cts.Token);
            return result.Content;
        }
        catch (TaskCanceledException)
        {
            return "Запрос отменен по таймауту";
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

### 2. Оптимизация производительности

```csharp
class OptimizedBot
{
    private readonly DeepSeekClient _client;

    public OptimizedBot(string apiKey)
    {
        // Оптимизированные настройки
        var options = new DeepSeekClientOptions
        {
            Temperature = 0.7,
            MaxTokens = 1000,
            Timeout = TimeSpan.FromSeconds(30),
            MaxHistorySize = 20,  // Ограниченная история для производительности
            ShowDebugInfo = false // Отключаем отладку в продакшене
        };

        _client = new DeepSeekClient(apiKey, options);
    }

    public async Task<string> GetQuickResponseAsync(string question)
    {
        // Для быстрых ответов используем SendMessageOnceAsync
        var result = await _client.SendMessageOnceAsync(question, stream: false);
        return result.Content;
    }

    public async Task<string> GetDetailedResponseAsync(string question)
    {
        // Для детальных ответов используем полную историю
        var result = await _client.SendMessageAsync(question, stream: true);
        return result.Content;
    }
}
```

### 3. Логирование и мониторинг

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

        // Логируем все события
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

### 4. Безопасность

```csharp
class SecureBot
{
    private readonly DeepSeekClient _client;
    private readonly string[] _forbiddenTopics = { "пароли", "ключи", "секреты" };

    public SecureBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    public async Task<string> SendSecureMessageAsync(string message)
    {
        // Проверка на безопасность
        if (ContainsSensitiveInfo(message))
        {
            throw new SecurityException("Сообщение содержит чувствительную информацию");
        }

        // Очистка от потенциально опасного контента
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
        // Удаляем потенциально опасные паттерны
        return message.Replace("password", "[FILTERED]")
                     .Replace("api_key", "[FILTERED]");
    }
}
```

### 5. Тестирование

```csharp
class TestableBot
{
    private readonly DeepSeekClient _client;

    public TestableBot(string apiKey)
    {
        _client = new DeepSeekClient(apiKey);
    }

    // Метод для тестирования с mock данными
    public async Task<string> SendTestMessageAsync(string message)
    {
        // В тестах можно подменить _client на mock
        var result = await _client.SendMessageAsync(message);
        return result.Content;
    }

    // Метод для интеграционных тестов
    public async Task<ChatResult> SendMessageWithResultAsync(string message)
    {
        return await _client.SendMessageAsync(message);
    }

    // Метод для проверки истории
    public List<ChatMessage> GetConversationHistory()
    {
        return _client.GetHistory();
    }

    // Метод для сброса состояния между тестами
    public void Reset()
    {
        _client.ClearHistory();
    }
}
```

---

## 🔧 Troubleshooting

### Распространенные проблемы

#### 1. "Invalid API key"

**Симптомы:**
- `HttpRequestException` с кодом 401
- Сообщение "Unauthorized"

**Решения:**
```csharp
// Проверьте API ключ
var client = new DeepSeekClient("sk-correct-api-key-here");

// Убедитесь, что ключ активен на platform.deepseek.com
```

#### 2. "Request timeout"

**Симптомы:**
- `TaskCanceledException`
- Долгое ожидание ответа

**Решения:**
```csharp
var options = new DeepSeekClientOptions
{
    Timeout = TimeSpan.FromMinutes(10)  // Увеличьте таймаут
};

var client = new DeepSeekClient("api-key", options);
```

#### 3. "Rate limit exceeded"

**Симптомы:**
- `HttpRequestException` с кодом 429
- Сообщение "Too Many Requests"

**Решения:**
```csharp
// Добавьте задержку между запросами
await Task.Delay(TimeSpan.FromSeconds(60));

// Или используйте повторные попытки с экспоненциальной задержкой
```

#### 4. "Maximum context length exceeded"

**Симптомы:**
- Ошибка при длинной истории
- `FinishReason = "length"`

**Решения:**
```csharp
// Ограничьте размер истории
var options = new DeepSeekClientOptions
{
    MaxHistorySize = 10
};

// Или очищайте историю периодически
if (client.HistoryCount > 20)
{
    client.ClearHistory();
}
```

#### 5. "Stream parsing errors"

**Симптомы:**
- `JsonException` при потоковом выводе
- Неполные или искаженные ответы

**Решения:**
```csharp
// Отключите потоковый режим для проблемных запросов
var result = await client.SendMessageAsync(message, stream: false);

// Или добавьте обработку ошибок
client.OnError += (sender, error) =>
{
    if (error is JsonException)
    {
        Console.WriteLine("Ошибка парсинга стрима, переключаюсь на обычный режим");
    }
};
```

### Отладка проблем

#### Включение детального логирования

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

#### Проверка сетевого соединения

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

#### Валидация конфигурации

```csharp
class ConfigurationValidator
{
    public static void ValidateOptions(DeepSeekClientOptions options)
    {
        if (string.IsNullOrEmpty(options.Model))
            throw new ArgumentException("Model не может быть пустым");

        if (options.Temperature < 0 || options.Temperature > 2)
            throw new ArgumentException("Temperature должен быть в диапазоне 0-2");

        if (options.MaxHistorySize < 0)
            throw new ArgumentException("MaxHistorySize не может быть отрицательным");
    }
}
```

### Производительность

#### Оптимизация для высокой нагрузки

```csharp
class HighPerformanceBot
{
    private readonly DeepSeekClient _client;

    public HighPerformanceBot(string apiKey)
    {
        var options = new DeepSeekClientOptions
        {
            Timeout = TimeSpan.FromSeconds(15),  // Короткий таймаут
            MaxHistorySize = 5,                  // Минимальная история
            ShowDebugInfo = false,               // Отключаем отладку
            ShowTokenUsage = false               // Отключаем статистику
        };

        _client = new DeepSeekClient(apiKey, options);
    }

    public async Task<string> GetFastResponseAsync(string question)
    {
        // Используем SendMessageOnceAsync для избежания overhead истории
        var result = await _client.SendMessageOnceAsync(question, stream: false);
        return result.Content;
    }
}
```

#### Мониторинг ресурсов

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

## 📝 Заключение

DeepSeek.Client - это мощная и гибкая библиотека для интеграции с DeepSeek AI API. Она предоставляет:

- ✅ **Простой API** для быстрого старта
- ✅ **Расширенные возможности** для сложных сценариев
- ✅ **Надежную обработку ошибок** для production использования
- ✅ **Оптимизацию производительности** для высоких нагрузок
- ✅ **Полную документацию** для легкого освоения

### Следующие шаги

1. **Изучите примеры** в папке `samples/`
2. **Попробуйте базовый пример** для первого знакомства
3. **Ознакомьтесь с API Reference** для глубокого понимания
4. **Изучите лучшие практики** для production использования
5. **Посмотрите troubleshooting** при возникновении проблем

### Поддержка

- 📖 **Документация**: Этот файл содержит полную информацию
- 💡 **Примеры**: Готовые примеры в папке `samples/`
- 🐛 **Issues**: Создавайте issues на GitHub для багов и фич
- 💬 **Discussions**: Обсуждайте идеи и задавайте вопросы

---

*Документация создана для DeepSeek.Client v1.1.0*
*Последнее обновление: 4 сентября 2025 года*
