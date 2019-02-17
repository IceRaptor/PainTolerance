# Pain Tolerance

This mod for the [HBS BattleTech](http://battletechgame.com/) makes skilled pilots more resistant to injuries. Any time the pilot would normally be injured by a head hit, torso destruction, ammo explosion, or knockdown, they make a check against their Guts skill rating to determine if they can shrug off the effect. 

This check is also made in the event of a Shutdown, if the optional _ShutdownCausesInjury_ value is set to true. This value can be set in either the `CombatGameConstants.json`, or can be introduced by [MechEngineer](https://github.com/BattletechModders/MechEngineer/blob/master/source/Features/ShutdownInjuryProtection/Patches/MechShutdownSequence_CheckForHeatDamage_Patch.cs). This check is also made if you are using MechEngineer's  [ReceiveHeatDamageInjury](https://github.com/BattletechModders/MechEngineer/blob/master/source/Features/ShutdownInjuryProtection/Patches/Mech_CheckForHeatDamage_Patch.cs) option. 

## Details

Each pilot's resist check is defined by their rating in the Guts skill, as well as any Abilities in that tree that have been taken. The table below defines the guts modifier that will be used as a modifier base. This value is then multiplied by the **ResistPerGuts** configuration value to determine a base check level. 

Pilot skills of 11-13 are used for elite pilots in the [RogueTech](www.roguetech.org) mod. Player pilots cannot reach this level.

| Skill                | 1    | 2    | 3    | 4    | 5    | 6    | 7    | 8    | 9    | 10   | 11   | 12   | 13   |
| -------------------- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Modifier             | +0   | +1   | +1   | +2   | +2   | +3   | +3   | +4   | +4   | +5   | +6   | +7   | +8   |
| with Level 5 Ability | +0   | +1   | +1   | +2   | +3   | +4   | +4   | +5   | +5   | +6   | +7   | +8   | +9   |
| with Level 8 Ability | +0   | +1   | +1   | +2   | +4   | +5   | +5   | +6   | +6   | +7   | +8   | +9   | +10  |

> Example: A pilot has guts 5 and the level 5 ability. Their base modifier is +3, which is multiplied by the ResistPerGuts config value of 15%. This gives them a 45% modifier to ignore damage sources. If the pilot improves their Guts skill to 6, their modifier would increase to 60%.

### Head Injuries

When a mech's head is hit, each point of damage reduces the resist chance by the **PenaltyPerHeadDamage** configuration value. If the attack only inflicts armor damage, the total damage is multiplied by the **HeadHitArmorOnlyMulti** value. By default this reduces the damage amount by 50%, making it easier for players to shrug off the hit.

> Example: A pilot with guts 7 (but no abilities) gets hit in the head of 3 damage. The damage doesn't penetrate the armor, so it becomes 3 * 0.5 = 1.5, rounded down to 1. The pilot's resist chance is 45% - 5% for the damage, or 40%.
>
> Example 2: A pilot with guts 10 and both abilities gets hit in the head for 18 damage. Some of the damage penetrates structure. The pilot's base resist chance is 7 * 15 =105%. The attack reduces that check by 18 * 5% = 90%. The pilot has 15% chance to avoid the injury.

### Overheat Injuries

When a mech takes overheat damage, each point of heat over the overheating limit reduces the resist chance by **PenaltyPerHeatDamageInjuryRatio**. The difference between the mech's maximum heat (at which point is shuts down) and it's overheating limit is taken as a spectrum from 0 - 100. The mech's current heat within this spectrum defines the resistance penalty.

> Example: A pilot with a resistance check of 60% overheats their Mech. The Mech has a maximum heat of 200, and an overheating limit of 120. 200 - 120 = 80 points on the overheating spectrum. If the Mech was currently as 180 heat points, the overheating points would be calculated as 180 -120 = 60 points. The ratio of points to spectrum is then calculated, or 80 / 60 = 0.75 * 100 = 75 points. Assuming PenaltyPerHeatDamageInjuryRatio = 1, then the resist check is reduced by -75%. The pilot cannot resist and thus takes the injury.

### Configuration

The mod is customizable through the `mod.json` in the root directory. The following values influence the mod's behavior:

* **Debug:** If true, the mod will print debugging information to the `PainTolerance/pain_tolerance.log` file.
* **ResistPerGuts:** The base chance to resist an injury per point of Guts skill. This defaults to 15%.
* **PenaltyPerHeadDamage:** The resist chance penalty for each point of damage taken on a head hit.  Defaults to 5% per point of damage.
* **HeadHitArmorOnlyMulti:** If the head hit didn't inflict structure damage, the damage value will be multiplied by this amount. Defaults to 0.5, to reduce damage by 50%.
* **PenaltyPerAmmoExplosionRatio:** During an ammo explosion, the ratio of available to total rounds in the exploding ammo box is calculated. For each percentage point in that ratio, the resist penalty will be reduced by this amount. Defaults to 1 point for each percentage point.
* **PenaltyPerHeatDamageInjuryRatio:** When a mech suffers heat damage (either from _HeatDamageInjury_ or _ShutdownInjury_) the ratio of overheat capacity to current heat value will be calculated. For each percentage point in the ratio, the resist penalty will be reduced by this amount. Defaults to 1 point for each percentage point.
* **KnockdownDamage:** When a mech suffers a knockdown, this value will be used as a faux damage amount to calculate the resist penalty. Defaults to 6 points of damage.
* **TorsoDestroyedDamage:** When a suffers a shutdown, this value will be used as a faux damage amount to calculate the resist penalty. Defaults to 10 points of damage.