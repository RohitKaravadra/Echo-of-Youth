using Unity.VisualScripting;
using UnityEngine;

public class DragAll : MonoBehaviour
{
    private GameObject selectedObject;

    private static Transform draggingTransform = null;
    private static Rigidbody2D draggingRB;
    private Vector3 offset;

    [Header("Movable objects")]
    [SerializeField] private float forceMultiplier;
    [SerializeField] private float damping;
    [SerializeField] private LayerMask movableLayers;

    [Header("Gun Point")]
    [SerializeField] private GameObject dragIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dragIndicator.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragIndicator.SetActive(true);

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                                                 float.PositiveInfinity, movableLayers);

            if (hit)
            {
                draggingTransform = hit.transform;
                draggingRB = hit.rigidbody;
                offset = draggingTransform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            draggingRB = null;
            draggingTransform = null;

            dragIndicator.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (draggingRB != null)
        {
            Vector2 V = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset - draggingTransform.position;
            draggingRB.AddForce(V * forceMultiplier);

            draggingRB.linearVelocity *= damping;
        }
    }

    public static Rigidbody2D GetDraggingRB()
    {
        return draggingRB;
    }
}
