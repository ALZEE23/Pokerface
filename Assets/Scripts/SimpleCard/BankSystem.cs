using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BankSystem : MonoBehaviour
{
    // Singleton instance
    public static BankSystem instance;

    [Header("Bank Settings")]
    [SerializeField] private int initialBalance = 1000;
    private int currentBalance;
    private int currentBet = 0;
    private int minimumBet = 10;
    private int maximumBet = 500;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private TextMeshProUGUI debtText;
    [SerializeField] private GameObject betPanel;

    // Event untuk notifikasi perubahan uang
    public event Action<int> OnBalanceChanged;
    public event Action<int> OnBetChanged;

    void Awake()
    {
        instance = this;
        currentBalance = initialBalance;
    }

    void Start()
    {
        UpdateBalanceUI();
        UpdateBetUI();
    }

    // Tambah uang ke balance
    public void AddBalance(int amount)
    {
        if (amount <= 0) return;

        currentBalance += amount;
        OnBalanceChanged?.Invoke(currentBalance);
        UpdateBalanceUI();
    }

    // Kurangi uang dari balance
    public bool RemoveBalance(int amount)
    {
        if (amount <= 0) return false;

        if (currentBalance >= amount)
        {
            currentBalance -= amount;
            OnBalanceChanged?.Invoke(currentBalance);
            UpdateBalanceUI();
            return true;
        }

        return false;
    }

    // Untuk memasang taruhan
    public bool PlaceBet(int amount)
    {
        if (amount < minimumBet || amount > maximumBet)
        {
            Debug.Log($"Bet harus antara {minimumBet} dan {maximumBet}");
            return false;
        }

        if (currentBalance >= amount)
        {
            currentBet = amount;
            RemoveBalance(amount);
            OnBetChanged?.Invoke(currentBet);
            UpdateBetUI();
            return true;
        }
        else
        {
            Debug.Log("Uang tidak cukup untuk bet");
            return false;
        }
    }

    // Mendapatkan hasil kemenangan (multiplier 2x untuk Blackjack standar)
    public void WinBet(float multiplier = 2.0f)
    {
        int winnings = Mathf.FloorToInt(currentBet * multiplier);
        AddBalance(winnings);

        Debug.Log($"Menang! +{winnings}");
        currentBet = 0;
        UpdateBetUI();
    }

    // Kalah taruhan
    public void LoseBet()
    {
        Debug.Log($"Kalah! -{currentBet}");
        currentBet = 0;
        UpdateBetUI();
    }

    // Mengembalikan taruhan (misalnya saat draw/push)
    public void ReturnBet()
    {
        AddBalance(currentBet);
        Debug.Log($"Draw/Push, taruhan dikembalikan");
        currentBet = 0;
        UpdateBetUI();
    }

    // Update UI balance
    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"Balance: ${currentBalance}";
        }
    }

    // Update UI bet
    private void UpdateBetUI()
    {
        if (betText != null)
        {
            betText.text = $"Current Bet: ${currentBet}";
        }
    }

    // Menampilkan panel bet
    public void ShowBetPanel(bool show)
    {
        if (betPanel != null)
        {
            betPanel.SetActive(show);
        }
    }

    // Getter
    public int GetCurrentBalance() { return currentBalance; }
    public int GetCurrentBet() { return currentBet; }
    public int GetMinimumBet() { return minimumBet; }
    public int GetMaximumBet() { return maximumBet; }
}
