﻿using System;
using System.Linq;
using BattleTech;

namespace PainTolerance {
    public class PilotHelper {

        public static int GetGunneryModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Gunnery, "AbilityDefG5", "AbilityDefG8");
        }

        public static int GetGutsModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Guts, "AbilityDefGu5", "AbilityDefGu8");
        }

        public static int GetPilotingModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Piloting, "AbilityDefP5", "AbilityDefP8");
        }

        public static int GetTacticsModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Tactics, "AbilityDefT5A", "AbilityDefT8A");
        }

        public static int GetModifier(Pilot pilot, int skillValue, string abilityDefIdL5, string abilityDefIdL8) {
            int normalizedVal = NormalizeSkill(skillValue);
            int mod = normalizedVal;
            foreach (Ability ability in pilot.Abilities.Distinct()) {
                //PainTolerance.Logger.LogIfDebug($"Pilot {pilot.Name} has ability:{ability.Def.Id}.");
                if (ability.Def.Id.ToLower().Equals(abilityDefIdL5.ToLower()) || ability.Def.Id.ToLower().Equals(abilityDefIdL8.ToLower())) {
                    Mod.Log.Debug($"Pilot {pilot.Name} has targeted ability:{ability.Def.Id}, boosting their modifier.");
                    mod += 1;
                } 

            }
            Mod.Log.Debug($"Pilot {pilot.Name} has final modifier:{mod} from normalizedSkill:{normalizedVal} and ability boosts.");
            return mod;
        }

        private static int NormalizeSkill(int rawValue) {
            int normalizedVal = rawValue;
            if (rawValue >= 11 && rawValue <= 14) {
                // 11, 12, 13, 14 normalizes to 11
                normalizedVal = 11;
            } else if (rawValue >= 15 && rawValue <= 18) {
                // 15, 16, 17, 18 normalizes to 14
                normalizedVal = 12;
            } else if (rawValue == 19 || rawValue == 20) {
                // 19, 20 normalizes to 13
                normalizedVal = 13;
            } else if (rawValue <= 0) {
                normalizedVal = 1;
            } else if (rawValue > 20) {
                normalizedVal = 13;
            }
            return normalizedVal;
        }

        public static bool MakeResistCheck(Pilot pilot) {
            int normalizedGunnery = PilotHelper.GetGunneryModifier(pilot);
            float baseResist = normalizedGunnery * Mod.Config.ResistPerGuts;
            float resistPenalty = ModState.InjuryResistPenalty;
            float resistChance = Math.Max(0, baseResist - resistPenalty);
            Mod.Log.Debug($"baseResist:{baseResist} - resistPenalty:{resistPenalty} = resistChance:{resistChance}");

            int check = Mod.Random.Next(0, 100);
            bool success = resistChance >= check;
            if (success) {
                Mod.Log.Info($"Pilot:{pilot?.Name} resisted injury with check:{check} <= resistChance:{resistChance}");
            } else {
                Mod.Log.Debug($"Pilot failed to resist injury with check:{check} > resistChance:{resistChance}");
            }

            return success;
        }

        public static void LogPilotStats(Pilot pilot) {
            if (Mod.Config.Debug) {
                int normedGuts = NormalizeSkill(pilot.Guts);
                int gutsMod = GetGutsModifier(pilot);
                int normdPilot = NormalizeSkill(pilot.Piloting);
                int pilotingMod = GetPilotingModifier(pilot);
                int normedTactics = NormalizeSkill(pilot.Tactics);
                int tacticsMod = GetTacticsModifier(pilot);

                Mod.Log.Debug($"{pilot.Name} skill profile is " +
                    $"g:{pilot.Guts}->{normedGuts}={gutsMod}" +
                    $"p:{pilot.Piloting}->{normdPilot}={pilotingMod} " +
                    $"t:{pilot.Tactics}->{normedTactics}={tacticsMod} "
                    );
            }
        }
    }
}
