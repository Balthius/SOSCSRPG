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
        private List<Race> Races { get; init; }
        public string Name { get; init; }
        public ObservableCollection<PlayerAttribute> PlayerAttributes { get; } =
            new ObservableCollection<PlayerAttribute>(); // what will be used to create the character after we "accept" them

        public ObservableCollection<PlayerAttributeModifier> BiasValues { get; } =
            new ObservableCollection<PlayerAttributeModifier>(); // after character creation does not need to be recreated

        public bool HasRaces =>
            Races.Any();

        public bool HasRaceAttributeModifiers =>
            Races.Any(r => r.PlayerAttributeModifiers.Any());

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

        public void ApplyAttributeBias()
        {
            //Apply bias from Race, Class
            foreach (PlayerAttributeModifier bias in BiasValues)
            {

                //Get Values From Race
                PlayerAttributeModifier attributeRaceModifier =
                    SelectedRace.PlayerAttributeModifiers
                                .FirstOrDefault(pam => pam.AttributeName.Equals(bias.AttributeName));

                int raceVal = attributeRaceModifier?.Modifier ?? 0;
                bias.Modifier += raceVal;
                // End Raceval
            }
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