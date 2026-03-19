using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;

    [Header("SFX")]
    public AudioClip buttonClickSFX;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    void PlayClick()
    {
        if (buttonClickSFX != null)
            audioSource.PlayOneShot(buttonClickSFX);
    }

    public void OnPlayPressed()     { PlayClick(); SceneManager.LoadScene("Level1"); }
    public void OnLevelSelectPressed() { PlayClick(); mainMenuPanel.SetActive(false); levelSelectPanel.SetActive(true); }
    public void OnQuitPressed()
    {
        PlayClick();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void OnLevel1Pressed()   { PlayClick(); SceneManager.LoadScene("Level1"); }
    public void OnLevel2Pressed()   { PlayClick(); SceneManager.LoadScene("Level2"); }
    public void OnLevel3Pressed()   { PlayClick(); SceneManager.LoadScene("Level3"); }

    public void OnBackPressed()     { PlayClick(); levelSelectPanel.SetActive(false); mainMenuPanel.SetActive(true); }
}
