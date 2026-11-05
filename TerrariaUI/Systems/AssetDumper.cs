using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerrariaUI.Systems;

/// <summary>
/// Development-only helper. Writes the vanilla UI textures we need as PNG together
/// with a size manifest, so the design work can use real dimensions instead of
/// guesses. Vanilla .xnb are LZX-compressed, so we let the game decode them for us.
///
/// Runs once per session after entering a world. Output:
///   %UserProfile%/Documents/My Games/Terraria/tModLoader/TerrariaUI-dump/
/// </summary>
public class AssetDumper : ModSystem
{
	private static bool pending;

	private static readonly HashSet<string> exact = new() {
		"Heart", "Heart2", "Heart3", "Mana", "Mana2",
		"InventoryBack", "InventoryTickOn", "InventoryTickOff", "InventorySort",
		"Cursors", "CursorRadial", "LockOnCursor",
		"Logo", "Logo2", "Logo3", "Logo4",
		"Map", "MapDeath", "MapPing", "MapIcon", "MapBGs",
		"Sun", "Moon", "Cloud",
	};

	private static readonly string[] prefixes = { "InventoryBack", "Buff" };

	/// <summary>
	/// Menu/settings sheets are not exposed as TextureAssets fields - vanilla loads them by
	/// path. Their exact canvas sizes are needed for the resource-pack skin, so dump by path.
	/// </summary>
	private static readonly string[] uiPaths = {
		"PanelBackground", "PanelBorder", "InnerPanelBackground", "ButtonBacking",
		"Settings_Panel", "Settings_Panel_2", "Settings_Inputs", "Settings_Inputs_Border",
		"Settings_Inputs_2", "Settings_Inputs_2_Border", "Settings_Toggle",
		"Scrollbar", "Scrollbar2", "ScrollbarInner", "Slider_Highlight", "Divider",
		"Craft", "TexturePackButtons", "TexturePackButtonsOutline", "TitleLinkButtons",
		"Achievement_Borders", "Achievement_InnerPanelTop", "Achievement_InnerPanelBottom",
		"ButtonPlay", "ButtonDelete", "ButtonFavoriteActive", "ButtonFavoriteInactive",
		"ButtonRename", "ButtonSeed", "ButtonCloudActive", "ButtonCloudInactive",
		"PlayerBackground", "Radial", "UI_BossBar", "UI_quickicon1",
		"PVP_0", "PVP_1", "PVP_2", "Sort_0", "Sort_1",
		"Craft_Toggle_0", "Craft_Toggle_1", "Craft_Toggle_2", "Craft_Toggle_3",
		"ChestCraft_0", "ChestStack_0", "Bestiary", "Achievements",
		"InfoIcon_0", "SearchCancel", "Glyphs_0", "OnePixel",
		// vanilla bars HUD style: skinning these makes the native bars glass
		"PlayerResourceSets/HorizontalBars/HP_Fill",
		"PlayerResourceSets/HorizontalBars/HP_Fill_Honey",
		"PlayerResourceSets/HorizontalBars/HP_Panel_Middle",
		"PlayerResourceSets/HorizontalBars/HP_Panel_Right",
		"PlayerResourceSets/HorizontalBars/MP_Fill",
		"PlayerResourceSets/HorizontalBars/MP_Panel_Middle",
		"PlayerResourceSets/HorizontalBars/MP_Panel_Right",
		"PlayerResourceSets/HorizontalBars/Panel_Left",

		// character creation plates
		"CharCreation/CategoryPanel",
		"CharCreation/CategoryPanelBorder",
		"CharCreation/CategoryPanelHighlight",
		"CharCreation/SmallPanel",
		"CharCreation/SmallPanelBorder",
		"CharCreation/PanelGrayscale",
		"CharCreation/Separator1",
		"CharCreation/Separator2",

		// world creation plates (icons and previews are art - skipped by the generator)
		"WorldCreation/PreviewBorder",

		// workshop / publishing plates
		"Workshop/ListBackground",
		"Workshop/IssueButton",
		"Workshop/IssueButton_Outline",
		"Workshop/Tags",
		"Workshop/Publish",

		// loading screen
		"Sunflower_Loading",
	};

	public override void OnWorldLoad()
	{
		pending = true;
	}

