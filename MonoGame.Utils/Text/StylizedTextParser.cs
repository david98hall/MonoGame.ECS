using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utils.Tuples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MonoGame.Utils.Text
{
    public class StylizedTextParser
    {
        #region Properties
        public SpriteFont DefaultFont { get; set; }
        public Color DefaultColor { get; set; }

        public float RowSpacing { get; set; }
        #endregion

        #region Fields
        private static readonly char newLineChar = '\n';

        // Content
        private readonly ContentManager content;
        private static readonly string contentPath = "../Content/";
        private static readonly string fontsPath = contentPath + "Fonts/";

        // Colors
        private readonly static IEnumerable<PropertyInfo> colorProperties =
            typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color));
        #endregion

        public StylizedTextParser(ContentManager content, SpriteFont defaultFont = null, Color defaultColor = default, float rowSpacing = 3)
        {
            this.content = content;
            DefaultFont = defaultFont;
            DefaultColor = defaultColor;
            RowSpacing = rowSpacing;
        }

        #region Public methods
        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The parsed text.</returns>
        public (IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)> Rows,
            (float Width, float Height) TextSize)
            ParseText(string text)
        {
            var stylizedText = ParseTextRows(text);
            return (stylizedText, GetTextSize(stylizedText, RowSpacing));
        }

        /// <summary>
        /// Parses the text and fits it to a max width, remaking the rows if necessary
        /// </summary>
        /// <param name="text">The text to parse and fit.</param>
        /// <param name="maxWidth">The max width of the text.</param>
        /// <returns>The parsed and fitted text.</returns>
        public (IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)> Rows,
            (float Width, float Height) TextSize)
            ParseAndFitTextHorizontally(string text, float maxWidth)
        {
            var (StylizedText, TextSize) = ParseText(text);

            // If the text already fits, return it as is
            if (TextSize.Width <= maxWidth)
            {
                return (StylizedText, TextSize);
            }

            // Otherwise, redo the rows
            var newStylizedText = FitTextHorizontally(StylizedText, maxWidth);
            return (newStylizedText, GetTextSize(newStylizedText, RowSpacing));
        }

        public IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)>
            ParseTextRows(string text)
        {
            var rowCount = text.Count(t => t == newLineChar) + 1;
            text = text.Replace("\r\n", "" + newLineChar).Replace('\r', newLineChar);

            // Add empty lists and default tuples
            var stylizedText = new List<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)>, MutableTuple<float, float>)>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                stylizedText.Add((
                    new LinkedList<(string Text, SpriteFont Font, Color TextColor)>(),
                    new MutableTuple<float, float>(-1, -1)
                ));
            }

            // Build rows
            var stylizedWords = ParseWords(text);
            var rowIndex = 0;
            foreach (var (WordText, WordFont, WordColor) in stylizedWords)
            {
                // Split the word into parts where there is a new line
                string[] wordParts = WordText.Split(newLineChar);
                foreach (var wordPart in wordParts)
                {
                    // Add the word part on the correct row
                    var textRow = stylizedText[rowIndex].Item1 as LinkedList<(string Text, SpriteFont Font, Color TextColor)>;
                    textRow.AddLast((wordPart, WordFont, WordColor));

                    // Go to the next row
                    rowIndex++;
                }

                // The last part of the word after splitting does 
                // not end with a new line, decrement to fix this
                rowIndex--;
            }

            // Calculate each row size
            foreach (var row in stylizedText)
            {
                var rowSize = GetRowSize(row.Item1);
                row.Item2.Item1 = rowSize.Item1;
                row.Item2.Item2 = rowSize.Item2;
            }

            return stylizedText;
        }

        #region Static methods
        public static (float Width, float Height) GetTextSize(
            IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText,
            MutableTuple<float, float> RowSize)> text,
            float rowSpacing)
        {
            float textWidth = 0;
            float textHeight = 0;

            foreach (var (_, RowSize) in text)
            {
                if (RowSize.Item1 < 0 || RowSize.Item2 < 0)
                {
                    // If a row is not measured, the text can't be
                    return (-1, -1);
                }

                // Width
                if (RowSize.Item1 > textWidth)
                {
                    textWidth = RowSize.Item1;
                }

                // Height
                textHeight += RowSize.Item2 + rowSpacing;
            }

            return (textWidth, textHeight - rowSpacing);
        }

        public static MutableTuple<float, float> GetRowSize(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> rowText)
        {
            float rowWidth = 0;
            float rowHeight = 0;
            foreach (var (WordText, WordFont, _) in rowText)
            {
                if (WordFont == null)
                {
                    // If a font is not available, the row can't be measured.
                    return new MutableTuple<float, float>(-1, -1);
                }

                // Row Width
                var wordSize = WordFont.MeasureString(WordText);
                rowWidth += wordSize.X;

                // Row Height
                if (wordSize.Y > rowHeight)
                {
                    rowHeight = wordSize.Y;
                }
            }

            return new MutableTuple<float, float>(rowWidth, rowHeight);
        }

        #endregion

        #endregion        

        #region Helper methods

        #region Helper parsing methods
        // Fits the text to a max width and remakes the rows if necessary
        private IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)>
            FitTextHorizontally(
                IEnumerable<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)> rows,
                float maxWidth)
        {
            var newRows =
                new List<(IEnumerable<(string Text, SpriteFont Font, Color TextColor)> RowText, MutableTuple<float, float> RowSize)>(
                    rows.Count())
                {
                    (new LinkedList<(string Text, SpriteFont Font, Color TextColor)>(), new MutableTuple<float, float>(0, 0))
                };

            // Build rows
            float rowWidth = 0;
            foreach (var (RowText, RowSize) in rows)
            {
                foreach (var word in RowText)
                {
                    if (word.Text.Length > 0)
                    {
                        var wordWidth = word.Font.MeasureString(word.Text).X;
                        var wordText = word.Text;
                        var newRowWidth = rowWidth + wordWidth;

                        if (newRowWidth > maxWidth)
                        {
                            rowWidth = wordWidth;
                            newRows.Add((new LinkedList<(string Text, SpriteFont Font, Color TextColor)>(),
                                new MutableTuple<float, float>(0, 0)));
                        }
                        else
                        {
                            rowWidth = newRowWidth;
                        }


                        var words = newRows.Last().RowText as LinkedList<(string Text, SpriteFont Font, Color TextColor)>;
                        words.AddLast(word);
                    }
                }
            }

            // Calculate row sizes
            foreach (var (RowText, RowSize) in newRows)
            {
                TrimRow(RowText as LinkedList<(string Text, SpriteFont Font, Color TextColor)>);

                var rowSize = GetRowSize(RowText);
                RowSize.Item1 = rowSize.Item1;
                RowSize.Item2 = rowSize.Item2;
            }

            return newRows;
        }

        private void TrimRow(LinkedList<(string Text, SpriteFont Font, Color TextColor)> row)
        {
            TrimRow(row, true);
            TrimRow(row, false);
        }

        private void TrimRow(LinkedList<(string Text, SpriteFont Font, Color TextColor)> row, bool fromLeft)
        {
            if (row.Count > 0)
            {
                (string, SpriteFont, Color) endWord = ("", null, default);
                (string Text, SpriteFont Font, Color TextColor) currentWord;
                while (row.Count > 0)
                {
                    // Remove word from row
                    if (fromLeft)
                    {
                        currentWord = row.First.Value;
                        row.RemoveFirst();
                    }
                    else
                    {
                        currentWord = row.Last.Value;
                        row.RemoveLast();
                    }

                    var wordText = currentWord.Text;
                    bool isWhitespace = wordText.Trim().Length == 0;

                    // Trim ends
                    if (!isWhitespace)
                    {
                        if (fromLeft)
                        {
                            wordText = wordText.TrimStart();
                        }
                        else
                        {
                            wordText = wordText.TrimEnd();
                        }

                        endWord = (wordText, currentWord.Font, currentWord.TextColor);

                        break;
                    }
                }

                // Add the trimmed word back
                if (fromLeft)
                {
                    row.AddFirst(endWord);
                }
                else
                {
                    row.AddLast(endWord);
                }

            }
        }

        public IEnumerable<(string Text, SpriteFont Font, Color TextColor)> ParseWords(string text)
        {
            return ParseWords((text, DefaultFont, DefaultColor));
        }

        private IEnumerable<(string TextPart, SpriteFont Font, Color TextColor)> ParseWords((string Text, SpriteFont Font, Color TextColor) stylizedText)
        {
            var (Text, Font, TextColor) = stylizedText;

            var stylizedWords = new List<(string TextPart, SpriteFont Font, Color TextColor)>();

            var startBraceCount = Text.Count(t => t == '{');
            var endBraceCount = Text.Count(t => t == '}');
            if (startBraceCount == 0 || endBraceCount == 0)
            {
                stylizedWords.AddRange(ParseStyle(Text, Font, TextColor));
                return stylizedWords;
            }

            var latestStartBrace = -1;

            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];

                if (c == '{')
                {
                    latestStartBrace = i;
                }
                else if (c == '}' && latestStartBrace > -1)
                {
                    // Parse the words to the right of the braced area
                    var rightPart = ParseWords((Text.Substring(i + 1), Font, TextColor));

                    // Extract the style of the first word to the right of the braced area 
                    // since the left side will have it as its default style
                    var (_, FirstRightFont, FirstRightColor) = rightPart.First();

                    // Parse the words to the left of the braced area with the extracted style as the default
                    var leftPart = ParseWords((Text.Substring(0, latestStartBrace), FirstRightFont, FirstRightColor));

                    // Add the words to the left to the result
                    stylizedWords.AddRange(leftPart);

                    // Parse the braced text and add the words to the result
                    var bracedText = Text.Substring(latestStartBrace + 1, i - latestStartBrace - 1);
                    foreach (var stylizedWord in ParseStyle(bracedText, Font, TextColor))
                    {
                        stylizedWords.AddRange(ParseWords(stylizedWord));
                    }

                    // Add the words to the right to the result
                    stylizedWords.AddRange(rightPart);

                    break;
                }
            }

            return stylizedWords;
        }

        private IEnumerable<(string TextPart, SpriteFont Font, Color TextColor)> ParseStyle(string text, SpriteFont parentFont, Color parentColor)
        {
            // Bracket indexes
            var lastStartBracket = text.LastIndexOf('[');
            var lastEndBracket = text.LastIndexOf(']');

            // If there is no style block in the text, return the text with the style of its parent text
            if (lastStartBracket < 0 || lastEndBracket < 0 || lastStartBracket > lastEndBracket)
            {
                return new (string TextPart, SpriteFont Font, Color TextColor)[] { (text, parentFont, parentColor) };
            }

            #region Set text styles

            // Extract styles
            var styleText = text.Substring(lastStartBracket + 1, lastEndBracket - lastStartBracket - 1);
            var styles = Regex.Replace(styleText, @"\s", "").Split(',');
            if (styles.Length == 0)
            {
                return new (string TextPart, SpriteFont Font, Color TextColor)[] { (text, parentFont, parentColor) };
            }

            // Styles
            var font = parentFont;
            var color = parentColor;

            foreach (var s in styles)
            {
                var styleName = s.Substring(0, s.LastIndexOf('='));
                var styleValue = s.Substring(styleName.Length + 1, s.Length - styleName.Length - 1);

                if (Enum.TryParse(styleName.ToUpper(), out Style style))
                {
                    switch (style)
                    {
                        case Style.COLOR:
                            var tempColor = GetColor(styleValue);
                            color = new Color(GetColor(styleValue), color.A);
                            break;
                        case Style.FONT:
                            if (content != null)
                                font = content.Load<SpriteFont>(fontsPath + styleValue);
                            break;
                        case Style.OPACITY:
                            color = new Color(color, float.Parse(styleValue));
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion

            #region Only style whatever is left of the style block
            // Split at style block. The reason for this is to only style text left to the style block
            var styleBlock = $"[{styleText}]";
            var styleBlockIndex = text.IndexOf(styleBlock);
            var leftPart = text.Substring(0, styleBlockIndex);

            // Check if there is a text part right of the style block
            int rightIndex = styleBlockIndex + styleBlock.Length + 1;
            bool rightPartExists = rightIndex < text.Length;

            // Add the text left of the style block and any right text if there is any
            var partCount = rightPartExists ? 2 : 1;
            var stylizedParts = new (string TextPart, SpriteFont Font, Color TextColor)[partCount];
            stylizedParts[0] = (leftPart, font, color);

            if (rightPartExists)
            {
                var rightPart = text.Substring(rightIndex);
                stylizedParts[1] = (rightPart, DefaultFont, DefaultColor);
            }
            #endregion

            return stylizedParts;
        }
        #endregion

        private Color GetColor(string colorName)
        {
            // There is not any color with no name
            if (colorName.Length == 0)
                return DefaultColor;

            // Change the color name to all lowercase
            colorName = colorName.ToLower();

            // Try to find an color with the same name

            foreach (var color in colorProperties)
            {
                if (color.Name.ToLower().Equals(colorName))
                {
                    return (Color)color.GetValue(null, null);
                }
            }

            // If such a color is not found, return the default
            return DefaultColor;
        }
        #endregion

        public enum Style
        {
            COLOR, FONT, OPACITY
        }


    }
}
