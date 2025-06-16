using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } 


    public GameObject startPanel;
    public GameObject mainMenuPanel;


    [Header("Main Menu Elements - Info")]
    public Image animalIconImage;
    public TextMeshProUGUI animalNameText;
    public TextMeshProUGUI animalLevelText; 

    [Header("Main Menu Elements - Status Bars")] 
    public Slider healthSlider;             
    public TextMeshProUGUI healthValueText; 
    public Slider foodSlider;               
    public TextMeshProUGUI foodValueText;   
    public Slider xpSlider;                
    public TextMeshProUGUI xpValueText;


    public AnimalData pisicaData;
    public AnimalData caineData;
    public AnimalData papagalData;
    public AnimalData hamsterData;




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeUIPanelsBasedOnGameState();
    }
    void InitializeUIPanelsBasedOnGameState()
    {
        if (GameManager.Instance != null && GameManager.Instance.selectedAnimalData != null)
        {
            Debug.Log("UIManager: Animal deja selectat (" + GameManager.Instance.selectedAnimalData.animalName + "). Afisez MainMenuPanel.");
            ShowMainMenu(); 
        }
        else
        {
            Debug.Log("UIManager: Niciun animal selectat. Afisez StartPanel.");
            ShowStartPanel();
        }
    }


    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        UpdateAllStatusDisplays();
    }



    public void SelectPisica()
    {
        GameManager.Instance.SelectAnimal(pisicaData);
        ShowMainMenu();
    }

    public void SelectCaine()
    {
        GameManager.Instance.SelectAnimal(caineData);
        ShowMainMenu();
    }

    public void SelectPapagal()
    {
        GameManager.Instance.SelectAnimal(papagalData);
        ShowMainMenu();
    }

    public void SelectHamster()
    {
        GameManager.Instance.SelectAnimal(hamsterData);
        ShowMainMenu();
    }



    public void UpdateMainMenuUI() 
    {
        if (GameManager.Instance == null || GameManager.Instance.selectedAnimalData == null)
        {
            Debug.LogWarning("UpdateMainMenuUI: GameManager sau selectedAnimalData lipseste.");
            if (animalNameText != null) animalNameText.text = "N/A";
            if (animalIconImage != null) animalIconImage.enabled = false;
            return;
        }

        if (animalNameText != null)
        {
            animalNameText.text = GameManager.Instance.selectedAnimalData.animalName;
        }

        if (animalIconImage != null)
        {
            animalIconImage.sprite = GameManager.Instance.selectedAnimalData.animalIcon;
            animalIconImage.enabled = (animalIconImage.sprite != null);
        }
    }

    public void UpdateAllStatusDisplays()
    {
        if (GameManager.Instance == null || GameManager.Instance.selectedAnimalData == null)
        {
            Debug.LogWarning("UIManager (UpdateAll): GameManager sau animalul selectat lipseste. UI-ul nu poate fi actualizat complet.");
            
            if (animalNameText != null) animalNameText.text = "N/A";
            if (animalIconImage != null) animalIconImage.enabled = false;
            if (animalLevelText != null) animalLevelText.text = "Nivel: -";
            if (healthSlider != null) { healthSlider.value = 0; healthSlider.maxValue = 1; }
            if (healthValueText != null) healthValueText.text = "-/-";
            if (foodSlider != null) { foodSlider.value = 0; foodSlider.maxValue = 1; }
            if (foodValueText != null) foodValueText.text = "-/-";
            if (xpSlider != null) { xpSlider.value = 0; xpSlider.maxValue = 1; }
            if (xpValueText != null) xpValueText.text = "-/-";
            return;
        }

        Debug.Log("UIManager: Se actualizeaza toate display-urile de status pentru " + GameManager.Instance.selectedAnimalData.animalName);

        UpdateMainMenuUI();

        if (animalLevelText != null)
        {
            animalLevelText.text = "Nivel: " + GameManager.Instance.CurrentLevel;
        }
        else { Debug.LogWarning("UIManager: Referinta animalLevelText lipseste!"); }

        if (healthSlider != null)
        {
            healthSlider.maxValue = GameManager.Instance.MaxHealthValue;
            healthSlider.value = GameManager.Instance.CurrentHealth;
            if (healthValueText != null)
            {
                healthValueText.text = GameManager.Instance.CurrentHealth + " / " + GameManager.Instance.MaxHealthValue;
            }
        }
        else { Debug.LogWarning("UIManager: Referinta healthSlider lipseste!"); }

        if (foodSlider != null)
        {
            foodSlider.maxValue = GameManager.Instance.MaxFoodValue;
            foodSlider.value = GameManager.Instance.CurrentFood;
            if (foodValueText != null)
            {
                foodValueText.text = GameManager.Instance.CurrentFood + " / " + GameManager.Instance.MaxFoodValue;
            }
        }
        else { Debug.LogWarning("UIManager: Referinta foodSlider lipseste!"); }

        if (xpSlider != null)
        {
            if (GameManager.Instance.XpForNextLevel > 0) 
            {
                xpSlider.maxValue = GameManager.Instance.XpForNextLevel;
                xpSlider.value = GameManager.Instance.CurrentXP;
                if (xpValueText != null)
                {
                    xpValueText.text = GameManager.Instance.CurrentXP + " / " + GameManager.Instance.XpForNextLevel;
                }
            }
            else 
            {
                xpSlider.maxValue = 1; 
                xpSlider.value = 1;    
                if (xpValueText != null)
                {
                    xpValueText.text = "MAX";
                }
            }
        }
        else { Debug.LogWarning("UIManager: Referinta xpSlider lipseste!"); }
    }

    public void LoadMapScene()
    {
        SceneManager.LoadScene("Map");
    }







}