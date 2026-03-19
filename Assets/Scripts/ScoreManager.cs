using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score")]
    public int currentScore;
    public int goalScore;
    public TMP_Text scoreText;

    [Header("Win UI")]
    public GameObject winPanel;
    public TMP_Text winTitleText;
    public TMP_Text winSubtitleText;
    public TMP_Text scoreSummaryText;
    public TMP_Text nextLevelButtonText;
    public ParticleSystem winParticles;

    [Header("SFX")]
    public AudioClip collectSFX;
    public AudioClip winSFX;

    private AudioSource audioSource;
    private bool hasWon = false;
    private bool isLastLevel = false;

    void Start()
    {
        currentScore = 0;
        audioSource = GetComponent<AudioSource>();

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes  = SceneManager.sceneCountInBuildSettings;
        isLastLevel = currentIndex >= totalScenes - 1;

        if (winPanel != null) winPanel.SetActive(false);
        UpdateScoreText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible") && !hasWon)
        {
            currentScore++;
            other.gameObject.SetActive(false);

            if (collectSFX != null)
                audioSource.PlayOneShot(collectSFX);

            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScore + " / " + goalScore;

        if (currentScore >= goalScore && !hasWon)
        {
            hasWon = true;
            scoreText.text = "You Won!";

            if (winSFX != null)
                audioSource.PlayOneShot(winSFX);

            ShowWinPanel();
        }
    }

    private void ShowWinPanel()
    {
        if (winPanel == null) return;

        winPanel.SetActive(true);
        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;

        // Update score summary
        if (scoreSummaryText != null)
            scoreSummaryText.text = "Collectibles: " + currentScore + " / " + goalScore;

        // Different message and button label for last level vs regular level
        if (isLastLevel)
        {
            if (winTitleText != null)    winTitleText.text    = "YOU WIN!";
            if (winSubtitleText != null) winSubtitleText.text = "You've completed all levels!";
            if (nextLevelButtonText != null) nextLevelButtonText.text = "RETRY LEVEL";
        }
        else
        {
            if (winTitleText != null)    winTitleText.text    = "LEVEL COMPLETE";
            if (winSubtitleText != null) winSubtitleText.text = "Well done, keep rolling!";
            if (nextLevelButtonText != null) nextLevelButtonText.text = "NEXT LEVEL";
        }

        if (winParticles != null)
            winParticles.Play();
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(next-1); // last level = retry
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
