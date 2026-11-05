using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrariaUI.Common;
using TerrariaUI.Configs;

namespace TerrariaUI.Systems;

/// <summary>
/// Draws the glass sheet *behind* the open inventory, so vanilla slots and items sit on top
/// of it instead of being covered.
///
/// The rect comes from the config: vanilla computes inventory positions inside
/// Main.DrawInventory from screen size, UI scale and (with other mods installed) extra
/// columns, and none of that is exposed. Config values are calibrated from screenshots -
/// see docs/LAYOUT.md.
/// </summary>
public class InventoryPanel : ModSystem
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
		if (UiConfig.Instance is { EnableInventory: true } cfg && Main.playerInventory && !Main.gameMenu)
			DrawPanel(Main.spriteBatch, cfg);

		orig(self);
	}

	private static void DrawPanel(SpriteBatch sb, UiConfig cfg)
	{
		float t = Common.Anim.Inventory;
		if (t <= 0.01f)
			return;

		int rise = (int)(6 * (1f - t));
		var rect = new Rectangle(cfg.PanelX, cfg.PanelY + rise, cfg.PanelW, cfg.PanelH);
		Glass.Panel(sb, rect, alpha: t);
	}
}
