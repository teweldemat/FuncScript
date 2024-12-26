using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using funcscript.core;
using funcscript.error;
using funcscript.funcs.misc;
using funcscript.model;
using OpenAI;
using OpenAI.Chat;

namespace funcscript.openai
{
    public class ChatGptFunction : IFsFunction
    {
                private static readonly string[] SupportedModels = new[] { "gpt-4o", "gpt-4o-mini" };

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            // Updated Parameter Count Check
            if (pars.Count < 3)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: too few parameters. Expected at least 3, got {pars.Count}.");

            // Retrieve API Key Parameter (First Parameter)
            var apiKey = pars.GetParameter(parent, 0)?.ToString();
            if (string.IsNullOrWhiteSpace(apiKey))
                return new FsError(FsError.ERROR_TYPE_MISMATCH,
                    $"{this.Symbol}: invalid OpenAI API key.");

            // Retrieve Model Parameter (Second Parameter)
            var model = pars.GetParameter(parent, 1)?.ToString();
            if (string.IsNullOrWhiteSpace(model))
                return new FsError(FsError.ERROR_TYPE_MISMATCH,
                    $"{this.Symbol}: invalid model.");
            if (!SupportedModels.Contains(model, StringComparer.OrdinalIgnoreCase))
                return new FsError(FsError.ERROR_TYPE_MISMATCH,
                    $"{this.Symbol}: unsupported model '{model}'.");

            // Retrieve Instruction Parameter (Third Parameter)
            var instruction = pars.GetParameter(parent, 2)?.ToString();
            if (string.IsNullOrWhiteSpace(instruction))
                return new FsError(FsError.ERROR_TYPE_MISMATCH,
                    $"{this.Symbol}: invalid instruction.");

            // Retrieve Optional System Instruction (Fourth Parameter)
            string systemInstruction = null;
            if (pars.Count >= 4)
            {
                systemInstruction = pars.GetParameter(parent, 3)?.ToString();
                if (systemInstruction == null)
                    return new FsError(FsError.ERROR_TYPE_MISMATCH,
                        $"{this.Symbol}: invalid system instruction.");
            }

            try
            {
                // Initialize OpenAI Client with Provided API Key
                var api = new OpenAIClient(apiKey);

                // Prepare Chat Messages
                var messages = new List<OpenAI.Chat.ChatMessage>();

                if (!string.IsNullOrEmpty(systemInstruction))
                {
                    messages.Add(ChatMessage.CreateSystemMessage(systemInstruction));
                }

                messages.Add(ChatMessage.CreateUserMessage(instruction));

                Fslogger.DefaultLogger.WriteLine($"ChatGPT: model:{model}\nSystem instruction:{systemInstruction}\nRequest:\n{instruction}");
                ClientResult<ChatCompletion> response = api.GetChatClient(model).CompleteChat(messages);
                

                if (response == null)
                {
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{this.Symbol}: No response from OpenAI.");
                }

                // Get the First Choice
                var choice = response.Value;

                if (choice == null)
                {
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{this.Symbol}: Invalid response structure from OpenAI.");
                }

                // Optionally, you can log or utilize choice.Index and choice.FinishReason
                // For this function, we'll return the message content
                return choice.Content[0].Text;
            }
            catch (AggregateException ae)
            {
                // Handle AggregateException which wraps the actual exception
                var ex = ae.Flatten().InnerException;
                return new FsError(FsError.ERROR_DEFAULT,
                    $"{this.Symbol}: Exception occurred - {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                return new FsError(FsError.ERROR_DEFAULT,
                    $"{this.Symbol}: Exception occurred - {ex.Message}");
            }
        }


        public string ParName(int index)
        {
            return index switch
            {
                0 => "api_key",
                1 => "model",
                2 => "instruction",
                3 => "system instruction",
                _ => ""
            };
        }

        public int MaxParsCount => 4;
        public CallType CallType => CallType.Prefix;
        public string Symbol => "ChatGPT";
        public int Precidence => 0;
    }
}