﻿using System.Collections.ObjectModel;
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

            ApplyAttributeModifiers();
        }

        public void ApplyAttributeModifiers()
        {
            foreach (PlayerAttribute playerAttribute in PlayerAttributes)
            {
                var attributeRaceModifier =
                    SelectedRace.PlayerAttributeModifiers
                                .FirstOrDefault(pam => pam.AttributeKey.Equals(playerAttribute.Abbreviation));

                playerAttribute.ModifiedValue =
                    playerAttribute.BaseValue + (attributeRaceModifier?.Modifier ?? 0);
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