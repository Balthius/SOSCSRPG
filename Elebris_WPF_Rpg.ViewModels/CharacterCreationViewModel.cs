using System.Collections.ObjectModel;
using System.ComponentModel;
using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Services;
using Elebris_WPF_Rpg.Services.Factories;

namespace Elebris_WPF_Rpg.ViewModels
{
    public class CharacterCreationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GameDetails GameDetails { get; }
        public Race SelectedRace { get; init; }
        public List<Race> Races { get; init; }
        public string Name { get; init; }
        public ObservableCollection<PlayerAttribute> PlayerAttributes { get; } =
            new ObservableCollection<PlayerAttribute>(); // what will be used to create the character after we "accept" them

        public ObservableCollection<BiasModifier> BiasValues { get; } =
            new ObservableCollection<BiasModifier>(); // after character creation does not need to be recreated

        public bool HasRaces =>
            Races.Any();

        public bool HasRaceAttributeModifiers =>
            Races.Any(r => r.BiasModifiers.Any());

        public CharacterCreationViewModel()
        {
            GameDetails = GameDetailsService.ReadGameDetails();
            Races = CharacterRaceFactory.GetRaces();
            if (HasRaces)
            {
                SelectedRace = Races.First();
            }

            RollNewCharacter();
        }

        public void RollNewCharacter()
        {
            PlayerAttributes.Clear();
            // after reseting the character, take all modifiers from race, class etc and use it to create a new dict to generate stats from
            Dictionary<string, int> modifiers = new Dictionary<string, int>();
            //convert from attributeModifiers to a dict
            foreach(var mod in BiasValues)
            {
                modifiers.Add(mod.AttributeName, mod.Modifier);
            }
           List<PlayerAttribute> new_attributes = AttributeSetFactory.GenerateAttributeSet(modifiers);
            
            foreach (PlayerAttribute playerAttribute in new_attributes)
            {
                PlayerAttributes.Add(playerAttribute);
            }
        }

        private void StackBiases(string name, int modifier)
        {
            BiasModifier biasModifier = BiasValues.FirstOrDefault(pam => pam.AttributeName.Equals(name));
            if (biasModifier != null)
            {
                biasModifier.Modifier += modifier;
            }
            else
            {
                BiasModifier newModifier = new BiasModifier
                {
                    AttributeName = name,
                    Modifier = modifier
                };
                BiasValues.Add(newModifier);
            }
        }

        public void ApplyAttributeBias()
        {
           BiasValues.Clear();
            //Apply bias from Race
            foreach (BiasModifier bias in SelectedRace.BiasModifiers)
            {
               StackBiases(bias.AttributeName, bias.Modifier);
            }
            //TODO Class
        }

        public Player GetPlayer()
        {
            Player player = new Player(Name, 0, 10, 10, PlayerAttributes, 10);

            // Give player default inventory items, weapons, recipes, etc.
            player.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            player.AddItemToInventory(ItemFactory.CreateGameItem(2001));
            player.LearnRecipe(RecipeFactory.RecipeByID(1));
            player.AddItemToInventory(ItemFactory.CreateGameItem(3001));
            player.AddItemToInventory(ItemFactory.CreateGameItem(3002));
            player.AddItemToInventory(ItemFactory.CreateGameItem(3003));

            return player;
        }
    }
}