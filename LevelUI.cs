using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUI : MonoBehaviour {

    public Text Line1;
    public Text Line2;
    public Text Time;

    public Slider[] health;

    public GameObject[] winner;
    public GameObject win;

    public static LevelUI instance;
    public static LevelUI GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    public void AddWinIndicator(int player)
    {
        GameObject go = Instantiate(win, transform.position, Quaternion.identity) as GameObject;
        go.transform.SetParent(winner[player].transform);
    }
}
