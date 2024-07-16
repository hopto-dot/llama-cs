# llama-cs
A sillytaven-like c# library for interfacing with llama.cpp inference server

## How to use

Use llama_cs:
```cs
using llama_cs;
```

## Simple method
```cs
var client = new LlmClient();
client.AddAssistantMessage("Hello, how can I help?");

string response = client.GetResponse("What is the meaning of life?").Result;

Console.Writeline(response); // print llm response
```

### Change instruct mode

The default instruct mode is Alpaca.

If you want to change it to Gemma instruct formatting:
```cs
var instructSequence = new InstructSequence("gemma"); // Also supports "command-r"

var client = new LlmClient(instructSequence)
```

### Change llm api parameters
```cs
var llmParameters = new LlmParameters();
llmParameters.temperature = 1;
llmParameters.n_predict = 50; // max tokens to generate (default 500)
llmParameters.grammar = File.ReadAllText(@"C:\Users\George\Documents\Git\llama.cpp\grammars\japanese_new.gbnf"); // specify grammar the llm follows
// LlmParameters supports most of llama.cpp's inference parameters

var client = new LlmClient(instructSequence, llmParameters);
```

### Other client options
```cs
var instructSequence = new InstructSequence("command-r");

var character = new Character(@"C:\Users\user\Downloads\Philomena Cunk.json"); // Make assistant roleplay as character
var user = new User("Gavin", "Gavin is a 20 year old guy that wants to learn about Britain."); // Set user's name

var llmParameters = new LlmParameters();
llmParameters.temperature = 1;
llmParameters.AddLogitBias(12309, -100); // ban token with ID 12309

// You can specify what port llama.cpp inference server is running on
var client = new LlmClient(instructSequence, llmParameters, character, user, 1235);
```
