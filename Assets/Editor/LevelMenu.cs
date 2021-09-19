using UnityEditor;

public class LevelMenu {
    
    [MenuItem("Level/Generate/Planet1")]
    public static void Generate01() {
        var levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/ScriptableObjects/Level01Data.asset");
        LevelGenerator.Generate(levelData);
    }
    [MenuItem("Level/Generate/Planet2")]
    public static void Generate02() {
        var levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/ScriptableObjects/Level02Data.asset");
        LevelGenerator.Generate(levelData);
    }
    [MenuItem("Level/Generate/Planet3")]
    public static void Generate03() {
        var levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/ScriptableObjects/Level03Data.asset");
        LevelGenerator.Generate(levelData);
    }
}
