using UnityEngine;

// Gère la musique d'ambiance du jeu
// Persiste entre les scènes (DontDestroyOnLoad)
// PATTERN SINGLETON :
// Une seule instance existe dans tout le jeu
// Accessible via AudioManager.Instance depuis n'importe où
// - Joue la musique d'ambiance en boucle
// - Persiste entre les changements de scène
// - Contrôle du volume
// - Pause/Resume/Stop de la musique

public class AudioManager : MonoBehaviour
{
    // PATTERN SINGLETON
    // "Instance" est accessible depuis n'importe où dans le code
    // Ex : AudioManager.Instance.SetMusicVolume(0.5f);
    public static AudioManager Instance { get; private set; }

    [Header("Musique")]
    [SerializeField] private AudioClip backgroundMusic; // Le fichier audio MP3/OGG
    [SerializeField] private float musicVolume = 0.5f;  // Volume (0 = silence, 1 = max)

    private AudioSource musicSource; // Composant Unity qui joue l'audio

    // Awake : Initialisation (avant Start)
    // 1. Vérifie si une instance existe déjà
    // 2. Si oui : détruit ce nouvel objet (évite les doublons)
    // 3. Si non : marque cet objet comme Instance et le rend persistant
    // Sans DontDestroyOnLoad, l'AudioManager serait détruit à chaque
    // changement de scène et la musique s'arrêterait
    // Avec DontDestroyOnLoad, la musique continue entre les niveaux
    void Awake()
    {
        // Si une instance existe DÉJÀ et que ce n'est pas cet objet
        if (Instance != null && Instance != this)
        {
            // Détruit ce nouvel objet (évite d'avoir 2 AudioManagers)
            Destroy(gameObject);
            return; // Arrête l'exécution du reste du code
        }

        // Sinon, cet objet DEVIENT l'instance unique
        Instance = this;

        // DontDestroyOnLoad : cet objet persiste entre les scènes
        // Normalement, tous les objets sont détruits quand on change de scène
        // Cette commande fait exception
        DontDestroyOnLoad(gameObject);

        // Configure les AudioSource
        SetupAudioSources();

        // Lance la musique
        PlayBackgroundMusic();
    }

    // SetupAudioSources : Crée et configure l'AudioSource
    // AddComponent crée un nouveau composant sur ce GameObject
    // Ensuite on configure ses propriétés
    // On crée l'AudioSource par code plutôt que de l'ajouter manuellement
    // C'est plus flexible et garantit que les paramètres sont corrects
    private void SetupAudioSources()
    {
        // Crée un composant AudioSource sur ce GameObject
        musicSource = gameObject.AddComponent<AudioSource>();

        // Configure les propriétés
        musicSource.loop = true;           // Boucle infinie (recommence quand terminé)
        musicSource.playOnAwake = false;   // Ne joue PAS automatiquement au démarrage
        musicSource.volume = musicVolume;  // Définit le volume initial
    }

    // PlayBackgroundMusic : Lance la musique d'ambiance
    // Vérifie qu'un clip est assigné et que la musique n'est pas déjà en cours
    private void PlayBackgroundMusic()
    {
        // Vérifie qu'un clip audio est assigné ET que la musique ne joue pas déjà
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic; // Assigne le fichier audio
            musicSource.Play();                 // Lance la lecture
        }
    }

    // SetMusicVolume : Change le volume de la musique
    // Mathf.Clamp01 force la valeur entre 0 et 1
    // Même si on passe -5 ou 3, ça sera automatiquement corrigé
    // C'est une sécurité pour éviter les valeurs invalides
    public void SetMusicVolume(float volume)
    {
        // Clamp01 = limite entre 0 et 1
        // Clamp01(-0.5) = 0
        // Clamp01(1.5) = 1
        // Clamp01(0.7) = 0.7
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume; // Applique le nouveau volume
        }
    }

    // StopMusic : Arrête complètement la musique
    // DIFFÉRENCE avec Pause :
    // - Stop : remet la lecture au début
    // - Pause : garde la position actuelle
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop(); // Arrête et remet au début
        }
    }

    // PauseMusic : Met la musique en pause
    // La position de lecture est conservée
    // Utilisé typiquement pour un menu pause
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause(); // Met en pause
        }
    }

    // ResumeMusic : Reprend la musique après une pause
    // Continue depuis la position où Pause() a été appelé
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause(); // Reprend la lecture
        }
    }
}
