using System;
using Terraria.ModLoader;

namespace TerrariaUI.Common;

/// <summary>
/// Tiny animation state for interface transitions. The design calls for 140 ms ease-out
/// reveals: panels fade and slide up a few pixels instead of snapping into place.
/// Values are advanced once per tick, so they are frame-rate independent.
/// </summary>
public static class Anim
{
	private const float Speed = 0.16f;

	private static float inventory;

	/// <summary>0 = closed, 1 = fully open.</summary>
	public static float Inventory => Ease(inventory);

	/// <summary>Raw linear progress, for effects that should not overshoot.</summary>
	public static float InventoryRaw => inventory;

	public static float Ease(float t)
	{
		t = Math.Clamp(t, 0f, 1f);
		return 1f - (1f - t) * (1f - t); // ease-out quad
	}

	public static void Step(bool open)
	{
		float target = open ? 1f : 0f;
		inventory = MathF.Abs(inventory - target) <= Speed
			? target
			: inventory + MathF.Sign(target - inventory) * Speed;
	}
}

/// <summary>Advances <see cref="Anim"/> every tick.</summary>
public class AnimSystem : ModSystem
{
	public override void PostUpdateEverything()
	{
		Anim.Step(Terraria.Main.playerInventory && !Terraria.Main.gameMenu);
	}
}
