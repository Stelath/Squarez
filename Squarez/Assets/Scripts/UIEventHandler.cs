using UnityEngine;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    public GameObject playerCount;
    public GameObject roundsToWin;

    public void DecreasePlayersButtonClicked()
    {
        if (int.Parse(playerCount.GetComponent<Text>().text) > 2)
        {
            playerCount.GetComponent<Text>().text = (int.Parse(playerCount.GetComponent<Text>().text) - 1).ToString();
        }
    }

    public void IncreasePlayersButtonClicked()
    {
        if (int.Parse(playerCount.GetComponent<Text>().text) < 4)
        {
            playerCount.GetComponent<Text>().text = (int.Parse(playerCount.GetComponent<Text>().text) + 1).ToString();
        }
    }
    
    public void DecreaseRoundsToWinButton()
    {
        if (int.Parse(roundsToWin.GetComponent<Text>().text) > 0)
        {
            roundsToWin.GetComponent<Text>().text = (int.Parse(roundsToWin.GetComponent<Text>().text) - 1).ToString();
        }
    }

    public void IncreaseRoundsToWinButton()
    {
        if (int.Parse(roundsToWin.GetComponent<Text>().text) < 20)
        {
            roundsToWin.GetComponent<Text>().text = (int.Parse(roundsToWin.GetComponent<Text>().text) + 1).ToString();
        }
    }
}
