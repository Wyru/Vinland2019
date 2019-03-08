using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour {
    public Image lifebar;
    public Image berserkBar;

    private int maxLife;
    private int life;

    private int maxBerserk;
    private int berserk;

    private void Start() {
    }

    private void Update() {
        maxLife = GameManager.Instance.player.maxLife;
        life = GameManager.Instance.player.life;
        lifebar.fillAmount = (float)life/maxLife;

        maxBerserk = GameManager.Instance.player.maxBerserkValue;
        berserk = GameManager.Instance.player.currentBerserk;
        berserkBar.fillAmount = (float)berserk/maxBerserk;
    }


    public void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}