using System.ComponentModel;

namespace Elebris_WPF_Rpg.Models
{
    public class PlayerAttribute : INotifyPropertyChanged
    {
        public string Key { get; }
        public string DisplayName { get; }
        public int BaseValue { get; set; }
        public int ModifiedValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;



        // Constructor that takes a baseValue and also uses it for modifiedValue,
        // for when we're creating a new attribute
        public PlayerAttribute(string key, string displayName,
                               int baseValue) :
            this(key, displayName, baseValue, baseValue)
        {
        }

        // This constructor is eventually called by the others, 
        // or used when reading a Player's attributes from a saved game file.
        public PlayerAttribute(string key, string displayName,
                               int baseValue, int modifiedValue)
        {
            Key = key;
            DisplayName = displayName;
            BaseValue = baseValue;
            ModifiedValue = modifiedValue;
        }
    }
}
