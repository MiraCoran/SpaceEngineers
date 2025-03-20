// armor-balance.cs

using System.Linq;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;

[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
public class ArmorBalance : MySessionComponentBase {
	// note: game ui will show damage resistance percentage relative to the modified value, so a 20%
	// damage reduction here will show in game as "25% resist".  the logic appears to be that the
	// block now only takes 80% of the total damage and the amount that was dropped was one quarter
	// of the block's effective health.  deformation appears to work the same.
	// 
	// example:
	// - block has 100 health and gets 20% resistance below
	// - weapon does 100 damage
	// - application of weapon to block drops damage to 80 points (i.e. 20% reduction)
	// - block takes 80 damage and resists 20 damage
	// - amount resisted appears to be one quarter of the damage it took, hence "25% resistance"
	//
	// have fun with that if you change these values.

	private const float armorDamageResist_ = 0.20f; // 25% damage resistance
	private const float armorDeformReduction_ = 0.20f; // 25% less deformation

	// include different block categories by adding the blocks' "<Description>" tag from the SBC file.
	private static readonly System.Collections.Generic.HashSet<string> blockTypes_ =
		new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase) {
			"Armor", // light, heavy, and blast armor
			"BeamBlock", // steel beams
			"CoverWall", // armored half walls
			"Embrasure" // armored full wall
		};

	public override void Init(MyObjectBuilder_SessionComponent unused) { }

	public override void LoadData() {
		modifyArmorResists();
	}

	private static void modifyArmorResists() {
		try {
			foreach (var def in MyDefinitionManager.Static.GetAllDefinitions()) {
				var blockDef = def as MyCubeBlockDefinition;
				if (blockDef == null)
					continue;

				if (shouldApplyResists(blockDef)) {
					blockDef.GeneralDamageMultiplier = 1 - armorDamageResist_;
					blockDef.DeformationRatio = 1 - armorDeformReduction_;
				}
			}
		}
		catch (System.Exception ex) {
			using (var writer =
			       MyAPIGateway.Utilities.WriteFileInWorldStorage(
				       file: "armor-balance.log",
				       callingType: typeof(ArmorBalance))) {
				writer.WriteLineAsync(ex.ToString());
			}
		}
	}

	private static bool shouldApplyResists(MyCubeBlockDefinition def) =>
		blockTypes_.Any(bt => def.Id.SubtypeName.Contains(bt));
}