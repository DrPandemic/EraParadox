//
//  DrawableChampion.cs
//
//  Author:
//       Jesse <>
//
//  Copyright (c) 2013 Jesse
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
using GREATClient;
using GREATLib.Entities.Player.Champions;
using Microsoft.Xna.Framework;
using GREATLib.Entities.Physics;
using GREATLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATClient.GameContent
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : IDraw
    {
		/// <summary>
		/// Whether we should use entity interpolation or not.
		/// </summary>
		public static bool USE_INTERPOLATION = true;
		/// <summary>
		/// Whether we should use entity extrapolation or not.
		/// </summary>
		public static bool USE_EXTRAPOLATION = true;
		/// <summary>
		/// We interpolate how long it will take until the next position update. We take twice the amount of time to account the possibility
		/// of packet loss or jitter.
		/// This basically represents how far we are in the past we are when we are on the "current frame". So, with a value of 100ms,
		/// we draw everything 100ms late.
		/// </summary>
		public static TimeSpan INTERPOLATION_TIME = TimeSpan.FromMilliseconds(GameMatch.STATE_UPDATE_INTERVAL.TotalMilliseconds * 2.0);
		public static TimeSpan EXTRAPOLATION_TIME = TimeSpan.FromMilliseconds(250.0);
		public static bool SHOW_DEBUG_RECT = false;

		Vec2 CurrentPosition { get; set; }

		DrawableImage Idle { get; set; }
		DrawableSprite Run { get; set; }
		public IChampion Champion { get; set; }

		DrawableRectangle RealPositionDebugRect { get; set; }

		/// <summary>
		/// Gets or sets the position snapshots.
		/// These are the positions updates that we get from this played at different
		/// times. The key is the time when we receive an update (total game seconds), and the value is the
		/// position at this particular time.
		/// </summary>
		/// <value>The position snapshots.</value>
		List<KeyValuePair<double, Vec2>> PositionSnapshots { get; set; }

        public DrawableChampion(IChampion champion, ChampionsInfo championsInfo)
        {
			Champion = champion;
			PositionSnapshots = new List<KeyValuePair<double, Vec2>>();

			CurrentPosition = champion.Position;

			Idle = new DrawableImage(championsInfo.GetInfo(champion.Type).AssetName + "_stand");
			Run = new DrawableSprite(championsInfo.GetInfo(champion.Type).AssetName + "_run",
			                         34, 33, 0, 20, 6,DrawableSprite.INFINITE);
			RealPositionDebugRect = new DrawableRectangle(
				new Rectangle((int)Champion.Position.X, (int)Champion.Position.Y, (int)Champion.CollisionWidth, (int)Champion.CollisionHeight), 
				Color.Green);
			RealPositionDebugRect.Alpha = 0.5f;

			Idle.OriginRelative = Run.OriginRelative = RealPositionDebugRect.OriginRelative = 
				new Vector2(0.5f, 1f); // position at the feet

			Run.Visible = false;

			RealPositionDebugRect.Visible = SHOW_DEBUG_RECT;
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			Parent.AddChild(Idle);
			Parent.AddChild(Run);
			Parent.AddChild(RealPositionDebugRect);
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			Idle.Visible = (Champion.CurrentAnimation == Animation.Idle);
			Run.Visible = !Idle.Visible;

			if (Run.Visible) {
				if (Champion.IsOnGround)
					Run.Play();
				else
					Run.Stop();
			}

			UpdatePosition();

			Run.Position = Idle.Position = CurrentPosition.ToVector2();
			Run.FlipX = Idle.FlipX = Champion.FacingLeft;

			RealPositionDebugRect.Visible = SHOW_DEBUG_RECT;
		}

		/// <summary>
		/// Updates the position of the player, based on the last position snapshots
		/// that we received.
		/// </summary>
		void UpdatePosition()
		{
			if (USE_INTERPOLATION) {
				InterpolatePosition();
			} else {
				CurrentPosition = Champion.Position;
			}

			RealPositionDebugRect.Position = Champion.Position.ToVector2();
		}

		/// <summary>
		/// Interpolates the position of the player, keeping the player's position in
		/// the past and lerping towards the last snapshot that we received.
		/// This gives a smoother feeling to the gameplay. Packets are unpredictable
		/// and arrive at varying rates with a delay (the ping). Therefore, we have
		/// to go a couple of snapshots behind (depending on what is the interpolation
		/// delay) and interpolate towards a closer snapshot.
		/// </summary>
		/// <see cref="https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking#Entity_interpolation"/>
		void InterpolatePosition()
		{
			bool interpolationWorked = false;

			const int MIN_POSITIONS_REQUIRED = 2; // we need at least 2 snapshots for interpolation.

			if (PositionSnapshots.Count >= MIN_POSITIONS_REQUIRED) { // we can interpolate!
				// We first find the two snapshots that we'll use for interpolation (the one before and after our drawing time)
				double drawingTime = Client.Instance.GetTime().TotalSeconds - INTERPOLATION_TIME.TotalSeconds;
				int previousSnapshot = PositionSnapshots.FindLastIndex(snapshot => snapshot.Key <= drawingTime);
				int nextSnapshot = PositionSnapshots.FindIndex(snapshot => snapshot.Key > drawingTime);

				KeyValuePair<double, Vec2>? previous = previousSnapshot >= 0 ? PositionSnapshots[previousSnapshot] : new KeyValuePair<double, Vec2>?();
				KeyValuePair<double, Vec2>? next = nextSnapshot >= 0 ? PositionSnapshots[nextSnapshot] : new KeyValuePair<double, Vec2>?();

				if (previous.HasValue && next.HasValue) { // we have enough snapshots to have one before and after our drawing time
					Debug.Assert(previous.Value.Key <= drawingTime, "Previous snapshot newer than current time.");
					Debug.Assert(next.Value.Key > drawingTime, "Next snapshot older than current time.");
					Debug.Assert(previous.Value.Key < next.Value.Key, "Previous snapshot is newer than next one.");

					PositionSnapshots.RemoveRange(0, previousSnapshot); // we clean the snapshots that are too old now

					// see how far we are in the snapshot transition
					double progress = (drawingTime - previous.Value.Key) / (next.Value.Key - previous.Value.Key);

					// move from our old position (the previous snapshot) to our next snapshot (using the progress so far)
					CurrentPosition = previous.Value.Value + (next.Value.Value - previous.Value.Value) * progress;

					interpolationWorked = true;
				} else if (previous.HasValue && next.HasValue) { // we have a previous snapshot but no new one (lost packets?), extrapolate if we should
					if (USE_EXTRAPOLATION) {
						ILogger.Log("No new snapshot. Extrapolating position.", LogPriority.Low);
						ExtrapolatePosition();
					}
				}
			}

			if (!interpolationWorked) {
				CurrentPosition = Champion.Position; // directly pick the real position for now, we'll get valid snapshots soon.
				ILogger.Log("Insufficient snapshots for interpolation for now. Picking real position instead.", LogPriority.Warning);
			}
		}

		void ExtrapolatePosition()
		{
		}


		protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			// Run and Idle take care of that.
		}

		/// <summary>
		/// When we receive a new position snapshot, we keep it to interpolate between the positions.
		/// </summary>
		/// <param name="totalGameSeconds">Total game seconds since the start of the game.</param>
		public void OnPositionUpdate(double totalGameSeconds, Vec2 position)
		{
			PositionSnapshots.Add(new KeyValuePair<double, Vec2>(totalGameSeconds, position));
		}
    }
}
