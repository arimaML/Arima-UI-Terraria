using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace TerrariaUI.Systems;

/// <summary>
/// Development probe. tModLoader generates the <c>On_</c> hook classes at runtime from the
/// game assembly, so their exact delegate signatures cannot be checked at compile time -
/// and the shipped XML docs do not cover them. This reflects them once at load and exposes
/// short lines that the debug overlay prints on screen, so one screenshot tells us the API.
/// </summary>
public class ApiProbe : ModSystem
{
	public static readonly List<string> Lines = new();

	public override void Load()
	{
		try {
			ProbeTypes();
		}
		catch (Exception e) {
			Lines.Add("probe failed: " + e.Message);
		}
	}

	private static void ProbeTypes()
	{
		Type itemSlot = typeof(Terraria.Main).Assembly.GetType("On_ItemSlot")
			?? Type.GetType("On_ItemSlot");
		Lines.Add(itemSlot == null ? "On_ItemSlot: MISSING" : "On_ItemSlot: ok");

		if (itemSlot != null) {
			FieldInfo[] hooks = itemSlot
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(f => f.Name.StartsWith("orig_", StringComparison.Ordinal))
				.Take(6)
				.ToArray();

			Lines.Add($"hooks: {hooks.Length}");
			foreach (FieldInfo field in hooks) {
				MethodInfo invoke = field.FieldType.GetMethod("Invoke");
				string args = invoke == null
					? "?"
					: string.Join(", ", invoke.GetParameters().Select(p => p.ParameterType.Name));
				Lines.Add($"{field.Name}({args})");
			}
		}

		// does the assembly expose inventory layout helpers at all?
		Type main = typeof(Terraria.Main);
		Lines.Add(main == null ? "Main: MISSING" : "Main: ok");

		// slot geometry would let the panel follow any mod-shifted layout
		string[] wanted = { "inventoryBackTexture", "UIScale", "screenWidth" };
		foreach (string name in wanted) {
			FieldInfo field = main?.GetField(name, BindingFlags.Public | BindingFlags.Static);
			Lines.Add($"Main.{name}: {(field == null ? "-" : field.FieldType.Name)}");
		}
	}
}
