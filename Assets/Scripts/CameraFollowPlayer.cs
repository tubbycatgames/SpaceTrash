using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollowPlayer : MonoBehaviour
{
    public Transform shipbody;
    public float cameraLerpSpeed;

    float zSet = -10f;

    void Start()
    {
        SetPlayer();
    }
    void Update()
    {           
        if(!shipbody)
        {
            SetPlayer();  
        }
    }

    private void LateUpdate() {
        if (shipbody) {
            var camPos = new Vector2(transform.position.x, transform.position.y);
            var shipPos = new Vector2(shipbody.position.x, shipbody.position.y);

            var newX = shipPos.x;
            var newY = shipPos.y;
            // Lerp instead of jump if viable
            // if (Vector2.Distance(camPos, shipPos) < 100f) {
            //     var newPos = Vector2.Lerp(camPos, shipPos, cameraLerpSpeed*Time.deltaTime);
            //     newX = newPos.x;
            //     newY = newPos.y;
            // }

            transform.position = new Vector3(
                newX,
                Mathf.Max(0, newY),
                zSet
            );
        }
    }

    void SetPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player) {
            shipbody = player.transform;
        }
    }
}
