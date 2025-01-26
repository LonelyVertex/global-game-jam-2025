using UnityEngine;
using UnityEngine.VFX;

public class SwitchOnPlatform : MonoBehaviour
{
    public VisualEffect visualEffect;
    public ParticleSystem particleSystem;

    private void Start()
    {
        #if UNITY_WEBGL
        Destroy(visualEffect);
        #endif

        #if UNITY_STANDALONE
        Destroy(particleSystem);
        #endif
    }
}
