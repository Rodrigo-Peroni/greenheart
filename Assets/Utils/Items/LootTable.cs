using Assets.Utils.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Awake()
    {
        Table = new Dictionary<float, LootItem>();

        foreach (LootTableEntry entry in lootTableEntries) {
            Table.Add(entry.percentageChance, new LootItem(entry.itemCode, entry.amount));
        }
    }

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
