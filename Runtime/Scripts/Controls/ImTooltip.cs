using System;
using Imui.Core;
using Imui.Style;
using UnityEngine;

namespace Imui.Controls
{
    [Flags]
    public enum ImTooltipShow
    {
        None = 0,
        OnHover = 1 << 0,
        OnActive = 1 << 1
    }

    public static class ImTooltip
    {
        public static void TooltipAtLastControl(this ImGui gui, ReadOnlySpan<char> text, ImTooltipShow show = ImTooltipShow.OnHover)
        {
            TooltipAtControl(gui, gui.LastControl, text, show);
        }

        public static void TooltipAtControl(this ImGui gui, uint control, ReadOnlySpan<char> text, ImTooltipShow show = ImTooltipShow.OnHover)
        {
            var shouldShow = false;

            shouldShow |= (show & ImTooltipShow.OnHover) != 0 && gui.IsControlHovered(control);
            shouldShow |= (show & ImTooltipShow.OnActive) != 0 && gui.IsControlActive(control);

            if (!shouldShow)
            {
                return;
            }

            Tooltip(gui, text, gui.Input.MousePosition, gui.Style.Tooltip.OffsetPixels / gui.Canvas.ScreenScale);
        }

        public static void Tooltip(this ImGui gui, ReadOnlySpan<char> text, Vector2 position, Vector2 offset = default)
        {
            var textSettings = GetTextSettings(gui);
            var safeRect = gui.Canvas.SafeScreenRect;

            ref var verticalOffset = ref offset.y;
            if (verticalOffset != 0)
            {
                if (gui.Style.Tooltip.AboveCursor ||
                    float.IsNegative(verticalOffset) 
                        ? position.y + verticalOffset < safeRect.Bottom
                        : position.y + verticalOffset > safeRect.Top)
                {
                    offset.y = -offset.y;
                }
            }

            position += offset;
            position.x = Math.Clamp(position.x, safeRect.Left, safeRect.Right);
            position.y = Math.Clamp(position.y, safeRect.Bottom, safeRect.Top);

            var bounds = gui.Canvas.SafeScreenRect.TakeRight(position.x).TakeBottom(position.y);
            var textSize = gui.MeasureTextSize(text, textSettings, bounds.Size);
            ref readonly var padding = ref gui.Style.Tooltip.Padding;
            var width = textSize.x + padding.Horizontal;
            var height = textSize.y + padding.Vertical;
            
            var widthDelta = bounds.Right - (width + position.x);
            var heightDelta = bounds.Top - (height + position.y);
            position.x += Math.Min(widthDelta, 0);
            position.y += Math.Min(heightDelta, 0);

            Tooltip(gui, text, new ImRect(position.x, position.y,width, height));
        }

        public static void Tooltip(this ImGui gui, ReadOnlySpan<char> text, ImRect rect)
        {
            gui.BeginPopup();
            gui.Box(rect, gui.Style.Tooltip.Box);
            gui.Text(text, GetTextSettings(gui), rect);
            gui.EndPopup();
        }

        public static ImTextSettings GetTextSettings(ImGui gui)
        {
            return new ImTextSettings(gui.Style.Layout.TextSize, 0.5f, 0.5f);
        }
    }
}