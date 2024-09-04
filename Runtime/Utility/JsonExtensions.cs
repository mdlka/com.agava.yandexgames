using System.Collections.Generic;
using System.Text;

namespace Agava.YandexGames
{
    internal static class JsonExtensions
    {
        private enum IterationState
        {
            StartingKeyQuote,
            Key,
            StartingValueQuote,
            Value
        }
        
        internal static string ToJson(this Dictionary<string, string> dictionary)
        {
            var jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.Append('{');

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                string maskedValue = MaskJsonString(pair.Value);
                jsonStringBuilder.Append($"\"{pair.Key}\":\"{maskedValue}\",");
            }

            if (dictionary.Count > 0)
                jsonStringBuilder.Length -= 1;

            jsonStringBuilder.Append('}');

            return jsonStringBuilder.ToString();
        }

        internal static Dictionary<string, string> DictionaryFromJson(this string json)
        {
            if (string.IsNullOrEmpty(json))
                json = "{}";

            string unparsedData = json.Trim('{', '}');

            var key = new StringBuilder();
            var value = new StringBuilder();
            var result = new Dictionary<string, string>();

            IterationState iterationState = IterationState.StartingKeyQuote;

            int characterIterator = 0;
            while (characterIterator < unparsedData.Length)
            {
                char character = unparsedData[characterIterator];

                switch (iterationState)
                {
                    case IterationState.StartingKeyQuote:
                        if (character == '"')
                        {
                            iterationState = IterationState.Key;
                        }

                        break;

                    case IterationState.Key:
                        if (character == '"')
                        {
                            iterationState = IterationState.StartingValueQuote;
                        }
                        else
                        {
                            key.Append(character);
                        }

                        break;

                    case IterationState.StartingValueQuote:
                        if (character == '"')
                        {
                            iterationState = IterationState.Value;
                        }

                        break;

                    case IterationState.Value:
                        if (character == '"')
                        {
                            iterationState = IterationState.StartingKeyQuote;

                            string unmaskedValue = UnmaskJsonString(value.ToString());
                            result[key.ToString()] = unmaskedValue;
                            key.Clear();
                            value.Clear();
                        }
                        else
                        {
                            value.Append(character);
                        }

                        break;
                }

                characterIterator += 1;
            }

            return result;
        }
        
        private static string MaskJsonString(string value)
        {
            const string openBracketMask = "#OBM#";
            const string closeBracketMask = "#CBM#";
            const string quoteMask = "#QuM#";
            
            var stringBuilder = new StringBuilder();
            
            int characterIterator = 0;
            while (characterIterator < value.Length)
            {
                char character = value[characterIterator];
                
                switch (character)
                {
                    case '{':
                        stringBuilder.Append(openBracketMask);
                        break;
                    case '}':
                        stringBuilder.Append(closeBracketMask);
                        break;
                    case '"':
                        stringBuilder.Append(quoteMask);
                        break;
                    default:
                        stringBuilder.Append(character);
                        break;
                }

				characterIterator += 1;
            }
            
            return stringBuilder.ToString();
        }

        private static string UnmaskJsonString(string value)
        {
            const string openBracketMask = "#OBM#";
            const string closeBracketMask = "#CBM#";
            const string quoteMask = "#QuM#";
            const char checker = '#';
            const int maskSize = 5;
            
            var stringBuilder = new StringBuilder();
            
            int characterIterator = 0;
            while (characterIterator < value.Length)
            {
                char character = value[characterIterator];

                if (characterIterator <= value.Length - maskSize && character == checker)
                {
                    string subString = value.Substring(characterIterator, maskSize);
                    
                    switch (subString)
                    {
                        case openBracketMask:
                            stringBuilder.Append('{');
                            characterIterator += maskSize;
                            break;
                        case closeBracketMask:
                            stringBuilder.Append('}');
                            characterIterator += maskSize;
                            break;
                        case quoteMask:
                            stringBuilder.Append('"');
                            characterIterator += maskSize;
                            break;
                        default:
                            stringBuilder.Append(character);
                            characterIterator += 1;
                            break;
                    }
                }
                else
                {
                    stringBuilder.Append(character);
                    characterIterator += 1;
                }
            }
            
            return stringBuilder.ToString();
        }
    }
}