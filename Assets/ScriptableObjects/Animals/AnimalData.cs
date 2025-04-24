using UnityEngine;

[CreateAssetMenu(fileName = "New AnimalData", menuName = "Game/Animal Data")]
public class AnimalData :ScriptableObject
{
    public string animalName = "New animal";
    public Sprite animalIcon;
    public int startHealth = 100;
    public int startAttack = 10;
}
