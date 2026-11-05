using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using TerrariaUI.Configs;

namespace TerrariaUI.Systems;

/// <summary>
/// Dims the world behind an open interface. "Deep dark background" is half of the brief:
/// panels read as glass only when the world behind them is calmer. One stretched vignette
/// texture, no layout knowledge needed, so it works at any resolution and UI scale.
/// </summary>
public class InterfaceScrim : ModSystem
{
	public override void Load()
	{
		On_Main.DrawInventory += HookDrawInventory;
	}

	public override void Unload()
	{
		On_Main.DrawInventory -= HookDrawInventory;
	}

	private static void HookDrawInventory(On_Main.orig_DrawInventory orig, Main self)
	{
		if (Main.playerInventory && !Main.gameMenu)
			Draw(Main.spriteBatch);

		orig(self);
	}

	private static void Draw(SpriteBatch sb)
	{
		float strength = UiConfig.Instance?.ScrimStrength ?? 0.45f;
		if (strength <= 0.001f)
			return;

		Texture2D scrim = ModContent.Request<Texture2D>("TerrariaUI/Assets/UI/scrim",
			AssetRequestMode.ImmediateLoad).Value;

		var screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
		sb.Draw(scrim, screen, Color.White * (strength * Common.Anim.InventoryRaw));
	}
}
