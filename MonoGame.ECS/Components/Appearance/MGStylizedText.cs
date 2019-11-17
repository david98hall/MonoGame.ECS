using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utils.Text.Stylized2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MonoGame.ECS.Components.Appearance
{
    public abstract class MGStylizedText : StylizedText<Color, SpriteFont>
    {

        private readonly static IEnumerable<PropertyInfo> colorProperties =
            typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color));

        public Vector2 RelativeCenterPosition { get; set; }

        public MGStylizedText(string text, Color defaultColor, SpriteFont defaultFont) : base(text, defaultColor, defaultFont)
        {
            RelativeCenterPosition = Vector2.Zero;
        }

        protected override Color GetColor(string colorName, float alpha = 1)
        {
            if (colorName == null)
            {
                throw new ArgumentException("The color name can't be null!");
            }

            // There is not any color with no name
            if (colorName.Length == 0)
                throw new ArgumentException("There is no color with no name");

            // Change the color name to all lowercase
            colorName = colorName.ToLower();

            // Try to find an color with the same name
            foreach (var color in colorProperties)
            {
                if (color.Name.ToLower().Equals(colorName))
                {
                    return (Color)color.GetValue(null, null) * alpha;
                }
            }

            throw new ArgumentException("A color with that name does not exist!");
        }

        protected override Color GetColor(Color color, float alpha) => color * alpha;

        protected override (float Width, float Height) MeasureString(SpriteFont font, string text)
        {
            var textSize = font.MeasureString(text);
            return (textSize.X, textSize.Y);
        }

    }
}
