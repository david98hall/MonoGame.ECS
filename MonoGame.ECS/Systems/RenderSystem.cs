﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.ECS.Components.Appearance;
using MonoGame.Utils.Extensions;

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

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(Transform2)).One(typeof(Sprite), typeof(Text)))
        {
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            textMapper = mapperService.GetMapper<Text>();
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
            if (textMapper.TryGet(entityId, out Text text))
            {
                // Start drawing at entity position       
                var startX = transform.Position.X + text.RelativeCenterPosition.X - text.Size.Width / 2;
                var startY = transform.Position.Y + text.RelativeCenterPosition.Y - text.Size.Height / 2;
                var drawPosition = new Vector2(startX, startY);

                // Draw all rows in the text
                foreach (var (RowText, RowSize) in text)
                {

                    // Set where the row should be drawn on the x-axis depending on the text alignment
                    switch (text.Alignment)
                    {
                        case Text.TextAlignment.LEFT:
                            drawPosition = new Vector2(startX, drawPosition.Y);
                            break;

                        case Text.TextAlignment.CENTER:
                            drawPosition = new Vector2(startX + (text.Size.Width - RowSize.Width) / 2, drawPosition.Y);
                            break;

                        case Text.TextAlignment.RIGHT:
                            drawPosition = new Vector2(startX + text.Size.Width - RowSize.Width, drawPosition.Y);
                            break;

                        default:
                            break;
                    }

                    // Draw all words in the current row
                    foreach (var (Text, Font, Color) in RowText)
                    {
                        // Draw word
                        spriteBatch.DrawString(Font, Text, drawPosition, Color);

                        // Update draw position
                        var wordWidth = Font.MeasureString(Text).X;
                        drawPosition += new Vector2(wordWidth, 0);
                    }

                    // Increment the vertical drawing coordinate for the newt row
                    drawPosition += new Vector2(0, RowSize.Height + text.RowSpacing);

                }

            }
        }

    }
}
