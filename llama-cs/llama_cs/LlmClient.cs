using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llama_cs
{
    public class LlmClient
    {
        private readonly string _apiUrl;
        private readonly InstructFormatter _formatter;
        private readonly HttpClient _httpClient;
        private LlmParameters _apiParameters;
        private Character _assistantCharacter;
        private User _user;

        public List<string> ChatHistory { get; } = new List<string>();
        public List<string> RawChatHistory { get; } = new List<string>();

        public LlmClient(InstructSequence sequence, LlmParameters llmParameters = null, Character assistantCharacter = null, User user = null, int port = 8080)
        {
            if (user == null)
            {
                user = new User("user");
            }
            if (sequence == null)
            {
                sequence = new InstructSequence();
            }

            if (user != null)
            {
                sequence.User = user;
            }

            if (llmParameters == null)
            {
                llmParameters = new LlmParameters();
            }
            else if (llmParameters.grammar == "japanese")
            {
                llmParameters.grammar = "root ::= (japanese-char | asterisk-token | quote-token | other-punctuation | special-token)+\n\njapanese-char ::= [ぁ-ゟァ-ヿ一-龯]\n\nasterisk-token ::= [^*]* \"*\" [^*]*\n\nquote-token ::= [^\"]* \"\\\"\" [^\"]*\n\nother-punctuation ::= [、。！？．：；｝｛ー～]\n\nspecial-token ::= \"<\" [^>]+ \">\" | \"<|\" [^|>]+ \"|>\" | \"[\" [^\\]]+ \"]\"";
            }

            assistantCharacter._instructSequence = sequence;
            sequence.User.Name = user.Name;
            sequence.AssistantName = assistantCharacter.Name;
            sequence.UpdateSystemMessage();

            _apiUrl = $"http://localhost:{port}/completion";
            _formatter = new InstructFormatter(sequence);
            _httpClient = new HttpClient();
            _apiParameters = llmParameters;
            _assistantCharacter = assistantCharacter;
            _user = user;
        }

        public void SetApiParameter<T>(string parameterName, T value)
        {
            var property = typeof(LlmParameters).GetProperty(parameterName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(_apiParameters, value);
            }
            else
            {
                throw new ArgumentException($"Invalid parameter name: {parameterName}");
            }
        }

        public void SetLlmParameters(LlmParameters parameters)
        {
            _apiParameters = parameters;
        }

        public void AddUserMessage(string message)
        {
            ChatHistory.Add($"{_formatter._sequence.User.Name}: {message}");
            RawChatHistory.Add(_formatter.FormatUserMessage(message));
        }

        public void AddAssistantMessage(string message)
        {
            ChatHistory.Add($"{_formatter._sequence.AssistantName}: {message}");
            RawChatHistory.Add(_formatter.FormatAssistantMessage(message));
        }

        public async Task<string> GetResponse(string userMessage)
        {
            AddUserMessage(userMessage);

            var instructString = _formatter.FormatMessage(RawChatHistory, userMessage, _assistantCharacter);

            var response = await SendCompletionRequest(instructString);

            var assistantResponse = TrimResponse(response);
            AddAssistantMessage(assistantResponse);

            return assistantResponse;
        }

        private async Task<string> SendCompletionRequest(string instructString)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "prompt", instructString }
            };

            foreach (var prop in typeof(LlmParameters).GetProperties())
            {
                var value = prop.GetValue(_apiParameters);
                if (value != null)
                {
                    requestBody[prop.Name] = value;
                }
            }

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var completionResponse = JsonConvert.DeserializeAnonymousType(responseBody, new { content = "" });

            string responseString = completionResponse.content.Trim().Replace(".  ", ". ");

            return responseString;
        }

        private string TrimResponse(string response)
        {
            var suffixIndex = response.IndexOf(_formatter._sequence.AssistantMessageSuffix);
            return suffixIndex >= 0 ? response.Substring(0, suffixIndex) : response;
        }

        public string GetInstructString()
        {
            return _formatter.FormatMessage(RawChatHistory, "", _assistantCharacter);
        }
    }
}
