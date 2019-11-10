﻿using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.ECS.Components.Input.Pointer;
using System;
using MonoGame.Utils.Geometry;

namespace MonoGame.ECS.Systems.Input.Pointer
{
    public abstract class PointerInputSystem : EntitySystem, IUpdateSystem, ISystem, IDisposable
    {
        // Component mappers        
        protected ComponentMapper<PointerInputBody> pointerInputBodyMapper;
        protected ComponentMapper<PointerInput> pointerInputMapper;
        protected ComponentMapper<Transform2> transformMapper;

        public ViewportAdapter ViewportAdapter { get; set; }

        public PointerInputSystem(ViewportAdapter viewportAdapter)
            : base(Aspect.All(typeof(PointerInputBody), typeof(PointerInput), typeof(Transform2)))
        {
            ViewportAdapter = viewportAdapter;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            pointerInputBodyMapper = mapperService.GetMapper<PointerInputBody>();
            pointerInputMapper = mapperService.GetMapper<PointerInput>();
            transformMapper = mapperService.GetMapper<Transform2>();
        }

        public abstract void Update(GameTime gameTime);

        protected bool IsWithinEntity(int entity, Vector2 pointerLocation)
        {
            var body = pointerInputBodyMapper.Get(entity).Body;
            var transform = transformMapper.Get(entity);
            var bodyPosition = body.RelativePosition + transform.Position;

            return GeometryUtils.IsWithinPolygon(
                pointerLocation,
                body.Shape.Vertices,
                bodyPosition,
                body.MaxWidth,
                body.MaxHeight
            );
        }

    }

}