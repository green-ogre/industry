using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Sprite full;
    public Sprite empty;
    public float spacing = 0.25f;
    private List<GameObject> fullSprites = new List<GameObject>();

    private Health health;

    void Start()
    {
        health = gameObject.transform.parent.gameObject.GetComponentInChildren<Health>();
        ConstructHealthBar(transform.position, health.Current(), health.Max());
    }

    void Update()
    {
        var i = 0;
        foreach (var sprite in fullSprites)
        {
            if (i < health.Current())
            {
                sprite.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                sprite.GetComponent<SpriteRenderer>().enabled = false;
            }

            i++;
        }
    }

    public void ConstructHealthBar(Vector3 position, int health, int maxHealth)
    {
        // var offset = new Vector3((float)maxHealth / 4f * spacing, 0, 0);
        for (int i = 0; i < maxHealth; i++)
        {
            var visible = i < health;
            var p = new Vector3(position.x + i * spacing, position.y, position.z);// - offset;
            SpawnSprite(p, empty, true);
            SpawnSprite(p, full, visible);
        }
    }

    private void SpawnSprite(Vector3 position, Sprite sprite, bool visible)
    {
        GameObject newSpriteObject = new GameObject("HealthSprite");
        SpriteRenderer renderer = newSpriteObject.AddComponent<SpriteRenderer>();
        newSpriteObject.transform.SetParent(gameObject.transform);
        renderer.sprite = sprite;
        renderer.enabled = visible;
        newSpriteObject.transform.position = position;

        if (sprite == full)
        {
            fullSprites.Add(newSpriteObject);
        }
    }
}
