using Terraria.Localization;

namespace TerrariaUI.Common;

/// <summary>
/// Localized strings for our own UI. Keeping them in Localization/*.hjson means the drawn
/// labels follow the game language instead of being hardcoded English.
/// </summary>
public static class UiText
{
	/// <summary>Design-lab labels.</summary>
	public static string Lab(string key) => Language.GetTextValue("Mods.TerrariaUI.Lab." + key);

	/// <summary>Inventory demo labels.</summary>
	public static string Demo(string key) => Language.GetTextValue("Mods.TerrariaUI.Demo." + key);
}
