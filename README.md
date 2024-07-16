# llama-cs
A sillytaven-like c# library for interfacing with llama.cpp's [inference server](https://github.com/ggerganov/llama.cpp/tree/master/examples/server).

## How to use

Use llama_cs
```cs
using llama_cs;
```

## Simple method
```cs
var client = new LlmClient();
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

var client = new LlmClient(instructSequence, llmParameters);
```

### Other client options
```cs
var instructSequence = new InstructSequence("command-r");

var character = new Character(@"C:\Users\user\Downloads\Philomena Cunk.json"); // Make assistant roleplay as a character exported from SillyTavern
var user = new User("Gavin", "Gavin is a 20 year old guy that wants to learn about Britain."); // Set user's name and description

var llmParameters = new LlmParameters();
llmParameters.AddLogitBias(12309, -100); // ban token with ID 12309

// You can specify what port llama.cpp inference server is running on
var client = new LlmClient(instructSequence, llmParameters, character, user, 1235);

// GetInstructString() returns a string that is the whole context of the conversation (what the llm sees).
client.GetInstructString(); // print me, at the moment this does nothing
```
## Advanced
### The library doesn't support a instruct pattern?
No problem!

Specify it yourself
```
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
