using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CardDatabase))]
public class DrawManager : MonoBehaviour {


    private static DrawManager instance;
    public static DrawManager Instance { get { return instance; } }

    [SerializeField] Transform cardHolder;                      // Parent of the card Prefabs
    [SerializeField] CardDatabase cardDatabase;                 // Referencing the database to generate new cards
    public List<CardInfo> drawnCards = new List<CardInfo>();    // List of the cards that were drawn

    public Sprite rarityBasic, rarityEpic, rarityRare;          // Sprite Images of each rarity
    public Sprite[] starRarity;                                 // Star Rarity Sprite of each item

    public DrawButton drawButton;                               // Reference the script to handle click events

    public int rarityIndex;

    private void Awake() {
        if(instance == null) {
            instance = this; // Singleton to access information regarding sprites and drawn cards
        }
    }


    // Generate list of cards that have been drawn. Not Visual, but behind the scenes.
    public void Draw() {
        if (drawButton.clicked)
            return;
        
        for (int i = 0; i < 10; i++) {

            GameObject go;
            // Object Pooling;
            if (drawnCards.Count < 10) {
                go = cardHolder.GetChild(i).gameObject;
            }
            else {
                go = drawnCards[i].gameObject;
            }


            CardInfo cardInfo = go.GetComponent<CardInfo>();
            int rand = Random.Range(0, cardDatabase.cardDatabase.Count - 1);
            go.transform.localScale = new Vector3(1,1,1);

            cardInfo.cardID = cardDatabase.cardDatabase[rand].ID;
            cardInfo.cardName = cardDatabase.cardDatabase[rand].Name;
            cardInfo.cardSprite = cardDatabase.cardArts[rand];
            cardInfo.cardStars = cardDatabase.cardDatabase[rand].Stars;

            cardInfo.cardRarity = cardDatabase.cardDatabase[rand].Rarity;

            go.SetActive(false);


            drawnCards.Add(cardInfo);
        }

        StartCoroutine("DrawCardVisual");

    }

    // Initiate the card visuals after drawing
    private IEnumerator DrawCardVisual() {
        yield return StartCoroutine(drawButton.InitiateCardOpen());
        int count = 0;
        while(count < 10) {
            drawnCards[count].GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(0.3f);
            drawnCards[count].gameObject.SetActive(true);
            if (count < 8) {
                drawnCards[count].UpdateInfo();
                drawnCards[count].GetComponent<Animator>().SetBool("Unveal", true);
            }
            rarityIndex = count;
            count++;
        }
        yield return new WaitForSeconds(.3f);
        yield return null;
    }



    // In case we do have a re-draw function, we can add this onto the script.
    public void Clear() {
        foreach(CardInfo ci in drawnCards) {
            ci.Reset();
            ci.gameObject.SetActive(false);
        }
    }

}
