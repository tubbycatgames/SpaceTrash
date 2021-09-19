using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipControl : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed, rotationalSpeed;

    public float resetTime = 1f;

    //FMOD Stuff
    [SerializeField]
    [FMODUnity.EventRef]
    private string shipAccelerateSoundEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    public string bounceSoundEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    private string crashSoundEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    private string outOfFuelSound;
    [SerializeField]
    [FMODUnity.EventRef]
    private string lowFuelLoopEvent;
    [SerializeField]
    [FMODUnity.EventRef]
    private string fallSoundEvent;

    [SerializeField]
    [FMODUnity.EventRef]
    private string resetSound;
    [SerializeField]
    [FMODUnity.EventRef]
    private string offThrust;

    private FMOD.Studio.EventInstance shipAccelerateSound;
    private FMOD.Studio.EventInstance lowFuelSound;
    private FMOD.Studio.EventInstance fallSound;
    FMOD.Studio.PARAMETER_ID shipLoopingParameterID, bounceSpeedParameterID, lowFuelParameterID;

    private float loopSoundValue;
    private bool isMoving = true;
    private bool isOutOfFuelSound = false;
    private bool isAccelerating = false;
    private bool isFlying = true;

    private bool isInitiallyFalling = true;
    private bool isResetting = false;


    public ParticleSystem collisionEffect;

    private ParticleSystem particleStream;

    

    void Start()
    {
        particleStream = GetComponent<ParticleSystem>();

        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        //Setting up audio events and parameters
        fallSound = FMODUnity.RuntimeManager.CreateInstance(fallSoundEvent);
        shipAccelerateSound = FMODUnity.RuntimeManager.CreateInstance(shipAccelerateSoundEvent);
        lowFuelSound = FMODUnity.RuntimeManager.CreateInstance(lowFuelLoopEvent);

        FMOD.Studio.EventDescription shipAccelerateSoundDescription = FMODUnity.RuntimeManager.GetEventDescription(shipAccelerateSoundEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION shipLoopingParameterDescription;
        shipAccelerateSoundDescription.getParameterDescriptionByName("ship_looping", out shipLoopingParameterDescription);
        shipLoopingParameterID = shipLoopingParameterDescription.id;


        FMOD.Studio.EventDescription bounceSoundDescription = FMODUnity.RuntimeManager.GetEventDescription(bounceSoundEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION bounceSoundParameterDescription;
        bounceSoundDescription.getParameterDescriptionByName("bounce velocity", out bounceSoundParameterDescription);
        bounceSpeedParameterID = bounceSoundParameterDescription.id;

        FMOD.Studio.EventDescription lowFuelSoundDescription = FMODUnity.RuntimeManager.GetEventDescription(lowFuelLoopEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION lowFuelParameterDescription;
        lowFuelSoundDescription.getParameterDescriptionByName("fuel_level", out lowFuelParameterDescription);
        lowFuelParameterID = lowFuelParameterDescription.id;

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(shipAccelerateSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(fallSound, GetComponent<Transform>(), GetComponent<Rigidbody>());

        FlingShip();    
    }

    public void Update() {
        if(transform.position.y >= GlobalGameController.control.currentLevelData.exitHeight)
        {
            GlobalGameController.control.EnableDialog(1);
            PlayAccelerationSound();
            particleStream.Play();
            rb.AddRelativeForce(Vector2.up * speed * Time.deltaTime);
            lowFuelSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            return;
        }
        float fuelRatio = GlobalGameController.control.GetFuelRatio();
        if (fuelRatio < 0.2) {
            lowFuelSound.setParameterByID(lowFuelParameterID, fuelRatio);
            FMOD.Studio.PLAYBACK_STATE playbackState;
            lowFuelSound.getPlaybackState(out playbackState);
            if ((playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED) && (isOutOfFuelSound == false))
                lowFuelSound.start();
        }

        if (Input.GetKeyUp(KeyCode.R) && gameObject.transform.position.y < 0)
        {
            ResetShip();
        }

        if (Input.GetKey(KeyCode.Space) && !IsOutOfFuel())
        {
            PlayAccelerationSound();
            //isAccelerating = true;
            isFlying = true;
            GlobalGameController.control.ConsumeFuel();
            particleStream.Play();
        }
        else 
        {
            if (Input.GetKeyUp(KeyCode.Space))

            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(offThrust, gameObject);
                //isAccelerating = false;
            }
            shipAccelerateSound.setParameterByID(shipLoopingParameterID, 0);
            particleStream.Stop();
        }
    }

    private void FixedUpdate() {
        if (rb.velocity.y < 0)
        {

            FMOD.Studio.PLAYBACK_STATE playbackState;
             fallSound.getPlaybackState(out playbackState);
            if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED && isFlying==true)
            {

                fallSound.start();
                fallSound.getPlaybackState(out playbackState);
                Debug.Log(playbackState);
            }

        }
        else if (rb.velocity.y == 0)
        {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            fallSound.getPlaybackState(out playbackState);
            if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {

                fallSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                fallSound.getPlaybackState(out playbackState);
                Debug.Log(playbackState);
            }
        }

        if (Input.GetKey(KeyCode.A))
            rb.AddTorque(rotationalSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.D))
            rb.AddTorque(-rotationalSpeed * Time.deltaTime);
        else if (!IsOutOfFuel())
            rb.AddTorque(-rb.angularVelocity * Time.deltaTime);
    //}
        //else
       //     fallSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //FUEL DEPENDANT SHIP CONTROL ---------
        if(IsOutOfFuel()){
            if (isOutOfFuelSound == false)
            {
                lowFuelSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                FMODUnity.RuntimeManager.PlayOneShotAttached(outOfFuelSound, gameObject);
                isOutOfFuelSound = true;
            }

            return;
        }
    
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector2.up * speed * Time.deltaTime);
        }
    }

    public void OnDestroy()
    {
        shipAccelerateSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        shipAccelerateSound.release();
        fallSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        fallSound.release();
    }

    public bool IsInitiallyFalling()
    {
        return isInitiallyFalling;
    }

    private void FlingShip()
    {
        var direction = Random.value < 0.5 ? Vector2.left : Vector2.right;
        rb.gravityScale = 5.0f;
        rb.AddForce(direction * 1000);
        rb.AddTorque(100);
    }

    private IEnumerator ResetShip()
    {
        yield return new WaitForSeconds(resetTime);

        var platformPosition = GameObject.FindGameObjectWithTag("Platform").transform.position;
        var newY = platformPosition.y + (GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
        gameObject.transform.position = new Vector3(
            platformPosition.x,
            newY,
            0
        );
        gameObject.transform.rotation = Quaternion.identity;

        rb.velocity = Vector3.zero;
        GlobalGameController.control.Refuel();
        isOutOfFuelSound = false;
        if (isInitiallyFalling) {
            rb.gravityScale = 1.0f;
            isInitiallyFalling = false;
            GlobalGameController.control.EnableDialog(0);
            enabled = false;
        }
        isResetting = false;

        FMODUnity.RuntimeManager.PlayOneShotAttached(resetSound, gameObject);
    }

    private void PlayAccelerationSound()
    {
        shipAccelerateSound.getParameterByID(shipLoopingParameterID, out loopSoundValue);
        FMOD.Studio.PLAYBACK_STATE playbackState;

        shipAccelerateSound.getPlaybackState(out playbackState);
        if ((playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING) && (loopSoundValue == 0))
        {
            shipAccelerateSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        if ((playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED) || (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPING))
        {
            shipAccelerateSound.setParameterByID(shipLoopingParameterID, 1);
            shipAccelerateSound.start();
        }
    }

    private bool IsOutOfFuel() {
        return GlobalGameController.control.GetFuelRatio() <= 0;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (IsHittingGround(other)) {
            fallSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            FMOD.Studio.EventInstance bounceSound = FMODUnity.RuntimeManager.CreateInstance(bounceSoundEvent);
            bounceSound.setParameterByID(bounceSpeedParameterID, rb.velocity.y);
            bounceSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            bounceSound.start();
            bounceSound.release();
            isFlying = false;
          
            //FMODUnity.RuntimeManager.PlayOneShot(bounceSound, gameObject.transform.position);
            //    Debug.Log("hi2");
            
            Instantiate(collisionEffect, other.GetContact(0).point, Quaternion.identity);
        };
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (IsHittingGround(other))
        {
            Vector2 vel = rb.velocity;
            if (vel.magnitude < 0.00001)
            {
                if (isMoving == true)
                {
                    fallSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    FMODUnity.RuntimeManager.PlayOneShot(crashSoundEvent, gameObject.transform.position);
                    isMoving = false;
                    isFlying = false;
                }

                if (GlobalGameController.control.GetFuelRatio() < 0.2 && !isResetting)
                {
                    isResetting = true;
                    StartCoroutine(ResetShip());
                }
            }
        }
    }

    private bool IsHittingGround(Collision2D other)
    {
        return (other.gameObject.name == "BounceGround(Clone)") 
            || (other.gameObject.name == "PlatformPlaceholder(Clone)");
    }
}
