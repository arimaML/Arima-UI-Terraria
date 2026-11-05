# Тексты для публикации

Готовые формулировки, чтобы не сочинять на сайте.

## About репозитория

Короткое (для поля **Description**):

```
Тёмный «liquid glass» интерфейс Terraria: панели, слоты, HUD, меню и курсоры. Мод + ресурспак.
```

Английская версия:

```
Dark liquid-glass interface for Terraria: panels, slots, HUD, menus and cursors. Mod + resource pack.
```

**Topics:** `terraria` `tmodloader` `resource-pack` `ui` `theme` `glassmorphism` `liquid-glass` `csharp`
`tmodloader-mod` `terraria-mod`

## Релиз 1.0

**Tag:** `v1.0`
**Название:** `Arima-UI Terraria 1.0`

**Описание (можно вставлять целиком):**

```markdown
Первый публичный релиз: полный редизайн интерфейса Terraria в стиле тёмного «liquid glass».

## Что меняется

- **Инвентарь** — стеклянная панель под сеткой слотов, затемнение мира за интерфейсом,
  плавное появление (fade + подъём).
- **Слоты** — тёмные пластины 52×52 со светлой кромкой (все варианты, включая сундуки и крафт).
- **HUD** — стеклянные сердца и звёзды маны 1:1, плюс опциональная полоса здоровья/маны
  с настройкой ширины, отступов и стороны.
- **Меню** — панели, кнопки, поля ввода, скроллбары, тултипы и чат в том же материале.
- **Курсоры** — 19 спрайтов, перекрашенных в палитру (формы сохранены: кирка/топор/меч).
- **Полосы HUD** — ванильный стиль «полосы» тоже в стекле: панели и заливки.

## Установка

1. `TerrariaUI.tmod` → `Documents\My Games\Terraria\tModLoader\Mods`
2. `Arima-UI Terraria.zip` → `Documents\My Games\Terraria\ResourcePacks`
3. Включить мод в tModLoader → Reload
4. Workshop → Resource Packs → включить пак
5. Перезапустить игру (пак читается при запуске)

## Требования

- Terraria 1.4.4.9 (desktop)
- tModLoader 2026.09

## Настройка

`Settings → Mod Config → Arima-UI Terraria`: плотность стекла, цвет стекла и акцента, яркость
кромки и блика, затемнение фона, параметры HUD, подгонка панели под раскладку других модов.
Всё локализовано на русский и английский.

## Известные ограничения

- Ползунки в ванильных настройках рисуются кодом, а не текстурой — пак их не перекрашивает.
- Свой шрифт невозможен без подмены атласа глифов `FontAssets`.
- Мод клиентский: в мультиплеере ничего не ломает, но и другие игроки увидят ванильный вид.
```

**Файлы релиза (загрузить вручную):**

| Файл | Откуда |
|---|---|
| `TerrariaUI.tmod` | `release/TerrariaUI.tmod` |
| `Arima-UI Terraria.zip` | `release/Arima-UI Terraria.zip` |

## Порядок публикации

```powershell
cd C:\Users\TANSHI\Documents\GitHub\Arima-UI-Terraria
git add .
git commit -m "Arima-UI Terraria 1.0"
git push
```

Затем на github.com: **Releases → Draft a new release** → tag `v1.0`, заголовок и текст выше,
приложить два файла из `release/`.
