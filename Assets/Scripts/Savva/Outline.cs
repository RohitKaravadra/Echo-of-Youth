using UnityEngine;

public class Outline : MonoBehaviour
{
    public KeyCode reverseKey = KeyCode.E;
    private Reverse positionHistory;
    private static Reverse activeReversingObject;

    [SerializeField] private GameObject outLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outLine.SetActive(false);

        positionHistory = GetComponent<Reverse>();
        if (positionHistory == null)
        {
            Debug.LogError("Reverse component is missing on this object.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(reverseKey) && activeReversingObject != null)
        {
            activeReversingObject.StopReversing();
            activeReversingObject = null;
        }  
    }

    private void OnMouseOver()
    {
        outLine.SetActive(true);

        if (Input.GetKeyDown(reverseKey))
        {
            activeReversingObject = positionHistory;
            activeReversingObject?.StartReversing();
        }
    }

    private void OnMouseExit()
    {
        outLine.SetActive(false);
    }
}
