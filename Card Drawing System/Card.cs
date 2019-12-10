using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {

    public int cardID;
    public string cardName;
    public Rarity cardRarity;
    public int cardStars;
    
    public Card(int id, string name, Rarity rarity, int stars) {
        cardID = id;
        cardName = name;
        cardRarity = rarity;
        cardStars = stars;
    }

    public int ID { get { return cardID; } }
    public string Name { get { return cardName; } }
    public Rarity Rarity { get { return cardRarity; } }
    public int Stars { get { return cardStars; } }
}

public enum Rarity { Basic ,Epic, Rare}
