using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Image lifebar;

    private int maxLife;
    private int life;

    private void Start() {
    }

    private void Update() {
        maxLife = GameManager.Instance.player.maxLife;
        life = GameManager.Instance.player.life;
        lifebar.fillAmount = (float)life/maxLife;
    }

}