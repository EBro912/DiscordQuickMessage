// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;



// for indexing purposes
enum Response : int
{
    YES = 0,
    NO = 1,
    MAYBE = 2,
    N_RESPONSE = 3
}

namespace quickMessage
{
    public class QuickMessage
    {

        // contains:
        private static string[] _promptPrefix = { "Accept this: ", "Decline this: ", "Give a maybe response: " };
        private static string[] _defaultResponses = { "YES", "NO", "MAYBE" };
        private string _prompt; // stores the given prompt from the constuctor
        private string[] _prompts = { String.Empty, String.Empty, String.Empty };  // stores the modified prompts to be used as completions
        private string[] _responses = { String.Empty, String.Empty, String.Empty }; // stores the generated responses for each completion
        private string _message; // stores the concatenated message string


        // struction
        public QuickMessage(string prompt)
        {
            // store original prompt
            _prompt = prompt;

            // initializes the arrays
            for (int i = 0; i < (int)Response.N_RESPONSE; i++)
            {
                _prompts[i] = _promptPrefix[i] + prompt;
                _responses[i] = _defaultResponses[i];
            }

            // make the message to be empty
            _message = String.Empty;

        }
        public QuickMessage() : this(String.Empty) { }

        // TO DO: completion generation


        // message generation
        // TO DO: RETURN VOID OR STRING?
        public void genMessage()
        {
            string s = "";
            for (int i = 0; i < _responses.Length; i++) { s += (i + 1).ToString() + ": \"" + _responses[i] + "\"\n"; }
            s += "CLICK BELOW TO SEND RESPONSE\n";
            s += ":one: :two: :three:\n";
            _message = s;
        }


        // getters for said contains
        public string prompt() => _prompt;
        public string message() => _message;
        public string yesPrompt() => _prompts[(int)Response.YES];
        public string noPrompt() => _prompts[(int)Response.NO];
        public string maybePrompt() => _prompts[(int)Response.MAYBE];
        public string yesResponse() => _responses[(int)Response.YES];
        public string noResponse() => _responses[(int)Response.NO];
        public string maybeResponse() => _responses[(int)Response.MAYBE];


        // for openai stuff
        //private static OpenAIAPI api = new OpenAIAPI(new APIAuthentication("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"));
        //var r = await api.Completions.CreateAndFormatCompletion(new CompletionRequest("Tell me something sad", temperature: 0.5, model: Model.AdaText));
        //r.ToString());

        public static string genQuickMessage(string prompt)
        {
            QuickMessage q = new QuickMessage(prompt);

            // TO DO: completions here

            q.genMessage();
            return q.message();
        }

    }
}