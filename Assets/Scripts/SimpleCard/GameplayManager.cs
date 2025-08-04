using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Tambahkan ini jika belum ada
using TMPro; // Tambahkan ini jika belum ada

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private CardHolder playerHand;
    [SerializeField] private CardHolder dealerHand;
    [SerializeField] private DeckSystem deckSystem;
    [SerializeField] private TextMeshProUGUI dealingText; // Tambahkan ini
    [SerializeField] private Button hitButton; // Tambahkan ini
    [SerializeField] private Button betButton; // Tambahkan ini
    [SerializeField] private Button standButton; // Tambahkan ini
    [SerializeField] private TextMeshProUGUI playerCalculate; // Tambahkan ini
    [SerializeField] private TextMeshProUGUI dealerCalculate; // Tambahkan ini
    [SerializeField] Animator betPanel;


    void Start()
    {
        // Cek VisualCardsHandler
        if (VisualCardsHandler.instance == null)
        {
            Debug.LogError("VisualCardsHandler instance tidak ditemukan! Membuat baru...");
            GameObject handlerObj = new GameObject("VisualCardsHandler");
            handlerObj.AddComponent<VisualCardsHandler>();
        }

        // Sekarang aman untuk memulai game
        NewGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        // Clear hands
        playerHand.ClearCards();
        dealerHand.ClearCards();

        // Initialize and shuffle deck
        deckSystem.InitializeDeck();
        deckSystem.ShuffleDeck();

        // Deal initial cards
        DealInitialCards();
    }

    private void DealInitialCards()
    {
        // Menampilkan pesan "Dealing..."
        if (dealingText != null)
            dealingText.gameObject.SetActive(true);

        // Mulai pembagian kartu dengan delay
        StartCoroutine(DealInitialCardsCoroutine());
    }

    private IEnumerator DealInitialCardsCoroutine()
    {
        // Tunggu sebentar sebelum mulai membagikan kartu
        yield return new WaitForSeconds(1.0f);

        // Deal 2 kartu ke player
        for (int i = 0; i < 2; i++)
        {
            Card playerCard = deckSystem.DealCard(playerHand);
            yield return new WaitForSeconds(0.8f);

            // Buka semua kartu player setelah delay
            StartCoroutine(OpenCardAfterDelay(playerCard));
        }

        // Deal 2 kartu ke dealer
        for (int i = 0; i < 2; i++)
        {
            Card dealerCard = deckSystem.DealCard(dealerHand);
            yield return new WaitForSeconds(0.8f);

            // Hanya buka kartu pertama dealer
            if (i == 0)
            {
                StartCoroutine(OpenCardAfterDelay(dealerCard));
                CalculateDealerScore();
            }


        }

        // Sembunyikan pesan "Dealing..."
        if (dealingText != null)
            dealingText.gameObject.SetActive(false);

        // Aktifkan tombol-tombol permainan
        EnableGameButtons(true);
    }

    private IEnumerator OpenCardAfterDelay(Card card)
    {
        // Tunggu animasi kartu datang selesai
        yield return new WaitForSeconds(5.5f);

        // Buka kartu
        card.OpenCard();
        CalculatePlayerScore();
    }

    private void EnableGameButtons(bool enabled)
    {
        // Enable/disable tombol-tombol permainan
        if (hitButton != null) hitButton.interactable = enabled;
        if (standButton != null) standButton.interactable = enabled;
        if (betButton != null) betButton.interactable = enabled;
        // dll
    }

    public void DealCardToPlayer()
    {
        Card playerCard = deckSystem.DealCard(playerHand);
        StartCoroutine(OpenCardAfterDelay(playerCard));
        EnableGameButtons(false);
        DealerAI();
    }

    // Button handlers for UI
    public void PlayerHit()
    {
        Card playerCard = deckSystem.DealCard(playerHand);
        EnableGameButtons(false); // Nonaktifkan tombol sementara animasi berjalan

        // Tunggu kartu selesai dianimasikan, baru buka dan evaluasi
        StartCoroutine(OpenPlayerCardAndEvaluate(playerCard));
    }

    private IEnumerator OpenPlayerCardAndEvaluate(Card card)
    {
        // Tunggu animasi kartu datang selesai
        yield return new WaitForSeconds(5.5f);

        // Buka kartu player
        card.OpenCard();
        CalculatePlayerScore();

        // Cek skor player
        int playerScore = playerHand.CalculateScore();

        if (playerScore > 21)
        {
            // Player bust, langsung evaluasi hasil
            EvaluateGameResult();
        }
        else if (playerScore == 21)
        {
            // Player mencapai 21, otomatis stand dan dealer ambil giliran
            StartCoroutine(DealerTakeTurn());
        }
        else
        {
            // Player belum 21 atau bust, aktifkan tombol lagi
            EnableGameButtons(true);
        }
    }

    public void CalculatePlayerScore()
    {
        int score = playerHand.CalculateScore();
        playerCalculate.text = "Player Score: " + score + "/21";
    }

    public void CalculateDealerScore()
    {
        int score = dealerHand.CalculateScore();
        dealerCalculate.text = "Dealer Score: " + score + "/21";
    }

    // Tambahkan button Bet
    public void BetButton()
    {
        // Hanya berfungsi jika player sudah yakin dengan kartu mereka
        EnableGameButtons(false);

        // Evaluasi hasil final
        EvaluateGameResult();
    }

    public void DealerHit()
    {
        deckSystem.DealCard(dealerHand);
    }

    public void ResetGame()
    {
        NewGame();
    }

    // Tambahkan function ini ke GameplayManager.cs
    public void DealerAI()
    {
        // JANGAN buka semua kartu dealer - kartu kedua tetap tertutup
        // Tapi kita tetap hitung skor yang sebenarnya meskipun kartu tertutup)

        // Hitung skor dealer (sistem tahu nilai sebenarnya meskipun kartu tertutup)
        int dealerScore = dealerHand.CalculateScore();
        // CalculateDealerScore(); // Jangan update tampilan skor dealer dulu

        // Delay awal
        StartCoroutine(DealerDecision(dealerScore));
    }

    private IEnumerator DealerDecision(int currentScore)
    {
        // Tunggu sebentar untuk simulasi dealer berpikir
        yield return new WaitForSeconds(1.5f);

        // Cek skor dealer
        if (currentScore <= 17)
        {
            // Dealer mengambil kartu baru
            Debug.Log("Dealer hits (score: " + currentScore + ")");
            Card dealerCard = deckSystem.DealCard(dealerHand);

            // Tunggu animasi selesai
            yield return new WaitForSeconds(5.5f);

            // Kartu baru juga tetap tertutup - dealer tidak memperlihatkan kartu
            // dealerCard.OpenCard(); // Jangan buka kartu

            // Update skor dealer (secara internal saja)
            int newScore = dealerHand.CalculateScore();

            // Evaluasi kembali dengan skor baru
            StartCoroutine(DealerDecision(newScore));
        }
        else
        {
            // Dealer stand
            Debug.Log("Dealer stands (internal score: " + currentScore + ")");

            // Evaluasi hasil game - tapi skor dealer tetap tersembunyi
            // EvaluateGameResult();
            EnableGameButtons(true); // Aktifkan tombol lagi
        }
    }

    private void EvaluateGameResult()
    {
        int playerScore = playerHand.CalculateScore();
        int dealerScore = dealerHand.CalculateScore(); // Skor dealer yang sebenarnya

        string result = "";

        // Evaluasi hasil berdasarkan aturan Blackjack sederhana
        if (dealerScore == 21)
        {
            result = "Dealer Blackjack! Dealer Wins!";
            // Setelah hasil ditentukan, baru buka kartu dealer
            RevealDealerCards();
        }
        else if (playerScore == 21)
        {
            result = "Player Blackjack! Player Wins!";
            RevealDealerCards();
        }
        else if (playerScore > 21 && dealerScore > 21)
        {
            result = "Both Bust! Dealer Winners!";
            RevealDealerCards();
        }
        else if (playerScore > 21)
        {
            result = "Player Bust! Dealer Wins!";
            // Setelah hasil ditentukan, baru buka kartu dealer
            RevealDealerCards();
        }
        else if (dealerScore > 21)
        {
            result = "Dealer Bust! Player Wins!";
            RevealDealerCards();
        }
        else if (playerScore > dealerScore)
        {
            result = "Player Wins!";
            RevealDealerCards();
        }
        // else if (dealerScore > playerScore)
        // {
        //     result = "Dealer Wins!";
        //     RevealDealerCards();
        // }
        // else
        // {
        //     result = "Push! It's a tie!";
        //     RevealDealerCards();
        // }

        // Tampilkan hasil
        if (dealingText != null)
        {
            dealingText.text = result;
            dealingText.gameObject.SetActive(true);
        }

        // Nonaktifkan tombol gameplay dan tampilkan tombol new game
        EnableGameButtons(false);
        // Tambahkan implementasi untuk menampilkan tombol new game jika diperlukan
    }

    // Tambahkan method baru untuk membuka kartu dealer di akhir permainan
    private void RevealDealerCards()
    {
        // Buka semua kartu dealer hanya di akhir permainan
        foreach (Card card in dealerHand.GetCards())
        {
            card.OpenCard();
        }

        // Update tampilan skor dealer setelah kartu terbuka
        CalculateDealerScore();
    }

    // Modifikasi PlayerStand untuk menjalankan satu giliran dealer saja
    public void PlayerStand()
    {
        // Player memilih stand, sekarang giliran dealer mengambil SATU kartu
        EnableGameButtons(false);
        StartCoroutine(DealerTakeTurn());
    }

    // Mengganti DealerAI dengan DealerTakeTurn untuk satu giliran saja
    private IEnumerator DealerTakeTurn()
    {
        // Tunggu sebentar untuk simulasi dealer berpikir
        yield return new WaitForSeconds(1.5f);

        // Hitung skor dealer saat ini (sistem tahu nilai sebenarnya)
        int dealerScore = dealerHand.CalculateScore();

        // Cek jika dealer sudah 21 atau lebih
        if (dealerScore >= 21)
        {
            // Dealer mencapai 21 atau lebih, evaluasi hasil
            EvaluateGameResult();
            yield break;
        }

        // Cek strategi dealer
        if (dealerScore < 17)
        {
            // Dealer mengambil SATU kartu baru
            Debug.Log("Dealer hits (current score: " + dealerScore + ")");
            Card dealerCard = deckSystem.DealCard(dealerHand);

            // Tunggu animasi selesai
            yield return new WaitForSeconds(5.5f);

            // Kartu baru tetap tertutup
            // dealerCard.OpenCard(); // Jangan buka kartu

            // Hitung skor dealer baru
            dealerScore = dealerHand.CalculateScore();

            // Cek apakah dealer mencapai 21 atau lebih
            if (dealerScore >= 21)
            {
                // Dealer mencapai 21 atau bust, evaluasi hasil
                EvaluateGameResult();
            }
            else
            {
                // Dealer belum 21, kembali ke giliran player
                Debug.Log("Back to player's turn (dealer's hidden score: " + dealerScore + ")");
                EnableGameButtons(true);
            }
        }
        else
        {
            // Dealer sudah memiliki 17+ tapi belum 21, kembali ke giliran player
            Debug.Log("Dealer stands (hidden score: " + dealerScore + "), back to player's turn");
            EnableGameButtons(true);
        }
    }
}