	/// <summary>
	/// Root-level sheets: the tooltip plate (<c>Text_Back</c>) and chat plate
	/// (<c>Chat_Back</c>). Skinning these in the resource pack covers every tooltip.
	/// </summary>
	private static readonly string[] rootPaths = {
		"Text_Back", "Chat_Back", "Bubble", "Chat", "Chat2",
	};
	public override void PostUpdateEverything()
	{
		if (!pending)
			return;

		pending = false;

		if (Configs.UiConfig.Instance is { DumpVanillaAssets: false })
			return;

		try {
			Dump();
		}
		catch (Exception e) {
			Mod.Logger.Error("TerrariaUI: asset dump failed - " + e);
		}
	}

	private void Dump()
	{
		string dir = Path.Combine(Main.SavePath, "TerrariaUI-dump");
		Directory.CreateDirectory(dir);

		// Each stage is isolated: a failure in one group must not lose the others, and every
		// report is written even when a stage throws half way through.
		int files = 0;
		try {
			files += DumpFields(dir);
		}
		catch (Exception e) {
			StageFailed(dir, "TextureAssets", e);
		}

		try {
			files += DumpPaths(dir, "Images/UI/", uiPaths, "menu_", "_menu-sizes.txt");
		}
		catch (Exception e) {
			StageFailed(dir, "menu sheets", e);
		}

		try {
			files += DumpPaths(dir, "Images/", rootPaths, "root_", "_root-sizes.txt");
		}
		catch (Exception e) {
			StageFailed(dir, "root sheets", e);
		}

		Mod.Logger.Info($"TerrariaUI: dumped {files} textures to {dir}");
	}

	private void StageFailed(string dir, string stage, Exception e)
	{
		Mod.Logger.Warn($"TerrariaUI: dump stage '{stage}' failed - {e.GetType().Name}: {e.Message}");

		try {
			File.AppendAllText(Path.Combine(dir, "_errors.txt"),
				$"{stage}\t{e.GetType().Name}\t{e.Message}{Environment.NewLine}");
		}
		catch {
			// nothing useful to do if even the error log cannot be written
		}
	}

	private int DumpFields(string dir)
	{
		var report = new StringBuilder();
		report.AppendLine("# name\twidth\theight");

		int files = 0;
		foreach (FieldInfo field in typeof(TextureAssets).GetFields(BindingFlags.Public | BindingFlags.Static)) {
			if (!IsWanted(field.Name))
				continue;

			object value = field.GetValue(null);

			if (value is Asset<Texture2D> single) {
				files += Save(dir, field.Name, single, report);
			}
			else if (value is Asset<Texture2D>[] many) {
				for (int i = 0; i < many.Length; i++)
					files += Save(dir, $"{field.Name}[{i}]", many[i], report);
			}
		}

		File.WriteAllText(Path.Combine(dir, "_sizes.txt"), report.ToString());
		return files;
	}

	private int DumpPaths(string dir, string prefix, string[] names, string filePrefix,
		string reportName)
	{
		var report = new StringBuilder();
		report.AppendLine("# name\twidth\theight");

		int files = 0;
		foreach (string name in names) {
			try {
				Asset<Texture2D> asset = Main.Assets.Request<Texture2D>(prefix + name,
					AssetRequestMode.ImmediateLoad);
				if (asset?.Value == null) {
					report.AppendLine($"{name}\tMISSING");
					continue;
				}

				files += Save(dir, filePrefix + name, asset, report);
			}
			catch (Exception e) {
				report.AppendLine($"{name}\tERROR\t{e.GetType().Name}");
			}
		}

		File.WriteAllText(Path.Combine(dir, reportName), report.ToString());
		return files;
	}


	private static bool IsWanted(string name)
	{
		if (exact.Contains(name))
			return true;

		foreach (string prefix in prefixes) {
			if (name.StartsWith(prefix, StringComparison.Ordinal))
				return true;
		}

		return false;
	}

	private static int Save(string dir, string name, Asset<Texture2D> asset, StringBuilder report)
	{
		Texture2D texture;
		try {
			texture = asset?.Value;
		}
		catch {
			return 0;
		}

		if (texture == null)
			return 0;

		string safe = name.Replace('[', '_').Replace(']', '_');
		string target = Path.Combine(dir, safe + ".png");
		Directory.CreateDirectory(Path.GetDirectoryName(target));
		using (var stream = File.Create(target))
			texture.SaveAsPng(stream, texture.Width, texture.Height);

		report.AppendLine($"{safe}\t{texture.Width}\t{texture.Height}");
		return 1;
	}
}
