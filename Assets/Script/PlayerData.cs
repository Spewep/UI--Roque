using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    [Header("HUD")]
    public Text moneyText;
    public Text pointsText;
    public Image xpBar;
    public Text levelText;

    [Header("Valores do Player")]
    public int money = 0;
    public int points = 0;
    public int level = 0;
    public int currentXP = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        money = PlayerPrefs.GetInt("Money", 0);
        points = PlayerPrefs.GetInt("Points", 0);
        level = PlayerPrefs.GetInt("Level", 0);
        currentXP = PlayerPrefs.GetInt("XP", 0);
    }

    // ===== Dinheiro e Pontos =====
    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void AddPoints(int amount)
    {
        points += amount;
        AddXP(amount);
    }

    // ===== Sistema de XP e Level =====
    public void AddXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= XPToNextLevel())
        {
            currentXP -= XPToNextLevel();
            level++;
        }
    }

    int XPToNextLevel()
    {
        return 2 * (int)Mathf.Pow(2, level);
    }

    // ===== Salvar =====
    public void SaveData()
    {
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetInt("Points", points);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("XP", currentXP);
        PlayerPrefs.Save();
        Debug.Log("Dados salvos!");
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void ResetData()
    {
        PlayerPrefs.DeleteKey("Money");
        PlayerPrefs.DeleteKey("Points");
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.DeleteKey("XP");
        PlayerPrefs.Save();

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.money = 0;
            PlayerData.Instance.points = 0;
            PlayerData.Instance.level = 0;
            PlayerData.Instance.currentXP = 0;
        }

        Debug.Log("PlayerData resetado!");
    }
}
