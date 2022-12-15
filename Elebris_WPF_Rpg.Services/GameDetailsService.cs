using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Models.Shared;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services
{
    public static class GameDetailsService
    {
        public static GameDetails ReadGameDetails()
        {
            //missing bin\Debug\net6.0-windows on my laptop. does the file path need to be altered, or do gamefiles need to be moved up to the root of the project?
            JObject gameDetailsJson =
                JObject.Parse(File.ReadAllText(".\\GameData\\GameDetails.json"));

            GameDetails gameDetails =
                new GameDetails(gameDetailsJson.StringValueOf("Title"),
                                gameDetailsJson.StringValueOf("SubTitle"),
                                gameDetailsJson.StringValueOf("Version"));
            if (gameDetailsJson["Races"] != null)
            {
                foreach (JToken token in gameDetailsJson["Races"])
                {
                    Race race = new Race
                    {
                        Key = token.StringValueOf("Key"),
                        DisplayName = token.StringValueOf("DisplayName")
                    };
                    gameDetails.Races.Add(race);
                }
            }

            return gameDetails;
        }
    }
}
