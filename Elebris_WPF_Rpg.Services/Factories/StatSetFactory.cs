using Elebris_WPF_Rpg.Models;

namespace Elebris_WPF_Rpg.Services.Factories
{
    public static class StatSetFactory
    {
        private static List<ValueDataModel> _startingStats = new List<ValueDataModel>();

        //Based on usage, is this more patterned as a prototype that a factory?

        public static void SetCustomStartingStats(List<ValueDataModel> initialstats)
        {
            _startingStats = initialstats;
            // This needs to be replaced with a call-out to either a locally stored file, or API that returns default stats when this factory is first called.

        }

        public static Dictionary<string, StatValue> GenerateHandlerDict(List<ValueDataModel> attributes)
        {
            Dictionary<string, StatValue> statDict = new Dictionary<string, StatValue>();
            AddAttributes(statDict, attributes);
            InitializeStats(statDict);
            return statDict;
        }

        private static void AddAttributes(Dictionary<string, StatValue> valueHandlerDict, List<ValueDataModel> attributes)
        {
            foreach (ValueDataModel stat in attributes)
            {
                valueHandlerDict.Add(stat.Name, new StatValue(stat.BaseValue));
            }
        }

        private static void InitializeStats(Dictionary<string, StatValue> valueHandlerDict)
        {

            foreach (ValueDataModel stat in _startingStats)
            {
                valueHandlerDict.Add(stat.Name, new StatValue( stat.BaseValue));
            }
        }

    }

}


