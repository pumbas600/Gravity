using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class CelestialOutline : MonoBehaviour
{
    private SpriteRenderer sr;
    private bool isSelected = false;
    private CelestialBody body;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        body = GetComponentInParent<CelestialBody>();

        if (body.Radius < CelestialManager.MinOutlineRadius)
        {
            transform.localScale *= (CelestialManager.MinOutlineRadius / body.Radius).GetAs(new Metre());
        }

        CameraController.OnSetCameraTarget += OnSetCameraTargetSubscriber;
    }

    private void OnSetCameraTargetSubscriber(object sender, CelestialBody targetBody)
    {
        isSelected = targetBody == body;
        sr.enabled = isSelected;
    }

    private void OnMouseEnter()
    {
        sr.enabled = true;
    }

    private void OnMouseExit()
    {
        if (!isSelected)
        {
            sr.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        body.OnClick();
    }
}
