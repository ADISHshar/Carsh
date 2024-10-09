using UnityEngine;

public class CarCollisionHandler : MonoBehaviour
{
    public string targetTag = "Movable"; // Tag of the objects you want to move

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        
        // Check if the collided object has the target tag
        if (collision.gameObject.CompareTag(targetTag))
        {
            Debug.Log("Collided with a movable object");
            
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            
            // If the object has a Rigidbody, apply a force to it
            if (rb != null)
            {
                Debug.Log("Applying force to the movable object");
                
                // Add force to make the object move
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }
    }
}
