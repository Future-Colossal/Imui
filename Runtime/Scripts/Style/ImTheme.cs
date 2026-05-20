using System;
using Color = UnityEngine.Color;
using Color32 = UnityEngine.Color32;

namespace Imui.Style
{
    [Serializable]
    public struct ImTheme : IEquatable<ImTheme>
    {
        public float TextSize;
        public float Spacing;
        public float InnerSpacing;
        public float Indent;
        public float ExtraRowHeight;
        public float ScrollBarSize;
        public float WindowBorderRadius;
        public float WindowBorderThickness;
        public float BorderRadius;
        public float BorderThickness;
        public float ReadOnlyColorMultiplier;

        public Color Background;
        public Color Foreground;
        public Color Control;
        public Color Accent;
        public float Contrast;
        public float BorderContrast;

        public bool Equals(ImTheme other)
        {
            return TextSize.Equals(other.TextSize) &&
                   Spacing.Equals(other.Spacing) &&
                   InnerSpacing.Equals(other.InnerSpacing) &&
                   Indent.Equals(other.Indent) &&
                   ExtraRowHeight.Equals(other.ExtraRowHeight) &&
                   ScrollBarSize.Equals(other.ScrollBarSize) &&
                   WindowBorderRadius.Equals(other.WindowBorderRadius) &&
                   WindowBorderThickness.Equals(other.WindowBorderThickness) &&
                   BorderRadius.Equals(other.BorderRadius) &&
                   BorderThickness.Equals(other.BorderThickness) &&
                   ReadOnlyColorMultiplier.Equals(other.ReadOnlyColorMultiplier) &&
                   Background.Equals(other.Background) &&
                   Foreground.Equals(other.Foreground) &&
                   Control.Equals(other.Control) &&
                   Accent.Equals(other.Accent) &&
                   Contrast.Equals(other.Contrast) &&
                   BorderContrast.Equals(other.BorderContrast);
        }

        public override bool Equals(object obj)
        {
            return obj is ImTheme other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(TextSize);
            hashCode.Add(Spacing);
            hashCode.Add(InnerSpacing);
            hashCode.Add(Indent);
            hashCode.Add(ExtraRowHeight);
            hashCode.Add(ScrollBarSize);
            hashCode.Add(WindowBorderRadius);
            hashCode.Add(WindowBorderThickness);
            hashCode.Add(BorderRadius);
            hashCode.Add(BorderThickness);
            hashCode.Add(ReadOnlyColorMultiplier);
            hashCode.Add(Background);
            hashCode.Add(Foreground);
            hashCode.Add(Control);
            hashCode.Add(Accent);
            hashCode.Add(Contrast);
            hashCode.Add(BorderContrast);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(ImTheme left, ImTheme right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImTheme left, ImTheme right)
        {
            return !left.Equals(right);
        }
    }
    
    public struct ImThemePalette
    {
        public bool IsDark;

        public Color Back;
        public Color Front;
        public Color Control;

        public Color Accent;
        public Color AccentFront;
    }

    public static class ImThemeBuiltin
    {
        public static ImTheme Wire()
        {
            return new ImTheme()
            {
                TextSize = 20f,
                Spacing = 3.25f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.9f,
                Background = new Color32(255, 255, 255, 255),
                Foreground = new Color32(30, 30, 30, 255),
                Accent = new Color32(194, 230, 255, 255),
                Control = new Color32(255, 255, 255, 255),
                Contrast = 0f,
                BorderContrast = 2f
            };
        }
        
        public static ImTheme LightTouch()
        {
            var theme = Light();

            theme.TextSize = 23f;
            theme.Spacing = 5f;
            theme.InnerSpacing = 6.5f;
            theme.ExtraRowHeight = 11f;

            return theme;
        }
        
        public static ImTheme DarkTouch()
        {
            var theme = Dark();

            theme.TextSize = 23f;
            theme.Spacing = 5f;
            theme.InnerSpacing = 6.5f;
            theme.ExtraRowHeight = 11f;

            return theme;
        }

        public static ImTheme Light()
        {
            return new ImTheme()
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.9f,
                Background = new Color32(238, 238, 238, 255),
                Foreground = new Color32(30, 30, 30, 255),
                Accent = new Color32(0, 144, 242, 255),
                Control = new Color32(221, 221, 221, 255),
                Contrast = -0.03f,
                BorderContrast = 0.17f
            };
        }

        public static ImTheme Dark()
        {
            return new ImTheme
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(49, 49, 49, 255),
                Foreground = new Color32(224, 224, 224, 255),
                Accent = new Color32(17, 121, 200, 255),
                Control = new Color32(79, 79, 79, 255),
                Contrast = 0f,
                BorderContrast = -0.04f
            };
        }

        public static ImTheme Dear()
        {
            return new ImTheme
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 3f,
                Indent = 8f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 12f,
                WindowBorderRadius = 0f,
                WindowBorderThickness = 1f,
                BorderRadius = 0f,
                BorderThickness = 0f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(10, 10, 10, 242),
                Foreground = new Color32(255, 255, 255, 255),
                Accent = new Color32(89, 148, 243, 255),
                Control = new Color32(75, 114, 200, 118),
            };
        }

        public static ImTheme Orange()
        {
            return new ImTheme
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(17, 18, 18, 245),
                Foreground = new Color32(224, 224, 224, 255),
                Accent = new Color32(211, 85, 12, 255),
                Control = new Color32(0, 121, 255, 11),
            };
        }

        public static ImTheme Terminal()
        {
            return new ImTheme
            {
                TextSize = 18f,
                Spacing = 1f,
                InnerSpacing = 2f,
                Indent = 8f,
                ExtraRowHeight = 0f,
                ScrollBarSize = 15f,
                WindowBorderRadius = 0f,
                WindowBorderThickness = 1f,
                BorderRadius = 0f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(0, 0, 0, 240),
                Foreground = new Color32(18, 255, 0, 255),
                Accent = new Color32(52, 224, 0, 255),
                Control = new Color32(0, 95, 3, 255),
                Contrast = 0f,
                BorderContrast = 0f
            };
        }
        
        public static ImTheme EnlargedForTouch(this ImTheme theme)
        {
            const float textSize = 23/20f;
            const float spacing = 5f/3f;
            const float innerSpacing = 6.5f/5f;
            const float extraRowHeight = 11f/4f;
            
            theme.TextSize *= textSize;
            theme.Spacing *= spacing;
            theme.InnerSpacing *= innerSpacing;
            theme.ExtraRowHeight *= extraRowHeight;
            return theme;
        }
    }
}
