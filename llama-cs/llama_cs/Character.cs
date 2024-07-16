using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llama_cs
{
    public class Character
    {
        private string _jsonString;
        
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Personality { get; private set; }
        public string Scenario { get; private set; }
        public string FirstMessage { get; private set; }
        public string MessageExamples { get; private set; }

        public InstructSequence _instructSequence;

        public Character(string jsonString)
        {
            if (jsonString.Trim() == "")
            {
                Name = "assistant";
                Description = "";
                Personality = "";
                Scenario = "";
                FirstMessage = "";
                MessageExamples = "";
            }
            
            _instructSequence = null;

            if (jsonString.EndsWith(".json"))
            {
                jsonString = File.ReadAllText(jsonString);
            }

            _jsonString = jsonString;

            ParseJson(jsonString);
        }

        public void ParseJson(string userName)
        {
            JObject json = JObject.Parse(_jsonString);

            Name = json["name"]?.ToString() ?? json["data"]?["name"]?.ToString() ?? "";
            Description = json["description"]?.ToString() ?? json["data"]?["description"]?.ToString() ?? "";
            Description = ReplacePlaceholderNames(Description);

            Personality = json["personality"]?.ToString() ?? json["data"]?["personality"]?.ToString() ?? "";
            Personality = ReplacePlaceholderNames(Personality);

            Scenario = json["scenario"]?.ToString() ?? json["data"]?["scenario"]?.ToString() ?? "";
            Scenario = ReplacePlaceholderNames(Scenario);

            string firstMessageTemp = json["first_mes"]?.ToString() ?? json["data"]?["first_mes"]?.ToString() ?? "";
            FirstMessage = $"{_instructSequence.UserMessagePrefix}{firstMessageTemp}{_instructSequence.UserMessageSuffix}";
            FirstMessage = ReplacePlaceholderNames(FirstMessage);

            string mesExample = json["mes_example"]?.ToString() ?? json["data"]?["mes_example"]?.ToString() ?? "";
            MessageExamples = mesExample;
            // MessageExamples = ConvertMessageExamplesToInstructFormat(mesExample);
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
                    exampleMessageHistory.Add($"{_instructSequence.UserMessagePrefix}{userMessage}{_instructSequence.UserMessageSuffix}");
                }
                else if (line.StartsWith("{{char}}:"))
                {
                    string assistantMessage = line.Substring("{{char}}:".Length).Trim();
                    assistantMessage = ReplacePlaceholderNames(assistantMessage);
                    exampleMessageHistory.Add($"{_instructSequence.AssistantMessagePrefix}{assistantMessage}{_instructSequence.AssistantMessageSuffix}");
                }
            }

            return string.Join("", exampleMessageHistory);
        }

        private string ReplacePlaceholderNames(string input)
        {
            return input.Replace("{{char}}", Name);//.Replace("{{user}}", _instructSequence.UserName);
        }
    }

    public class User
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public User (string name, string description)
        {
            Name = name;
            Description = description;
        }

        public User (string name)
        {
            Name = name;
            Description = "";
        }

        public User ()
        {
            Name = "user";
            Description = "";
        }
    }
}
