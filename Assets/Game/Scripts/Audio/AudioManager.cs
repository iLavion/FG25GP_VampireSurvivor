using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect {
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    [SerializeField] private int mainMenuSceneIndex = 0;
    [Header("Background Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField, Range(0f, 1f)] private float menuMusicVolume = 0.7f;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField, Range(0f, 1f)] private float gameMusicVolume = 0.7f;
    [Header("Sound Effects")]
    [SerializeField] private List<SoundEffect> soundEffects = new();
    private float masterVolume = 1f;
    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new();
    private const int SFX_POOL_SIZE = 8;
    private void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAudioSources();
    }
    private void Start() { SceneManager.sceneLoaded += OnSceneLoaded; PlayMusicForCurrentScene(); }
    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => PlayMusicForScene(scene.buildIndex);
    private void PlayMusicForCurrentScene() => PlayMusicForScene(SceneManager.GetActiveScene().buildIndex);
    private void PlayMusicForScene(int sceneIndex) {
        if (sceneIndex == mainMenuSceneIndex) PlayMenuMusic();
        else PlayGameMusic();
    }
    private void InitializeAudioSources() {
        GameObject bgmObj = new("BGM");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        for (int i = 0; i < SFX_POOL_SIZE; i++) {
            GameObject sfxObj = new($"SFX_{i}");
            sfxObj.transform.SetParent(transform);
            AudioSource sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSources.Add(sfxSource);
        }
    }
    public void PlayMenuMusic() => PlayBackgroundMusic(menuMusic, menuMusicVolume);
    public void PlayGameMusic() {
        if (gameMusic != null) PlayBackgroundMusic(gameMusic, gameMusicVolume);
        else StopMusic();
    }
    public void StopMusic() { if (bgmSource != null) bgmSource.Stop(); }
    public void PlaySoundEffect(string effectName) {
        SoundEffect effect = soundEffects.Find(s => s.name == effectName);
        if (effect == null || effect.clip == null) return;
        AudioSource source = GetAvailableSFXSource();
        if (source != null) {
            source.clip = effect.clip;
            source.volume = effect.volume * masterVolume;
            source.pitch = 1f;
            source.Play();
        }
    }
    public void SetMasterVolume(float volume) {
        masterVolume = Mathf.Clamp01(volume);
        if (bgmSource != null && bgmSource.isPlaying) bgmSource.volume = GetCurrentMusicVolume() * masterVolume;
    }
    private float GetCurrentMusicVolume() {
        if (bgmSource == null || bgmSource.clip == null) return 0.7f;
        if (bgmSource.clip == menuMusic) return menuMusicVolume;
        if (bgmSource.clip == gameMusic) return gameMusicVolume;
        return 0.7f;
    }
    private void PlayBackgroundMusic(AudioClip clip, float volume) {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.volume = volume * masterVolume;
        bgmSource.Play();
    }
    private AudioSource GetAvailableSFXSource() {
        foreach (var source in sfxSources) if (!source.isPlaying) return source;
        return sfxSources[0];
    }
}
