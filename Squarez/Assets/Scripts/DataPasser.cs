using UnityEngine;
using UnityEngine.UI;

public class DataPasser : MonoBehaviour
{
    public static int playerCount;
    public static int roundsToWin;

    public void ChangeingScene()
    {
        foreach (Transform child in GameObject.Find("Canvas").transform)
        {
            if (child.name == "PlayerCount")
            {
                playerCount = int.Parse(child.GetComponent<Text>().text);
            }
            else if (child.name == "RoundsToWin")
            {
                roundsToWin = int.Parse(child.GetComponent<Text>().text);
            }
        }
    }
}
