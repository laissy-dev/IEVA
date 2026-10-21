using UnityEngine;

[CreateAssetMenu(fileName = "DinoSettings", menuName = "IEVA/Réglages du dino")]
public class DinoSettings : ScriptableObject
{
    public float PatrolSpeed = 2.5f;
    public float ChaseSpeed = 4.5f;
    public float RepathInterval = 0.2f;

    public Vector3 PatrolCenter = Vector3.zero;
    public float PatrolHalfSize = 20.0f;
}
