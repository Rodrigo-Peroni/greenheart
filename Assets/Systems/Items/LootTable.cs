using Assets.Systems.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// This class is responsible to generate loot randomizally based
/// on the loot table with drop chances set in Prefabs that contain
/// a LootTable (e.g. the Pig prefab)
/// 
/// </summary>
public class LootTable : MonoBehaviour
{
    [Serializable]
    public struct LootTableEntry
    {
        public float percentageChance;
        public ItemCode itemCode;
        public int amount;
    }

    public LootTableEntry[] lootTableEntries;

    public Dictionary<float, LootItem> Table { get; set; }

    /// <summary>
    /// Constructor created exclusively for testing purposes.
    /// </summary>
    /// <param name="lootTable">the Loot Table</param>
    public LootTable(Dictionary<float, LootItem> lootTable)
    {
        this.Table = lootTable;
    }

    /// <summary>
    /// The Table Dictionary is loaded on the Awake() method because 
    /// loading a Dictionary directly from the Unity Editor was not 
    /// available in the Unity version where it was developed.
    /// </summary>
    void Awake()
    {
        Table = new Dictionary<float, LootItem>();

        foreach (LootTableEntry entry in lootTableEntries) {
            Table.Add(entry.percentageChance, new LootItem(entry.itemCode, entry.amount));
        }
    }

    /// <summary>
    /// Gets a LootItem based on the LootTable set in the prefab.
    /// A LootItem instance contains the item and the amount of items to be dropped.
    /// </summary>
    /// <returns>The randomized LootItem</returns>
    public LootItem GetLootFromLootTable()
    {
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        float accumulatedChance = 0f;

        foreach (KeyValuePair<float, LootItem> entry in Table)
        {
            if (randomNumber < (entry.Key + accumulatedChance))
            {
                return entry.Value;
            }
            accumulatedChance += entry.Key;
        }

        return new LootItem(ItemCode.Nothing, 0);
    }
}
