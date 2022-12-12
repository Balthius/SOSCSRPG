using Elebris_WPF_Rpg.Models;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class MonsterFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Monsters.json";

        private static readonly GameDetails s_gameDetails;
        private static readonly List<Monster> s_baseMonsters = new List<Monster>();

        static MonsterFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                s_gameDetails = GameDetailsService.ReadGameDetails();

                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                string rootImagePath = (string)data["RootImagePath"];
                JArray nodes = (JArray)data["Monsters"];
                LoadMonstersFromNodes(nodes, rootImagePath);



            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        public static Monster GetMonsterFromLocation(Location location)
        {
            if (!location.MonstersHere.Any())
            {
                return null;
            }

            // Total the percentages of all monsters at this location.
            int totalChances = location.MonstersHere.Sum(m => m.ChanceOfEncountering);

            // Select a random number between 1 and the total (in case the total chances is not 100).
            int randomNumber = 40;

            // Loop through the monster list, 
            // adding the monster's percentage chance of appearing to the runningTotal variable.
            // When the random number is lower than the runningTotal,
            // that is the monster to return.
            int runningTotal = 0;

            foreach (MonsterEncounter monsterEncounter in location.MonstersHere)
            {
                runningTotal += monsterEncounter.ChanceOfEncountering;

                if (randomNumber <= runningTotal)
                {
                    return GetMonster(monsterEncounter.MonsterID);
                }
            }

            // If there was a problem, return the last monster in the list.
            return GetMonster(location.MonstersHere.Last().MonsterID);
        }

        private static void LoadMonstersFromNodes(JArray nodes, string rootImagePath)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (JToken node in nodes)
            {
                var attributes = s_gameDetails.PlayerAttributes;

                attributes.First(a => a.Key.Equals("AGI")).BaseValue =
                    Convert.ToInt32(node["Agility"]);
                attributes.First(a => a.Key.Equals("AGI")).ModifiedValue =
                    Convert.ToInt32(node["Agility"]);

                Monster monster =
                    new Monster((int)node["ID"],
                                (string)node["Name"],
                                $".{rootImagePath}{(string)node["ImageName"]}",
                                (int)node["MaximumHitPoints"],
                                attributes,
                                ItemFactory.CreateGameItem((int)node["WeaponID"]),
                                (int)node["RewardXP"],
                                (int)node["Gold"]);

                if (node["LootItems"] != null)
                {
                    foreach (JToken lootItemNode in node["LootItems"])
                    {
                        monster.AddItemToLootTable((int)lootItemNode["ID"],
                                                   (int)lootItemNode["Percentage"]);
                    }
                }

                s_baseMonsters.Add(monster);
            }
        }

        private static Monster GetMonster(int id)
        {
            Monster newMonster = s_baseMonsters.FirstOrDefault(m => m.ID == id).Clone();
            Random rand = new Random();
            foreach (ItemPercentage itemPercentage in newMonster.LootTable)
            {
                // Populate the new monster's inventory, using the loot table
                if (rand.Next(100) <= itemPercentage.Percentage)
                {
                    newMonster.AddItemToInventory(ItemFactory.CreateGameItem(itemPercentage.ID));
                }
            }

            return newMonster;
        }
    }
}