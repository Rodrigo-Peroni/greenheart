using System.Collections;
using System.Collections.Generic;
using Assets.Systems.Items;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class ItemsTestScript
{    
    private LootTable lootTable;

    [SetUp]
    public void Setup()
    {
        Dictionary<float, LootItem> lootDropRates = new Dictionary<float, LootItem>
        {
            { 0.40f, new LootItem(ItemCode.BlueDiamond, 1) },   // 40% chance for 1 Blue Diamond
            { 0.30f, new LootItem(ItemCode.BlueDiamond, 2) },   // 30% chance for 2 Blue Diamonds
            { 0.10f, new LootItem(ItemCode.BlueDiamond, 3) },   // 10% chance for 3 Blue Diamonds
            { 0.05f, new LootItem(ItemCode.PurpleDiamond, 1) }, // 5% chance for 1 Purple Diamond
            { 0.15f, new LootItem(ItemCode.Heart, 1) },         // 15% chance for 1 Heart
        };        
        this.lootTable = new LootTable(lootDropRates);
    }

    [Test]
    public void LootTable_ReturnsValidLootItem()
    {
        // ARRANGE - Done in the Setup

        // ACT
        LootItem lootItem = lootTable.GetLootFromLootTable();

        // ASSERT
        Assert.IsNotNull(lootItem);
        Assert.IsInstanceOf<LootItem>(lootItem);        
    }

    [Test]
    public void LootTable_GeneratesDifferentItems()
    {
        // There is a small chance that this test would fail (in case it generates 1000 times the same loot)
        int maximumIterations = 1000;

        // ARRANGE - Get first loot drop
        LootItem firstLootItem = lootTable.GetLootFromLootTable();        

        // ACT
        for (int i = 0; i < maximumIterations; i++)
        {
            LootItem newLoot = lootTable.GetLootFromLootTable();

            // ASSERT
            // If a different LootItem is generated, we consider it passed.
            if (newLoot.Code != firstLootItem.Code || newLoot.Amount != firstLootItem.Amount)
            {
                Assert.Pass("The LootTable generated varied loot.");
            }
        }
        
        // The test fails if no different loot is generated after 1000 iterations.
        Assert.Fail("The LootTable generated the same loot for 1000 iterations.");
    }
}
