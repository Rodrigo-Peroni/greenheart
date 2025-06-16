using Assets.Systems.Items;
using UnityEngine;

/// <summary>
/// This class is responsible for effectively spawning the loot on the screen
/// taking into consideration the item to de dropped and its amount.
/// 
/// This script should be added to the Prefab from where the items will be
/// dropper (e.g. the "Pig" prefab)
/// </summary>
public class LootDropper : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject blueDiamond;
    public GameObject purpleDiamond;
    public GameObject heart;

    public void DropLoot(LootItem lootItem)
    {
        for (int i=0; i<lootItem.Amount; i++)
        {
            GameObject item;
            switch (lootItem.Code)
            {
                default:
                case ItemCode.BlueDiamond:
                    item = Instantiate(blueDiamond, spawnPoint.position, spawnPoint.rotation);
                    break;
                case ItemCode.PurpleDiamond:
                    item = Instantiate(purpleDiamond, spawnPoint.position, spawnPoint.rotation);
                    break;
                case ItemCode.Heart:
                    item = Instantiate(heart, spawnPoint.position, spawnPoint.rotation);
                    break;
            }

            int positive = Random.Range(0, 2) * 2 - 1;
            float xForce = 100f + (Random.Range(0f, 50f));            
            item.gameObject.GetComponent<DroppableItem>().SetIsDroppable(true);
            if (positive < 0)
            {
                item.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-xForce, 300f));
            }
            else
            {
                item.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xForce, 300f));
            }
        }
    }

}
