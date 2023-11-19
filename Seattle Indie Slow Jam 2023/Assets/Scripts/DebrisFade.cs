using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisFade : MonoBehaviour
{
    Rigidbody thisrigidbody;
    private float PieceFadeSpeed = 2.5f;
    private float PieceDestroyDelay = 0.5f;
    private float PieceSleepCheckDelay = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        thisrigidbody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(FadeOutRigidBody(thisrigidbody));
    }

    private IEnumerator FadeOutRigidBody(Rigidbody rigidbody)
    {
        WaitForSeconds Wait = new WaitForSeconds(PieceSleepCheckDelay);

        while (rigidbody.IsSleeping() == false)
        {
            yield return Wait;
        }

        yield return new WaitForSeconds(PieceDestroyDelay);

        float time = 0;
        Renderer renderer = rigidbody.GetComponent<Renderer>();

        Destroy(rigidbody.GetComponent<Collider>());
        Destroy(rigidbody);

        while (time < 1)
        {
            float step = Time.deltaTime * PieceFadeSpeed;
            renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
            time += step;
            yield return null;
        }
            
        Destroy(renderer.gameObject);
        Destroy(gameObject);
    }
}
