﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Frostbyte
{
    class AudioManager
    {
        internal Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        internal Dictionary<string, SoundEffectInstance> loopingSoundEffects = new Dictionary<string, SoundEffectInstance>();
        internal Dictionary<string, Song> backgroundMusic = new Dictionary<string, Song>();

        ~AudioManager()
        {
            MediaPlayer.Stop();
        }

        internal void AddBackgroundMusic(string name)
        {
            if (!backgroundMusic.ContainsKey(name))
            {
                try
                {
                    backgroundMusic[name] = This.Game.Content.Load<Song>("Audio/" + name);
                }
                catch (NoAudioHardwareException)
                {
                    // Ignore it...
                }
            }
        }

        internal void PlayBackgroundMusic(string name)
        {
            Song s; 
            if (backgroundMusic.TryGetValue(name,out s))
            {
                try
                {
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(s);
                }
                catch (InvalidOperationException)
                {
                    MediaPlayer.Stop();
                }
            }
        }

        internal float BackgroundMusicVolume
        {
            get{
                return MediaPlayer.Volume;
            }
            set
            {
                MediaPlayer.Volume = value;
            }
        }

        internal void AddSoundEffect(string name)
        {
            if (!soundEffects.ContainsKey(name))
            {
                try
                {
                    soundEffects[name] = This.Game.Content.Load<SoundEffect>("Audio/" + name);
                }
                catch (NoAudioHardwareException)
                {
                    // Ignore it...
                }
            }
        }

        internal void PlaySoundEffect(string name, float volume=1f)
        {
            SoundEffect se;
            if (soundEffects.TryGetValue(name,out se))
            {
                var sound = se.CreateInstance();
                sound.Volume = volume;
                sound.Play();
            }
        }

        internal void InitializeLoopingSoundEffect(string name, float volume=1f)
        {
            SoundEffect se;
            if (soundEffects.TryGetValue(name, out se))
            {
                var sound = se.CreateInstance();
                sound.Volume = volume;
                sound.IsLooped = true;

                loopingSoundEffects[name] = sound;
            }
        }

        internal void StopAllLoopingSoundEffects()
        {
            foreach (SoundEffectInstance i in loopingSoundEffects.Values)
            {
                i.Pause();
            }
        }

        internal void StopLoopingSoundEffect(string MovementAudioName)
        {
            if (loopingSoundEffects.ContainsKey(MovementAudioName))
            {
                loopingSoundEffects[MovementAudioName].Pause();
            }
        }

        internal void PlayLoopingSoundEffect(string MovementAudioName)
        {
            if (loopingSoundEffects.ContainsKey(MovementAudioName) &&
                loopingSoundEffects[MovementAudioName].State != SoundState.Playing)
            {
                loopingSoundEffects[MovementAudioName].Play();
            }
        }

        internal void Pause()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
        }

        internal void Stop()
        {
            MediaPlayer.Stop();
        }
    }
}