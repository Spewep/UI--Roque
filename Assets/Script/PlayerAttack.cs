using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("HUD")]
    public Text moneyText;
    public Text pointsText;
    public PlayerData playerData;

    [Header("Config")]
    public float attackRange = 2f;
    public int money = 0;
    public int points = 0;

    [Header("Cooldown")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
        {
            if (hit.collider.CompareTag("toycoon"))
            {
                money += 1;
                points += 1;
                UpdateHUD();
                playerData.AddMoney(1);
                playerData.AddPoints(1);
            }
        }
        Debug.DrawRay(origin, direction * attackRange, Color.red, 0.5f);
    }

    void UpdateHUD()
    {
        if (moneyText != null) moneyText.text = "Dinheiro: " + money;
        if (pointsText != null) pointsText.text = "Pontos: " + points;
    }
}
