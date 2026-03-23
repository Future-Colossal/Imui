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

        public static void TextBox(this ImGui gui, ReadOnlySpan<char> text, ImTextSettings? textSettings = null, ImRect? rect = null, bool centerHorizontally = false, bool forcePushLayout = false)
        {
            textSettings ??= gui.GetDefaultCenteredTextSettings();

            ImRect textRect;
            bool pushLayout;

            if (rect is null)
            {
                textRect = gui.GetTextRectFromLayout(text, textSettings);
                pushLayout = true;
            }
            else
            {
                textRect = rect.Value;
                pushLayout = forcePushLayout;
            }

            if (centerHorizontally)
            {
                var availableWidth = gui.Layout.GetAvailableWidth();
                var remainingWidth = availableWidth - textRect.W;
                if (remainingWidth > 0)
                {
                    textRect.X += remainingWidth * 0.5f;
                }
            }

            var id = gui.GetNextControlId();
            gui.RegisterControl(id, textRect);
            gui.Box(textRect, gui.Style.TextEdit.Normal.Box);

            gui.Text(text, textSettings.Value, textRect);
            if (pushLayout)
            {
                gui.Layout.AddRect(textRect);
            }
        }

        private static ImTextSettings GetDefaultCenteredTextSettings(this ImGui gui)
        {
            var settings = ImText.GetTextSettings(gui, true, ImTextOverflow.Truncate);
            settings.Align = new ImAlignment(0.5f, 0.5f);
            return settings;
        }

        public static ImRect GetTextRectFromLayout(this ImGui gui, ReadOnlySpan<char> text, ImTextSettings? textSettings = null)
        {
            // measure the text
            var availableSize = gui.Layout.GetAvailableSize();
            var textSpacing = gui.Style.Layout.InnerSpacing;
            var maxWidth = availableSize.x - textSpacing - textSpacing;
            var maxHeight = availableSize.y - textSpacing - textSpacing;

            var textLineHeight = gui.GetTextLineHeight();
            if (maxHeight < textLineHeight)
            {
                maxHeight = 0;
            }
                
            textSettings ??= GetDefaultCenteredTextSettings(gui);
            var measured = gui.MeasureTextSize(text, textSettings.Value, bounds: new Vector2(maxWidth, maxHeight));
            
            var size = new Vector2(measured.x + textSpacing, measured.y + textSpacing);
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