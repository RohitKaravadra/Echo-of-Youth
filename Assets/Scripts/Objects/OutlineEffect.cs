
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    [SerializeField][Range(0, 1f)] float _Tickness;
    [SerializeField] GameObject _OutlineObject;
    [SerializeField] Color _Color;

    public bool Enabled { get => _OutlineObject.activeSelf; set { _OutlineObject.SetActive(value); } }

    public void Set(SpriteRenderer mySR)
    {
        SpriteRenderer objSR = _OutlineObject.GetComponent<SpriteRenderer>();

        objSR.sprite = mySR.sprite;
        objSR.sortingLayerID = mySR.sortingLayerID;
        objSR.sortingOrder = mySR.sortingOrder - 1;
        objSR.material.SetColor("_Color", _Color);

        _OutlineObject.transform.localScale = Vector3.one * (_Tickness + 1);

        Enabled = false;
    }
}
