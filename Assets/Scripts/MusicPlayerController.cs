using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayerController : MonoBehaviour {

  public List<AudioClip> musicTracks = new List<AudioClip>();
  public ToggleButtonController playPauseButton;
  public ToggleButtonController shuffleButton;
  public Text trackInfo;
  public float secondsToRestartTrack = 5f;

  private AudioSource _musicSource;
  private AudioClip _currentTrack;
  private bool _isShuffleMode;

  private bool _isPlaying;
  private Coroutine _autoPlay;

  void Start() {
    _musicSource = GetComponent<AudioSource>();
  }

  public void OnPlayPauseButton() {
    if(_isPlaying) {
      PauseMusic();
    } else {
      PlayTrack();
    }
    if(playPauseButton != null) {
      playPauseButton.UpdateState(_isPlaying);
    }
  }

  public void OnNextTrackButton() {
    _currentTrack = GetNextTrack(_currentTrack);
    if(_isPlaying) {
      PlayTrack();
    }
  }

  public void OnPreviousTrackButton() {
    if(IsPlayedEnoughToRestart()) {
      RestartCurrentTrack();
    } else {
      _currentTrack = GetPreviousTrack(_currentTrack);
      if(_isPlaying) {
        PlayTrack();
      }
    }
  }

  public void OnShuffleButton() {
    _isShuffleMode = !_isShuffleMode;
    if(shuffleButton != null) {
      shuffleButton.ToggleState();
    }
  }

  private void PlayTrack() {
    if(_currentTrack == null) {
      _currentTrack = GetNextTrack(_currentTrack);
    }
    PlayTrack(_currentTrack);
  }

  private void RestartCurrentTrack() {
    if(_musicSource != null) {
      _musicSource.Stop();
      PlayTrack(_currentTrack);
    }
  }

  private bool IsPlayedEnoughToRestart() {
    bool result = false;
    if(_musicSource != null) {
      if(_musicSource.time > secondsToRestartTrack) {
        Debug.Log(_musicSource.time);
        result = true;
      }
    }
    return result;
  }

  private AudioClip GetNextTrack(AudioClip currentTrack) {
    if(musicTracks == null || musicTracks.Count == 0) {
      return null;
    }
    int index = GetTrackIndex(currentTrack, true);
    return musicTracks[index];
  }

  private AudioClip GetPreviousTrack(AudioClip currentTrack) {
    if(musicTracks == null || musicTracks.Count == 0) {
      return null;
    }
    int index = GetTrackIndex(currentTrack, false);
    return musicTracks[index];
  }

  private int GetTrackIndex(AudioClip currentTrack, bool isNextTrackIndexRequired) {
    int result = 0;
    if(currentTrack != null) {
      int currentTrackIndex = musicTracks.IndexOf(currentTrack);
      if(_isShuffleMode) {
        do {
          result = UnityEngine.Random.Range(0, musicTracks.Count);
        } while (result == currentTrackIndex);
      } else if(isNextTrackIndexRequired) {
        result = currentTrackIndex + 1 < musicTracks.Count ? currentTrackIndex + 1 : 0;
      } else {
        result = currentTrackIndex > 0 ? currentTrackIndex - 1 : 0;
      }
    }
    return result;
  }

  private void PlayTrack(AudioClip track) {
    if(_musicSource != null) {
      if(_musicSource.clip != track) {
        _musicSource.Stop();
        _musicSource.clip = track;
        _musicSource.Play();
      } else {
        _musicSource.Play();
      }
      _isPlaying = true;
      UpdateTrackInfo(track);
      if(_autoPlay != null) {
        StopCoroutine(_autoPlay);
      }
      _autoPlay = StartCoroutine(AutoPlayNextTrack(_currentTrack));
    }
  }

  IEnumerator AutoPlayNextTrack(AudioClip currentTrack) {
    yield return new WaitForSeconds(currentTrack.length - _musicSource.time);
    OnNextTrackButton();
  }

  private void UpdateTrackInfo(AudioClip track) {
    if(trackInfo != null) {
      String trackName = track.name;
      int trackIndex = musicTracks != null ? musicTracks.IndexOf(track) : 0;
      trackInfo.text = trackIndex + ". " + trackName;
    }
  }

  private void PauseMusic() {
    if(_musicSource != null) {
      _musicSource.Pause();
      _isPlaying = false;
      if(_autoPlay != null) {
        StopCoroutine(_autoPlay);
      }
    }
  }
}
