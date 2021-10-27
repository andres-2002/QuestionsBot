using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Questions.Bot.Common.Models.EntityModel
{
    public class EntityLuis
    {
        [JsonProperty("$instance")]
        public Instance _instance { get; set; }
        public List<List<string>> ListQuestions { get; set; }
    }

    public class Instance
    {
        public List<ListQuestions> ListQuestions { get; set; }
    }
    public class ListQuestions
    {
        public string type { get; set; }
        public string text { get; set; }
        public string modelType { get; set; }
    }
}
