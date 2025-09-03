using System;
using System.Runtime.CompilerServices;
using Imui.Core;
using Imui.Rendering;
using Imui.Style;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Imui.Controls
{
    public static class ImDrawingUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Box(this ImGui gui, ImRect rect, in ImStyleBox style)
        {
            gui.Canvas.RectWithOutline(rect, style.BackColor, style.BorderColor, style.BorderThickness, style.BorderRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Box(this ImGui gui, ImRect rect)
        {
            ref readonly var style = ref gui.Style.Window.Box;
            gui.Canvas.RectWithOutline(rect, style.BackColor, style.FrontColor, style.BorderThickness, style.BorderRadius);
        }

        public static void TextBox(this ImGui gui, ReadOnlySpan<char> text, ImTextSettings? textSettings = null, ImRect? rect = null, bool pushLayout = true, float inset = 0f, bool centerHorizontally = false)
        {
            textSettings ??= GetDefaultCenteredTextSettings(gui);
            var thisRect = rect ?? GetTextRectFromLayout(gui, text, textSettings, inset);

            if (centerHorizontally)
            {
                var availableWidth = gui.Layout.GetAvailableWidth();
                var remainingWidth = availableWidth - thisRect.W;
                if (remainingWidth > 0)
                {
                    thisRect.X += remainingWidth * 0.5f;
                }
            }

            var id = gui.GetNextControlId();
            
            if (pushLayout)
            {
                gui.Layout.AddRect(thisRect);
            }
            
            gui.RegisterControl(id, thisRect);
            gui.Box(thisRect, gui.Style.Window.Box);

            var textRect = thisRect;
            textRect.X += inset;
            textRect.Y += inset;
            var doubleInset = inset * 2;
            textRect.W -= doubleInset;
            textRect.H -= doubleInset;
            gui.Text(text, textSettings.Value, textRect);
        }

        private static ImTextSettings GetDefaultCenteredTextSettings(ImGui gui)
        {
            var settings = ImText.GetTextSettings(gui, true, ImTextOverflow.Truncate);
            settings.Align = new ImAlignment(0.5f, 0.5f);
            return settings;
        }

        public static ImRect GetTextRectFromLayout(this ImGui gui, ReadOnlySpan<char> text, ImTextSettings? textSettings = null, float inset = 0f)
        {
            // measure the text
            var spacing = gui.Style.Layout.InnerSpacing + inset;
            var totalSpacing = spacing * 2;
            var availableWidth = gui.Layout.GetAvailableWidth();
            var availableHeight = gui.Layout.GetAvailableHeight();
            var maxWidth = availableWidth - totalSpacing;
            var maxHeight = availableHeight - totalSpacing;

            if (maxHeight < gui.GetTextLineHeight())
            {
                maxHeight = 0;
            }
                
            textSettings ??= GetDefaultCenteredTextSettings(gui);
            var measured = gui.MeasureTextSize(text, textSettings.Value, bounds: new Vector2(maxWidth, maxHeight));
            
            var size = new Vector2(measured.x + totalSpacing, measured.y + totalSpacing);
            
            var bottomLeft = gui.Layout.GetContentRect().BottomLeft;
            var rect = new ImRect(new(bottomLeft.x, bottomLeft.y - size.y), size);
            return rect;
        }

        public static bool LoadingBar(this ImGui gui, float progress, ReadOnlySpan<char> label, Color? low = null, Color? high = null, ImStyleBox? style = null, float maxWidth = float.MaxValue, ImRect? rect = null)
        {
            style ??= gui.Style.Window.Box;
            var bgStyle = style.Value;
            var fgStyle = bgStyle;
            fgStyle.BackColor = Color.Lerp(low ?? Color.red, high ?? Color.green, progress);
            fgStyle.BorderThickness = 0f;
            
            progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
            rect ??= gui.Layout.AddRect(MathF.Min(gui.Layout.GetAvailableWidth(), maxWidth), gui.GetRowHeight() + gui.Style.Layout.InnerSpacing * 2);//new ImSize(maxWidth, 0, ImSizeMode.Fill)));
            var progressRect = rect.Value;
            progressRect.W *= progress;
            gui.RegisterControl(gui.GetNextControlId(), rect.Value);
                            
            gui.Box(rect.Value, bgStyle);
            gui.Box(progressRect, fgStyle);
            
            var textSettings = ImText.GetTextSettings(gui, false, ImTextOverflow.Ellipsis);
            textSettings.Align = new ImAlignment(0.5f, 0.5f);
            gui.Text(text: label, settings: textSettings, rect: rect.Value);

            return progress >= 1f;
        }

        public static bool IsGuiElementHovered(this ImGui gui, ImRect rect)
        {
            ref readonly var meshProperties = ref gui.Canvas.GetActiveSettings();
            
            if (meshProperties.ClipRect.Enabled && !meshProperties.ClipRect.Rect.Contains(gui.Input.MousePosition))
            {
                return false;
            }

            return meshProperties.Order >= gui.nextFrameData.HoveredControl.Order && rect.Contains(gui.Input.MousePosition);
        }

        public static Vector2Int AsInt(this Vector2 vec2)
        {
            return new Vector2Int((int)vec2.x, (int)vec2.y);
        }
    }
}