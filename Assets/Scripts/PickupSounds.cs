using UnityEngine;

[CreateAssetMenu(fileName = "PickupSounds", menuName = "SoundData/Pickup Sounds", order = 1)]
public class PickupSounds : ScriptableObject
{
    [System.Serializable]
    public class CustomSound {
        public GameObject prefab;

        [SerializeField]
        [FMODUnity.EventRef]
        public string sound;
    }

    [SerializeField]
    [FMODUnity.EventRef]
    public string pickupSound;
    [SerializeField]
    [FMODUnity.EventRef]
    public string hitSound;

    public CustomSound[] customSounds;
}
