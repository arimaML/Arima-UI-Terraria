using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using TerrariaUI.Configs;

namespace TerrariaUI.Common;

/// <summary>
/// Draws the Liquid Glass materials from the layered 9-slice assets in Assets/UI.
///
/// Layer order matters: a tinted body cannot keep a light rim, so the rim and the
/// top sheen are separate white textures drawn on top (see tools/gen_glass_assets.py).
/// Every colour comes from <see cref="UiConfig"/> so the whole look stays tunable in-game.
/// </summary>
public static class Glass
{
	// documented defaults, mirrored in UiConfig so the config UI shows real numbers
	public static readonly Color DefaultPanel = new(17, 21, 27);
	public static readonly Color DefaultAccent = new(124, 140, 248);
	public static readonly Color DefaultText = new(243, 246, 251);
	public static readonly Color DefaultTextDim = new(139, 148, 163);

	private static UiConfig Cfg => UiConfig.Instance;

	public static Color PanelFill => Cfg?.PanelTint ?? DefaultPanel;
	public static Color SlotFill => Cfg?.SlotTint ?? Color.White;
	public static Color Accent => Cfg?.AccentColor ?? DefaultAccent;
	public static Color TextHi => Cfg?.TextColor ?? DefaultText;
	public static Color TextLow => Cfg?.TextDimColor ?? DefaultTextDim;

	private static float Opacity => Cfg?.PanelOpacity ?? 0.55f;
	private static float SlotOpacity => Cfg?.SlotOpacity ?? 0.12f;
	private static float RimStrength => Cfg?.RimStrength ?? 0.25f;
	private static float SheenStrength => Cfg?.SheenStrength ?? 0.12f;

	private static Texture2D Tex(string path)
	{
		Asset<Texture2D> asset = ModContent.Request<Texture2D>("TerrariaUI/Assets/UI/" + path);
		return asset.Value;
	}

