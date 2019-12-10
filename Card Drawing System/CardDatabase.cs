using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour {

    // Create a list of items to reference

    public List<Card> cardDatabase = new List<Card>();
    public Sprite[] cardArts;
    public GameObject[] rarityPrefabs;

	// Use this for initialization
	void Awake () {
        GenerateCardDatabase();
	}

    private void GenerateCardDatabase() {
        cardDatabase.Add(new Card(1,"Candles",Rarity.Basic, 1));
        cardDatabase.Add(new Card(2,"Flowers", Rarity.Rare, 2));
        cardDatabase.Add(new Card(3,"Guitar", Rarity.Epic, 3));
        cardDatabase.Add(new Card(4,"Maracas", Rarity.Basic, 4));
        cardDatabase.Add(new Card(5,"Margarita", Rarity.Rare, 5));
        cardDatabase.Add(new Card(6,"Musician", Rarity.Epic, 4));
        cardDatabase.Add(new Card(7,"Scarf", Rarity.Basic, 3));
        cardDatabase.Add(new Card(8,"Skeleton", Rarity.Rare, 2));
        cardDatabase.Add(new Card(9,"Sombrero", Rarity.Epic, 1));
        cardDatabase.Add(new Card(10,"Taco", Rarity.Basic, 5));

        Debug.Log("Database Generated");
    }
	
}
