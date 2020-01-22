using Newtonsoft.Json;
using System.Collections.Generic;
namespace RockPaperScissor
{
    public class Game
    {

        public enum Move { Rock, Paper, Scissors }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string GameName { get; set; }
        public string CreatorName { get; set; }
        public string JoinerName { get; set; }
        public List<Move> CreatorMove { get; set; }
        public List<Move> JoinerMove { get; set; }
        public int CreatorScore { get; set; }
        public int JoinerScore { get; set; }
        public bool IsGameCompleted { get; set; }
        //public override string ToString()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}
    }
}
