using Elebris_WPF_Rpg.Models;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class TraderFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Traders.json";

        private static readonly List<Trader> _traders = new List<Trader>();

        static TraderFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                JArray nodes = (JArray)data["Traders"];
                LoadTradersFromNodes(nodes);
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadTradersFromNodes(JArray nodes)
        {
            foreach (JToken node in nodes)
            {
                Trader trader =
                    new Trader((int)node["ID"],
                               (string)node["Name"]);

                foreach (JToken childNode in node["InventoryItems"])
                {
                    int quantity = (int)childNode["Quantity"];

                    // Create a new GameItem object for each item we add.
                    // This is to allow for unique items, like swords with enchantments.
                    for (int i = 0; i < quantity; i++)
                    {
                        trader.AddItemToInventory(ItemFactory.CreateGameItem((int)childNode["ID"]));
                    }
                }
                _traders.Add(trader);
            }
        }

        public static Trader GetTraderByID(int id)
        {
            return _traders.FirstOrDefault(t => t.ID == id);
        }
    }
}
