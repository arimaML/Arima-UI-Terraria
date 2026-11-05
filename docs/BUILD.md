# Сборка мода

## Требования

- tModLoader **2026.09**, таргет `net8.0`
- .NET SDK (проверено на `10.0.401`) — `dotnet --version`

Путь к tModLoader берётся из переменной окружения `TML_PATH` или свойства `-p:TMLPath`;
по умолчанию — стандартная установка Steam. Если ничего не найдено, сборка падает
с понятным сообщением.

```powershell
$env:TML_PATH = "D:\Steam\steamapps\common\tModLoader"
dotnet build -c Release
```

## Два режима

### 1. Проверка компиляции (без записи наружу)

```powershell
cd mod\TerrariaUI
dotnet build -c Release -p:BuildMod=false
```

Собирает `bin\Release\net8.0\TerrariaUI.dll` и проверяет код против реальных сборок игры,
но **не** упаковывает `.tmod`. Это режим для агента и для быстрой проверки после правок.

### 2. Полная сборка с упаковкой

```powershell
cd mod\TerrariaUI
dotnet build -c Release
```

`tMLMod.targets` после компиляции запускает `dotnet tModLoader.dll -server -build ...`,
и tModLoader кладёт `TerrariaUI.tmod` в
`%USERPROFILE%\Documents\My Games\Terraria\tModLoader\Mods`.

Важно: путь назначения задаёт сам tModLoader, переопределить его аргументом нельзя.
Если сборка запускается из песочницы (агент), запись в `Документы` будет отклонена —
ошибка выглядит так: `Mod Build error TML001: Access to the path '...\Mods\TerrariaUI.tmod' is denied`.
Тогда либо запускать сборку в обычном терминале, либо дать агенту доступ на запись.

## Как загрузить мод в игре

1. Собрать (режим 2) или скопировать `TerrariaUI.tmod` в `...\tModLoader\Mods`.
2. Запустить tModLoader → Workshop → Mods → включить **Arima-UI Terraria** → Reload.
3. Загрузить любой мир: мод один раз за сессию выгружает ванильные UI-текстуры
   в `...\tModLoader\TerrariaUI-dump\` (`*.png` + `_sizes.txt`).

Дамп нужен только для разработки: ванильные `.xnb` сжаты LZX, и размеры/референсы проще
получить из самой игры, чем распаковывать сторонними утилитами.

## Проверено

| Дата | Что | Результат |
|---|---|---|
| 2026-02 | `dotnet build -p:BuildMod=false` | 0 ошибок, `TerrariaUI.dll` 9.7 КБ |
| 2026-02 | `dotnet build` (упаковка, с разрешением на запись) | 0 ошибок, `TerrariaUI.tmod` 17 580 байт в `Mods` |
| 2026-02 | пакет 46 801 б: конфиг, стекло, дизайн-лаборатория | в `Mods`, требует Reload |

## Обязательные проверки перед сборкой

```powershell
python tools\lint_mod.py           # конфиг-атрибуты и ссылки на ассеты
python tools\lint_mod.py --selftest
```

Линтер ловит именно тот краш, который уже случался: `[Header("...")]` с пробелами валит загрузку
мода (`ValueNotTranslationKeyException`), потому что tModLoader ждёт ключ локализации.
Также проверяет формат `[DefaultValue(typeof(Color), "r, g, b, a")]`, парность `[Increment]`+`[Range]`
и существование каждого `Assets/UI/*.png`, на который ссылается код.

## Меню-пак (меню, настройки, выбор мира)

Мод не может перекрасить меню хуком: `On_Utils` в этой сборке не генерируется, а панели/кнопки
меню рисуются через `Utils.DrawSplicedPanel`. Поэтому меню делается **ресурспаком**, который
подменяет `Content/Images/UI/*` на уровне ассетов.

1. Загрузить мир с включённым модом → дампер пишет `TerrariaUI-dump/_menu-sizes.txt`
   (размеры меню-текстур, загруженных по путям).
2. Скопировать дамп в `dump/`.
3. Сгенерировать пак и проверить:

```powershell
python tools\gen_menu_pack.py --sizes dump\_menu-sizes.txt --out pack
python tools\build_pack.py --pack pack --manifest menu-manifest.json -o dist
```

Генератор держит холст равным оригиналу (иначе пак не применится), кладёт градиенты только
в угловые полосы ≤6 px (чтобы 9-slice не растягивал блик) и тонирует материалы по имени ассета:
`panel`, `button`, `input`, `scrollbar`, `slider`, `divider`.

