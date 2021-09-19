using UnityEngine;

public class LevelGeneratingBehaviour : MonoBehaviour
{
    public LevelData levelData;
    public bool generateLevel;
    void Start()
    {
        GlobalGameController.control.currentLevelData = levelData;
        if(generateLevel)
            LevelGenerator.Generate(levelData);
    }
}
