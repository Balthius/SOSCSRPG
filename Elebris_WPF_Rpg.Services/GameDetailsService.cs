﻿using Elebris_WPF_Rpg.Models;
using Elebris_WPF_Rpg.Models.Shared;
using Newtonsoft.Json.Linq;

namespace Elebris_WPF_Rpg.Services
{
    public static class GameDetailsService
    {
        public static GameDetails ReadGameDetails()
        {
            JObject gameDetailsJson =
                JObject.Parse(File.ReadAllText(".\\GameData\\GameDetails.json"));

            GameDetails gameDetails =
                new GameDetails(gameDetailsJson.StringValueOf("Title"),
                                gameDetailsJson.StringValueOf("SubTitle"),
                                gameDetailsJson.StringValueOf("Version"));

            foreach (JToken token in gameDetailsJson["PlayerAttributes"])
            {
                gameDetails.PlayerAttributes.Add(new PlayerAttribute(token.StringValueOf("Key"),
                                                                     token.StringValueOf("Name"),
                                                                     token.IntValueOf("Base")));
            }

            if (gameDetailsJson["Races"] != null)
            {
                foreach (JToken token in gameDetailsJson["Races"])
                {
                    Race race = new Race
                    {
                        Key = token.StringValueOf("Key"),
                        DisplayName = token.StringValueOf("Name")
                    };

                    if (token["PlayerAttributeModifiers"] != null)
                    {
                        foreach (JToken childToken in token["PlayerAttributeModifiers"])
                        {
                            race.PlayerAttributeModifiers.Add(new PlayerAttributeModifier
                            {
                                AttributeKey = childToken.StringValueOf("Key"),
                                Modifier = childToken.IntValueOf("Modifier")
                            });
                        }
                    }

                    gameDetails.Races.Add(race);
                }
            }

            return gameDetails;
        }
    }
}
