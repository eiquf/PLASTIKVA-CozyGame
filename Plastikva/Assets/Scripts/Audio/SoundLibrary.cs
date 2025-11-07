using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [System.Serializable]
    public struct SoundEntry
    {
        public GameSound sound;
        public AudioClip clip;
    }

    [SerializeField] private SoundEntry[] _sounds;

    private readonly Dictionary<GameSound, AudioClip> _map = new();

    private bool _initialized = false;

    public void Initialize()
    {
        if (_initialized) return;

        _map.Clear();
        foreach (var entry in _sounds)
        {
            if (!_map.ContainsKey(entry.sound))
                _map.Add(entry.sound, entry.clip);
        }

        _initialized = true;
    }

    public AudioClip GetClip(GameSound sound)
    {
        if (!_initialized) Initialize();

        return _map.TryGetValue(sound, out var clip) ? clip : null;
    }
    private void OnValidate() => _initialized = false;
#if UNITY_EDITOR
    [ContextMenu("Debug/Print Contents")]
    private void DebugPrint()
    {
        Initialize();
        foreach (var kv in _map)
            Debug.Log($"[{name}] {kv.Key} -> {kv.Value}");
    }
#endif

}
