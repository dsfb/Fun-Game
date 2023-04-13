﻿using Rhisis.Core.IO;
using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Server.Snapshots;
using Rhisis.Game.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Rhisis.Game;

public sealed class Inventory : ItemContainer
{
    public static readonly int InventorySize = 42;
    public static readonly int InventoryEquipParts = 31;
    public static readonly int EquipOffset = InventorySize;

    private static readonly Item Hand = new(GameResources.Current.Items.Get(DefineItem.II_WEA_HAN_HAND));

    private readonly Player _owner;
    private readonly Dictionary<CoolTimeType, long> _itemsCoolTimes = new()
    {
        { CoolTimeType.None, 0 },
        { CoolTimeType.Food, 0 },
        { CoolTimeType.Pills, 0 },
        { CoolTimeType.Skill, 0 }
    };

    public Inventory(Player owner)
        : base(InventorySize, InventoryEquipParts)
    {
        _owner = owner;
    }

    public void MoveItem(int sourceSlot, int destinationSlot)
    {
        if (sourceSlot < 0 || sourceSlot >= MaxCapacity)
        {
            throw new InvalidOperationException("Source slot is out of inventory range.");
        }

        if (destinationSlot < 0 || destinationSlot >= MaxCapacity)
        {
            throw new InvalidOperationException("Destination slot is out of inventory range.");
        }

        if (sourceSlot == destinationSlot)
        {
            throw new InvalidOperationException("Cannot move an item to the same slot.");
        }

        ItemContainerSlot source = GetAtSlot(sourceSlot) ?? throw new InvalidOperationException("Source slot not found.");
        ItemContainerSlot destination = GetAtSlot(destinationSlot);

        if (source.HasItem && destination.HasItem && source.Item.Id == destination.Item.Id && source.Item.Properties.IsStackable)
        {
            // TODO: stack items
        }
        else
        {
            SwapItem(sourceSlot, destinationSlot);

            using MoveItemSnapshot moveItemSnapshot = new(_owner, sourceSlot, destinationSlot);
            _owner.Send(moveItemSnapshot);
        }
    }

    public Item GetEquipedItem(ItemPartType equipedItemPart)
    {
        ItemContainerSlot equipedItemSlot = GetEquipedItemSlot(equipedItemPart);

        return equipedItemSlot.HasItem ? equipedItemSlot.Item : Hand;
    }

    public bool Equip(Item item)
    {
        if (!IsItemEquipable(item))
        {
            return false;
        }

        ItemContainerSlot equipedItemSlot = GetEquipedItemSlot(item.Properties.Parts);

        if (equipedItemSlot.HasItem)
        {
            UnequipSlot(equipedItemSlot);
        }

        ItemContainerSlot itemSlot = _items.FirstOrDefault(x => x.HasItem && x.Item.Id == item.Id && x.Item.SerialNumber == item.SerialNumber);

        if (itemSlot is not null && itemSlot.HasItem)
        {
            SwapItem(itemSlot.Slot, equipedItemSlot.Slot);

            using DoEquipSnapshot equipSnapshot = new(_owner, item, itemSlot.Index, true);
            _owner.Send(equipSnapshot);

            return true;
        }

        return false;
    }

    public bool Unequip(Item item)
    {
        ItemContainerSlot itemSlot = _items.FirstOrDefault(x => x.HasItem && x.Item.Id == item.Id && x.Item.SerialNumber == item.SerialNumber);

        if (itemSlot is null)
        {
            return false;
        }

        if (UnequipSlot(itemSlot))
        {
            using DoEquipSnapshot equipSnapshot = new(_owner, item, itemSlot.Index, false);
            _owner.Send(equipSnapshot);

            return true;
        }

        return false;
    }

