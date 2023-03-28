using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Models.Shared
{
    public static class ExtensionMethods
    {
        public static string StringValueOf(this JObject jsonObject, string key)
        {
            return jsonObject[key].ToString();
        }

        public static string StringValueOf(this JToken jsonToken, string key)
        {
            return jsonToken[key].ToString();
        }

        public static int IntValueOf(this JToken jsonToken, string key)
        {
            return Convert.ToInt32(jsonToken[key]);
        }

        public static ValueDataModel GetAttribute(this LivingEntity entity, string attributeKey)
        {
            return entity.Attributes
                         .First(pa => pa.Abbreviation.Equals(attributeKey,
                                                    StringComparison.CurrentCultureIgnoreCase));
        }

        public static List<GameItem> ItemsThatAre(this IEnumerable<GameItem> inventory,
            GameItem.ItemCategory category)
        {
            return inventory.Where(i => i.Category == category).ToList();
        }
    }
}
