using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace llama_cs
{
    public class LlmParameters
    {
        public bool? cache_prompt { get; set; }
        public int? n_predict { get; set; }
        public double? top_p { get; set; }

        public double? top_k { get; set; }
        public double? min_p { get; set; }
        public double? typical_p { get; set; }
        public double? temperature { get; set; }
        public double? repeat_penalty { get; set; }
        public double? frequency_penalty { get; set; }
        public double? presence_penalty { get; set; }
        public int? seed { get; set; }
        public List<List<int>> logit_bias { get; set; }
        public string grammar { get; set; }

        public LlmParameters()
        {
            cache_prompt = false;
            n_predict = 500;
            temperature = 1;
            top_k = 20;
            top_p = 0.9;
            min_p = 0.05;
            typical_p = 1;
            repeat_penalty = 1.05;
            frequency_penalty = 0.0;
            presence_penalty = 0.0;
            grammar = null;
            logit_bias = new List<List<int>>();
        }

        public void LogitBiasFromString(string biasString)
        {
            // Example valid string: `23586: -100, 27441: -100, 37988: -100, 125785: -100, 166340: -100, 205708: -100, 215237: -100, 236160: -100`

            // Remove any whitespace and line breaks
            biasString = Regex.Replace(biasString, @"\s+", "");

            var biasEntries = biasString.Split(',');

            logit_bias.Clear();

            foreach (var entry in biasEntries)
            {
                var parts = entry.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int tokenId) && int.TryParse(parts[1], out int bias))
                {
                    logit_bias.Add(new List<int> { tokenId, bias });
                }
                else
                {
                    throw new ArgumentException($"Invalid bias entry: {entry}");
                }
            }
            Console.Beep();
        }

        public void AddLogitBias(int tokenId, int bias)
        {
            // Check if there's already an entry for this tokenId
            var existingEntry = logit_bias.FirstOrDefault(entry => entry[0] == tokenId);

            if (existingEntry != null)
            {
                existingEntry[1] = bias;
            }
            else
            {
                logit_bias.Add(new List<int> { tokenId, bias });
            }
        }
    }
}
