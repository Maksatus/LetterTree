#if UNITY_EDITOR
using TriInspector;
using UnityEngine;
public class LetterAttached : MonoBehaviour
{
    [OnValueChanged("ArrangeLettersInCircle")]
    public RectTransform[] Letters;
    [OnValueChanged("ArrangeLettersInCircle")]
    public float Radius = 100f;     
    [OnValueChanged("ArrangeLettersInCircle")]
    public float OffsetY = 0f;
    [OnValueChanged("ArrangeLettersInCircle")]
    public float OffsetAngle = 0f;      
    

    private void ArrangeLettersInCircle()
    {
        if (Letters == null || Letters.Length == 0)
            return;

        float angleStep = 360f / Letters.Length; 
        float angle = OffsetAngle;

        for (int i = 0; i < Letters.Length; i++)
        {
            if (Letters[i] == null) continue;
            
            float radians = angle * Mathf.Deg2Rad;
            
            float x = Mathf.Cos(radians) * Radius;
            float y = Mathf.Sin(radians) * Radius + OffsetY;
            
            Letters[i].anchoredPosition = new Vector2(x, y);
            
            angle += angleStep;
        }
    }
}
#endif