using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utils.Text;
using MonoGame.Utils.Tuples;
using System;
using System.Collections;
using System.Collections.Generic;
using static MonoGame.Utils.Text.StylizedTextParser;

namespace MonoGame.ECS.Components.Appearance
{
    /// <summary>
    /// Represents text with possibly different fonts, colors and sizes within itself. It is enumerable over the rows of its own text.
    /// </summary>
    public class Text : IEnumerable<(IEnumerable<Word> Row, (float Width, float Height) Size)>
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

        private List<(IEnumerable<Word> Row, MutableTuple<float, float> Size)> rows;
        #endregion

        #region Constructors
        private Text(
            ContentManager content,
            SpriteFont defaultFont = null,
            Color defaultColor = default,
            float rowSpacing = 3,
            char newLine = '\n')
        {
            textParser = new StylizedTextParser(content, defaultFont, defaultColor, rowSpacing, newLine);
            Size = (-1, -1);
            RelativeCenterPosition = new Vector2(0, 0);
            RowSpacing = 3;
            Alignment = TextAlignment.LEFT;
            rows = new List<(IEnumerable<Word> Row, MutableTuple<float, float> Size)>(1);
        }

        /// <summary>
        /// Sets the values of this struct and parses a string with the StylizedTextParser class.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <param name="content">The content, used to load fonts etc.</param>
        /// <param name="defaultFont">The default font where nothing else is specified.</param>
        /// <param name="defaultColor">The default color where nothing else is specified.</param>
        /// <param name="rowSpacing">The spacing between each row in the text</param>
        /// <param name="newLine">The character that is used for declaring new lines.</param>
        public Text(
            string text,
            ContentManager content,
            SpriteFont defaultFont = null,
            Color defaultColor = default,
            float rowSpacing = 3,
            char newLine = '\n')
            : this(content, defaultFont, defaultColor, rowSpacing, newLine)
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
        /// <param name="newLine">The character that is considered as a new line.</param>
        public Text(
            string text,
            float maxWidth,
            ContentManager content,
            SpriteFont defaultFont = null,
            Color defaultColor = default,
            float rowSpacing = 3,
            char newLine = '\n')
            : this(content, defaultFont, defaultColor, rowSpacing, newLine)
        {
            AddRows(textParser.ParseAndFitTextHorizontally(text, maxWidth));
        }

        private void AddRows((IEnumerable<(IEnumerable<Word> Row, MutableTuple<float, float> Size)> Rows, (float Width, float Height) TextSize) text)
        {
            rows.AddRange(text.Rows);
            Size = text.TextSize;
        }
        #endregion

        #region Implementing IEnumerable
        public IEnumerator<(IEnumerable<Word> Row, (float Width, float Height) Size)> GetEnumerator()
        {
            return new RowEnumerator(rows);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Datatypes        
        public enum TextAlignment
        {
            LEFT, CENTER, RIGHT
        }

        public class RowEnumerator : IEnumerator<(IEnumerable<Word> Row, (float Width, float Height) Size)>
        {

            public (IEnumerable<Word> Row, (float Width, float Height) Size) Current
            {
                get
                {
                    var (Row, Size) = rowEnumerator.Current;
                    return (Row, (Size.Item1, Size.Item2));
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            private IEnumerable<(IEnumerable<Word> Row, MutableTuple<float, float> Size)> rows;
            private IEnumerator<(IEnumerable<Word> Row, MutableTuple<float, float> Size)> rowEnumerator;

            public RowEnumerator(IList<(IEnumerable<Word> Row, MutableTuple<float, float> Size)> rows)
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
