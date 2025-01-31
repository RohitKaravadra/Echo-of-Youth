
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    [SerializeField][Range(0, 1f)] float _Tickness;
    [SerializeField] Color _Color;

    GameObject _OutlineObject;
    public bool EnableOutline { get => _OutlineObject.activeSelf; set { _OutlineObject.SetActive(value); } }

    public void Set(GameObject outlineObject, SpriteRenderer mySR)
    {
        SpriteRenderer objSR = outlineObject.GetComponent<SpriteRenderer>();

        objSR.sprite = mySR.sprite;
        objSR.sortingLayerID = mySR.sortingLayerID;
        objSR.sortingOrder = mySR.sortingOrder - 1;
        objSR.material.SetColor("_Color", _Color);

        _OutlineObject = outlineObject;
        _OutlineObject.transform.localScale = Vector3.one * (_Tickness + 1);

        EnableOutline = false;
    }
}
