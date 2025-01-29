using UnityEditor.Compilation;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    
    public Transform orientation;
    public Transform playerObj;

    private Rigidbody2D rb;

    private PlayerMovement pm;

    public float slideForce;
    public float slideYScale;
    private float startYScale;

    public KeyCode slideKey = KeyCode.S;
    private float horizontalInput;
    private float verticalInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
