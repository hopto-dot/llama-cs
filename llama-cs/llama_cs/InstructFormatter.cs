using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace llama_cs
{
    public class InstructFormatter
    {
        public InstructSequence _sequence;

        public InstructFormatter(InstructSequence sequence)
        {
            _sequence = sequence;
        }

        public string FormatMessage(List<string> chatHistory, string userMessage, Character assistantCharacter)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_sequence.SystemMessage))
            {
                string systemMessage = _sequence.SystemMessage;


                if (assistantCharacter != null)
                {
                    var characterPromptBuilder = new StringBuilder();
                    if (assistantCharacter.Description != "")
                    {
                        characterPromptBuilder.Append(assistantCharacter.Name + ":");
                        characterPromptBuilder.AppendLine(ReplacePlaceholderNames(assistantCharacter.Description));
                    }
                    if (assistantCharacter.Personality != "")
                    {
                        characterPromptBuilder.AppendLine(ReplacePlaceholderNames(assistantCharacter.Personality));
                    }
                    if (assistantCharacter.MessageExamples != "")
                    {
                        characterPromptBuilder.Append(ConvertMessageExamplesToInstructFormat(assistantCharacter.MessageExamples));
                    }
                    if (assistantCharacter.Scenario != "")
                    {
                        characterPromptBuilder.AppendLine(ReplacePlaceholderNames(assistantCharacter.Scenario));
                    }
                    // characterPromptBuilder.AppendLine(assistantCharacter.FirstMessage);

                    string characterPrompt = characterPromptBuilder.ToString();

                    systemMessage = systemMessage.Replace("{{char_prompt}}", characterPrompt);
                }
                else
                {
                    systemMessage = systemMessage.Replace("{{char_prompt}}", "");
                }

                if (_sequence.User.Name != "")
                {
                    var userPromptBuilder = new StringBuilder();

                    if (_sequence.User.Description != "")
                    {
                        // userPromptBuilder.AppendLine(_sequence.User.Name + ":");
                        userPromptBuilder.Append(_sequence.User.Description);
                    }

                    string userPrompt = userPromptBuilder.ToString();

                    systemMessage = systemMessage.Replace("{{user_prompt}}", userPrompt);
                }

                sb.Append(systemMessage + "\n");
            }

            foreach (var message in chatHistory)
            {
                sb.Append(message);
            }

            if (userMessage != "")
            {
                sb.Append(FormatAssistantMessagePrefix());
            }

            string returnString = sb.ToString();

            return returnString;
        }

        public string FormatUserMessage(string message)
        {
            return $"{_sequence.UserMessagePrefix}{(_sequence.IncludeNames ? $"{_sequence.User.Name}: " : "")}{message}{_sequence.UserMessageSuffix}";
        }

        public string FormatAssistantMessage(string message)
        {
            return $"{_sequence.AssistantMessagePrefix}{message}{_sequence.AssistantMessageSuffix}";
        }

        public string FormatAssistantMessagePrefix()
        {
            return $"{_sequence.AssistantMessagePrefix}{(_sequence.IncludeNames ? $"{_sequence.AssistantName}: " : "")}";
        }

        private string ConvertMessageExamplesToInstructFormat(string mesExample)
        {
            var exampleMessageHistory = new List<string>();
            var lines = mesExample.Split('\n');

            foreach (var line in lines)
            {
                if (line.Trim() == "<START>") continue;

                if (line.StartsWith("{{user}}:"))
                {
                    string userMessage = line.Substring("{{user}}:".Length).Trim();
                    userMessage = ReplacePlaceholderNames(userMessage);
                    exampleMessageHistory.Add($"{_sequence.UserMessagePrefix}{userMessage}{_sequence.UserMessageSuffix}");
                }
                else if (line.StartsWith("{{char}}:"))
                {
                    string assistantMessage = line.Substring("{{char}}:".Length).Trim();
                    assistantMessage = ReplacePlaceholderNames(assistantMessage);
                    exampleMessageHistory.Add($"{_sequence.AssistantMessagePrefix}{assistantMessage}{_sequence.AssistantMessageSuffix}");
                }
            }

            return string.Join("", exampleMessageHistory);
        }

        private string ReplacePlaceholderNames(string input)
        {
            return input.Replace("{{user}}", _sequence.User.Name).Replace("{{char}}", _sequence.AssistantName);
        }
    }
}
