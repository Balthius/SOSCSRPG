using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Models.Shared;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class WorldFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Locations.json";

        public static World CreateWorld()
        {
            World world = new World();

            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                string rootImagePath = (string)data["RootImagePath"];
                JArray nodes = (JArray)data["Locations"];
                LoadLocationsFromNodes(world,
                                       rootImagePath,
                                      nodes);
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }

            return world;
        }

        private static void LoadLocationsFromNodes(World world, string rootImagePath, JArray nodes)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (JToken node in nodes)
            {
                Location location =
                    new Location((int)node["X"],
                                (int)node["Y"],
                                (string)node["Name"],
                                 (string)node["Description"],
                                 $".{rootImagePath}{(string)node["ImageName"]}");

                AddMonsters(location, (JArray)node["Monsters"]);
                AddQuests(location, (JArray)node["Quests"]);
                AddTrader(location, (JToken)node["Trader_Id"]);

                world.AddLocation(location);
            }
        }

        private static void AddMonsters(Location location, JArray monsters)
        {
            if (monsters == null)
            {
                return;
            }

            foreach (JToken monster in monsters)
            {
                location.AddMonster((int)monster["ID"],
                                    (int)monster["Percent"]);
            }
        }

        private static void AddQuests(Location location, JArray quests)
        {
            if (quests == null)
            {
                return;
            }

            foreach (JToken quest in quests)
            {
                location.QuestsAvailableHere
                        .Add(QuestFactory.GetQuestByID((int)quest["ID"]));
            }
        }

        private static void AddTrader(Location location, JToken trader)
        {
            if (trader == null)
            {
                return;
            }

            location.TraderHere =
                TraderFactory.GetTraderByID((int)trader);
        }
    }
}
