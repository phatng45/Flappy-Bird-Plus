using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private       int         level = 1;

    float[] xSpikeRange = { -2.87f, 2.84f };
    float[] ySpikeRange = { 3.77f, 3.037f, 2.304f, 1.571f, 0.838f, 0.105f, -0.628f, -1.361f, -2.094f, -2.827f, -3.56f };

    // WIDER SPIKES, MAKE 1 HOLE GAP EASIER TO GET THROUGH BUT HARDER EVERYWHERE ELSE
    //float[] ySpikeRange = { 3.77f, 3.037f, 1.77f, 0.77f, -0.77f, -1.77f, -2.77f, -3.77f };

    float[] xSawRange = { -3.1f, 3.07f };


    void Awake() {
        Instance = this;
    }

    [SerializeField] private GameObject  player,    spike, star, saw, mace;
    [SerializeField] private AudioSource loseSound, winSound;

    // FOR DIFFICULTY SETTINGS
    private int[]
        _minSpikes,
        _maxSpikes,
        _minSaws,
        _maxSaws,
        _minMaces,
        _maxMaces;

    private bool isCasual = false;

    private readonly int[]
        _minSpikesCasual   = { 0, 2, 3, 4, 4, 8, 5, 5, 5, 7, 10 },
        _maxSpikesCasual   = { 0, 5, 6, 7, 9, 16, 11, 11, 11, 13, 16 },
        _minSawsCasual     = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        _maxSawsCasual     = { 0, 0, 1, 1, 1, 0, 1, 1, 2, 2, 2 },
        _minMacesCasual    = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        _maxMacesCasual    = { 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1 },
        _minSpikesHardcore = { 0, 15, 16, 20, 16, 25, 20, 22, 22, 22, 22 },
        _maxSpikesHardcore = { 0, 15, 16, 20, 16, 25, 20, 22, 22, 22, 22 },
        _minSawsHardcore   = { 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1 },
        _maxSawsHardcore   = { 0, 0, 1, 0, 1, 0, 2, 2, 3, 3, 5 },
        _minMacesHardcore  = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
        _maxMacesHardcore  = { 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1 };

    public GameState State;
    public static event Action<GameState> onGameStateChanged;

    private void Start() {
        SwapDifficulty();
        UpdateGameState(GameState.MainScreen);
    }

    public void SwapDifficulty() {
        isCasual = !isCasual;
        if (isCasual) {
            _minSpikes = _minSpikesCasual;
            _maxSpikes = _maxSpikesCasual;
            _minSaws   = _minSawsCasual;
            _maxSaws   = _maxSawsCasual;
            _minMaces  = _minMacesCasual;
            _maxMaces  = _maxMacesCasual;
        }
        else {
            _minSpikes = _minSpikesHardcore;
            _maxSpikes = _maxSpikesHardcore;
            _minSaws   = _minSawsHardcore;
            _maxSaws   = _maxSawsHardcore;
            _minMaces  = _minMacesHardcore;
            _maxMaces  = _maxMacesHardcore;
        }
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

        var scale = level < 10 ? level : 10;

        int numberOfSpikes = Random.Range(_minSpikes[scale], _maxSpikes[scale] + 1);
        int numberOfSaws   = Random.Range(_minSaws[scale], _maxSaws[scale] + 1);
        int numberOfMaces  = Random.Range(_minMaces[scale], _maxMaces[scale] + 1);

        // SPAWN SPIKES
        for (int i = 0; i < numberOfSpikes; ++i) {
            float x        = xSpikeRange[Random.Range(0, 2)];
            float rotation = x == -2.87f ? -90f : 90f;
            float y        = ySpikeRange[Random.Range(0, ySpikeRange.Length)];
            Instantiate(spike, new Vector2(x, y), transform.rotation * Quaternion.Euler(0f, 0, rotation));
        }

        // SPAWN SAWS
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
            var starPosition = new Vector2(Random.Range(-2.33f, 2.3f), Random.Range(3.77f, -3.77f));
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


        // AT LEVEL 10:
        // minSpikes = 10;
        // maxSpikes = 16;
        // minSaws = 1;
        // maxSaws = 2;
        // minMaces = 1;
        // maxMaces = 2;
    }


    public void ResetLevel() {
        level = 1;
    }
}

public enum GameState {
    MainScreen,
    Play,
    Lose,
    Win
}