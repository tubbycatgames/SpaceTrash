using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Create Level Data Object", order = 1)]
public class LevelData : ScriptableObject
{
    public GameObject background;
    public GameObject bounceGround;
    public GameObject launchPlatform;
    public GameObject ship;

    public int skyBreadth;

    public int cloudCount;
    public float cloudDrift;
    public float cloudHeight;
    public float cloudStopHeight;
    public GameObject[] clouds;

    public float trashHeight;
    public float trashDrift;
    public int badTrashCount;
    public GameObject[] badTrash;
    public int goodTrashCount;
    public GameObject[] goodTrash;

    public float exitHeight;
}
