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
    public class SoundService
    {
		private ContentManager Content { get; set; }

		public CameraService CameraService { get; set; }

		Vector2 TmpCameraPos { get; set; }

		public SoundService(ContentManager content) {
			MediaPlayer.IsRepeating = true;
			Content = content;
			CameraService = null;
			TmpCameraPos = new Vector2(5,5);
        }
		public void StopMusic() {
			MediaPlayer.Stop();
		}
		public void PauseMusic() {
			MediaPlayer.Pause();
		}
		public void ResumeMusic() {
			MediaPlayer.Resume();
		}
		/// <summary>
		/// Plaies the music.
		/// By doing so, it remove all queued music.
		/// </summary>
		/// <param name="musicName">Music name.</param>
		public void PlayMusic(string musicName) {
			MediaPlayer.Play(Content.Load<Song>(musicName));
		}
		public void QueuMusics(params string[] list) {
			SongCollection collection = new SongCollection();
			foreach (string song in list) {
				collection.Add(Content.Load<Song>(song));
			}
			MediaPlayer.Play(collection);
		}
		public void ChangeMusicVolume(float volume) {
			MediaPlayer.Volume = volume;
		}
		public void PlaySound(string soundName, Vector2? soundSource = null) {
			SoundEffect effect = Content.Load<SoundEffect>(soundName);
			if (soundSource == null) {
				effect.Play(1f, 0f, 0f);
			} else {
				// f(x) = -x/2000 + 1
				float volume = - (float)Math.Sqrt((TmpCameraPos.X-soundSource.Value.X) * (TmpCameraPos.X-soundSource.Value.X) + 
				                           (TmpCameraPos.Y-soundSource.Value.Y) * (TmpCameraPos.Y-soundSource.Value.Y)) /2000 + 1;
				volume = Math.Max(volume,0);

				effect.Play(volume, 0f, 0f);
			}
		}
    }
}

