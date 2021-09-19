using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipControl_Backup : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed, rotationalSpeed, maxFuel, fuelConsumptionRate;
    [SerializeField]
    [FMODUnity.EventRef]
    private string shipAccelerateSoundEvent;

    private FMOD.Studio.EventInstance shipAccelerateSound;
    FMOD.Studio.PARAMETER_ID shipLoopingParameterID, shipSpeedParameterID;
    FMOD.Studio.PLAYBACK_STATE playbackState;

    private float fuel;
    float loopSoundValue;

    void Start()
    {
        fuel = maxFuel;
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        shipAccelerateSound = FMODUnity.RuntimeManager.CreateInstance(shipAccelerateSoundEvent);
        FMOD.Studio.EventDescription shipAccelerateSoundDescription = FMODUnity.RuntimeManager.GetEventDescription(shipAccelerateSoundEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION shipLoopingParameterDescription;
        shipAccelerateSoundDescription.getParameterDescriptionByName("ship_looping", out shipLoopingParameterDescription);
        shipLoopingParameterID = shipLoopingParameterDescription.id;

        

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(shipAccelerateSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    public void Update() {

        if(transform.position.y >= GlobalGameController.control.currentLevelData.exitHeight)
            GlobalGameController.control.LoadNextLevel();
        // SFX for low fuel
        if (fuel / maxFuel < 0.1f) {

        }
        if (Input.GetKey(KeyCode.Space))
        {

            //shipAccelerateSound.getParameterByID(shipLoopingParameterID, out loopSoundValue);


            //if (loopSoundValue == 0)
            //{
            //   shipAccelerateSound.getParameterByID(shipLoopingParameterID, out loopSoundValue);
            //  Debug.Log(loopSoundValue);

            // shipAccelerateSound.getPlaybackState(out playbackState);
            //  if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            //    shipAccelerateSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            //}

            shipAccelerateSound.getPlaybackState(out playbackState);
            if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                shipAccelerateSound.start();
            }
        }
        else
        {
            shipAccelerateSound.setParameterByID(shipLoopingParameterID, 0);

        }
    }

    private void FixedUpdate() {
        //FUEL DEPENDANT SHIP CONTROL ---------
        if(fuel <= 0)
            return;
        
        if (Input.GetKey(KeyCode.Space))
        {
                                     
            fuel -= (fuelConsumptionRate * Time.deltaTime);
            rb.AddRelativeForce(Vector2.up * speed * Time.deltaTime);
        }
        
            
        if (Input.GetKey(KeyCode.A))
            rb.AddTorque(rotationalSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.D))
            rb.AddTorque(-rotationalSpeed * Time.deltaTime);
        else
            rb.AddTorque(-rb.angularVelocity * Time.deltaTime);
    }
    public void OnDestroy()
    {
        shipAccelerateSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        shipAccelerateSound.release();
    }
}
