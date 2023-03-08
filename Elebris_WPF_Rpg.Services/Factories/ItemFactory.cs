using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Models.Actions;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class ItemFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\GameItems.json";

        private static readonly List<GameItem> _standardGameItems = new List<GameItem>();

        static ItemFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                JToken gameItems = (JToken)data["GameItems"];

                LoadItemsFromNodes(gameItems, "Weapons");
                LoadItemsFromNodes(gameItems, "HealingItems");
                LoadItemsFromNodes(gameItems, "MiscellaneousItems");
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        public static GameItem CreateGameItem(int itemTypeID)
        {
            return _standardGameItems.FirstOrDefault(item => item.ItemTypeID == itemTypeID)?.Clone();
        }

        private static void LoadItemsFromNodes(JToken parent, string category)
        {
            JArray nodes = (JArray)parent[category];
            if (nodes == null)
            {
                return;
            }

            foreach (JToken node in nodes)
            {
                GameItem.ItemCategory itemCategory = DetermineItemCategory(category);

                GameItem gameItem =
                    new GameItem(itemCategory,
                                 (int)node["ID"],
                                 (string)node[nameof(gameItem.Name)],
                                 (int)node[nameof(gameItem.Price)]);

                if (itemCategory == GameItem.ItemCategory.Weapon)
                {
                    gameItem.Action =
                        new AttackWithWeapon(gameItem, (int)node["Damage"]);
                }
                else if (itemCategory == GameItem.ItemCategory.Consumable)
                {
                    gameItem.Action =
                        new Heal(gameItem,
                                 (int)node["HitPointsToHeal"]);
                }

                _standardGameItems.Add(gameItem);
            }
        }

        private static GameItem.ItemCategory DetermineItemCategory(string itemType)
        {
            switch (itemType)
            {
                case "Weapons":
                    return GameItem.ItemCategory.Weapon;
                case "HealingItems":
                    return GameItem.ItemCategory.Consumable;
                case "MiscellaneousItems":
                    return GameItem.ItemCategory.Miscellaneous;
                default:
                    return GameItem.ItemCategory.Miscellaneous;
            }
        }
    }
}
