using UnityEngine;

public class LevelGenerator {
    
    public static void Generate(LevelData levelData) 
    {
        if(GameObject.Find("Level Container"))
        {
            GameObject.DestroyImmediate(GameObject.Find("Level Container"));
        }
        var screenBottom = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
        var parent = GenerateWrapper(screenBottom, levelData);

        GameObject BG = GameObject.Instantiate(levelData.background, parent);
        BG.GetComponent<BackgroundBuilder>().BuildBackGround();
        GenerateGroundStack(screenBottom, levelData, parent);

        var driftDirection = Random.value < .5 ? 1 : -1;
        GenerateClouds(levelData, parent, driftDirection);
        GenerateGoodTrash(levelData, parent, driftDirection);
        GenerateBadTrash(levelData, parent, driftDirection);
    }

    private static Transform GenerateWrapper(float screenBottom, LevelData levelData) {
        var container = new GameObject("Level Container");

        var collider = container.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        var totalHeight = levelData.exitHeight - screenBottom;
        collider.size = new Vector2(
            levelData.skyBreadth * 2,
            totalHeight
        );
        collider.offset = new Vector2(
            0,
            (totalHeight / 2) + screenBottom
        );

        container.AddComponent<LevelContainer>();

        return container.transform;
    }

    private static void GenerateGroundStack(float screenBottom, LevelData levelData, Transform parent)
    {
        var start = screenBottom - levelData.bounceGround.GetComponent<SpriteRenderer>().bounds.size.y;
        GameObject bounceGround = CreateStackedSprite(start, levelData.bounceGround, parent);
        GameObject platform = CreateStackedSprite(GetSpriteTop(bounceGround), levelData.launchPlatform, parent);
        
        var go = GameObject.Instantiate(
            levelData.ship,
            new Vector2(0, levelData.exitHeight - (GetSpriteHalfHeight(levelData.ship) * 3)),
            Quaternion.identity
        );
        go.transform.parent = parent;
    }

    private static void GenerateClouds(LevelData levelData, Transform parent, float driftDirection)
    {
        GenerateSpriteSwarm(
            levelData.clouds, 
            parent, 
            levelData.cloudCount,
            levelData.skyBreadth, 
            levelData.cloudHeight, 
            levelData.cloudStopHeight,
            levelData.cloudDrift,
            1f,
            driftDirection
        );
    }
    
    private static void GenerateGoodTrash(LevelData levelData, Transform parent, float driftDirection)
    {
        var goodTrash = GenerateSpriteSwarm(
            levelData.goodTrash, 
            parent, 
            levelData.goodTrashCount,
            levelData.skyBreadth, 
            levelData.trashHeight, 
            levelData.exitHeight,
            levelData.trashDrift,
            0f,
            driftDirection
        );

        for(var i = 0; i < levelData.goodTrashCount - 1; i++)
        {
            var spriteOutline = goodTrash[i].AddComponent<SpriteOutline>();
            spriteOutline.color = Color.green;

            var trashBody = goodTrash[i].AddComponent<TrashBody>();
            trashBody.pickup = true;
        }
    }

    private static void GenerateBadTrash(LevelData levelData, Transform parent, float driftDirection)
    {
        var badTrash = GenerateSpriteSwarm(
            levelData.badTrash, 
            parent, 
            levelData.badTrashCount,
            levelData.skyBreadth, 
            levelData.trashHeight, 
            levelData.exitHeight,
            levelData.trashDrift,
            0f,
            driftDirection
        );

        for(var i = 0; i < levelData.badTrashCount - 1; i++)
        {
            var spriteOutline = badTrash[i].AddComponent<SpriteOutline>();
            spriteOutline.color = Color.red;

            var trashBody = badTrash[i].AddComponent<TrashBody>();
            trashBody.pickup = false;
            trashBody.knockbackForce = 30;
        }
    }

    private static GameObject[] GenerateSpriteSwarm(
        GameObject[] spritePrefabs,
        Transform parent, 
        int count, 
        float breadth, 
        float startY, 
        float endY, 
        float driftRate, 
        float verticalFlux,
        float driftDirection) {

        GameObject[] gameObjects = new GameObject[count];
    
        var breadthBuffer = 50;
        for (var i = 0; i < count; i++) {
            var prefab = spritePrefabs[Random.Range(0, spritePrefabs.Length)];
            var position = new Vector2(
                Random.Range(-breadth - breadthBuffer, breadth + breadthBuffer), 
                Random.Range(startY, endY)
            );
            var go = GameObject.Instantiate(prefab, position, Quaternion.identity);
            go.transform.parent = parent;

            gameObjects[i] = go;

            var drifter = go.AddComponent<Drifter>();
            drifter.driftRate = driftRate;
            drifter.driftDirection = driftDirection;
            drifter.verticalFlux = verticalFlux;
        }

        return gameObjects;
    }

    private static float GetSpriteHalfHeight(GameObject spritePrefab)
    {
        return 0.5f * spritePrefab.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private static float GetSpriteTop(GameObject spritePrefab)
    {
        return GetSpriteHalfHeight(spritePrefab) + spritePrefab.transform.position.y;
    }

    private static GameObject CreateStackedSprite(float stackTop, GameObject spritePrefab, Transform parent)
    {
        var center = stackTop + GetSpriteHalfHeight(spritePrefab);
        var go = GameObject.Instantiate(
            spritePrefab,
            new Vector2(0, center),
            Quaternion.identity
        );
        go.transform.parent = parent;
        return go;
    }
}
