using TMPro;
using UnityEngine;
using UnityEngine.UI; 


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } 

    public GameObject startPanel;
    public GameObject mainMenuPanel;
    public GameObject mapPanel;
    public GameObject fightPanel;

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
        ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        mapPanel.SetActive(false);
        fightPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        mapPanel.SetActive(false);
        fightPanel.SetActive(false);

         UpdateMainMenuUI();
    }

    public void ShowMapPanel()
    {
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        mapPanel.SetActive(true);
        fightPanel.SetActive(false);
    }

    public void ShowFightPanel()
    {
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        mapPanel.SetActive(false);
        fightPanel.SetActive(true);
    }

    public AnimalData pisicaData;
    public AnimalData caineData;
    public AnimalData papagalData;
    public AnimalData hamsterData;


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


    [Header("Main Menu Elements")]
    public Image animalIconImage;
    public TextMeshProUGUI animalNameText;

    public void UpdateMainMenuUI()
    {
        if (GameManager.Instance.selectedAnimalData != null)
        {
            Debug.Log("Actualizare UI Main Menu pentru: " + GameManager.Instance.selectedAnimalData.animalName);
            if (animalNameText != null)
            {
                animalNameText.text = GameManager.Instance.selectedAnimalData.animalName;
                Debug.Log("nume animal: " + animalNameText.text);
            }
            else { Debug.LogWarning("Referința animalNameText lipsește!"); }

            if (animalIconImage != null)
            {
                animalIconImage.sprite = GameManager.Instance.selectedAnimalData.animalIcon;
                animalIconImage.enabled = (animalIconImage.sprite != null);
            }
            else { Debug.LogWarning("Referința animalIconImage lipsește!"); }
        }
        else
        {
            Debug.LogWarning("Nu există animal selectat pentru a actualiza Main Menu UI.");
        }
    }


    


}