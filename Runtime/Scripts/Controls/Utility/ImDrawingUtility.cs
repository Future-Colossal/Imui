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

        public static uint TextBox(this ImGui gui, ReadOnlySpan<char> text, in ImStyleBox? style = null, ImTextSettings? textSettings = null, ImRect? rect = null)
        {
            if (textSettings == null)
            {
                var settings = ImText.GetTextSettings(gui, true, ImTextOverflow.Truncate);
                settings.Align = new ImAlignment(0.5f, 0.5f);
                textSettings = settings;
            }
            
            if (rect == null)
            {
                // measure the text
                var maxWidth = gui.Layout.GetAvailableWidth();
                var availableHeight = gui.Layout.GetAvailableHeight();
                var maxHeight = availableHeight;

                var extraSpacing= gui.Style.Layout.InnerSpacing * 2; 
                if (maxHeight < gui.GetTextLineHeight() + extraSpacing)
                {
                    maxHeight = 0;
                }
                
                var measured = gui.MeasureTextSize(text, textSettings.Value, bounds: new Vector2(maxWidth, maxHeight));
                rect = gui.AddLayoutRectWithSpacing(new Vector2(measured.x + extraSpacing, measured.y + extraSpacing));
            }

            var id = gui.GetNextControlId();
            gui.Layout.Push(gui.Layout.Axis, rect.Value);
            gui.RegisterControl(id, rect.Value);
            gui.Box(rect.Value, style ?? gui.Style.Window.Box);
            gui.Text(text, textSettings.Value, rect.Value);
            gui.Layout.Pop();
            
            return id;
        }

        public static bool LoadingBar(this ImGui gui, float progress, ReadOnlySpan<char> label, Color? low = null, Color? high = null, ImStyleBox? style = null)
        {
            style ??= gui.Style.Window.Box;
            progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
            var bgStyle = style.Value;
            var fgStyle = bgStyle;
            var rect = gui.AddSingleRowRect(new ImSize(0, 0, ImSizeMode.Fill));
            var progressRect = rect;
            progressRect.W *= progress;
                            
            fgStyle.BackColor = Color.Lerp(low ?? Color.red, high ?? Color.green, MathF.Sqrt(progress));
            gui.Box(rect, bgStyle);
            gui.Box(progressRect, fgStyle);
            gui.Text(label, rect);

            return progress >= 1f;
        }
        
        public static bool IsHovered(this ImGui gui, ImRect rect)
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