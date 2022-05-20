using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private       int         level = 1;

    void Awake() {
        Instance = this;
    }

    [SerializeField] private GameObject  player, spike, star, saw, mace;
    [SerializeField] private AudioSource loseSound, winSound;
    
    // FOR DIFFICULTY SETTINGS
    private int
        _minSpikes = 2,
        _maxSpikes = 5,
        _minSaws   = 0,
        _maxSaws   = 1,
        _minMaces  = 0,
        _maxMaces  = 1;

    public GameState State;
    public static event Action<GameState> onGameStateChanged;

    private void Start() {
        UpdateGameState(GameState.MainScreen);
    }

    public void UpdateGameState(GameState newState) {
        State = newState;

        switch (newState) {
            case GameState.MainScreen:
                break;
            case GameState.Play:
                Init();
                break;
            case GameState.Lose:
                loseSound.Play();
                DestroyElements();
                UpdateLevelText();
                break;
            case GameState.Win:
                winSound.Play();
                DestroyElements();
                UpdateLevelText();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        onGameStateChanged?.Invoke(newState);
    }

    private void UpdateLevelText() {
        // UPDATE LEVEL TEXT
        MenuManager.Instance.UpdateLevelText(level);
    }

    private void Init() {
        // SPAWN OBSTACLES HERE
        player.SetActive(true);
        player.transform.position = Vector3.zero;

        int numberOfSpikes = Random.Range(_minSpikes, _maxSpikes + 1);
        int numberOfSaws   = Random.Range(_minSaws, _maxSaws + 1);
        int numberOfMaces  = Random.Range(_minMaces, _maxMaces + 1);

        // SPAWN SPIKES
        float[] xSpikeRange = { -2.87f, 2.84f };
        float[] ySpikeRange = { 3.77f, 2.77f, 1.77f, 0.77f, -0.77f, -1.77f, -2.77f, -3.77f };

        for (int i = 0; i < numberOfSpikes; ++i) {
            float x        = xSpikeRange[Random.Range(0, 2)];
            float rotation = x == -2.87f ? -90f : 90f;
            float y        = ySpikeRange[Random.Range(0, 8)];
            Instantiate(spike, new Vector2(x, y), transform.rotation * Quaternion.Euler(0f, 0, rotation));
        }
        
        // SPAWN SAWS
        float[] xSawRange = { -3.1f, 3.07f };
        switch (numberOfSaws) {
            case 1:
                Instantiate(saw, new Vector2(xSawRange[Random.Range(0, 2)], Random.Range(-3.6f, 3.6f)),
                    Quaternion.identity);
                break;
            case 2:
                Instantiate(saw, new Vector2(xSawRange[0], Random.Range(-3.6f, 3.6f)), Quaternion.identity);
                Instantiate(saw, new Vector2(xSawRange[1], Random.Range(-3.6f, 3.6f)), Quaternion.identity);
                break;
        }

        // SPAWN MACES
        for (int i = 0; i < numberOfMaces; ++i) {
            Instantiate(mace, new Vector2(Random.Range(-2.09f, 2.13f), 5.38f),
                transform.rotation * Quaternion.Euler(0f, 0, Random.Range(-10f, 10f)));
        }
        
        // SPAWN STARS
        for (int i = 0; i < 3; ++i) {
            var starPosition = new Vector2(Random.Range(-2.48f, 2.45f), Random.Range(3.77f, -3.77f));
            Instantiate(star, starPosition, Quaternion.identity);
        }
    }

    public void DestroyElements() {
        player.SetActive(false);

        // DESTROY OBSTACLES HERE
        var allSpikeClones = GameObject.FindGameObjectsWithTag("Spike");
        foreach (var spikeClone in allSpikeClones) {
            Destroy(spikeClone);
        }

        var allStarClones = GameObject.FindGameObjectsWithTag("Star");
        foreach (var starClone in allStarClones) {
            Destroy(starClone);
        }
    }

    public void IncreaseLevel() {
        level++;

        switch (level) {
            case 2:
                _minSpikes = 3;
                _maxSpikes = 6;
                _maxSaws   = 2;
                break;
            case 3:
                _minSpikes = 4;
                _maxSpikes = 7;
                break;
            case 4:
                _minSpikes = 4;
                _maxSpikes = 9;
                _maxMaces  = 2;
                break;
            case 5:
                _minSpikes = 5;
                _maxSpikes = 11;
                _minSaws   = 1;
                break;
            case 7:
                _maxSpikes = 11;
                _minMaces  = 1;
                break;
            case 9:
                _minSpikes = 7;
                _maxSpikes = 13;
                break;
            case 10:
                _minSpikes = 10;
                _maxSpikes = 16;
                break;

            // AT LEVEL 10:
            // minSpikes = 10;
            // maxSpikes = 16;
            // minSaws = 1;
            // maxSaws = 2;
            // minMaces = 1;
            // maxMaces = 2;
        }
    }

    public void ResetLevel() {
        level      = 1;
        _minSpikes = 2;
        _maxSpikes = 5;
        _minSaws   = 0;
        _maxSaws   = 1;
        _minMaces  = 0;
        _maxMaces  = 1;
    }
}

public enum GameState {
    MainScreen,
    Play,
    Lose,
    Win
}