	/// <summary>Draws a texture as a 9-slice so corners keep their radius at any size.</summary>
	public static void DrawNineSlice(SpriteBatch sb, Texture2D tex, Rectangle dst, int corner,
		Color color)
	{
		int tw = tex.Width;
		int th = tex.Height;
		corner = Math.Min(corner, Math.Min(tw / 2, th / 2));
		if (corner <= 0 || dst.Width <= 0 || dst.Height <= 0)
			return;

		int[] srcW = { corner, tw - 2 * corner, corner };
		int[] srcH = { corner, th - 2 * corner, corner };
		int[] srcX = { 0, corner, tw - corner };
		int[] srcY = { 0, corner, th - corner };

		int dcx = Math.Min(corner, dst.Width / 2);
		int dcy = Math.Min(corner, dst.Height / 2);
		int[] dstW = { dcx, dst.Width - 2 * dcx, dcx };
		int[] dstH = { dcy, dst.Height - 2 * dcy, dcy };
		int[] dstX = { dst.X, dst.X + dcx, dst.Right - dcx };
		int[] dstY = { dst.Y, dst.Y + dcy, dst.Bottom - dcy };

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (srcW[i] <= 0 || srcH[j] <= 0 || dstW[i] <= 0 || dstH[j] <= 0)
					continue;

				var src = new Rectangle(srcX[i], srcY[j], srcW[i], srcH[j]);
				var target = new Rectangle(dstX[i], dstY[j], dstW[i], dstH[j]);
				sb.Draw(tex, target, src, color);
			}
		}
	}

	/// <summary>
	/// Applies an alpha factor **and premultiplies** the colour.
	///
	/// Terraria draws with premultiplied-alpha blending (BlendState.AlphaBlend in FNA is
	/// One / InverseSourceAlpha), so a texture that keeps full-bright RGB with a low alpha
	/// renders as a bright wash instead of a transparent tint. Scaling RGB by the final
	/// alpha is what makes "12% white" actually look like 12% white.
	/// </summary>
	public static Color Fade(Color c, float a)
	{
		a = Math.Clamp(a, 0f, 1f);
		byte alpha = (byte)Math.Clamp((int)(c.A * a), 0, 255);
		return new Color(
			(byte)(c.R * alpha / 255),
			(byte)(c.G * alpha / 255),
			(byte)(c.B * alpha / 255),
			alpha);
	}

	/// <summary>A glass panel: drop shadow, tinted body, sheen, rim. 28 px radius.</summary>
	public static void Panel(SpriteBatch sb, Rectangle dst, bool shadow = true, float alpha = 1f)
	{
		if (shadow) {
			var shadowRect = new Rectangle(dst.X - 16, dst.Y - 4, dst.Width + 32, dst.Height + 32);
			DrawNineSlice(sb, Tex("panel_shadow"), shadowRect, 30, new Color(0, 0, 0) * 0.5f);
		}

		float a = Math.Clamp(alpha, 0f, 1f);
		Color fill = Fade(PanelFill, Opacity * a);
		DrawNineSlice(sb, Tex("panel_body"), dst, 28, fill);
		DrawNineSlice(sb, Tex("panel_sheen"), dst, 28, Color.White * (SheenStrength * a));
		DrawNineSlice(sb, Tex("panel_rim"), dst, 28, Color.White * (RimStrength * a));
	}

	/// <summary>A glass button: capsule body, sheen, rim. Height picks the asset.</summary>
	public static void Pill(SpriteBatch sb, Rectangle dst, bool accent = false)
	{
		int h = dst.Height;
		int asset = h <= 24 ? 22 : h <= 28 ? 26 : h <= 32 ? 30 : 34;
		string name = "pill_" + asset;
		int corner = asset / 2;

		Color fill = accent ? Fade(Accent, 0.95f) : Fade(SlotFill, SlotOpacity * 1.6f);
		DrawNineSlice(sb, Tex(name + "_body"), dst, corner, fill);
		DrawNineSlice(sb, Tex(name + "_sheen"), dst, corner,
			Color.White * (accent ? SheenStrength * 1.6f : SheenStrength * 0.7f));
		DrawNineSlice(sb, Tex(name + "_rim"), dst, corner,
			Color.White * (accent ? RimStrength * 1.8f : RimStrength));
	}

	/// <summary>An inventory slot: recessed body plus a light rim.</summary>
	public static void Slot(SpriteBatch sb, Rectangle dst, bool hovered = false, bool selected = false)
	{
		if (selected) {
			var glow = new Rectangle(dst.X - 3, dst.Y - 3, dst.Width + 6, dst.Height + 6);
			DrawNineSlice(sb, Tex("slot_body"), glow, 14, Fade(Accent, 0.35f));
		}

		float a = SlotOpacity * (selected ? 2.2f : hovered ? 1.5f : 1f);
		DrawNineSlice(sb, Tex("slot_body"), dst, 12, Fade(SlotFill, a));
		DrawNineSlice(sb, Tex("slot_sheen"), dst, 12, Color.White * (SheenStrength * 0.6f));
		DrawNineSlice(sb, Tex("slot_rim"), dst, 12,
			selected ? Accent : Color.White * (RimStrength * (hovered ? 0.8f : 0.55f)));
	}

	/// <summary>A thin rounded bar: slider track, divider, progress fill.</summary>
	public static void Bar(SpriteBatch sb, Rectangle dst, Color color, int corner = 4)
	{
		DrawNineSlice(sb, Tex("panel_body"), dst, Math.Min(corner, Math.Min(dst.Height / 2, 20)),
			color);
	}

	/// <summary>A circle, built from the capsule asset (equal width and height).</summary>
	public static void Dot(SpriteBatch sb, int cx, int cy, int diameter, Color color)
	{
		int d = Math.Max(diameter, 22);
		var dst = new Rectangle(cx - d / 2, cy - d / 2, d, d);
		DrawNineSlice(sb, Tex("pill_22_body"), dst, d / 2, color);
		DrawNineSlice(sb, Tex("pill_22_rim"), dst, d / 2, Color.White * RimStrength);
	}

	/// <summary>Toggle: a sunken track and a knob that sits left (off) or right (on).</summary>
	public static void Toggle(SpriteBatch sb, Rectangle dst, bool on)
	{
		var track = new Rectangle(dst.X, dst.Y + dst.Height / 4, dst.Width, dst.Height / 2);
		Bar(sb, track, on ? Fade(Accent, 0.9f) : Fade(SlotFill, SlotOpacity * 2.4f));
		DrawNineSlice(sb, Tex("panel_rim"), track, Math.Min(track.Height / 2, 8),
			Color.White * (RimStrength * 0.5f));

		int knob = dst.Height;
		int kx = on ? track.Right - knob : track.X;
		Dot(sb, kx + knob / 2, track.Y + track.Height / 2, knob, Color.White);
	}

	/// <summary>Slider: track, accent fill and a knob at <paramref name="value"/> (0..1).</summary>
	public static void Slider(SpriteBatch sb, Rectangle dst, float value)
	{
		int h = 6;
		var track = new Rectangle(dst.X, dst.Y + (dst.Height - h) / 2, dst.Width, h);
		Bar(sb, track, Fade(SlotFill, SlotOpacity * 2.4f));

		int fill = (int)(dst.Width * Math.Clamp(value, 0f, 1f));
		if (fill > 4)
			Bar(sb, new Rectangle(track.X, track.Y, fill, h), Fade(Accent, 0.95f));

		Dot(sb, track.X + fill, track.Y + h / 2, 16, Color.White);
	}

	/// <summary>Sunken input field.</summary>
	public static void Field(SpriteBatch sb, Rectangle dst)
	{
		DrawNineSlice(sb, Tex("panel_body"), dst, 12, Fade(new Color(0, 0, 0), 0.42f));
		DrawNineSlice(sb, Tex("panel_rim"), dst, 12, Color.White * (RimStrength * 0.35f));
	}
}
