using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaUI.Common;
using TerrariaUI.Configs;

namespace TerrariaUI.Systems;

/// <summary>
/// Optional full HUD replacement: a glass capsule with life and mana bars.
///
/// Hides the vanilla hearts without any hook: <see cref="HudSprites"/> swaps the heart and
/// mana textures for fully transparent sprites of the same size when this module is on, so
/// vanilla keeps drawing - it just draws nothing. That keeps every vanilla layout and input
/// path intact, which a detour of Main.DrawPlayerLife would not (and that method exposes no
/// tModLoader hook in this build anyway).
/// </summary>
public class GlassHud : ModSystem
{
	private const string LayerName = "TerrariaUI: hud";

	public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
	{
		int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
		if (index < 0)
			index = layers.Count;

		layers.Insert(index, new LegacyGameInterfaceLayer(LayerName, Draw, InterfaceScaleType.UI));
	}

	private static bool Draw()
	{
		if (UiConfig.Instance is not { ReplaceVanillaHud: true })
			return true;

		Player player = Main.LocalPlayer;
		if (player == null || !player.active || Main.gameMenu)
			return true;

		SpriteBatch sb = Main.spriteBatch;

		UiConfig cfg = UiConfig.Instance;
		int width = Math.Clamp(cfg?.HudW ?? 320, 80, 900);
		int barH = Math.Clamp(width / 18, 12, 26);
		int gap = barH + 10;

		// vanilla puts life/mana in the top-right corner, left of the minimap
		int x = cfg == null || cfg.HudAnchorRight
			? Main.screenWidth - width - (cfg?.HudX ?? 150)
			: cfg.HudX;
		int y = cfg?.HudY ?? 14;

		int panelH = barH * 2 + gap + 20;
		Glass.Panel(sb, new Rectangle(x - 10, y - 10, width + 20, panelH), shadow: false);

		DrawStat(sb, new Rectangle(x, y, width, barH),
			player.statLife / (float)Math.Max(player.statLifeMax2, 1),
			new Color(206, 84, 100), player.statLife, player.statLifeMax2);

		DrawStat(sb, new Rectangle(x, y + gap, width, barH),
			player.statMana / (float)Math.Max(player.statManaMax2, 1),
			new Color(110, 140, 232), player.statMana, player.statManaMax2);

		return true;
	}

	private static void DrawStat(SpriteBatch sb, Rectangle rect, float fill, Color color,
		int value, int max)
	{
		Glass.Bar(sb, rect, Glass.Fade(Color.White, 0.10f), rect.Height / 2);

		int w = (int)(rect.Width * Math.Clamp(fill, 0f, 1f));
		if (w > 3)
			Glass.Bar(sb, new Rectangle(rect.X, rect.Y, w, rect.Height), color, rect.Height / 2);

		Utils.DrawBorderString(sb, $"{value} / {max}",
			new Vector2(rect.X + 8, rect.Y + 2), Glass.TextHi, 0.5f);
	}
}