    public bool IsItemEquipable(Item item)
    {
        if (item.Properties.ItemSex != int.MaxValue && (GenderType)item.Properties.ItemSex != _owner.Appearence.Gender)
        {
            _owner.SendDefinedText(DefineText.TID_GAME_WRONGSEX, item.Name);

            return false;
        }

        if (_owner.Level < item.Properties.LimitLevel)
        {
            _owner.SendDefinedText(DefineText.TID_GAME_REQLEVEL, item.Properties.LimitLevel.ToString());

            return false;
        }

        if (!_owner.Job.IsAnteriorJob(item.Properties.ItemJob))
        {
            _owner.SendDefinedText(DefineText.TID_GAME_WRONGJOB);

            return false;
        }

        Item equipedItem = GetEquipedItem(ItemPartType.RightWeapon);

        if (item.Properties.ItemKind3 == ItemKind3.ARROW && (equipedItem == null || equipedItem.Properties.ItemKind3 != ItemKind3.BOW))
        {
            return false;
        }

        return true;
    }

    public void UseItem(Item item)
    {
        ItemContainerSlot itemSlot = _items.FirstOrDefault(x => x.HasItem && x.Item.Id == item.Id && x.Item.SerialNumber == item.SerialNumber);

        if (itemSlot is null)
        {
            throw new InvalidOperationException("Failed to find item in inventory.");
        }

        if (item.Properties.IsUseable && item.Quantity > 0)
        {
            if (ItemHasCoolTime(item) && !CanUseItemWithCoolTime(item))
            {
                return;
            }

            // TODO: implement custom item usage
            // TODO: check for custom items usages

            switch (item.Properties.ItemKind2)
            {
                case ItemKind2.POTION:
                case ItemKind2.REFRESHER:
                case ItemKind2.FOOD:
                    //_inventoryItemUsage.Value.UseFoodItem(_player, item);
                    break;
                case ItemKind2.BLINKWING:
                    //_inventoryItemUsage.Value.UseBlinkwingItem(_player, item);
                    break;
                case ItemKind2.MAGIC:
                    //_inventoryItemUsage.Value.UseMagicItem(_player, item);
                    break;
                default: throw new NotImplementedException($"Item usage {item.Properties.ItemKind2} is not implemented.");
            }
        }
    }

    public bool ItemHasCoolTime(Item item) => GetItemCoolTimeGroup(item) != CoolTimeType.None;

    public bool CanUseItemWithCoolTime(Item item)
    {
        CoolTimeType group = GetItemCoolTimeGroup(item);

        return group != CoolTimeType.None && _itemsCoolTimes[group] < Time.GetElapsedTime();
    }

    public void SetCoolTime(Item item, int cooltime)
    {
        CoolTimeType group = GetItemCoolTimeGroup(item);

        if (group != CoolTimeType.None)
        {
            _itemsCoolTimes[group] = Time.GetElapsedTime() + cooltime;
        }
    }

    /// <summary>
    /// Gets the item cool time group.
    /// </summary>
    /// <param name="item">Item.</param>
    /// <returns>Returns the item cool time group.</returns>
    private static CoolTimeType GetItemCoolTimeGroup(Item item)
    {
        if (item.Properties.CoolTime <= 0)
        {
            return CoolTimeType.None;
        }

        return item.Properties.ItemKind2 switch
        {
            ItemKind2.FOOD => item.Properties.ItemKind3 == ItemKind3.PILL ? CoolTimeType.Pills : CoolTimeType.Food,
            ItemKind2.SKILL => CoolTimeType.Skill,
            _ => CoolTimeType.None,
        };
    }

    private bool UnequipSlot(ItemContainerSlot itemSlot)
    {
        ItemContainerSlot emptySlot = GetEmptySlot();

        if (emptySlot is not null && !emptySlot.HasItem)
        {
            SwapItem(itemSlot.Slot, emptySlot.Slot);

            return true;
        }

        return false;
    }

    private ItemContainerSlot GetEmptySlot()
    {
        return _items.FirstOrDefault(x => !x.HasItem);
    }

    private ItemContainerSlot GetEquipedItemSlot(ItemPartType equipedItemPart)
    {
        int equipedItemSlot = EquipOffset + (int)equipedItemPart;

        if (equipedItemSlot > MaxCapacity || equipedItemSlot < EquipOffset)
        {
            return ItemContainerSlot.Empty;
        }

        return GetAtSlot(equipedItemSlot);
    }
}
