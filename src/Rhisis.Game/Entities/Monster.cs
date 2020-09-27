﻿using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures;
using Rhisis.Game.Abstractions.Behavior;
using Rhisis.Game.Abstractions.Components;
using Rhisis.Game.Abstractions.Entities;
using Rhisis.Game.Abstractions.Features;
using Rhisis.Game.Abstractions.Map;
using Rhisis.Game.Abstractions.Systems;
using Rhisis.Game.Common;
using Rhisis.Game.Common.Resources;
using Rhisis.Game.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rhisis.Game.Entities
{
    [DebuggerDisplay("{Name} Lv.{Level}")]
    public class Monster : IMonster
    {
        private readonly Lazy<IFollowSystem> _followSystem;

        public uint Id { get; }

        public WorldObjectType Type => WorldObjectType.Mover;

        public int ModelId => Data.Id;

        public IMap Map { get; set; }

        public IMapLayer MapLayer { get; set; }

        public Vector3 Position { get; set; }

        public float Angle { get; set; }

        public short Size { get; set; }

        public string Name => Data.Name;

        public IServiceProvider Systems { get; set; }

        public bool IsAggresive { get; set; }

        public bool IsFlying => Data.IsFlying;

        public int Level
        {
            get => Data.Level;
            set => throw new InvalidOperationException();
        }

        public Vector3 DestinationPosition { get; set; } = new Vector3();

        public float Speed
        {
            get
            {
                // TODO: add bonus attributes
                return Data.Speed * SpeedFactor;
            }
        }

        public float SpeedFactor { get; set; } = 1;

        public bool IsMoving => ObjectState.HasFlag(ObjectState.OBJSTA_MOVE_ALL) && !DestinationPosition.IsZero();

        public MoverData Data { get; set; }

        public bool Spawned { get; set; }

        public ObjectState ObjectState { get; set; }

        public StateFlags ObjectStateFlags { get; set; }

        public IHealth Health { get; }

        public IAttributes Attributes { get; set; }

        public IStatistics Statistics { get; }

        public IList<IWorldObject> VisibleObjects { get; set; }

        public bool CanRespawn => RespawnRegion != null;

        public IMapRespawnRegion RespawnRegion { get; set; }

        public IBehavior Behavior { get; set; }

        public IMonsterTimers Timers { get; }

        public IWorldObject FollowTarget { get; set; }

        public float FollowDistance { get; set; }

        public Monster()
        {
            Id = RandomHelper.GenerateUniqueId();
            Health = new Health(this);
            Attributes = new Attributes(this);
            Statistics = new StatisticsComponent();
            VisibleObjects = new List<IWorldObject>();
            Timers = new MonsterTimersComponent();
            _followSystem = new Lazy<IFollowSystem>(() => Systems.GetService<IFollowSystem>());
        }

        public void Follow(IWorldObject worldObject) => _followSystem.Value.Follow(this, worldObject);

        public void Unfollow() => _followSystem.Value.Unfollow(this);

        public bool Equals(IWorldObject other) => Id == other.Id;
    }
}
