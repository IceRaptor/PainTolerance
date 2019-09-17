using BattleTech;

namespace PainTolerance.Helper {
    public static class HeatHelper {
        public static float CalculateOverheatRatio(Mech mech) {
            int overheatLevel = mech.OverheatLevel;
            int maxHeat = mech.MaxHeat;
            int overheatRange = maxHeat - overheatLevel;

            int currentHeat = mech.CurrentHeat;
            int currentOverheat = currentHeat - overheatLevel;
            Mod.Log.Debug($"Actor:{mech.DisplayName}_{mech?.pilot?.Name} has maxHeat:{maxHeat}, overheat:{overheatLevel}, currentHeat:{currentHeat}");

            float overheatRatio = (float)currentOverheat / (float)overheatRange;
            Mod.Log.Debug($"overheatRatio:{overheatRatio}% = currentOverheat:{currentOverheat} / overheatRange:{overheatRange}");
            return overheatRatio * 100f;
        }
    }
}
