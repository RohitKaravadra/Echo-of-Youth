using UnityEngine;

public class Gun : MonoBehaviour
{
    public Camera cam; // For mouse tracking
    public Transform gunTransform;

    Vector2 mousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    // Updates independent of FPS.
    private void FixedUpdate()
    {
        Vector2 lookDirection = new Vector2(mousePosition.x - gunTransform.position.x,
                                            mousePosition.y - gunTransform.position.y);
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg + 270; // I am too tired to do it the normal way, so for now it's +270 degrees.
        gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gunTransform.position, 0.1f);
    }
}
