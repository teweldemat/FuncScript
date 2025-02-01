using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Openai
{
    public class ChatGptFunction : IFsFunction
    {
        private static readonly string[] SupportedModels = 
        { 
            "gpt-3.5-turbo", 
            "gpt-4",
            "gpt-4o", 
            "gpt-4o-mini", 
            "o1-mini", 
            "o1" 
        };

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 3)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol}: too few parameters. Expected at least 3, got {pars.Length}.");

            var apiKey = pars[0]?.ToString();
            if (string.IsNullOrWhiteSpace(apiKey))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{Symbol}: invalid OpenAI API key.");

            var model = pars[1]?.ToString();
            if (string.IsNullOrWhiteSpace(model) 
                || !SupportedModels.Contains(model, StringComparer.OrdinalIgnoreCase))
            {
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{Symbol}: unsupported or missing model '{model}'.");
            }

            var systemInstruction = (pars.Length >= 4) ? pars[3]?.ToString() : null;

            if (pars[2] is FsList conversationList)
            {
                try
                {
                    var messages = new List<object>();
                    if (!string.IsNullOrEmpty(systemInstruction))
                        messages.Add(new { role = "developer", content = systemInstruction });

                    for (int i = 0; i < conversationList.Length; i++)
                    {
                        var content = conversationList[i]?.ToString() ?? "";
                        messages.Add(new
                        {
                            role = (i % 2 == 0) ? "user" : "assistant",
                            content
                        });
                    }

                    var responseData = SendChatCompletionRequest(apiKey, model, messages);
                    if (responseData?.choices == null || responseData.choices.Count == 0)
                        return new FsError(FsError.ERROR_DEFAULT,
                            $"{Symbol}: invalid response structure from OpenAI.");

                    return responseData.choices[0].message.content;
                }
                catch (Exception ex)
                {
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: Exception occurred - {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            else
            {
                var instruction = pars[2]?.ToString();
                if (string.IsNullOrWhiteSpace(instruction))
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                        $"{Symbol}: invalid instruction.");

                try
                {
                    var messages = new List<object>();
                    if (!string.IsNullOrEmpty(systemInstruction))
                            messages.Add(new { role = "developer", content = systemInstruction });

                    messages.Add(new { role = "user", content = instruction });

                    var responseData = SendChatCompletionRequest(apiKey, model, messages);
                    if (responseData?.choices == null || responseData.choices.Count == 0)
                        return new FsError(FsError.ERROR_DEFAULT,
                            $"{Symbol}: invalid response structure from OpenAI.");

                    return responseData.choices[0].message.content;
                }
                catch (Exception ex)
                {
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: Exception occurred - {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }

        private ChatCompletionsResponse SendChatCompletionRequest(
            string apiKey, string model, List<object> messages)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model,
                messages
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(
                "https://api.openai.com/v1/chat/completions", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception($"OpenAI request failed with status code: {response.StatusCode}: {responseString}");

            return JsonConvert.DeserializeObject<ChatCompletionsResponse>(responseString);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "api_key",
                1 => "model",
                2 => "prompt_or_conversation",
                3 => "system_instruction",
                _ => ""
            };
        }

        public CallType CallType => CallType.Prefix;
        public string Symbol => "ChatGPT";
    }

    public class ChatCompletionsResponse
    {
        public string id { get; set; }
        public string model { get; set; }
        public long created { get; set; }
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }
        public string finish_reason { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}