using Elebris_WPF_Rpg.Models;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class AttributeSetFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Values.json";

        private const int DEFAULT_BIAS_VALUE = 9;

        private const int DEFAULT_MAX_TOTAL_VALUE = 60;

        private const int DEFAULT_MAX_ATTRIBUTE_VALUE = 20;

        private static readonly List<PlayerAttribute> _baseAttributes = new List<PlayerAttribute>();

        static AttributeSetFactory()
        {
            //Initialize base values for creating new characters
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
                                 (string)node[nameof(PlayerAttribute.Abbreviation)],
                                 (string)node[nameof(PlayerAttribute.Name)],
                                 (int)node[nameof(PlayerAttribute.BaseValue)]);
                _baseAttributes.Add(attribute);
            }
        }
        //Roll new, unbiased set
        public static List<PlayerAttribute> GenerateAttributeSet()
        {
            Dictionary<string, int> emptyDict = new Dictionary<string, int>();
            return GenerateAttributeSet(emptyDict);
        }
        // return a set grouping of saved attributes
        public static Dictionary<string, StatValue> GenerateAttributeSet(Guid guid)
        {
            //load from a database, flat file etc. based on guid
            //instead of generating, we are going to grab known attribute values for the unit
            throw new NotImplementedException();
        }
        //roll a set of attributes with slight bias towards certain values
        public static List<PlayerAttribute> GenerateAttributeSet(Dictionary<string, int> classAttributes)
        {
            Dictionary<string, int> biasedAttributes = GenerateCharacterAttributeSpread(classAttributes);
            string[] convertedBiasList = GenerateBiasArray(biasedAttributes);
            Dictionary<string, int> characterAttributes = RollAttributes(convertedBiasList);

            List<PlayerAttribute> attributes = ConvertToAttributeList(characterAttributes);

            return attributes;
        }

        // this adds values to each attribute, so the biases are added to the existing dict format.
        private static Dictionary<string, int> GenerateCharacterAttributeSpread(Dictionary<string, int> biasAttributes)
        {

            Dictionary<string, int> charAttributeSpread = new Dictionary<string, int>();

            foreach (var att in _baseAttributes)
            {
                //Set base for new roll
                charAttributeSpread.Add(att.Name, att.BaseValue);
            }
            foreach (var classItem in biasAttributes)
            {   // then add biased values multiplied by the const bias value
                charAttributeSpread[classItem.Key] += classItem.Value * DEFAULT_BIAS_VALUE;
            }
            return charAttributeSpread;
        }
        // Once the method is chosen (baised vs non) for attribute rolling, this creates a 'wide' list of string-value attributes
        private static string[] GenerateBiasArray(Dictionary<string, int> biasList)
        {
            List<string> biasDataList = new List<string>();

            if (biasList != null)
            {
                for (int i = 0; i < biasList.Count; i++)//for each "Attribute slot"
                {
                    for (int j = 0; j < biasList.ElementAt(i).Value; j++)// add a number of AttributeData values to a list, minimum of base * attributes (so 3 * 6 for my uses) maximum of base * attributes plus all bias values
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
            foreach (var item in _baseAttributes)
            {
                characterAttributes[item.Name] = 1;
            }

            while (characterAttributes.Sum(item => item.Value) < DEFAULT_MAX_TOTAL_VALUE)
            {
                //for each attribute check if the random roll returned value matches
                //if it does add one to that particular attribute
                int randomRoll = rand.Next(0, convertedBiasList.Length);
                string current = convertedBiasList[randomRoll];
                characterAttributes[current] = characterAttributes[current] + 1 > DEFAULT_MAX_ATTRIBUTE_VALUE
                    ? DEFAULT_MAX_ATTRIBUTE_VALUE : characterAttributes[current] + 1;
            }
            return characterAttributes;
        }
        //I am unsure if this, or the ConvertToStatValueDict will end up being my prefered approach
        // but for now I'll use this to minimize change while testing functionality of the app
        private static List<PlayerAttribute> ConvertToAttributeList(Dictionary<string, int> characterAttributes)
        {
            List<PlayerAttribute> attributes = new List<PlayerAttribute>();
            foreach (var key in characterAttributes.Keys)
            {
                string abbreviation = _baseAttributes.First(a => a.Name.Equals(key)).Abbreviation;
                string name = key;
                int val = characterAttributes[key]; // get value from key  

                attributes.Add(new PlayerAttribute(abbreviation, name, val));
            }
            return attributes;
        }

        private static Dictionary<string, StatValue> ConvertToStatValueDict(Dictionary<string, int> characterAttributes)
        {
            Dictionary<string, StatValue> attributes = new Dictionary<string, StatValue>();
            foreach (var key in characterAttributes.Keys)
            {
                int val = characterAttributes[key]; // get value from key
                StatValue statValue = new StatValue(val); // assign value to a statValue
                attributes.Add(key, statValue); // add the statvalue in place of the basic int value and return the new dictionary
            }
            return attributes;
        }
    }
}
