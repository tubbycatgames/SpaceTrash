using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBody : MonoBehaviour
{
    public bool pickup;
    public float knockbackForce;
    [SerializeField]
    [FMODUnity.EventRef]
    private string pickupSound = "event:/Objects/junk/collided_normal";
    [SerializeField]
    [FMODUnity.EventRef]
    private string hitSound = "event:/Objects/junk/pickup_normal";
    [SerializeField]
    [FMODUnity.EventRef]
    private string duckSound = "event:/Objects/junk/pickup_duck";

    private PickupSounds pickupSounds;

    private void Start() {
        pickupSounds = Resources.Load<PickupSounds>("ScriptableObjects/PickupSounds");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            var shipControl = collider.gameObject.GetComponent<ShipControl>();
            if (!shipControl.IsInitiallyFalling())
            {
                var sound = pickup ? pickupSounds.pickupSound : pickupSounds.hitSound; 
                foreach(var customSound in pickupSounds.customSounds) {
                    if (gameObject.name == (customSound.prefab.name + "(Clone)"))
                    {
                        sound = customSound.sound;
                    }
                }
                FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);

                if(pickup)
                {
                    GlobalGameController.control.CollectTrash();
                }
                else
                {
                    Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();
                    rb.AddForce(-rb.velocity * knockbackForce);
                }
                Destroy(gameObject);
            }
        }
    }
}
