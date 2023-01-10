using System.ComponentModel;

namespace Elebris_WPF_Rpg.Models
{
    public class PlayerAttribute : INotifyPropertyChanged
    {
        public string Abbreviation { get; }
        public string Name { get; }
        public int BaseValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        // This constructor is eventually called by the others, 
        // or used when reading a Player's attributes from a saved game file.
        public PlayerAttribute(string abbreviation, string name,
                               int baseValue)
        {
            Abbreviation = abbreviation;
            Name = name;
            BaseValue = baseValue;
        }
    }
}
