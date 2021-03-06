﻿
namespace PainTolerance {

    public class ModConfig {

        public bool Debug = false;
        public bool Trace = false;

        public float ResistPerGuts = 10.0f;

        public float PenaltyPerHeadDamage = 5.0f;

        // Reduces resist by this multiplied the capacity ratio of an ammo explosion
        public float PenaltyPerAmmoExplosionRatio = 1.0f;

        // Reduces resist by this multiplied the capacity ratio of an head damage injury
        public float PenaltyPerHeatDamageInjuryRatio = 1.0f;

        public float KnockdownDamage = 6f;

        public float TorsoDestroyedDamage = 10f;

        public float HeadHitArmorOnlyMulti = 0.5f;
            
        public override string ToString() {
            return $"Debug:{Debug}, ResistPerGuts:{ResistPerGuts}, PenaltyPerHeadDamage:{PenaltyPerHeadDamage}, " +
                $"PenaltyPerAmmoExplosionRatio:{PenaltyPerAmmoExplosionRatio}, PenaltyPerHeatDamageInjuryRatio:{PenaltyPerHeatDamageInjuryRatio}, " +
                $"KnockdownDamage:{KnockdownDamage}, TorsoDestroyedDamage:{TorsoDestroyedDamage}, " +
                $"HeadHitArmorOnlyMulti:{HeadHitArmorOnlyMulti} ";
        }
    }
}
