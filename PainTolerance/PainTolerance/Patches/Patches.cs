using BattleTech;
using Harmony;
using PainTolerance.Helper;
using System;

namespace PainTolerance.Patches {

    [HarmonyAfter(new string[] { "MechEngineer" })]
    [HarmonyPatch(typeof(Mech), "DamageLocation")]
    public static class Mech_DamageLocation {
        public static void Prefix(Mech __instance, WeaponHitInfo hitInfo, ArmorLocation aLoc, Weapon weapon, float totalDamage, int hitIndex, 
            AttackImpactQuality impactQuality, DamageType damageType) {

            if (aLoc == ArmorLocation.Head) {
                PainTolerance.Logger.Log($"Head hit from weapon:{weapon?.UIName} for {totalDamage} damage. Quality was:{impactQuality} with type:{damageType}");

                float currHeadArmor = __instance.GetCurrentArmor(aLoc);
                int damageMod = (int)totalDamage;
                float damageThroughArmor = totalDamage - currHeadArmor;
                PainTolerance.Logger.LogIfDebug($"TotalDamage:{totalDamage} - Head armor:{currHeadArmor} = throughArmor:{damageThroughArmor}");

                if (totalDamage - currHeadArmor <= 0) {                    
                    damageMod = (int)Math.Floor(damageMod * PainTolerance.Config.HeadHitArmorOnlyMulti);
                    PainTolerance.Logger.Log($"Head hit impacted armor only, reduced damage to:{damageMod}");
                }

                ModState.InjuryResistPenalty = damageMod * PainTolerance.Config.PenaltyPerHeadDamage;
                PainTolerance.Logger.Log($"Setting resist penalty to:{damageMod} x {PainTolerance.Config.PenaltyPerHeadDamage} = {ModState.InjuryResistPenalty}");                
            }
        }
    }

    [HarmonyAfter(new string[] { "MechEngineer" })]
    [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
    public static class AmmunitionBox_DamageComponent {
        public static void Prefix(AmmunitionBox __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects, CombatGameState ___combat) {
            if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && ___combat.Constants.PilotingConstants.InjuryFromAmmoExplosion) {
                int value = __instance.StatCollection.GetValue<int>("CurrentAmmo");
                int capacity = __instance.ammunitionBoxDef.Capacity;
                float ratio = (float)value / (float)capacity;
                PainTolerance.Logger.LogIfDebug($"Ammo explosion ratio:{ratio} = current:{value} / capacity:{capacity}");
                int resistPenalty = (int)Math.Floor(ratio * PainTolerance.Config.PenaltyPerAmmoExplosionRatio);
                PainTolerance.Logger.LogIfDebug($"Ammo explosion resist penalty:{resistPenalty} = " +
                    $"Floor( ratio:{ratio}% * penaltyPerAmmoExplosion:{PainTolerance.Config.PenaltyPerAmmoExplosionRatio} )");

                if (ratio >= 0.5f) {
                    ModState.InjuryResistPenalty = resistPenalty;
                    PainTolerance.Logger.LogIfDebug($"Ammo explosion will reduce resist by: {resistPenalty}");
                }
                
            }
        }
    }

    [HarmonyPatch(typeof(Pilot), "SetNeedsInjury")]
    public static class Pilot_SetNeedsInjury {

        // Set state to true if needsInjury is already set; otherwise we override the value back to false.
        public static void Prefix(Pilot __instance, bool __state, bool ___needsInjury) {
            __state = ___needsInjury ? true : false;
        }
        
