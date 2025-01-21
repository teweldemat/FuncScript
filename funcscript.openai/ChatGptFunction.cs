using System.ClientModel;
using FuncScript.Core;
using FuncScript.Model;
using OpenAI;
using OpenAI.Chat;

namespace FuncScript.Openai
{
    public class ChatGptFunction : IFsFunction
    {
        private static readonly string[] SupportedModels = new[] { "gpt-4o", "gpt-4o-mini","o1-mini" };
        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol}: too few parameters. Expected at least 2, got {pars.Length}.");

            var apiKey = pars[0]?.ToString();
            if (string.IsNullOrWhiteSpace(apiKey))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{Symbol}: invalid OpenAI API key.");
            
            var model = pars[1]?.ToString();
            if (string.IsNullOrWhiteSpace(model))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{Symbol}: invalid model.");
            if (!SupportedModels.Contains(model, StringComparer.OrdinalIgnoreCase))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{Symbol}: unsupported model '{model}'.");

            if (pars.Length < 3)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol}: too few parameters. Expected at least 3, got {pars.Length}.");

            var systemInstruction = pars.Length >= 4
                ? pars[3]?.ToString()
                : null;

            // If second parameter is a FsList, treat it as the conversation
            if (pars[2] is FsList conversationList)
            {
                if (conversationList.Length % 2 == 0)
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                        $"{Symbol}: the conversation must have an odd number of items.");


                try
                {
                    var api = new OpenAIClient(apiKey);
                    var messages = new List<ChatMessage>();

                    if (!string.IsNullOrEmpty(systemInstruction))
                    {
                        messages.Add(ChatMessage.CreateSystemMessage(systemInstruction));
                    }

                    for (int i = 0; i < conversationList.Length; i++)
                    {
                        var content = conversationList[i]?.ToString() ?? "";
                        messages.Add(i % 2 == 0
                            ? ChatMessage.CreateUserMessage(content)
                            : ChatMessage.CreateAssistantMessage(content));
                    }

                    var client = api.GetChatClient(model);
                    var response = client.CompleteChat(messages);
                    if (response == null)
                    {
                        return new FsError(FsError.ERROR_DEFAULT,
                            $"{Symbol}: No response from OpenAI.");
                    }

                    var choice = response.Value;
                    if (choice == null)
                    {
                        return new FsError(FsError.ERROR_DEFAULT,
                            $"{Symbol}: Invalid response structure from OpenAI.");
                    }

                    return choice.Content[0].Text;
                }
                catch (AggregateException ae)
                {
                    var ex = ae.Flatten().InnerException;
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: Exception occurred - {ex.Message}");
                }
                catch (Exception ex)
                {
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: Exception occurred - {ex.Message}");
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
                    var api = new OpenAIClient(apiKey);
                    var messages = new List<ChatMessage>();

                    if (!string.IsNullOrEmpty(systemInstruction))
                    {
                        messages.Add(ChatMessage.CreateSystemMessage(systemInstruction));
                    }

                    messages.Add(ChatMessage.CreateUserMessage(instruction));

                    var client = api.GetChatClient(model);
                    var response = client.CompleteChat(messages);
                    if (response == null)
                    {
                        return new FsError(FsError.ERROR_DEFAULT,
                            $"{Symbol}: No response from OpenAI.");
                    }

                    var choice = response.Value;
                    if (choice == null)
                    {
                        return new FsError(FsError.ERROR_DEFAULT,
                            $"{Symbol}: Invalid response structure from OpenAI.");
                    }

                    return choice.Content[0].Text;
                }
                catch (AggregateException ae)
                {
                    var ex = ae.Flatten().InnerException;
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: Exception occurred - {ex.Message}");
                }
                catch (Exception ex)
                {
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: Exception occurred - {ex.Message}");
                }
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "api_key",
                1 => "model_or_conversation",
                2 => "instruction_or_model",
                3 => "system_instruction",
                _ => ""
            };
        }

        public CallType CallType => CallType.Prefix;
        public string Symbol => "ChatGPT";
    }
}