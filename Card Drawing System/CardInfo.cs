using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour {

    public int cardID;
    public string cardName;
    public Sprite cardSprite;
    public Rarity cardRarity;
    public int cardStars;

    public Text cardNameObj;
    public Image cardArtObj;
    public Transform cardStarsHolder;
    public GameObject cardRarityObj;
    public Image leftBackground, rightBackground;

    // Update the visual information of the cards
    public void UpdateInfo() {

        Reset();
        cardNameObj.text = cardName;
        cardArtObj.sprite = cardSprite;

        switch (cardRarity) {
            case Rarity.Basic:
                leftBackground.sprite = rightBackground.sprite = DrawManager.Instance.rarityBasic;
                break;
            case Rarity.Epic:
                leftBackground.sprite = rightBackground.sprite = DrawManager.Instance.rarityEpic;
                break;
            case Rarity.Rare:
                leftBackground.sprite = rightBackground.sprite = DrawManager.Instance.rarityRare;

                break;
        }
        for (int i = 0; i < cardStars; i++) {
            cardStarsHolder.GetChild(i).gameObject.SetActive(true);
        }
    }

    // Reset the star value upon drawing cards.
    public void Reset() {

        for (int i = 0; i < cardStars; i++) {
            cardStarsHolder.GetChild(i).GetComponent<Image>().sprite = DrawManager.Instance.starRarity[(int)cardRarity];
        }

    }


    // Called function when button press
    public void FlipCard() {
        Invoke("UpdateInfo", 0.3f);
        GetComponent<Animator>().SetBool("Unveal", true);
    }
}
