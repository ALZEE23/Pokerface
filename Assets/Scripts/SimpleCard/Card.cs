using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("Card Data")]
    public CardData data;

    [Header("Visual Reference")]
    public CardVisual cardVisual;
    // Pastikan cardVisualPrefab diassign di Inspector
    [SerializeField] private GameObject cardVisualPrefab;

    void Awake()
    {
        // Debug untuk memeriksa prefab ada
        if (cardVisualPrefab == null)
        {
            Debug.LogError("Card Visual Prefab tidak diassign di " + gameObject.name);
            return;
        }
    }

    // Pembuatan CardVisual dipanggil dari luar, bukan di Start()
    public void CreateVisual()
    {

        // Debug untuk proses instansiasi
        Debug.Log("Membuat CardVisual untuk " + gameObject.name);

        // Cek VisualCardsHandler ada
        if (VisualCardsHandler.instance == null)
        {
            Debug.LogError("VisualCardsHandler.instance tidak ditemukan!");
            return;
        }

        GameObject visualObj = Instantiate(cardVisualPrefab, VisualCardsHandler.instance.transform);
        visualObj.name = gameObject.name + "_Visual"; // Nama yang jelas untuk debugging
        cardVisual = visualObj.GetComponent<CardVisual>();

        if (cardVisual == null)
        {
            Debug.LogError("CardVisual component tidak ditemukan di prefab!");
            return;
        }

        cardVisual.Initialize(this);
        Debug.Log("CardVisual berhasil dibuat: " + visualObj.name);

        // Update visual jika data sudah ada
        if (data != null)
        {
            cardVisual.UpdateCardVisuals();
        }

    }

    public void SetCardData(CardData cardData)
    {
        this.data = cardData;

        // Let CardVisual handle all visual updates
        if (cardVisual != null)
        {
            cardVisual.UpdateCardVisuals();
        }
    }

    // Method untuk membuka kartu
    public void OpenCard()
    {
        if (cardVisual != null)
        {
            cardVisual.OpenCard();
        }
    }

    // Method untuk menutup kartu
    public void CloseCard()
    {
        if (cardVisual != null)
        {
            cardVisual.CloseCard();
        }
    }
}