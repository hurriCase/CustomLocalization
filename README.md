# Unity Custom Localization Package

A lightweight localization system for Unity that syncs with Google Sheets, making it easy to manage and update translations for your game.

## Features

- Sync translations directly from Google Sheets
- Support for multiple sheets/tabs in a single Google Spreadsheet
- Automatic fallback to English when translations are missing
- Editor window for easy management of localization settings
- Component-based localization for UI Text and Dropdowns
- Event system for language changes

## Setup

1. Create a Google Spreadsheet for your translations
2. Make sure the spreadsheet is publicly accessible (File -> Share -> Anyone with the link -> Viewer)
3. Create your first sheet with translations following this format:
   ```
   Key,English,Spanish,French
   welcome_message,Welcome!,¡Bienvenido!,Bienvenue!
   play_button,Play,Jugar,Jouer
   ```

4. In Unity, go to Project -> Localization -> Settings
5. Set up the following:
   - Table ID (from your Google Sheets URL)
   - Save Folder (must be inside Resources folder)
   - Press "Resolve Sheets" to fetch available sheets
   - Press "Download Sheets" to download translations

## Usage

### Basic Text Localization

1. Add the `LocalizedText` component to any GameObject with a UI Text component
2. Set the Localization Key that matches your spreadsheet
3. The text will automatically update when:
   - The scene starts
   - The language changes

```csharp
// Example of setting localized text from code
GetComponent<Text>().text = LocalizationManager.Localize("welcome_message");
```

### Dropdown Localization

1. Add the `LocalizedDropdown` component to any GameObject with a Dropdown component
2. Set the Localization Keys array to match your spreadsheet keys
3. The dropdown options will automatically update when the language changes

### Changing Language

```csharp
// The language will automatically update all localized components
LocalizationManager.Language = "Spanish";
```

### Format Strings

You can use format strings in your translations:

```csharp
// In spreadsheet:
// points_earned,You earned {0} points!,¡Ganaste {0} puntos!

string message = LocalizationManager.Localize("points_earned", 100);
// Returns: "You earned 100 points!" or "¡Ganaste 100 puntos!"
```

## Project Structure

- `LocalizationManager.cs`: Core localization functionality
- `LocalizationSettings.cs`: ScriptableObject for managing Google Sheets integration
- `LocalizedText.cs`: Component for UI Text localization
- `LocalizedDropdown.cs`: Component for Dropdown localization
- `Sheet.cs`: Data structure for sheet information
- `LocalizationSettingsWindow.cs`: Editor window for management

## Best Practices

1. Use descriptive keys in your spreadsheet (e.g., `main_menu_play_button` instead of just `play`)
2. Always provide English translations as fallback
3. Keep your Google Sheets organized by using multiple sheets for different sections of your game
4. Regularly backup your translation sheets
5. Test your game with different languages to ensure proper text fitting and layout

## Known Limitations

- Supports only CSV format for downloading sheets
- UI Text and Dropdown components only (TMP support needs to be added manually)
- All translations must be downloaded during edit time

## Troubleshooting

1. **Sheets not downloading?**
   - Check if your Google Sheet is publicly accessible
   - Verify your Table ID is correct
   - Ensure you have a stable internet connection

2. **Missing translations?**
   - Check if the localization key exactly matches your spreadsheet
   - Verify the language exists in your spreadsheet
   - Check if the Save Folder is properly set in Resources

3. **Text not updating?**
   - Ensure LocalizedText component is properly attached
   - Verify the localization key exists in your spreadsheet
   - Check if the language is properly set

## Notes

- This package is for personal use only
- Built for Unity 2020.3 and later versions
- Requires Unity's EditorCoroutines package