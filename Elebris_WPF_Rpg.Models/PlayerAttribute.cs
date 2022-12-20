using System.ComponentModel;

namespace Elebris_WPF_Rpg.Models
{
    public class PlayerAttribute : INotifyPropertyChanged
    {
        public string Abbreviation { get; }
        public string Name { get; }
        public int BaseValue { get; set; }
        public int BiasValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;



        // Constructor that takes a baseValue and also uses it for modifiedValue,
        // for when we're creating a new attribute
        public PlayerAttribute(string key, string displayName,
                               int baseValue) :
            this(key, displayName, baseValue, 0)
        {
        }

        // This constructor is eventually called by the others, 
        // or used when reading a Player's attributes from a saved game file.
        public PlayerAttribute(string abbreviation, string name,
                               int baseValue, int biasvalue)
        {
            Abbreviation = abbreviation;
            Name = name;
            BaseValue = baseValue;
            BiasValue = biasvalue;
        }
    }
}
