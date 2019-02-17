# Pain Tolerance

This mod for the [HBS BattleTech](http://battletechgame.com/) improves the injury mechanic.

For a head hit, torso destroyed, ammo explosion, knockdown, etc:

- Multiply guts * modifier (10% for instance)
- Reduce modifier by -5% for each point of damage
- If you roll under this, you ignore the injury
- If the injury comes from a head hit, but the armor isn't breached, it's reduced by some factor (0.5)
- If ReceiveHeatDamageInjury, check heat values and apply a damage modifier equal to how deep into overheat you are. overheat capacity = maxHeat - overheatLevel. capacity - currentHeat / capacity  = over heat ratio. Each point of ratio = 1 damage point?
- MechShutdownSequence:CheckForHeatDamage invokes InjurePilot with type of Damage.Overheat : Damage.OverheatSelf. Could patch InjurePilot to avoid the injury as well.