        public static void Postfix(Pilot __instance, InjuryReason reason, bool __state, ref bool ___needsInjury, ref InjuryReason ___injuryReason) {

            // Check for ReceiveHeatDamageInjury
            if (__instance?.ParentActor?.GetType() == typeof(Mech)) {
                Mech mech = (Mech)__instance.ParentActor;
                Statistic receiveHeatDamageInjuryStat = mech.StatCollection.GetStatistic("ReceiveHeatDamageInjury");
                PainTolerance.Logger.LogIfDebug($"Checking actor with injuryReason:{reason} and receiveHeatDamageInjury:{receiveHeatDamageInjuryStat}");

                // If the below is true, we likely are coming from a ME patch - 
                // see https://github.com/BattletechModders/MechEngineer/blob/master/source/Features/ShutdownInjuryProtection/Patches/Mech_CheckForHeatDamage_Patch.cs
                if (reason == InjuryReason.NotSet && mech.IsOverheated && mech.StatCollection.GetStatistic("ReceiveHeatDamageInjury") != null) {
                    PainTolerance.Logger.LogIfDebug($"Actor received a heatDamage injury, computing overheat ratio.");
                    float overheatRatio = HeatHelper.CalculateOverheatRatio(mech);

                    int overheatPenalty = (int)Math.Floor(overheatRatio * PainTolerance.Config.PenaltyPerHeatDamageInjuryRatio);
                    PainTolerance.Logger.LogIfDebug($"overheatPenalty:{overheatPenalty} = " +
                        $"Floor( overheatRatio:{overheatRatio} * penaltyPerOverheatDamage{PainTolerance.Config.PenaltyPerHeatDamageInjuryRatio} )");
                    ModState.InjuryResistPenalty = overheatPenalty;
                }

                // Set explicit damage values for known damage types
                if (reason == InjuryReason.Knockdown) {
                    ModState.InjuryResistPenalty = PainTolerance.Config.KnockdownDamage;
                } else if (reason == InjuryReason.SideTorsoDestroyed) {
                    ModState.InjuryResistPenalty = PainTolerance.Config.TorsoDestroyedDamage;
                }
            }

            if (ModState.InjuryResistPenalty != -1) {
                bool success = PilotHelper.MakeResistCheck(__instance);
                if (success) {
                    // If the state value is true, then there was already an injury set on the pilot. Do nothign.
                    if (__state) {
                        PainTolerance.Logger.Log($"Pilot has an outstanding injury, not ignoring!");
                    } else {
                        ___needsInjury = false;
                        ___injuryReason = InjuryReason.NotSet;
                    }
                } 

                // Reset our mod state
                ModState.InjuryResistPenalty = -1;
            }
        }
    }

    [HarmonyAfter(new string[] { "dZ.Zappo.Pilot_Quirks" })]
    [HarmonyBefore(new string[] { "us.frostraptor.SkillBasedInit" })]
    [HarmonyPatch(typeof(Pilot), "InjurePilot")]
    public static class Pilot_InjurePilot {

        public static void Prefix(Pilot __instance, ref int dmg, DamageType damageType) {
            if (damageType == DamageType.Overheat || damageType == DamageType.OverheatSelf) {
                PainTolerance.Logger.LogIfDebug($"Pilot:{__instance?.Name} will be injured by overheating.");

                Mech mech = __instance?.ParentActor as Mech;
                float overheatRatio = HeatHelper.CalculateOverheatRatio(mech);
                int overheatPenalty = (int)Math.Floor(overheatRatio * PainTolerance.Config.PenaltyPerHeatDamageInjuryRatio);
                PainTolerance.Logger.LogIfDebug($"overheatPenalty:{overheatPenalty} = " +
                    $"Floor( overheatRatio:{overheatRatio} * penaltyPerOverheatDamage{PainTolerance.Config.PenaltyPerHeatDamageInjuryRatio}");
                ModState.InjuryResistPenalty = overheatPenalty;

                bool success = PilotHelper.MakeResistCheck(__instance);
                if (success) {
                    dmg = 0;
                }

                // Reset the mod state
                ModState.InjuryResistPenalty = -1;

            }
        }
    }


    [HarmonyPatch(typeof(TurnDirector), "OnTurnActorActivateComplete")]
    public static class TurnDirectror_OnTurnActorActivateComplete {
        public static void Postfix(TurnDirector __instance) {
            // Reset the attack penalty in case we've flipped actors.
            ModState.InjuryResistPenalty = -1;
        }
    }
    
}
