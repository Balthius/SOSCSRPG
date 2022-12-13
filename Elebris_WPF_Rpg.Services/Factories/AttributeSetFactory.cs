using Elebris_WPF_Rpg.Models;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class AttributeSetFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Values.json";

        private static readonly List<PlayerAttribute> _baseAttributes = new List<PlayerAttribute>();
        static AttributeSetFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                JObject data = JObject.Parse(File.ReadAllText(GAME_DATA_FILENAME));

                JArray nodes = (JArray)data["Attributes"];
                LoadAttributesFromNodes(nodes);
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadAttributesFromNodes(JArray nodes)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (JToken node in nodes)
            {
                PlayerAttribute attribute =
                    new PlayerAttribute(
                                 (string)node[nameof(PlayerAttribute.Key)],
                                 (string)node[nameof(PlayerAttribute.DisplayName)],
                                 (int)node[nameof(PlayerAttribute.BaseValue)]);
                _baseAttributes.Add(attribute);

            }
        }
        public static List<PlayerAttribute> GenerateAttributeSet()
        {
            string[] arr = Array.Empty<string>(); //pass no bias arguments
            return GenerateAttributeSet(arr);
        }

        public static List<PlayerAttribute> GenerateAttributeSet(Guid guid)
        { 
            //load from a database, flat file etc. based on guid
            //instead of generating, we are going to grab known attribute values for the unit
            throw new NotImplementedException();
        }
        public static List<PlayerAttribute> GenerateAttributeSet(params string[] classAttributes)
        {
            Dictionary<string, int> biasedAttributes = GenerateCharacterAttributeSpread(classAttributes);
            string[] convertedBiasList = GenerateBiasArray(biasedAttributes);
            Dictionary<string, int> characterAttributes = RollAttributes(convertedBiasList);

            List<PlayerAttribute> attributes = ConvertToStatModels(characterAttributes);

            return attributes;
        }

        private static Dictionary<string, int> GenerateCharacterAttributeSpread(params string[] biasAttributes)
        {
           
            Dictionary<string, int> charAttributeSpread = new Dictionary<string, int>();
            //For each attribute, set the biased initial value to the base value
            //then make a string comparison for each class Attribute
            //On a match, add a value to the matching attribute, to increase its likelihood of being selected
            foreach (var charItem in baseAttributeSpread.Keys.ToList())
            {
                charAttributeSpread[charItem] = baseAttributeSpread[charItem];
                foreach (var classItem in biasAttributes)
                {
                    if (charItem == classItem)
                    {
                        charAttributeSpread[charItem] += CharacterConfig.DEFAULT_CLASS_BIAS;
                    }
                }
            }
            return charAttributeSpread;
        }

        private static string[] GenerateBiasArray(Dictionary<string, int> biasList)
        {
            List<string> biasDataList = new List<string>();

            if (biasList != null)
            {
                for (int i = 0; i < biasList.Count; i++)//for each "Attribute slot"
                {
                    for (int j = 0; j < biasList.ElementAt(i).Value; j++)// add a number of AttributeData values to a list, typically totaling around 100  by the end
                    {
                        biasDataList.Add(biasList.ElementAt(i).Key);
                    }
                }
            }
            //returns a large list filled with different attribute values
            return biasDataList.ToArray();
        }

        private static Dictionary<string, int> RollAttributes(string[] convertedBiasList)
        {
            Random rand = new Random();
            Dictionary<string, int> characterAttributes = new Dictionary<string, int>();
            foreach (var item in baseAttributeSpread)
            {
                characterAttributes[item.Key] = 1;
            }

            while (characterAttributes.Sum(item => item.Value) < CharacterConfig.DEFAULT_MAX_TOTAL_VALUE)
            {
                //for each attribute check if the random roll returned value matches
                //if it does add one to that particular attribute
                int randomRoll = rand.Next(0, convertedBiasList.Length);
                string current = convertedBiasList[randomRoll];
                characterAttributes[current] = characterAttributes[current] + 1 > CharacterConfig.DEFAULT_MAX_ATTRIBUTE_VALUE
                    ? CharacterConfig.DEFAULT_MAX_ATTRIBUTE_VALUE : characterAttributes[current] + 1;
            }
            return characterAttributes;
        }
        private static List<PlayerAttribute> ConvertToStatModels(Dictionary<string, int> characterAttributes)
        {
            List<PlayerAttribute> attributes = new List<PlayerAttribute>();
            foreach (var item in characterAttributes.Keys)
            {
                CharacterStatValue val = new CharacterStatValue(characterAttributes[item]);
                PlayerAttribute model = new StatDTO { Name = item.ToString(), CharacterStat = val };
                attributes.Add(model);
            }

            return attributes;
        }
    }
}
