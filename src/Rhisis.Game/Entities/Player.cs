﻿using Rhisis.Game.Common;
using Rhisis.Game.Protocol.Packets.World.Server.Snapshots;
using Rhisis.Game.Resources.Properties;
using Rhisis.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rhisis.Game.Entities;

[DebuggerDisplay("{Name} Lv.{Level} ({Job.Id})")]
public sealed class Player : Mover
{
    private readonly FFUserConnection _connection;

    /// <summary>
    /// Gets the player's id.
    /// </summary>
    /// <remarks>
    /// This id is the player's id as stored in the database.
    /// </remarks>
    public int Id { get; init; }

    /// <summary>
    /// Gets the player login date.
    /// </summary>
    public DateTime LoggedInAt { get; init; }

    /// <summary>
    /// Gets the player slot.
    /// </summary>
    public int Slot { get; init; }

    /// <summary>
    /// Gets the player authority.
    /// </summary>
    public AuthorityType Authority { get; init; }

    /// <summary>
    /// Gets or sets the player's job.
    /// </summary>
    public JobProperties Job { get; set; }

    /// <summary>
    /// Gets or sets the player death level.
    /// </summary>
    public int DeathLevel { get; set; }

    /// <summary>
    /// Gets or sets the player mode.
    /// </summary>
    public ModeType Mode { get; set; }

    /// <summary>
    /// Gets the player's appearance.
    /// </summary>
    public HumanVisualAppearance Appearence { get; init; }

    /// <summary>
    /// Gets or sets the player's inventory.
    /// </summary>
    public Inventory Inventory { get; }

    /// <summary>
    /// Gets or sets the player's available statistic points.
    /// </summary>
    public int AvailablePoints { get; set; }

    /// <summary>
    /// Gets or sets the player's available skill points.
    /// </summary>
    public ushort SkillPoints { get; set; }

    /// <summary>
    /// Gets the player's gold.
    /// </summary>
    public Gold Gold { get; }

    /// <summary>
    /// Gets the player's experience.
    /// </summary>
    public Experience Experience { get; }

    /// <summary>
    /// Gets the player's skills.
    /// </summary>
    public SkillTree Skills { get; }

    /// <summary>
    /// Gets the player's quest diary.
    /// </summary>
    public QuestDiary QuestDiary { get; }

    public Player(FFUserConnection connection, MoverProperties properties)
        : base(properties)
    {
        _connection = connection;
        Inventory = new Inventory(owner: this);
        Gold = new Gold(this);
        Experience = new Experience(this);
        Skills = new SkillTree(this);
        QuestDiary = new QuestDiary(this);
    }

    public void Update()
    {
        if (IsDead || !IsSpawned)
        {
            return;
        }

        LookAround();
    }

    public void LookAround()
    {
        if (!IsSpawned || !IsVisible)
        {
            return;
        }

        IEnumerable<WorldObject> currentVisibleEntities = MapLayer.GetVisibleObjects(this).ToList();
        IEnumerable<WorldObject> appearingEntities = currentVisibleEntities.Except(VisibleObjects).ToList();
        IEnumerable<WorldObject> disapearingEntities = VisibleObjects.Except(currentVisibleEntities).ToList();

        if (appearingEntities.Any() || disapearingEntities.Any())
        {
            FFSnapshot snapshot = new();

            foreach (WorldObject appearingObject in appearingEntities)
            {
                snapshot.Merge(new AddObjectSnapshot(appearingObject, AddObjectSnapshot.PlayerAddObjMethodType.ExcludeItems));

                if (appearingObject is Mover appearingMover && appearingMover.IsMoving)
                {
                    snapshot.Merge(new DestPositionSnapshot(appearingMover));
                }

                AddVisibleEntity(appearingObject);
            }

            foreach (WorldObject disapearingObject in disapearingEntities)
            {
                snapshot.Merge(new DeleteObjectSnapshot(disapearingObject));

                RemoveVisibleEntity(disapearingObject);
            }

            Send(snapshot);
        }
    }

    public void Follow(WorldObject target)
    {
        throw new NotImplementedException();
    }

    public void Unfollow()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Item> GetEquipedItems() 
        => Inventory.GetRange(Inventory.InventorySize, Inventory.InventoryEquipParts)
            .Select(x => x.Item);

    /// <summary>
    /// Adds the given amount of skill points to the current player.
    /// </summary>
    /// <param name="skillPointsToAdd">Skill point amount to add.</param>
    /// <param name="sendToPlayer">Boolean value that indicates if the packet should be sent to the player.</param>
    public void AddSkillPoints(ushort skillPointsToAdd, bool sendToPlayer = true)
    {
        SkillPoints += skillPointsToAdd;

        if (sendToPlayer)
        {
            using SetExperienceSnapshot snapshot = new(this);
            Send(snapshot);
        }
    }

    /// <summary>
    /// Resets the player skill levels.
    /// </summary>
    public void Reskill()
    {
        foreach (Skill skill in Skills)
        {
            SkillPoints += (ushort)(skill.Level * SkillTree.SkillPointUsage[skill.Properties.JobType]);
            skill.Level = 0;
        }
    }

    /// <summary>
    /// Resets the avaialble skill points.
    /// </summary>
    public void ResetAvailableSkillPoints()
    {
        SkillPoints = 0;
    }

    /// <summary>
    /// Sends a packet to the player.
    /// </summary>
    /// <param name="packet">Packet to send.</param>
    public override void Send(FFPacket packet) => _connection.Send(packet);

    public void SendDefinedText(DefineText text, params object[] parameters)
    {
        using DefinedTextSnapshot snapshot = new(this, text, parameters);
        Send(snapshot);
    }

    private void AddVisibleEntity(WorldObject entity)
    {
        if (!VisibleObjects.Contains(entity))
        {
            VisibleObjects.Add(entity);
        }

        if (entity is not Player && !entity.VisibleObjects.Contains(this))
        {
            entity.VisibleObjects.Add(this);
        }
    }

    private void RemoveVisibleEntity(WorldObject entity)
    {
        if (VisibleObjects.Contains(entity))
        {
            VisibleObjects.Remove(entity);
        }

        if (entity.VisibleObjects.Contains(this))
        {
            entity.VisibleObjects.Remove(this);
        }
    }
}
