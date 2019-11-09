using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.ECS.Components.Physics;
using System;

namespace MonoGame.ECS.Systems
{
    public class MovementSystem : EntitySystem, IUpdateSystem, ISystem, IDisposable
    {

        private ComponentMapper<Transform2> transformMapper;
        private ComponentMapper<Dynamic> dynamicMapper;

        public MovementSystem() : base(Aspect.All(typeof(Transform2), typeof(Dynamic)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            dynamicMapper = mapperService.GetMapper<Dynamic>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                ApplyMovement(entityId, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        private void ApplyMovement(int entityId, float deltaTime)
        {
            if (IsMovable(entityId))
            {
                var transform = transformMapper.Get(entityId);
                var dynamic = dynamicMapper.Get(entityId);

                // Apply velocity 
                var newWorldPosition = new Vector2(
                    transform.WorldPosition.X + dynamic.Velocity.X * deltaTime,
                    transform.WorldPosition.Y + dynamic.Velocity.Y * deltaTime
                    );
                transform.Position = newWorldPosition;

                // Apply gravity
                dynamic.Velocity += dynamic.Gravity * deltaTime;
            }
        }

        private bool IsMovable(int entityId)
        {
            return transformMapper.Has(entityId) && dynamicMapper.Has(entityId);
        }

    }
}
