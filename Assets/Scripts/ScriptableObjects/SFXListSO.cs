using UnityEngine;

[CreateAssetMenu(fileName = "SFXList_SO", menuName = "Scriptable Objects/SFXList_SO")]
public class SFXListSO : ScriptableObject {
    [Header("Ball Sounds")]
    public AudioClip[] paletteHitSounds;
    public AudioClip[] wallHitSounds;
    public AudioClip[] floorHitSounds;

    [Header("Alarm Sounds")] 
    public AudioClip countdownSoundShort;
    public AudioClip countdownSoundLong;

    [Header("Misc")] 
    public AudioClip shrinkingSound;
    public AudioClip buttonHoverSound;
}
