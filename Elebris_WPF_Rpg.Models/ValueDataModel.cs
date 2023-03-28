using System.ComponentModel;

namespace Elebris_WPF_Rpg.Models
{


    public class ValueDataModel : INotifyPropertyChanged
    {
        public string Abbreviation { get; }
        public string Name { get; }
        public int BaseValue { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;


        // This constructor is eventually called by the others, 
        // or used when reading a Player's attributes from a saved game file.
        public ValueDataModel(string abbreviation, string name,
                               int baseValue)
        {
            Abbreviation = abbreviation;
            Name = name;
            BaseValue = baseValue;
        }
    }
}
