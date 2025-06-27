using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public void Collect()
    {
        Destroy(gameObject);
    }
}
