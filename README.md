# llama-cs
A sillytaven-like c# library for interfacing with llama.cpp's [inference server](https://github.com/ggerganov/llama.cpp/tree/master/examples/server).

If you are on Windows, you can find a build for the inference server by going to llama.cpp's latest [releases](https://github.com/ggerganov/llama.cpp/releases/latest) then downloading the file containing `bin-win-cuda` and depending on your version of cuda: `cu11.7.1`, or `cu12.2.0`. Avoid any file starting with `cudart`. You should run `llama-server.exe -m "gguf path here"` (along with any other arguments you want) to start the server.

See simple example app [here](https://github.com/hopto-dot/llama-cs?tab=readme-ov-file#example-simple-chat-app).

## How to use

Use llama_cs
```cs
using llama_cs;
```

## Simple method
```cs
var client = new LlmClient(new InstructSequence()); // Default InstructSequence uses Alpaca instruct formatting
client.AddAssistantMessage("Hello, how can I help?"); // Manually add a message from the assistant

string response = client.GetResponse("What is the meaning of life?").Result;

Console.Writeline(response); // print llm response
```
_(Note: calling `GetResponse()` adds the message you pass into it to the chat history for you)_

### Change instruct mode

The default instruct mode is Alpaca.

If you want to change it to Gemma instruct formatting:
```cs
var instructSequence = new InstructSequence("gemma"); // Also supports "command-r"

var client = new LlmClient(instructSequence)
```

### Change llm api parameters
```cs
// LlmParameters supports most of llama.cpp's inference parameters
var llmParameters = new LlmParameters();
llmParameters.temperature = 1;
llmParameters.n_predict = 50; // max tokens to generate (default 500)
llmParameters.grammar = File.ReadAllText(@"C:\Users\user\Documents\Git\llama.cpp\grammars\japanese.gbnf"); // specify grammar the llm follows

var client = new LlmClient(instructSequence, llmParameters, port: 1234); // specify the port the server is running
```

### Roleplay as a person with a character
```cs
var instructSequence = new InstructSequence("command-r");

var character = new Character(@"C:\Users\user\Downloads\Philomena Cunk.json"); // Make assistant roleplay as a character exported from SillyTavern
var user = new User("Gavin", "Gavin is a 20 year old guy that wants to learn about Britain."); // Set user's name and description

var llmParameters = new LlmParameters();
llmParameters.AddLogitBias(12309, -100); // ban token with ID 12309

var client = new LlmClient(instructSequence, llmParameters, character, user);
```
## Advanced
### The library doesn't support an instruct pattern?
No problem!

Specify it yourself
```cs
var newInstructSequence = new InstructSequence("### Instruction:\n", // UserMessagePrefix
                                               "\n\n",               // UserMessageSuffix
                                               "### Response:\n",    // AssistantMessagePrefix
                                               "\n\n",               // AssistantMessageSuffix
                                               false,                // IncludeNames
                                               false,                // UseUserMessageAsSystem
                                               // system message:
                                               "You are an expert actor that can fully immerse yourself into any role given. You do not break character for any reason and always talk in first person. Currently your role is {{char}}, which is described in detail below. As {{char}}, continue the exchange with {{user}}.\n{{char_prompt}}\n{{user_prompt}}"
                                               );
```

In the system prompt, write `{{user}}` where you want to be replaced with the user's name and `{{char}}` for the assistant's name.

If you want to specify a character and username, you should include `{{char_prompt}}` and `{{user_prompt}}` towards the end of the system prompt like the above example.

Sometimes enabling `IncludeNames` makes it easier for the llm to understand who is who, however can confuse it just as much. Experiment with it.

Currently, `UseUserMessageAsSystem` does nothing.

## See what text the llm is passed
`GetInstructString()` returns the whole context of the conversation (what the llm sees) as a string.

```cs
client.GetInstructString(); // print with Console.WriteLine(), at the moment this does nothing
```

## Example simple chat app
```cs
var instructSequence = new InstructSequence();

var character = new Character(@"C:\Users\user\Downloads\Philomena Cunk.json"); // Make assistant roleplay as a character exported from SillyTavern
var user = new User("Gavin", "Gavin is a 20 year old guy that wants to learn about Britain."); // Set user's name and description

var llmParameters = new LlmParameters();
llmParameters.temperature = 1;

var client = new LlmClient(instructSequence, llmParameters, character, user); // assumes server is running on port 8000

string userMessage = "";
while (true)
{
    Console.Write("User: ");
    userMessage = Console.ReadLine();

    string response = client.GetResponse(userMessage).Result;

    Console.WriteLine($"\nAssistant: {response}\n");
}
```

