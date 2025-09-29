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

        UpdateHUD();
    }

    void Update()
    {
        // Se apertar R, reseta os dados
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetData();
        }
    }

    // ===== Dinheiro e Pontos =====
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateHUD();
    }

    public void AddPoints(int amount)
    {
        points += amount;
        AddXP(amount);
        UpdateHUD();
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
        UpdateHUD();
    }

    int XPToNextLevel()
    {
        return 2 * (int)Mathf.Pow(2, level);
    }

    void UpdateHUD()
    {
        if (moneyText != null) moneyText.text = "Dinheiro: " + money;
        if (pointsText != null) pointsText.text = "Pontos: " + points;
        if (levelText != null) levelText.text = "Nível: " + level;
        if (xpBar != null)
            xpBar.fillAmount = (float)currentXP / XPToNextLevel();
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
            PlayerData.Instance.UpdateHUD();
        }

        Debug.Log("PlayerData resetado!");
    }
}
