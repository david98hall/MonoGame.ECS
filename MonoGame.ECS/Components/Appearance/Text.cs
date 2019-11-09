using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utils.Text;
using MonoGame.Utils.Tuples;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.ECS.Components.Appearance
{
    /// <summary>
    /// Represents text with possibly different fonts, colors and sizes within itself. It is enumerable over the rows of its own text.
    /// </summary>
    public class Text : IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, (float Width, float Height) RowSize)>
    {

        #region Properties
        /// <summary>
        /// The size of the text in number of pixels.
        /// </summary>
        public (float Width, float Height) Size { get; private set; }

        /// <summary>
        /// The relative position to where the text should be drawn.
        /// </summary>
        public Vector2 RelativeCenterPosition { get; set; }

        /// <summary>
        /// The spacing between the text rows in pixels.
        /// </summary>
        public float RowSpacing
        {
            get => textParser.RowSpacing;
            set => textParser.RowSpacing = value;
        }

        /// <summary>
        /// The text alignment.
        /// </summary>
        public TextAlignment Alignment { get; set; }

        /// <summary>
        /// The default font of this text. 
        /// If nothing else is specified, this is the font that will be used.
        /// </summary>
        public SpriteFont DefaultFont
        {
            get => textParser.DefaultFont;
            set => textParser.DefaultFont = value;
        }

        /// <summary>
        /// The default color of this text. 
        /// If nothing else is specified, this is the color that will be used.
        /// </summary>
        public Color DefaultColor
        {
            get => textParser.DefaultColor;
            set => textParser.DefaultColor = value;
        }
        #endregion

        #region Fields
        private readonly StylizedTextParser textParser;

        private List<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, MutableTuple<float, float> RowSize)> rows;
        #endregion

        #region Constructors
        private Text(ContentManager content, SpriteFont defaultFont = null, Color defaultColor = default, float rowSpacing = 3)
        {
            textParser = new StylizedTextParser(content, defaultFont, defaultColor, rowSpacing);
            Size = (-1, -1);
            RelativeCenterPosition = new Vector2(0, 0);
            RowSpacing = 3;
            Alignment = TextAlignment.LEFT;
            rows = new List<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, MutableTuple<float, float> RowSize)>(1);
        }

        /// <summary>
        /// Sets the values of this struct and parses a string with the StylizedTextParser class.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <param name="content">The content, used to load fonts etc.</param>
        /// <param name="defaultFont">The default font where nothing else is specified.</param>
        /// <param name="defaultColor">The default color where nothing else is specified.</param>
        /// <param name="rowSpacing">The spacing between each row in the text</param>
        public Text(string text, ContentManager content, SpriteFont defaultFont = null, Color defaultColor = default, float rowSpacing = 3)
            : this(content, defaultFont, defaultColor, rowSpacing)
        {
            AddRows(textParser.ParseText(text));
        }

        /// <summary>
        /// Sets the values of this struct and parses a string with the StylizedTextParser class. 
        /// It also fits the text to the given max width and overrides any specified new lines in the original text.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <param name="maxWidth">The max width of the text.</param>
        /// <param name="content">The content, used to load fonts etc.</param>
        /// <param name="defaultFont">The default font where nothing else is specified.</param>
        /// <param name="defaultColor">The default color where nothing else is specified.</param>
        /// <param name="rowSpacing">The spacing between each row in the text</param>
        public Text(string text, float maxWidth, ContentManager content, SpriteFont defaultFont = null, Color defaultColor = default, float rowSpacing = 3)
            : this(content, defaultFont, defaultColor, rowSpacing)
        {
            AddRows(textParser.ParseAndFitTextHorizontally(text, maxWidth));
        }

        private void AddRows((IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)> Rows,
            (float Width, float Height) TextSize) text)
        {
            rows.AddRange(text.Rows);
            Size = text.TextSize;
        }
        #endregion

        #region Implementing IEnumerable
        public IEnumerator<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, (float Width, float Height) RowSize)> GetEnumerator()
        {
            return new RowEnumerator(rows);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Enums and classes
        /// <summary>
        /// Text alignment.
        /// </summary>
        public enum TextAlignment
        {
            LEFT, CENTER, RIGHT
        }

        public class RowEnumerator : IEnumerator<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, (float Width, float Height) RowSize)>
        {

            public (IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, (float Width, float Height) RowSize) Current
            {
                get
                {
                    var (RowText, RowSize) = rowEnumerator.Current;
                    return (RowText, (RowSize.Item1, RowSize.Item2));
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            private IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, MutableTuple<float, float> RowSize)> rows;
            private IEnumerator<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, MutableTuple<float, float> RowSize)> rowEnumerator;

            public RowEnumerator(IList<(IEnumerable<(string Text, SpriteFont Font, Color Color)> RowText, MutableTuple<float, float> RowSize)> rows)
            {
                this.rows = rows;
                Reset();
            }

            public void Dispose()
            {
                rowEnumerator.Dispose();
                rows = null;
            }

            public bool MoveNext() => rowEnumerator.MoveNext();

            public void Reset()
            {
                rowEnumerator = rows.GetEnumerator();
            }
        }
        #endregion

    }
}
