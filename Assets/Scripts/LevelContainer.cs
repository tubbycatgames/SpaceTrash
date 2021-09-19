using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other) {
        other.transform.position = new Vector2(
            -other.transform.transform.position.x,
            other.transform.position.y
        );
    }
}
