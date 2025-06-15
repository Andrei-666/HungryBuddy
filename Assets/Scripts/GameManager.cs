// GameManager.cs - VERSIUNE REVIZUITA
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AnimalData selectedAnimalData { get; private set; }

    public int CurrentHealth { get; private set; }
    public int CurrentFood { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentXP { get; private set; }

    public int MaxHealthValue { get; private set; }
    public int MaxFoodValue { get; private set; }
    public int XpForNextLevel { get; private set; }

    [Header("Game Data")]
    public AnimalData[] allAnimalDatas;

    public int ActiveProfileSlot { get; private set; }

    private static readonly int[] xpProgressionForSubsequentLevels = new int[] {
        0,   
        0,  
        150,  
        300,  
        500, 
        800,  
        1200  
        
    };




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SelectAnimal(AnimalData animal)
    {
        if (animal == null)
        {
            Debug.LogError("GameManager: SelectAnimal a fost chemat cu animal NULL!");
            selectedAnimalData = null;
            return;
        }

        selectedAnimalData = animal;
        Debug.Log($"GameManager: Animal selectat: {selectedAnimalData.animalName}");

        MaxHealthValue = selectedAnimalData.maxHealth;
        MaxFoodValue = selectedAnimalData.maxFood;

        CurrentHealth = MaxHealthValue;
        CurrentFood = MaxFoodValue;

        CurrentLevel = selectedAnimalData.initialLevel;
        CurrentXP = 0;

        XpForNextLevel = CalculateXpNeededForLevel(CurrentLevel);

        Debug.Log($"GameManager: Statistici initializate - Viata: {CurrentHealth}/{MaxHealthValue}, Mancare: {CurrentFood}/{MaxFoodValue}, Nivel: {CurrentLevel}, XP: {CurrentXP}/{XpForNextLevel}");
        NotifyUIOfStatChanges();

        SaveGame();

        NotifyUIOfStatChanges();
    }

    public void PrepareNewGame(int slotNumber)
    {
        ActiveProfileSlot = slotNumber;
        selectedAnimalData = null;
        Debug.Log("GameManager: Pregatire pentru un joc nou in slotul " + slotNumber);
    }


    public void SaveGame()
    {
        if (ActiveProfileSlot <= 0 || ActiveProfileSlot > 3) { Debug.LogError("SaveGame: Slot de profil invalid: " + ActiveProfileSlot); return; }
        if (selectedAnimalData == null) { Debug.LogError("SaveGame: Nu se poate salva, niciun animal selectat!"); return; }

        string prefix = "Profile_" + ActiveProfileSlot + "_";
        PlayerPrefs.SetInt(prefix + "Used", 1);
        PlayerPrefs.SetString(prefix + "AnimalType", selectedAnimalData.animalName);
        PlayerPrefs.SetInt(prefix + "Level", CurrentLevel);
        PlayerPrefs.SetInt(prefix + "XP", CurrentXP);
        PlayerPrefs.SetInt(prefix + "Health", CurrentHealth);
        PlayerPrefs.SetInt(prefix + "Food", CurrentFood);

        PlayerPrefs.Save();
        Debug.Log("Jocul a fost salvat in slotul " + ActiveProfileSlot);
    }

    public void LoadGame(int slotNumber)
    {
        ActiveProfileSlot = slotNumber;
        string prefix = "Profile_" + slotNumber + "_";

        if (PlayerPrefs.GetInt(prefix + "Used", 0) == 0) { Debug.LogError("LoadGame: Slotul " + slotNumber + " este gol."); return; }

        string animalType = PlayerPrefs.GetString(prefix + "AnimalType");
        AnimalData dataToLoad = FindAnimalDataByName(animalType);
        if (dataToLoad == null) { Debug.LogError("LoadGame: Nu s-a gasit AnimalData pentru tipul: " + animalType); return; }
        selectedAnimalData = dataToLoad;

        MaxHealthValue = selectedAnimalData.maxHealth;
        MaxFoodValue = selectedAnimalData.maxFood;
        CurrentLevel = PlayerPrefs.GetInt(prefix + "Level");
        CurrentXP = PlayerPrefs.GetInt(prefix + "XP");
        CurrentHealth = PlayerPrefs.GetInt(prefix + "Health");
        CurrentFood = PlayerPrefs.GetInt(prefix + "Food");
        XpForNextLevel = CalculateXpNeededForLevel(CurrentLevel);

        Debug.Log("Jocul a fost incarcat din slotul " + slotNumber + " cu animalul " + animalType);
        NotifyUIOfStatChanges();
    }

    private AnimalData FindAnimalDataByName(string name)
    {
        foreach (AnimalData data in allAnimalDatas)
        {
            if (data.animalName == name)
            {
                return data;
            }
        }
        return null;
    }

    private int CalculateXpNeededForLevel(int completedLevel) 
    {
        if (selectedAnimalData == null)
        {
            Debug.LogError("CalculateXpNeededForLevel: selectedAnimalData este null!");
            return 999999; 
        }

        if (completedLevel == selectedAnimalData.initialLevel)
        {
            
            return selectedAnimalData.xpToCompleteInitialLevel;
        }
        if (completedLevel > 0 && completedLevel < xpProgressionForSubsequentLevels.Length)
        {
            return xpProgressionForSubsequentLevels[completedLevel];
        }
        else if (completedLevel >= xpProgressionForSubsequentLevels.Length - 1 && xpProgressionForSubsequentLevels.Length > 1)
        {
            int lastDefinedXp = xpProgressionForSubsequentLevels[xpProgressionForSubsequentLevels.Length - 1];
            int levelsBeyondArrayEnd = completedLevel - (xpProgressionForSubsequentLevels.Length - 1);
            return Mathf.RoundToInt(lastDefinedXp * Mathf.Pow(1.25f, levelsBeyondArrayEnd));
        }

        Debug.LogWarning($"GameManager: Nu s-a putut calcula XP pentru nivelul completat {completedLevel}. Folosind valoare mare default.");
        return 999999;
    }

    public void ChangeHealth(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealthValue);
        Debug.Log($"Viata schimbata la: {CurrentHealth}/{MaxHealthValue}");
        NotifyUIOfStatChanges();
    }

    public void ChangeFood(int amount)
    {
        CurrentFood += amount;
        CurrentFood = Mathf.Clamp(CurrentFood, 0, MaxFoodValue);
        Debug.Log($"Mancare schimbata la: {CurrentFood}/{MaxFoodValue}");
        NotifyUIOfStatChanges();

        if (CurrentFood == 0)
        {
            Debug.Log("Animalului ii este foarte foame! Ar putea suferi penalizari.");
        }
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;
        CurrentXP += amount;
        Debug.Log($"XP adaugat: +{amount}. XP Curent: {CurrentXP}/{XpForNextLevel}");

        while (CurrentXP >= XpForNextLevel && XpForNextLevel > 0)
        {
            LevelUp();
        }
        NotifyUIOfStatChanges();
    }

    private void LevelUp()
    {
        CurrentXP -= XpForNextLevel;
        CurrentLevel++;
        XpForNextLevel = CalculateXpNeededForLevel(CurrentLevel);

        Debug.Log($"LEVEL UP! Noul Nivel: {CurrentLevel}. XP necesar pentru urmatorul: {XpForNextLevel}. XP Curent: {CurrentXP}");
    }

    public void ApplyNoSleepPenalty(int damageAmount = 5)
    {
        Debug.Log($"GameManager: Se aplica penalizare de somn: -{damageAmount} viata.");
        ChangeHealth(-damageAmount);
    }

    public void ConsumeFoodPeriodically(int amountConsumed = 1)
    {
        Debug.Log($"GameManager: Consum periodic de mancare: -{amountConsumed}.");
        ChangeFood(-amountConsumed);
    }

    private void NotifyUIOfStatChanges()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAllStatusDisplays();
        }
        else
        {
            Debug.LogWarning("GameManager: UIManager.Instance este null. Nu se poate notifica UI-ul.");
        }
    }
}