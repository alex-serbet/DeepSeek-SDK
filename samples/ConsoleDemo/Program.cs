using DeepSeek.Client;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("DeepSeek.Client Console Demo");
        Console.WriteLine("============================");

        // Initialize client
        var options = new DeepSeekClientOptions
        {
            Model = "deepseek-chat",
            Temperature = 0.7,
            MaxTokens = 500
        };

        // TODO: Replace with your actual API key
        var client = new DeepSeekClient("sk-e1cc8e86b4ba48f6bfb2083c894ceeeb", options);

        // Handle streaming chunks
        client.OnStreamChunkReceived += (sender, chunk) =>
        {
            Console.Write(chunk);
        };

        // Handle errors
        client.OnError += (sender, error) =>
        {
            Console.WriteLine($"\n[ERROR] {error.Message}");
        };

        // Handle debug info
        client.OnDebugInfo += (sender, debugInfo) =>
        {
            if (client.Options.ShowDebugInfo)
            {
                Console.WriteLine($"[DEBUG] {debugInfo}");
            }
        };

        // Handle token usage
        client.OnTokenUsage += (sender, usage) =>
        {
            if (client.Options.ShowTokenUsage)
            {
                Console.WriteLine($"[USAGE] Tokens: {usage.TotalTokens} (Prompt: {usage.PromptTokens}, Completion: {usage.CompletionTokens})");
            }
        };

        while (true)
        {
            Console.Write("\n\nYou: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.ToLower() == "exit" || input.ToLower() == "quit")
                break;

            try
            {
                Console.Write("Assistant: ");

                // Send message with automatic history management
                var result = await client.SendMessageAsync(input, stream: true);

                if (client.Options.ShowDebugInfo)
                {
                    Console.WriteLine($"[Finish reason: {result.FinishReason}]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        Console.WriteLine("\nGoodbye!");
    }
}
