using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Models.Shared;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services
{
    public static class GameDetailsService
    {
        //'Could not find a part of the path 'C:\Users\joshf\Documents\Personal\Scott_Lilly_Convert\SOSCSRPG\WPFUI\GameData\GameDetails.json'.'
        //missing bin\Debug\net6.0-windows on my laptop. does the file path need to be altered, or do gamefiles need to be moved up to the root of the project?
        //https://jeremybytes.blogspot.com/2020/02/set-working-directory-in-visual-studio.html
        private const string GAME_DATA_FILENAME = ".\\GameData\\GameDetails.json";
        public static GameDetails ReadGameDetails()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject gameDetailsJson =
                JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                GameDetails gameDetails =
                new GameDetails(gameDetailsJson.StringValueOf("Title"),
                                gameDetailsJson.StringValueOf("SubTitle"),
                                gameDetailsJson.StringValueOf("Version"));
                return gameDetails;
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }



        }
    }
}
