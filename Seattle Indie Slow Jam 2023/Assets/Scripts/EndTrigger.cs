using UnityEngine;

public class EndTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            GameManager.TriggerLevelComplete();
        }
        
    }
}
