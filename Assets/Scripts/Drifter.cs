using UnityEngine;

public class Drifter : MonoBehaviour
{
    public float driftRate = 1;
    public float driftDirection = 1;
    public float verticalFlux = 1;

    void Update()
    {
        var deltaX = driftRate * Time.deltaTime * driftDirection;
        var deltaY = Random.Range(-verticalFlux, verticalFlux) * Time.deltaTime;
        transform.position = new Vector2(
            transform.position.x + deltaX,
            transform.position.y + deltaY
        );
    }
}
