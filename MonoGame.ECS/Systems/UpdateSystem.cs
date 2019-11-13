using Microsoft.Xna.Framework;
using MonoGame.ECS.Components.Updating;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;

namespace MonoGame.ECS.Systems
{
    public class UpdateSystem : EntitySystem, IUpdateSystem, ISystem, IDisposable
    {

        // Mappers
        private ComponentMapper<UpdateListener> updateListenerMapper;

        public UpdateSystem() : base(Aspect.All(typeof(UpdateListener)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            updateListenerMapper = mapperService.GetMapper<UpdateListener>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                updateListenerMapper.Get(entity).RegisterUpdate(gameTime);
            }
        }
    }
}
