using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    [SerializeField] Sprite dmgSprite;
    [SerializeField] AudioClip chopSound1;
    [SerializeField] AudioClip chopSound2;
    [SerializeField] int hp = 4;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall (int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        SoundManager.instance.RandomiseSfx(chopSound1, chopSound2); 
        hp -= loss;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
