using UnityEngine;

public class Attack : MonoBehaviour
{
    public float speed;
    public float timeDestroy;
    public int attackPower;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeDestroy);    
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
