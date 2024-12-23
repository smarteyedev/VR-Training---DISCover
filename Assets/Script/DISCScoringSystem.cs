using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DISCScoringSystem : MonoBehaviour
{
    // Question Setting
    [System.Serializable]
    public struct Question
    {
        public string questionText;
        public string answerA;
        public string answerB;
        public string dimensionA;
        public string dimensionB;

        // GameObjects untuk visual jawaban
        public GameObject visualA;
        public GameObject visualB;
    }

    [Header("Question Setting")]
    public Question[] questions;

    [Header("Question Attributes")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button buttonA;
    public Button buttonB;
    public TextMeshProUGUI buttonAText;
    public TextMeshProUGUI buttonBText;
    public TextMeshProUGUI resultText;

    [Header("Main Question")]
    public GameObject startPanel;
    public TextMeshProUGUI startPanelQuestionText;

    [Header("Fade Effect")]
    public Image fadeOverlay;

    [Header("Character Animation")]
    public Animator characterAnimator;

    [Header("Teleportation Point")]
    public Transform teleportationStartPoint; // Titik teleportasi untuk pemain
    public GameObject player; // Referensi ke player (rig atau kamera utama VR)

    [Header("Result Images")]
    public Image resultImage; // Tempat untuk menampilkan gambar hasil DISC
    public Sprite dominanceImage; // Gambar untuk "Dominance"
    public Sprite influenceImage; // Gambar untuk "Influence"
    public Sprite steadinessImage; // Gambar untuk "Steadiness"
    public Sprite complianceImage; // Gambar untuk "Compliance"

    [Header("Result Button")]
    public Button resultButton; // Tombol yang muncul bersamaan dengan hasil

    private int currentQuestionIndex = 0;

    private int scoreD = 0;
    private int scoreI = 0;
    private int scoreS = 0;
    private int scoreC = 0;

    void Start()
    {
        // Pastikan overlay mulai dalam keadaan transparan
        if (fadeOverlay != null)
        {
            Color color = fadeOverlay.color;
            color.a = 0; // Transparan
            fadeOverlay.color = color;
        }

        // Setup tombol
        buttonA.onClick.AddListener(() => OnAnswerSelected("A"));
        buttonB.onClick.AddListener(() => OnAnswerSelected("B"));

        // Atur tampilan awal
        if (startPanel != null)
        {
            startPanel.SetActive(true);
            startPanel.transform.localScale = Vector3.zero;
            startPanelQuestionText.transform.localScale = Vector3.zero;

            AnimateStartPanelAndText();
        }

        questionPanel.SetActive(false);
        questionPanel.transform.localScale = Vector3.zero;
        questionText.transform.localScale = Vector3.zero;
        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);
        buttonAText.transform.localScale = Vector3.zero;
        buttonBText.transform.localScale = Vector3.zero;

        // Set tombol hasil sebagai tidak aktif pada awalnya
        resultButton.gameObject.SetActive(false);
    }

    void AnimateStartPanelAndText()
    {
        if (currentQuestionIndex < questions.Length)
        {
            startPanelQuestionText.text = questions[currentQuestionIndex].questionText;
        }

        LeanTween.scale(startPanel, Vector3.one, 0.5f).setEaseOutBack();
        LeanTween.scale(startPanelQuestionText.gameObject, Vector3.one, 0.5f).setEaseOutBack();

        // Pastikan questionText di questionPanel tidak muncul saat startPanel tampil
        questionText.gameObject.SetActive(false);

        LeanTween.delayedCall(2f, () =>
        {
            LeanTween.scale(startPanelQuestionText.gameObject, Vector3.zero, 0.5f).setEaseInBack();
            LeanTween.scale(startPanel, Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() =>
            {
                startPanel.SetActive(false);
                AnimateQuestionPanelAndText(); // Lanjutkan animasi panel pertanyaan setelah start panel selesai
            });
        });
    }

    void AnimateQuestionPanelAndText()
    {
        if (currentQuestionIndex < questions.Length)
        {
            questionText.text = questions[currentQuestionIndex].questionText;
            buttonAText.text = questions[currentQuestionIndex].answerA;
            buttonBText.text = questions[currentQuestionIndex].answerB;
        }

        questionPanel.SetActive(true);
        buttonA.gameObject.SetActive(true);
        buttonB.gameObject.SetActive(true);

        questionPanel.transform.localScale = Vector3.zero;
        questionText.transform.localScale = Vector3.zero;

        // Aktifkan questionText hanya setelah questionPanel mulai muncul
        LeanTween.scale(questionPanel, Vector3.one, 0.5f).setEaseOutBack();
        LeanTween.delayedCall(0.2f, () =>
        {
            questionText.gameObject.SetActive(true);
            LeanTween.scale(questionText.gameObject, Vector3.one, 0.5f).setEaseOutBack();
        });

        TriggerCharacterAnimation();

        AnimateButtons();
    }

    void AnimateButtons()
    {
        Question q = questions[currentQuestionIndex];

        // Atur skala awal tombol dan teks
        buttonA.transform.localScale = Vector3.one * 0.1f;
        buttonB.transform.localScale = Vector3.one * 0.1f;
        buttonAText.transform.localScale = Vector3.one * 0.1f;
        buttonBText.transform.localScale = Vector3.one * 0.1f;

        // Aktifkan visual A terlebih dahulu dengan delay 4 detik
        LeanTween.delayedCall(2f, () =>
        {
            if (q.visualA != null)
            {
                q.visualA.SetActive(true);
                q.visualA.transform.localScale = Vector3.one * 0.1f;
                LeanTween.scale(q.visualA, Vector3.one, 0.5f).setEaseOutBack();
            }

            LeanTween.scale(buttonA.gameObject, Vector3.one * 7.5f, 0.5f).setEaseOutBounce();
            LeanTween.scale(buttonAText.gameObject, Vector3.one * 0.9f, 0.5f).setEaseOutBounce();
        });

        // Aktifkan visual B dan tombol B dengan delay 2 detik setelah A
        LeanTween.delayedCall(4f, () => // 4 detik untuk A + 2 detik untuk B
        {
            if (q.visualB != null)
            {
                q.visualB.SetActive(true);
                q.visualB.transform.localScale = Vector3.one * 0.1f;
                LeanTween.scale(q.visualB, Vector3.one, 0.5f).setEaseOutBack();
            }

            LeanTween.scale(buttonB.gameObject, Vector3.one * 7.5f, 0.5f).setEaseOutBounce();
            LeanTween.scale(buttonBText.gameObject, Vector3.one * 0.9f, 0.5f).setEaseOutBounce();
        });
    }

    void TriggerCharacterAnimation()
    {
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("StartAnimation");
        }
    }

    void OnAnswerSelected(string answer)
    {
        Question q = questions[currentQuestionIndex];

        // Nonaktifkan semua visual sebelum mengaktifkan yang sesuai
        if (q.visualA != null) q.visualA.SetActive(false);
        if (q.visualB != null) q.visualB.SetActive(false);

        if (answer == "A")
        {
            UpdateScore(q.dimensionA);
        }
        else if (answer == "B")
        {
            UpdateScore(q.dimensionB);
        }

        currentQuestionIndex++;

        StartCoroutine(FadeEffect(() => LoadQuestion()));
    }

    private IEnumerator FadeEffect(System.Action onFadeComplete)
    {
        // Fade in (Layar menjadi hitam)
        yield return StartCoroutine(Fade(0, 1, 0.5f));

        // Teleport pemain setelah layar menjadi hitam (fade-in selesai)
        if (teleportationStartPoint != null && player != null)
        {
            // Teleport pemain ke posisi yang diinginkan (posisi teleportationStartPoint)
            player.transform.position = teleportationStartPoint.position;
            player.transform.rotation = teleportationStartPoint.rotation; // Jika perlu, ikutkan rotasi
        }

        questionPanel.gameObject.SetActive(false);
        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        // Lanjutkan dengan onFadeComplete setelah delay
        onFadeComplete?.Invoke();

        // Fade out (Layar kembali transparan)
        yield return StartCoroutine(Fade(1, 0, 0.5f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0;
        Color color = fadeOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            color.a = alpha;
            fadeOverlay.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeOverlay.color = color;
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            Question q = questions[currentQuestionIndex];

            // Perbarui teks pertanyaan dan jawaban
            questionText.text = q.questionText;
            buttonAText.text = q.answerA;
            buttonBText.text = q.answerB;

            // Tampilkan animasi start panel sebelum ke animasi pertanyaan
            startPanel.SetActive(true);
            startPanel.transform.localScale = Vector3.zero;
            startPanelQuestionText.text = q.questionText;
            startPanelQuestionText.transform.localScale = Vector3.zero;

            AnimateStartPanelAndText(); // Tambahkan animasi untuk transisi ke pertanyaan baru
        }
        else
        {
            ShowResult(); // Tampilkan hasil jika semua pertanyaan selesai
        }
    }

    void UpdateScore(string dimension)
    {
        switch (dimension)
        {
            case "D": scoreD++; break;
            case "I": scoreI++; break;
            case "S": scoreS++; break;
            case "C": scoreC++; break;
        }
    }

    void ShowResult()
    {
        int maxScore = Mathf.Max(scoreD, scoreI, scoreS, scoreC);
        string personality = "";

        if (maxScore == scoreD) personality = "Dominance";
        else if (maxScore == scoreI) personality = "Influence";
        else if (maxScore == scoreS) personality = "Steadiness";
        else if (maxScore == scoreC) personality = "Compliance";

        questionText.enabled = false;
        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);

        resultText.text = $"Your DISC Personality: {personality}";

        // Ganti gambar sesuai dengan hasil
        switch (personality)
        {
            case "Dominance":
                resultImage.sprite = dominanceImage;
                break;
            case "Influence":
                resultImage.sprite = influenceImage;
                break;
            case "Steadiness":
                resultImage.sprite = steadinessImage;
                break;
            case "Compliance":
                resultImage.sprite = complianceImage;
                break;
        }

        resultImage.gameObject.SetActive(true); // Tampilkan gambar hasil
        resultButton.gameObject.SetActive(true); // Tampilkan tombol setelah hasil muncul
    }
}
