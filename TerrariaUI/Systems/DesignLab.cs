using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaUI.Common;
using TerrariaUI.Configs;

namespace TerrariaUI.Systems;

/// <summary>
/// Living style guide: every component of the design system on one screen, drawn with the
/// same materials the real UI uses. Enable "Design lab" in the config to look at it.
/// This is how material changes get judged without hunting through the game.
/// </summary>
public class DesignLab : ModSystem
{
	private const string LayerName = "TerrariaUI: design lab";

	public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
	{
		int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
		if (index < 0)
			index = layers.Count;

		layers.Insert(index, new LegacyGameInterfaceLayer(LayerName, Draw, InterfaceScaleType.UI));
	}

	private static bool Draw()
	{
		if (UiConfig.Instance is not { ShowDesignLab: true })
			return true;

		SpriteBatch sb = Main.spriteBatch;

		int w = 640;
		int h = 470;
		var panel = new Rectangle((Main.screenWidth - w) / 2, (Main.screenHeight - h) / 2, w, h);
		Glass.Panel(sb, panel);

		int x = panel.X + 20;
		int y = panel.Y + 18;

		Text(sb, UiText.Lab("Title"), x, y, 0.95f, Glass.TextHi);
		TextRight(sb, UiText.Lab("Subtitle"), panel.Right - 20, y + 4, 0.55f, Glass.TextLow);
		y += 26;
		Line(sb, panel.X + 20, y, w - 40);
		y += 14;

		// ---- buttons
		Label(sb, UiText.Lab("Buttons"), x, y, 0.5f, Glass.TextLow);
		y += 14;
		var p1 = new Rectangle(x, y, 108, 30);
		var p2 = new Rectangle(p1.Right + 8, y, 108, 30);
		var p3 = new Rectangle(p2.Right + 8, y, 108, 30);
		Glass.Pill(sb, p1, accent: true);
		Glass.Pill(sb, p2);
		Glass.Pill(sb, p3);
		Center(sb, UiText.Lab("Create"), p1, 0.6f, new Color(10, 13, 20));
		Center(sb, UiText.Lab("Cancel"), p2, 0.6f, Glass.TextHi);
		Center(sb, UiText.Lab("Locked"), p3, 0.6f, Glass.TextLow);
		y += 48;

		// ---- tabs
		Label(sb, UiText.Lab("Tabs"), x, y, 0.5f, Glass.TextLow);
		y += 14;
		var track = new Rectangle(x, y, 340, 28);
		Glass.Field(sb, track);
		int seg = (track.Width - 6) / 3;
		var active = new Rectangle(track.X + 3, track.Y + 3, seg, 22);
		Glass.Pill(sb, active);
		Center(sb, UiText.Lab("Items"), active, 0.55f, Glass.TextHi);
		Text(sb, UiText.Lab("Crafting"), track.X + 3 + seg + 14, track.Y + 7, 0.55f, Glass.TextLow);
		Text(sb, UiText.Lab("Loadout"), track.X + 3 + seg * 2 + 14, track.Y + 7, 0.55f, Glass.TextLow);
		y += 46;

		// ---- slots
		Label(sb, UiText.Lab("Slots"), x, y, 0.5f, Glass.TextLow);
		y += 14;
		for (int i = 0; i < 4; i++) {
			var cell = new Rectangle(x + i * 48, y, 44, 44);
			Glass.Slot(sb, cell, hovered: i == 1, selected: i == 2);
		}
		Text(sb, UiText.Lab("Idle"), x, y + 46, 0.45f, Glass.TextLow);
		Text(sb, UiText.Lab("Hover"), x + 48, y + 46, 0.45f, Glass.TextLow);
		Text(sb, UiText.Lab("Selected"), x + 96, y + 46, 0.45f, Glass.TextLow);
		y += 66;

		// ---- controls
		Label(sb, UiText.Lab("Controls"), x, y, 0.5f, Glass.TextLow);
		y += 16;
		Glass.Toggle(sb, new Rectangle(x, y, 44, 20), false);
		Text(sb, UiText.Lab("Off"), x + 52, y + 2, 0.5f, Glass.TextLow);
		Glass.Toggle(sb, new Rectangle(x + 100, y, 44, 20), true);
		Text(sb, UiText.Lab("On"), x + 152, y + 2, 0.5f, Glass.TextLow);
		y += 34;

		Glass.Slider(sb, new Rectangle(x, y, 200, 20), 0.58f);
		Text(sb, UiText.Lab("Value"), x + 212, y + 2, 0.5f, Glass.TextLow);
		y += 34;

		var field = new Rectangle(x, y, 260, 30);
		Glass.Field(sb, field);
		Text(sb, UiText.Lab("Search"), field.X + 12, field.Y + 7, 0.55f, Glass.TextLow);
		y += 46;

		// ---- text
		Label(sb, UiText.Lab("Text"), x, y, 0.5f, Glass.TextLow);
		y += 14;
		Text(sb, UiText.Lab("ItemName"), x, y, 0.75f, Glass.TextHi);
		Text(sb, UiText.Lab("ItemStat"), x + 130, y + 3, 0.55f, Glass.TextLow);
		y += 26;

		// ---- tooltip
		var tip = new Rectangle(panel.Right - 250, panel.Bottom - 96, 230, 76);
		Glass.Panel(sb, tip);
		Text(sb, UiText.Lab("ItemName"), tip.X + 14, tip.Y + 10, 0.7f, Glass.TextHi);
		Line(sb, tip.X + 14, tip.Y + 32, tip.Width - 28);
		Text(sb, UiText.Lab("Radius"), tip.X + 14, tip.Y + 40, 0.5f, Glass.TextLow);
		Text(sb, UiText.Lab("Accent"), tip.X + 14, tip.Y + 54, 0.5f, Glass.TextLow);

		TextRight(sb, UiText.Lab("Hint"), panel.Right - 20, panel.Bottom - 14, 0.5f, Glass.TextLow);

		return true;
	}

	private static void Line(SpriteBatch sb, int x, int y, int w)
	{
		sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y, w, 1),
			new Color(255, 255, 255) * 0.18f);
	}

	private static void Text(SpriteBatch sb, string text, int x, int y, float scale, Color color)
	{
		Utils.DrawBorderString(sb, text, new Vector2(x, y), color, scale);
	}

	private static void TextRight(SpriteBatch sb, string text, int right, int y, float scale, Color color)
	{
		float w = FontAssets.MouseText.Value.MeasureString(text).X * scale;
		Utils.DrawBorderString(sb, text, new Vector2(right - w, y), color, scale);
	}

	private static void Label(SpriteBatch sb, string text, int x, int y, float scale, Color color)
	{
		Utils.DrawBorderString(sb, text, new Vector2(x, y), color, scale);
	}

	private static void Center(SpriteBatch sb, string text, Rectangle r, float scale, Color color)
	{
		Vector2 size = FontAssets.MouseText.Value.MeasureString(text) * scale;
		Utils.DrawBorderString(sb, text,
			new Vector2(r.X + (r.Width - size.X) / 2f, r.Y + (r.Height - size.Y) / 2f), color, scale);
	}
}
