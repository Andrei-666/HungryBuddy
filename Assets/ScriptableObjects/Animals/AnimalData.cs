using UnityEngine;

[CreateAssetMenu(fileName = "New AnimalData", menuName = "Game/Animal Data")]
public class AnimalData :ScriptableObject
{
    [Header("Info")]
    public string animalName = "New Animal";
    public Sprite animalIcon;

    [Header("Starting Stats")]
    public int maxHealth = 100;         
    public int maxFood = 100;           
    public int initialLevel = 1;        
    public int xpToCompleteInitialLevel = 100; 

    [Header("Combat Stats")]
    public int baseAttack = 10;         
}
