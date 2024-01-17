using UnityEngine;
using UnityEngine.SceneManagement;

public class PreBattleButtonScript : MonoBehaviour
{

    public void OnClickPreBattleButton()
    {
        SceneManager.LoadScene("PreBattle");
    }

}