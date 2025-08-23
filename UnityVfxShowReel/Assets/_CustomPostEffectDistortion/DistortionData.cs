using UnityEngine;

[CreateAssetMenu(fileName = "DistortionData", menuName = "ayy/CustomPostEffectDistortion/DistortionData")]
public class DistortionData : ScriptableObject
{
    public float CenterX;
    public float CenterY;
    public float ZoomFactor;

    public float LowerThreshold;
    public float IncThreshold;
    public float DecThreshold;

    public float TestScrollDelta = 1.0f;
}
