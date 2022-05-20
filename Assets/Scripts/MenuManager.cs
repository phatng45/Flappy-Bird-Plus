using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    [SerializeField] private GameObject      
        LosePanel, WinPanel, MenuPanel, CasualDiff, HardcoreDiff, warningText, xdText;
    [SerializeField] private TextMeshProUGUI LoseLevel, WinLevel;
    [SerializeField] private AudioSource     btnClickedSound;
    
    public static            MenuManager Instance;
    
    private void Awake() {
        GameManager.onGameStateChanged += GameManagerOnGameStateChanged;
        Instance                       =  this;
    }

    private void OnDestroy() {
        GameManager.onGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state) {
        MenuPanel.SetActive(state == GameState.MainScreen);
        LosePanel.SetActive(state == GameState.Lose);
        WinPanel.SetActive(state == GameState.Win);
    }
    
    public void OnHomePressed() {
        btnClickedSound.Play();
        GameManager.Instance.UpdateGameState(GameState.MainScreen);
    }
    
    public void OnPlayPressed() {
        btnClickedSound.Play();
        GameManager.Instance.ResetLevel();
        GameManager.Instance.UpdateGameState(GameState.Play);
    }
    
    public void OnRetryPressed() {
        btnClickedSound.Play();
        GameManager.Instance.ResetLevel();
        GameManager.Instance.UpdateGameState(GameState.Play);
    }

    public void OnNextLevelPressed() {
        btnClickedSound.Play();
        GameManager.Instance.IncreaseLevel();
        GameManager.Instance.UpdateGameState(GameState.Play);
    }

    public void UpdateLevelText(int level) {
        LoseLevel.text = "level: " + level;
        WinLevel.text  = "level: " + level;
        LoseLevel.ForceMeshUpdate(true);
        WinLevel.ForceMeshUpdate(true);
    }
    
    public void onExitPressed() {
        Application.Quit();
    }

    public void onDifficultyPressed() {
        btnClickedSound.Play();
        GameManager.Instance.SwapDifficulty();
        CasualDiff.SetActive(!CasualDiff.activeSelf);
        HardcoreDiff.SetActive(!HardcoreDiff.activeSelf);
        warningText.SetActive(!warningText.activeSelf);
        xdText.SetActive(!warningText.activeSelf);
    }
}