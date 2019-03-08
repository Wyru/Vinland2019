using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    public static HUDController Instance;


    public Image lifebar;
    public Image berserkBar;

    public Slider levelProgress;

    public GameObject gameOverHud;
    public GameObject transitionHud;

    private int maxLife;
    private int life;

    private int maxBerserk;
    private int berserk;

    private void Start()
    {
        if (transitionHud != null)
            transitionHud.SetActive(true);

        Instance = this;
    }


    private void Update()
    {
        maxLife = GameManager.Instance.player.maxLife;
        life = GameManager.Instance.player.life;
        lifebar.fillAmount = (float)life / maxLife;

        maxBerserk = GameManager.Instance.player.maxBerserkValue;
        berserk = GameManager.Instance.player.currentBerserk;
        berserkBar.fillAmount = (float)berserk / maxBerserk;

        levelProgress.maxValue = GameManager.Instance.numberOfEnemies;
        levelProgress.value = GameManager.Instance.CountDeadEnemies();

    }

    public void ShowGameOver()
    {
        gameOverHud.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverHud.GetComponent<GameOver>().Out();
    }

}