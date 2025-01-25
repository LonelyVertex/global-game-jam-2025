using UnityEngine;

public class RocketExplosionDetergent : MonoBehaviour
{
    public AnimationCurve sizeOverTime;
    public float maxRadius;
    public float destroyAfter;
    
    float _startTime;
    
    void Start()
    {
        _startTime = Time.time;
        
        Destroy(gameObject, destroyAfter);
    }
    
    void Update()
    {
        var t = (Time.time - _startTime) / destroyAfter;
        var radius = sizeOverTime.Evaluate(t) * maxRadius;
        transform.localScale = new Vector3(radius, radius, 1);
    }
}
