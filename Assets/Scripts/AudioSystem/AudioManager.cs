using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AudioManager : MonoSingletonHungry<AudioManager>   //使用单例模板：有挂载的饿汉单例，在游戏开始时创建物体并挂载脚本
{
    
    //音量控制变量
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float bgmVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    
    // 音频源引用
    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private GameObject audioSourcesContainer;
    
    // 配置参数
    public int initialAudioPoolSize = 10;
    
    // 当前播放的BGM
    private AudioClip currentBGM;
    
    // 初始化音频管理器
    private void Initialize()
    {
        // 创建音频源容器
        audioSourcesContainer = new GameObject("AudioSources");
        audioSourcesContainer.transform.SetParent(transform);
        
        // 创建专用BGM音频源
        GameObject bgmObject = new GameObject("BGMSource");
        bgmObject.transform.SetParent(audioSourcesContainer.transform);
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        
        // 初始化音频池
        for (int i = 0; i < initialAudioPoolSize; i++)
        {
            CreateNewSFXSource();
        }
        
        // 应用初始音量设置
        UpdateAllVolumes();
    }
    
    // 创建新的SFX音频源
    private AudioSource CreateNewSFXSource()
    {
        GameObject sfxObject = new GameObject("SFXSource_" + sfxSources.Count);
        sfxObject.transform.SetParent(audioSourcesContainer.transform);
        AudioSource source = sfxObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        sfxSources.Add(source);
        return source;
    }
    
    // 更新所有音频源的音量
    private void UpdateAllVolumes()
    {
        // 更新BGM音量
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
        
        // 更新所有SFX音量
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }
    }
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clip">音频剪辑</param>
    /// <param name="fadeDuration">淡入淡出时间(秒)</param>
    public void PlayBGM(AudioClip clip, float fadeDuration = 0.5f)
    {
        if (clip == null) return;
        if (currentBGM == clip) return;
        
        currentBGM = clip;
        StartCoroutine(CrossFadeBGM(clip, fadeDuration));
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    /// <param name="fadeDuration">淡出时间(秒)</param>
    public void StopBGM(float fadeDuration = 0.5f)
    {
        StartCoroutine(FadeOutBGM(fadeDuration));
    }
    

    // 播放音效
    /// <param name="clip">音频剪辑</param>
    /// <param name="volumeScale">音量缩放(0-1)</param>
    /// <param name="pitch">音调调整</param>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        
        // 查找可用的音频源
        AudioSource availableSource = sfxSources.Find(source => !source.isPlaying);
        
        // 如果没有可用源，创建新的
        if (availableSource == null)
        {
            availableSource = CreateNewSFXSource();
        }
        
        // 设置音频源属性
        availableSource.clip = clip;
        availableSource.volume = sfxVolume * masterVolume * volumeScale;
        availableSource.pitch = pitch;
        availableSource.Play();
    }
    
    /// 停止所有音效
    public void StopAllSFX()
    {
        foreach (AudioSource source in sfxSources)
        {
            source.Stop();
        }
    }
    
    /// 设置主音量
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    /// 设置背景音乐音量
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    /// 设置音效音量
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    /// 暂停所有音频
    public void PauseAll()
    {
        bgmSource.Pause();
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying)
            {
                source.Pause();
            }
        }
    }
    
    /// 恢复所有音频
    public void ResumeAll()
    {
        bgmSource.UnPause();
        foreach (AudioSource source in sfxSources)
        {
            if (source.clip != null && !source.isPlaying)
            {
                source.UnPause();
            }
        }
    }
    
    // BGM淡入淡出协程
    private IEnumerator CrossFadeBGM(AudioClip newClip, float fadeDuration)
    {
        // 如果当前有BGM正在播放，先淡出
        if (bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutBGM(fadeDuration / 2));
        }
        
        // 设置新BGM并淡入
        bgmSource.clip = newClip;
        bgmSource.Play();
        yield return StartCoroutine(FadeInBGM(fadeDuration));
    }
     
    // BGM淡出协程
    private IEnumerator FadeOutBGM(float fadeDuration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0f;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }
        
        bgmSource.Stop();
        bgmSource.volume = bgmVolume * masterVolume; // 重置音量
    }
    
    // BGM淡入协程
    private IEnumerator FadeInBGM(float fadeDuration)
    {
        float targetVolume = bgmVolume * masterVolume;
        float timer = 0f;
        
        bgmSource.volume = 0f;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }
    }
    

}
