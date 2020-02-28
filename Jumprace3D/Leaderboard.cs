using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    // Start is called before the first frame update

    private static Leaderboard instance;
    public static Leaderboard Instance { get { return instance; } set { instance = value; } }

    public Text[] rankText;
    public Text[] completionRank;

    public GameObject rankCrown;


    public Dictionary<string,int> userDatas = new Dictionary<string,int>();

    private void Awake() {
        if(instance == null)
            instance = this;
    }
    void Start()
    {
        userDatas.Add("Manuel", 28);
        userDatas.Add("Mary", 26);
        userDatas.Add("Charles", 14);
        userDatas.Add("Michael", 26);
        userDatas.Add("You", 30);
    }

    public void UpdateLeaderboard(int score) {
        userDatas["You"] = score;

        IOrderedEnumerable<KeyValuePair<string, int>> sortedPlayer = userDatas.OrderBy(x => x.Value).OrderByDescending(x => x.Value);
        int i = 0;
        foreach (KeyValuePair<string, int> item in sortedPlayer) {
            rankText[(rankText.Length - 1) - i].text = rankText.Length -i + ". " + item.Key;
            i++;
        }

        UpdateCompletionBoard();

        for (int j = 0; j < completionRank.Length; j++) {
            if (completionRank[j].text == "You") {
                if (j >= 3) {
                    rankText[2].text = rankText[j].text;
                    break;
                }
            }
        }

        if (completionRank[0].text == "You") {
            rankCrown.SetActive(true);
        } else {
            rankCrown.SetActive(false);
        }
    }

    public void UpdateCompletionBoard() {

        IOrderedEnumerable<KeyValuePair<string, int>> sortedPlayer = userDatas.OrderBy(x => x.Value).OrderByDescending(x => x.Value);
        int i = 0;
        foreach (KeyValuePair<string, int> item in sortedPlayer) {
            completionRank[(completionRank.Length - 1) - i].text = item.Key;
            i++;
        }
    }
}


