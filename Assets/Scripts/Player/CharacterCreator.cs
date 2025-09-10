using System;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] PlayerSpriteRenderers[] _Renderers;

    private void SetSprite(BodyPart _part, Sprite _sprite)
    {
        for (int i = 0; i < _Renderers.Length; i++)
            if (_Renderers[i].part == _part)
            {
                _Renderers[i].renderer.sprite = _sprite;
                return;
            }
    }

    public void Create(PlayerSprites[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
            SetSprite(sprites[i].part, sprites[i].sprite);
    }
}