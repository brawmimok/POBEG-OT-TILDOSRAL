using UnityEngine;

[CreateAssetMenu(fileName = "NewDoorInfo", menuName = "Scriptable Objects/NewDoorInfo")]
public class DoorInfo : ScriptableObject
{
    public Vector3 openOffset;
    public float moveDuration = 1.25f;
    public AudioClip[] openSounds;
    public AudioClip[] closeSounds;
}