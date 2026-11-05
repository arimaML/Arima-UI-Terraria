using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace TerrariaUI.Configs;

public class UiConfig : ModConfig
{
	public static UiConfig Instance;

	public override ConfigScope Mode => ConfigScope.ClientSide;

	// ---------------------------------------------------------------- modules

	[Header("Modules")]
	[DefaultValue(true)]
	public bool EnableInventory { get; set; }

	// ---------------------------------------------------------------- glass

	[Header("Glass")]
	[DefaultValue(typeof(Color), "17, 21, 27, 255")]
	public Color PanelTint { get; set; }

	[DefaultValue(0.62f)]
	[Range(0.05f, 1f)]
	[Increment(0.05f)]
	public float PanelOpacity { get; set; }

	[DefaultValue(typeof(Color), "255, 255, 255, 255")]
	public Color SlotTint { get; set; }

	[DefaultValue(0.12f)]
	[Range(0f, 0.6f)]
	[Increment(0.02f)]
	public float SlotOpacity { get; set; }

	[DefaultValue(typeof(Color), "124, 140, 248, 255")]
	public Color AccentColor { get; set; }

	[DefaultValue(typeof(Color), "243, 246, 251, 255")]
	public Color TextColor { get; set; }

	[DefaultValue(typeof(Color), "139, 148, 163, 255")]
	public Color TextDimColor { get; set; }

	[DefaultValue(0.25f)]
	[Range(0f, 1f)]
	[Increment(0.05f)]
	public float RimStrength { get; set; }

	[DefaultValue(0.12f)]
	[Range(0f, 1f)]
	[Increment(0.05f)]
	public float SheenStrength { get; set; }

	[DefaultValue(true)]
	public bool RealBlur { get; set; }

	[DefaultValue(0.7f)]
	[Range(0.3f, 1f)]
	[Increment(0.05f)]
	public float BlurStrength { get; set; }

	// ---------------------------------------------------------------- layout

	[Header("Layout")]
	[DefaultValue(8)]
	[Range(-200, 3000)]
	public int PanelX { get; set; }

	[DefaultValue(4)]
	[Range(-200, 3000)]
	public int PanelY { get; set; }

	[DefaultValue(568)]
	[Range(100, 3000)]
	public int PanelW { get; set; }

	[DefaultValue(340)]
	[Range(100, 3000)]
	public int PanelH { get; set; }

	// ---------------------------------------------------------------- scrim

	[Header("Hud")]

	/// <summary>Draw a glass bar instead of vanilla hearts/mana.</summary>
	[DefaultValue(false)]
	public bool ReplaceVanillaHud { get; set; }

	[DefaultValue(150)]
	[Range(0, 2000)]
	public int HudX { get; set; }

	[DefaultValue(320)]
	[Range(80, 900)]
	public int HudW { get; set; }

	[DefaultValue(14)]
	[Range(0, 2000)]
	public int HudY { get; set; }

	/// <summary>Anchor the glass HUD bar to the right edge (vanilla position) or the left.</summary>
	[DefaultValue(true)]
	public bool HudAnchorRight { get; set; }

	/// <summary>Darkens the world behind an open interface (0 = off).</summary>
	[DefaultValue(0.45f)]
	[Range(0f, 0.8f)]
	[Increment(0.05f)]
	public float ScrimStrength { get; set; }

	// ---------------------------------------------------------------- debug

	[Header("Debug")]
	[DefaultValue(false)]
	public bool DebugInfo { get; set; }

	/// <summary>Shows the component style guide on screen.</summary>
	[DefaultValue(false)]
	public bool ShowDesignLab { get; set; }

	[DefaultValue(true)]
	public bool DumpVanillaAssets { get; set; }

	public override void OnLoaded()
	{
		Instance = this;
	}
}
