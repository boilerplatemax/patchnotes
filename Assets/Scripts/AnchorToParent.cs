using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnchorToParent : MonoBehaviour
{
    public Transform parentTransform;
    public Vector2 anchor = new Vector2(0, 0); // 0-1 range: (0,0)=bottom-left, (0.5,0.5)=center, (1,1)=top-right
    private SpriteRenderer parentSprite;

    void Start()
    {
        parentSprite = parentTransform.GetComponent<SpriteRenderer>();
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (!parentSprite) return;

        Vector2 parentSize = parentSprite.bounds.size;
        Vector2 offset = new Vector2(
            (anchor.x - 0.5f) * parentSize.x,
            (anchor.y - 0.5f) * parentSize.y
        );

        transform.position = parentTransform.position + (Vector3)offset;
    }

    void LateUpdate()
    {
        UpdatePosition(); // keep it updated if parent moves
    }
}

