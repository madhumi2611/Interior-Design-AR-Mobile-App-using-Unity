using UnityEngine;

public class SimpleGestureInteractable : MonoBehaviour
{
    private float previousDistance = 0f;
    private Vector2 previousTouchPos0, previousTouchPos1;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // scale - pinch
            float currentDistance = Vector2.Distance(t0.position, t1.position);
            if (previousDistance > 0)
            {
                float scaleFactor = currentDistance / previousDistance;
                transform.localScale *= scaleFactor;
            }
            previousDistance = currentDistance;

            // rotate based on midpoint drag
            Vector2 curMid = (t0.position + t1.position) / 2f;
            Vector2 prevMid = (previousTouchPos0 + previousTouchPos1) / 2f;
            Vector2 delta = curMid - prevMid;
            float rotateAmount = delta.x * 0.1f; // tweak sensitivity
            transform.Rotate(0, -rotateAmount, 0, Space.World);

            previousTouchPos0 = t0.position;
            previousTouchPos1 = t1.position;
        }
        else
        {
            previousDistance = 0;
        }
    }
}