//
//  SoundManager.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using GREATClient.BaseClass.ScreenInformation;
using GameContent;
using Microsoft.Xna.Framework;

namespace GREATClient.BaseClass
{
	public enum Sounds
	{
		Won,
		Lost,

		YouDied,
		YouKilled,
		AllyDied,
		EnemyDied,

		ManMega_1,
		ManMega_2,
		ManMega_3,
		ManMega_4,
		ManMega_Revive,
		ManMega_Death,

		Zero_1,
		Zero_2,
		Zero_3,
		Zero_4,
		Zero_Revive,
		Zero_Death,

		LeftTowerShot,
		RightTowerShot,
		Explosion,

		OpenMenu,
		CloseMenu
	}
	public static class SoundsHelper
	{
		public static string GetSoundPath(Sounds sound)
		{
			switch (sound) {
				case Sounds.Won: return "Sounds/Effects/won";
				case Sounds.Lost: return "Sounds/Effects/lost";

				case Sounds.YouDied: return "Sounds/Effects/youdied";
				case Sounds.YouKilled: return "Sounds/Effects/youkilled";
				case Sounds.AllyDied: return "Sounds/Effects/allydied";
				case Sounds.EnemyDied: return "Sounds/Effects/enemydied";

				case Sounds.ManMega_1: return "Sounds/Effects/manmega_1";
				case Sounds.ManMega_2: return "Sounds/Effects/manmega_2";
				case Sounds.ManMega_3: return "Sounds/Effects/manmega_3";
				case Sounds.ManMega_4: return "Sounds/Effects/manmega_4";
				case Sounds.ManMega_Revive: return "Sounds/Effects/manmega_revive";
				case Sounds.ManMega_Death: return "Sounds/Effects/manmega_death";

				case Sounds.Zero_1: return "Sounds/Effects/zero_1";
				case Sounds.Zero_2: return "Sounds/Effects/zero_2";
				case Sounds.Zero_3: return "Sounds/Effects/zero_3";
				case Sounds.Zero_4: return "Sounds/Effects/zero_4";
				case Sounds.Zero_Revive: return "Sounds/Effects/zero_revive";
				case Sounds.Zero_Death: return "Sounds/Effects/zero_death";

				case Sounds.LeftTowerShot: return "Sounds/Effects/lefttowershot";
				case Sounds.RightTowerShot: return "Sounds/Effects/righttowershot";
				case Sounds.Explosion: return "Sounds/Effects/explosion";

				case Sounds.OpenMenu: return "Sounds/Effects/openmenu";
				case Sounds.CloseMenu: return "Sounds/Effects/closemenu";
			}

			throw new NotImplementedException();
		}
	}

    public class SoundService
    {
		private ContentManager Content { get; set; }

		public CameraService CameraService { get; set; }

		// The music player isn't working well on Windows, so we are doing a music
		// player with sound effects.
		SoundEffectInstance MusicPlayer { get; set; }

		public SoundService(ContentManager content) {
			MediaPlayer.IsRepeating = true;
			Content = content;
			CameraService = null;
			MusicPlayer = null;
        }
		public void StopMusic() {
			if(MusicPlayer != null) {
				MusicPlayer.Stop();
			}
		}
		public void PauseMusic() {
			if(MusicPlayer != null) {
				MusicPlayer.Pause();
			}
		}
		public void ResumeMusic() {
			if(MusicPlayer != null) {
				MusicPlayer.Resume();
			}
		}
		/// <summary>
		/// Plaies the music.
		/// By doing so, it remove all queued music.
		/// </summary>
		/// <param name="musicName">Music name.</param>
		public void PlayMusic(string musicName) {
			MusicPlayer = Content.Load<SoundEffect>(musicName).CreateInstance();
			MusicPlayer.Volume = 0.45f;
			MusicPlayer.Pan = 0f;
			MusicPlayer.Pitch = 0f;
			MusicPlayer.Play();

			MusicPlayer.IsLooped = true;
		}
		/*public void QueueMusics(params string[] list) {
			SongCollection collection = new SongCollection();
			foreach (string song in list) {
				collection.Add(Content.Load<Song>(song));
			}
			MediaPlayer.Play(collection);
		}*/
		public void ChangeMusicVolume(float volume) {
			if(MusicPlayer != null) {
				MusicPlayer.Volume = volume;
			}
		}
		public void PlaySound(string soundName, float screenWidth, float screenHeight, Vector2? soundSource = null) {
			SoundEffect effect = Content.Load<SoundEffect>(soundName);
			if (soundSource == null) {
				effect.Play(1f, 0f, 0f);
			} else {
				// f(x) = -x/2000 + 1
				Vector2 target = GameLibHelper.ToVector2(CameraService.GetTarget(screenWidth, screenHeight));
				float volume = - (float)Math.Sqrt((target.X-soundSource.Value.X) * (target.X-soundSource.Value.X) + 
				                                  (target.Y-soundSource.Value.Y) * (target.Y-soundSource.Value.Y)) /1000 + 1;
				volume = Math.Max(volume,0);

				effect.Play(volume, 0f, 0f);
			}
		}
    }
}

