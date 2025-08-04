using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using UnityEditor.Animations;

public class CardVisual : MonoBehaviour
{
    [Header("Card Reference")]
    public Card parentCard;

    [Header("Visual Elements")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private RectTransform cardShadow;
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private TextMeshProUGUI cornerTopText;
    [SerializeField] private TextMeshProUGUI cornerBottomText;
    [SerializeField] private Image topSuitImage;
    [SerializeField] private Image bottomSuitImage;

    [Header("Suit Sprites")]
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite diamondSprite;
    [SerializeField] private Sprite clubSprite;
    [SerializeField] private Sprite spadeSprite;

    [Header("Animation")]
    [SerializeField] private AnimatorController animatorController; // Untuk animasi dari deck
    [SerializeField] private float initialDelayAtDeck = 5f;  // Waktu di deck transform
    [SerializeField] private float moveDuration = 0.5f;      // Durasi animasi perpindahan

    [Header("Card State")]
    [SerializeField] private bool isOpen = false;

    // Referensi ke animator
    private Animator animator;
    private Vector3 originalScale;
    private bool initialized = false;
    private bool isAnimatingFromDeck = false;
    private Vector3 targetPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animatorController = GetComponent<AnimatorController>();
        originalScale = transform.localScale;

        if (parentCard != null)
        {
            // Simpan posisi target kartu (posisi akhir)
            targetPosition = parentCard.transform.position;

            // Pindahkan visual ke posisi deck jika ada
            if (DeckSystem.instance != null && DeckSystem.instance.deckTransform != null)
            {
                transform.position = DeckSystem.instance.deckTransform.position;

                // Mulai animasi
                StartCoroutine(AnimateFromDeck());
            }
        }
    }

    // Coroutine untuk animasi dari deck
    private IEnumerator AnimateFromDeck()
    {
        isAnimatingFromDeck = true;

        // Tunggu di deck selama waktu yang ditentukan
        yield return new WaitForSeconds(initialDelayAtDeck);

        // Dapatkan posisi pivot tengah target
        Vector3 centerPivotPosition = targetPosition;

        // Jika RectTransform tersedia, gunakan center dari corners
        RectTransform parentRect = parentCard.GetComponent<RectTransform>();
        if (parentRect != null)
        {
            Vector3[] corners = new Vector3[4];
            parentRect.GetWorldCorners(corners);
            centerPivotPosition = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
        }

        // Animasi dari deck ke posisi target
        transform.DOMove(centerPivotPosition, moveDuration).SetEase(Ease.OutQuad);

        // Tunggu animasi selesai
        yield return new WaitForSeconds(moveDuration);
        animator.enabled = true;

        isAnimatingFromDeck = false;
    }

    // Ikuti posisi Card
    void LateUpdate()
    {
        // Jangan ikuti posisi kartu jika sedang dianimasikan dari deck
        if (isAnimatingFromDeck) return;

        // Hanya ikuti posisi Card jika tidak sedang dianimasikan oleh Tween
        if (parentCard != null && !DOTween.IsTweening(transform))
        {
            // Untuk UI Elements (Canvas)
            RectTransform parentRect = parentCard.GetComponent<RectTransform>();
            if (parentRect != null)
            {
                // Dapatkan posisi pivot tengah parentCard
                Vector3[] corners = new Vector3[4];
                parentRect.GetWorldCorners(corners);

                // Hitung posisi tengah dari 4 corner
                Vector3 centerPos = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;

                // Set posisi CardVisual
                transform.position = centerPos;
            }
            else
            {
                // Fallback untuk non-UI objects
                transform.position = parentCard.transform.position;
            }
        }
    }

    public void Initialize(Card card)
    {
        parentCard = card;
        initialized = true;

        if (card.data != null)
        {
            UpdateCardVisuals();
        }

        // Simpan posisi target
        targetPosition = card.transform.position;

        // Pindahkan ke posisi deck jika ada
        if (DeckSystem.instance != null && DeckSystem.instance.deckTransform != null)
        {
            transform.position = DeckSystem.instance.deckTransform.position;

            // Mulai animasi
            StartCoroutine(AnimateFromDeck());
        }
    }

    public void OnCardHoverEnter(Card card)
    {
        transform.DOScale(originalScale * 1.15f, 0.2f);
    }

    public void OnCardHoverExit(Card card)
    {
        transform.DOScale(originalScale, 0.2f);
    }

    public void UpdateCardVisuals()
    {
        if (parentCard == null || parentCard.data == null) return;

        CardData data = parentCard.data;

        // Update text
        string displayValue = data.GetDisplayName();

        if (centerText != null)
        {
            centerText.text = displayValue;
            centerText.color = data.GetSuitColor();
        }

        if (cornerTopText != null)
        {
            cornerTopText.text = displayValue;
            cornerTopText.color = data.GetSuitColor();
        }

        if (cornerBottomText != null)
        {
            cornerBottomText.text = displayValue;
            cornerBottomText.color = data.GetSuitColor();
        }

        // Update suit images
        Sprite suitSprite = GetSuitSprite(data.suit);

        if (topSuitImage != null)
        {
            topSuitImage.sprite = suitSprite;
            topSuitImage.color = data.GetSuitColor();
        }

        if (bottomSuitImage != null)
        {
            bottomSuitImage.sprite = suitSprite;
            bottomSuitImage.color = data.GetSuitColor();
        }
    }

    private Sprite GetSuitSprite(CardData.Suit suit)
    {
        switch (suit)
        {
            case CardData.Suit.Hearts: return heartSprite;
            case CardData.Suit.Diamonds: return diamondSprite;
            case CardData.Suit.Clubs: return clubSprite;
            case CardData.Suit.Spades: return spadeSprite;
            default: return null;
        }
    }

    // Method untuk membuka kartu
    public void OpenCard()
    {
        isOpen = true;

        // Update animator jika ada
        if (animator != null)
        {
            animator.SetBool("open", true);
        }

        // Tambahkan efek lain jika perlu
        Debug.Log("Kartu dibuka: " + gameObject.name);
    }

    // Method untuk menutup kartu
    public void CloseCard()
    {
        isOpen = false;

        // Update animator jika ada
        if (animator != null)
        {
            animator.SetBool("open", false);
        }

        // Tambahkan efek lain jika perlu
        Debug.Log("Kartu ditutup: " + gameObject.name);
    }

    // Dapatkan status kartu
    public bool IsOpen()
    {
        return isOpen;
    }
}
