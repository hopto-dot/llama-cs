using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    characterPromptBuilder.AppendLine(assistantCharacter.Description);
                    characterPromptBuilder.AppendLine(assistantCharacter.Personality);
                    characterPromptBuilder.Append(assistantCharacter.MessageExamples);
                    characterPromptBuilder.AppendLine(assistantCharacter.Scenario);
                    // characterPromptBuilder.AppendLine(assistantCharacter.FirstMessage);

                    string characterPrompt = characterPromptBuilder.ToString();

                    systemMessage = systemMessage.Replace("ASS_CHARACTER", "\n" + characterPrompt);
                }

                sb.Append(systemMessage + "\n");
            }

            foreach (var message in chatHistory)
            {
                sb.Append(message);
            }

            sb.Append(FormatAssistantMessagePrefix());

            string returnString = sb.ToString();

            return returnString;
        }

        public string FormatUserMessage(string message)
        {
            return $"{_sequence.UserMessagePrefix}{(_sequence.IncludeNames ? $"{_sequence.UserName}: " : "")}{message}{_sequence.UserMessageSuffix}";
        }

        public string FormatAssistantMessage(string message)
        {
            return $"{_sequence.AssistantMessagePrefix}{message}{_sequence.AssistantMessageSuffix}";
        }

        public string FormatAssistantMessagePrefix()
        {
            return $"{_sequence.AssistantMessagePrefix}{(_sequence.IncludeNames ? $"{_sequence.AssistantName}: " : "")}";
        }
    }
}
