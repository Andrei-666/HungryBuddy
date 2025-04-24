using UnityEngine;

public class GameManager :MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AnimalData selectedAnimalData;

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
        selectedAnimalData = animal;
        Debug.Log("Animal selectat: " + animal.animalName);
    }





}
