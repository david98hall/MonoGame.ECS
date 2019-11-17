using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ECS.Components.Appearance;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.Utils.Extensions;
using MonoGame.Utils.Text;
using System.Collections.Generic;

namespace MonoGame.ECS.Systems
{
    public class RenderSystem : EntityDrawSystem
    {

        // Drawing
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;

        // Mappers
        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<Text> textMapper;
        private ComponentMapper<MGStylizedText> mgTextMapper;

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(Transform2)).One(typeof(Sprite), typeof(MGStylizedText), typeof(Text)))
        {
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            textMapper = mapperService.GetMapper<Text>();
            mgTextMapper = mapperService.GetMapper<MGStylizedText>();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            foreach (var entityId in ActiveEntities)
            {
                DrawEntity(entityId);
            }

            spriteBatch.End();
        }

        private void DrawEntity(int entityId)
        {
            if (transformMapper.TryGet(entityId, out Transform2 transform))
            {
                DrawSprite(entityId, transform);
                DrawEntityText(entityId, transform);
            }
        }

        private void DrawSprite(int entityId, Transform2 transform)
        {
            if (spriteMapper.TryGet(entityId, out Sprite sprite))
            {
                spriteBatch.Draw(sprite, transform);
            }
        }

        private void DrawEntityText(int entityId, Transform2 transform)
        {
            if (textMapper.TryGet(entityId, out Text text1))
            {
                DrawText(transform,
                    text1.RelativeCenterPosition,
                    text1.Size.Width, text1.Size.Height,
                    text1.RowSpacing,
                    text1.Alignment,
                    text1);
            }

            if (mgTextMapper.TryGet(entityId, out MGStylizedText text2))
            {
                DrawText(transform,
                    text2.RelativeCenterPosition,
                    text2.Size.Width, text1.Size.Height,
                    text2.RowSpacing,
                    text2.Alignment,
                    text2);
            }
        }

        private void DrawText(
            Transform2 transform,
            Vector2 relativePosition,
            float textWidth,
            float textHeight,
            float rowSpacing,
            TextAlignment textAlignment,
            IEnumerable<(IEnumerable<TextPart<Color, SpriteFont>> TextParts, (float Width, float Height) RowSize)> text)
        {
            // Start drawing at entity position       
            var startX = transform.Position.X + relativePosition.X - textWidth / 2;
            var startY = transform.Position.Y + relativePosition.Y - textHeight / 2;
            var drawPosition = new Vector2(startX, startY);

            // Draw all rows in the text
            foreach (var (Row, RowSize) in text)
            {

                // Set where the row should be drawn on the x-axis depending on the text alignment
                switch (textAlignment)
                {
                    case TextAlignment.LEFT:
                        drawPosition = new Vector2(startX, drawPosition.Y);
                        break;

                    case TextAlignment.CENTER:
                        drawPosition = new Vector2(startX + (textWidth - RowSize.Width) / 2, drawPosition.Y);
                        break;

                    case TextAlignment.RIGHT:
                        drawPosition = new Vector2(startX + textWidth - RowSize.Width, drawPosition.Y);
                        break;

                    default:
                        break;
                }

                // Draw all words in the current row
                foreach (var word in Row)
                {
                    // Draw word
                    spriteBatch.DrawString(word.Font, word.Text, drawPosition, word.Color);

                    // Update draw position
                    var wordWidth = word.Font.MeasureString(word.Text).X;
                    drawPosition += new Vector2(wordWidth, 0);
                }

                // Increment the vertical drawing coordinate for the newt row
                drawPosition += new Vector2(0, RowSize.Height + rowSpacing);

            }
        }

    }
}
