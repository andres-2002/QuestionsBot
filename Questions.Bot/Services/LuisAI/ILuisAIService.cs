using Microsoft.Bot.Builder.AI.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Questions.Bot.Services.LuisAI
{
    public interface ILuisAIService
    {
        public LuisRecognizer _luisRecognizer { get; set; }
    }
}
