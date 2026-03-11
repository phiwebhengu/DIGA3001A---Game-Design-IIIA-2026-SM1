using UnityEngine;

public class Collector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        IItem item = collision.GetComponent<IItem>();
        if (item != null)
        {
            item.Collect();
        }
    }
}
