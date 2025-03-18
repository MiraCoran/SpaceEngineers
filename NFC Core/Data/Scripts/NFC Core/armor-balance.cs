// armor-balance.cs
// copyright (c) 2025 by jay baxter (jaybaxter@me.com)

using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;

[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
public class ArmorBalance : MySessionComponentBase {
	public override void Init(MyObjectBuilder_SessionComponent unused) { }

	public override void LoadData() {
		modifyArmorResists();
	}

	private void modifyArmorResists()
	{
		const float armorDamageResist = 0.20f; // 25% damage resistance
		const float armorDeformReduction = 0.20f; // 25% less deformation

		try {
			foreach (var def in MyDefinitionManager.Static.GetAllDefinitions()) {
				var blockDef = def as MyCubeBlockDefinition;

				// try to exclude non-armor blocks
				if (blockDef == null) continue;
				//if (blockDef.BlockTopology == null) continue;
				if (blockDef.BlockTopology == MyBlockTopology.TriangleMesh) continue;
				if (blockDef.CubeDefinition == null) continue;
				if (!blockDef.CubeDefinition.ShowEdges) continue;

				blockDef.GeneralDamageMultiplier = 1 - armorDamageResist;
				blockDef.DeformationRatio = 1 - armorDeformReduction;
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