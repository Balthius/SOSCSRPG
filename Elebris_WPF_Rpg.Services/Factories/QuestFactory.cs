using Elebris_WPF_Rpg.Models;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    internal static class QuestFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Quests.json";

        private static readonly List<Quest> _quests = new List<Quest>();

        static QuestFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                JArray nodes = (JArray)data["Quests"];
                LoadQuestsFromNodes(nodes);
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadQuestsFromNodes(JArray nodes)
        {
            foreach (JToken node in nodes)
            {
                // Declare the items need to complete the quest, and its reward items
                List<ItemQuantity> itemsToComplete = new List<ItemQuantity>();
                List<ItemQuantity> rewardItems = new List<ItemQuantity>();

                foreach (JToken childNode in node["ItemsToComplete"])
                {
                    GameItem item = ItemFactory.CreateGameItem((int)childNode["ID"]);

                    itemsToComplete.Add(new ItemQuantity(item, (int)childNode["Quantity"]));
                }

                foreach (JToken childNode in node["RewardItems"])
                {
                    GameItem item = ItemFactory.CreateGameItem((int)childNode["ID"]);

                    rewardItems.Add(new ItemQuantity(item, (int)childNode["Quantity"]));
                }

                _quests.Add(new Quest((int)node["ID"],
                                      (string)node["Name"],
                                      (string)node["Description"],
                                      itemsToComplete,
                                      (int)node["RewardExperiencePoints"],
                                      (int)node["RewardGold"],
                                      rewardItems));
            }
        }

        internal static Quest GetQuestByID(int id)
        {
            return _quests.FirstOrDefault(quest => quest.ID == id);
        }
    }
}
