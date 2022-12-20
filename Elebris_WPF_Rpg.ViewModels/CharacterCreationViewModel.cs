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
        public string Name { get; init; }
        public ObservableCollection<PlayerAttribute> PlayerAttributes { get; } =
            new ObservableCollection<PlayerAttribute>();

        public bool HasRaces =>
            GameDetails.Races.Any();

        public bool HasRaceAttributeModifiers =>
            HasRaces && GameDetails.Races.Any(r => r.PlayerAttributeModifiers.Any());

        public CharacterCreationViewModel()
        {
            GameDetails = GameDetailsService.ReadGameDetails();

            if (HasRaces)
            {
                SelectedRace = GameDetails.Races.First();
            }

            RollNewCharacter();
        }

        public void RollNewCharacter()
        {
            PlayerAttributes.Clear();
            Dictionary<string, int> modifiers = new Dictionary<string, int>();
            foreach(PlayerAttributeModifier mod in SelectedRace.PlayerAttributeModifiers)
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
            foreach (PlayerAttribute playerAttribute in PlayerAttributes)
            {
                var attributeRaceModifier =
                    SelectedRace.PlayerAttributeModifiers
                                .FirstOrDefault(pam => pam.AttributeName.Equals(playerAttribute.Name));

                int val = attributeRaceModifier?.Modifier ?? 0;
                playerAttribute.BiasValue = val;
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