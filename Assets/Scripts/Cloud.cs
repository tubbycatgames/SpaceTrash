using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField]
    [FMODUnity.EventRef]
    private string cloudEat;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(cloudEat, gameObject);
            var particles = GetComponent<ParticleSystem>();
            particles.Play();
            Destroy(gameObject, particles.main.duration);
        }
    }
}
