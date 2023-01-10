using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Services.Factories;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class RecipeFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Recipes.json";

        private static readonly List<Recipe> _recipes = new List<Recipe>();

        static RecipeFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                JArray nodes = (JArray)data["Recipes"];
                LoadRecipesFromNodes(nodes);
             
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadRecipesFromNodes(JArray nodes)
        {
            foreach (JToken node in nodes)
            {
                var ingredients = new List<ItemQuantity>();

                foreach (JToken childNode in node["Ingredients"])
                {
                    GameItem item = ItemFactory.CreateGameItem((int)childNode["ID"]);

                    ingredients.Add(new ItemQuantity(item, (int)childNode["Quantity"]));
                }

                var outputItems = new List<ItemQuantity>();

                foreach (JToken childNode in node["OutputItems"])
                {
                    GameItem item = ItemFactory.CreateGameItem((int)childNode["ID"]);

                    outputItems.Add(new ItemQuantity(item, (int)childNode["Quantity"]));
                }

                Recipe recipe =
                    new Recipe((int)node["ID"],
                        (string)node["Name"],
                        ingredients, outputItems);

                _recipes.Add(recipe);
            }
        }

        public static Recipe RecipeByID(int id)
        {
            return _recipes.FirstOrDefault(x => x.ID == id);
        }
    }
}
