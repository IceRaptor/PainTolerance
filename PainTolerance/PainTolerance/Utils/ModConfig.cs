
namespace PainTolerance {

    public class ModConfig {

        // If true, extra logging will be used
        public bool Debug = false;

        public float ResistPerGuts = 10.0f;

        public float PenaltyPerHeadDamage = 5.0f;

        // Reduces resist by this multiplied the capacity ratio of an ammo explosion
        public float PenaltyPerAmmoExplosionRatio = 1.0f;

        // Reduces resist by this multiplied the capacity ratio of an head damage injury
        public float PenaltyPerHeatDamageInjuryRatio = 1.0f;

        public float KnockdownDamage = 6f;

        public float ShutdownDamage = 12f;

        public float TorsoDestroyedDamage = 10f;

        public float HeadHitArmorOnlyMulti = 0.5f;
            
        public override string ToString() {
            return $"Debug:{Debug}, ResistPerGuts:{ResistPerGuts}, PenaltyPerHeadDamage:{PenaltyPerHeadDamage}, " +
                $"PenaltyPerAmmoExplosionRatio:{PenaltyPerAmmoExplosionRatio}, PenaltyPerHeatDamageInjuryRatio:{PenaltyPerHeatDamageInjuryRatio}, " +
                $"KnockdownDamage:{KnockdownDamage}, ShutdownDamage:{ShutdownDamage}, TorsoDestroyedDamage:{TorsoDestroyedDamage}, " +
                $"HeadHitArmorOnlyMulti:{HeadHitArmorOnlyMulti} ";
        }
    }
}
