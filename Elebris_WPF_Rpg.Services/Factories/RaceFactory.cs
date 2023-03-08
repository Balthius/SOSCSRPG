using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Models.Shared;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class CharacterRaceFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Races.json";
        private static readonly List<Race> _races = new List<Race>();

        static CharacterRaceFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data =
                JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                JArray nodes = (JArray)data["Races"];

                foreach (JToken token in nodes)
                {
                    Race race = new Race
                    {
                        Key = token.StringValueOf("Key"),
                        DisplayName = token.StringValueOf("DisplayName")

                    };
                    JArray mods = (JArray)token["BiasModifiers"];
                    foreach (JToken mod in mods)
                    {
                        BiasModifier new_mod = new BiasModifier
                        {
                            AttributeName = mod.StringValueOf("Key"),
                            Modifier = mod.IntValueOf("Modifier")
                        };
                        race.BiasModifiers.Add(new_mod);
                    }
                    _races.Add(race);
                }
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }
        public static List<Race> GetRaces()
        {
            List<Race> Races = new List<Race>();
            foreach (var item in _races)
            {
                Race race = new Race
                {
                    Key = item.Key ,
                    DisplayName = item.DisplayName
                };

                foreach (var mod in item.BiasModifiers)
                {
                    BiasModifier new_mod = new BiasModifier
                    { 
                        AttributeName = mod.AttributeName,
                        Modifier = mod.Modifier
                    };
                    race.BiasModifiers.Add(new_mod);
                }
                Races.Add(race);
            }
            return Races;
        }
    }

}
