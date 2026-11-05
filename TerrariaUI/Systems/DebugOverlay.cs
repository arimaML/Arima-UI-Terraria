using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaUI.Configs;

namespace TerrariaUI.Systems;

/// <summary>
/// Calibration overlay, off by default: resolution, UI scale, an on-screen ruler in UI units
/// and the API probe output. It exists because vanilla does not expose inventory geometry -
/// one screenshot with this enabled is enough to derive exact offsets for any layout,
/// including ones shifted by other mods.
/// </summary>
public class DebugOverlay : ModSystem
{
	private const string LayerName = "TerrariaUI: debug overlay";

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		if (UiConfig.Instance is not { DebugInfo: true })
			return;

		layers.Insert(0, new LegacyGameInterfaceLayer(LayerName, Draw, InterfaceScaleType.UI));
	}

	private static bool Draw()
	{
		SpriteBatch sb = Main.spriteBatch;
		Color dim = new(139, 148, 163);

		Label(sb, $"{Main.screenWidth}x{Main.screenHeight}  ui {Main.UIScale:0.00}  inv {(Main.playerInventory ? 1 : 0)}",
			8, 6, 0.5f, dim);

		float y = 20;
		foreach (string line in ApiProbe.Lines.Take(8)) {
			Label(sb, line, 8, (int)y, 0.45f, new Color(120, 150, 200));
			y += 13;
		}

		Texture2D pixel = TextureAssets.MagicPixel.Value;
		Color tick = Color.White * 0.28f;
		Color tickBig = Color.White * 0.5f;

		for (int x = 0; x < Main.screenWidth; x += 50) {
			bool big = x % 200 == 0;
			sb.Draw(pixel, new Rectangle(x, 0, 1, big ? 10 : 5), big ? tickBig : tick);
			if (big)
				Label(sb, x.ToString(), x + 2, 12, 0.42f, tickBig);
		}

		for (int y2 = 0; y2 < Main.screenHeight; y2 += 50) {
			bool big = y2 % 200 == 0;
			sb.Draw(pixel, new Rectangle(0, y2, big ? 10 : 5, 1), big ? tickBig : tick);
			if (big)
				Label(sb, y2.ToString(), 12, y2 + 2, 0.42f, tickBig);
		}

		return true;
	}

	private static void Label(SpriteBatch sb, string text, int x, int y, float scale, Color color)
	{
		Utils.DrawBorderString(sb, text, new Vector2(x, y), color, scale);
	}
}
