using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;

public class GlobalGameController : MonoBehaviour
{
    public static GlobalGameController control;
    public int trashCollected;
    public LevelData currentLevelData;
    public Slider fuelGague;
    public Slider altimeter;
    public Text tankLevel;
    public GameObject player;

    public float maxFuel;
    private float initialMaxFuel;
    public float fuelConsumptionRate;

    private float currentFuel;

    private int currentLevel = 1;

    public int fuelUpgradeThreshold = 10;
    public float fuelUpgradeFactor = 0.1f;

    private void Awake() {
        if(!control)
            control = this;
        else if(control != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void ResetStats()
    {
        maxFuel = initialMaxFuel;
        currentLevel = 1;
    }
    void Start()
    {   
        ResetFuelGague();
        ResetAltimeter();
        ResetTankLevel();
        initialMaxFuel = maxFuel;
        SceneManager.sceneLoaded += (Scene, LoadSceneMode) => { 
            ResetFuelGague(); 
            ResetAltimeter();
            ResetTankLevel();
        };
    }

    void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player && altimeter)
        { 
            altimeter.value = player.transform.position.y / currentLevelData.exitHeight;
        }
    }
    public void EnableDialog(int dialogIndex)
    {
        if (dialogIndex == 0)       
            GameObject.FindGameObjectWithTag("Dialog").GetComponent<TextDisplays>().DisplayOpeningSceneText();
        else if(dialogIndex == 1)       
            GameObject.FindGameObjectWithTag("Dialog").GetComponent<TextDisplays>().DisplayClosingSceneText();
    }
    public void EnableControl()
    {
        player.GetComponent<ShipControl>().enabled = true;
        Debug.Log("Ship is Enabled: " + player.GetComponent<ShipControl>().isActiveAndEnabled);
    }

    private void ResetFuelGague()
    {
        if (GameObject.FindGameObjectWithTag("Fuel Gague") == null)
            return;
        currentFuel = 0;
        fuelGague = GameObject.FindGameObjectWithTag("Fuel Gague").GetComponent<Slider>();
        fuelGague.value = GetFuelRatio();
    }

    private void ResetAltimeter()
    {

        if (GameObject.FindGameObjectWithTag("Altimeter") == null)
            return;
        altimeter = GameObject.FindGameObjectWithTag("Altimeter").GetComponent<Slider>();
    }

    private void ResetTankLevel()
    {

        if (GameObject.FindGameObjectWithTag("Tank Level") == null)
            return;
        tankLevel = GameObject.FindGameObjectWithTag("Tank Level").GetComponent<Text>();
        tankLevel.text = "Tank Level: " + ((trashCollected / fuelUpgradeThreshold) + 1) ;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public float GetFuelRatio() {
        return currentFuel / maxFuel;
    }

    public void ConsumeFuel() {
        currentFuel -= fuelConsumptionRate * Time.deltaTime;
        fuelGague.value = GetFuelRatio();
    }

    public void Refuel() {
        currentFuel = maxFuel;
        fuelGague.value = GetFuelRatio();
    }

    public void CollectTrash() {
        trashCollected++;
        if (trashCollected % fuelUpgradeThreshold == 0) {
            var originalMaxFuel = maxFuel;
            maxFuel *= (1 + fuelUpgradeFactor);
            currentFuel += (maxFuel - originalMaxFuel);
            ResetTankLevel();
        }
    }
}
