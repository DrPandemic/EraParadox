//
//  DrawableTower.cs
//
//  Author:
//       HPSETUP3 <${AuthorEmail}>
//
//  Copyright (c) 2013 HPSETUP3
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
using GREATLib.Entities;
using GREATLib.Entities.Structures;
using GREATClient.BaseClass;
using Microsoft.Xna.Framework;
using GREATLib;
using GREATClient.BaseClass.Particle;
using GREATClient.BaseClass.BaseAction;

namespace GREATClient.GameContent
{
    public class DrawableTower : DrawableStructure
    {
		/// <summary>
		/// The alert duration in ms.
		/// </summary>
		static int AlertDurationMs = 500;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.GameContent.DrawableTower"/> is in alert mode.
		/// The alert mode set the tower red.
		/// </summary>
		/// <value><c>true</c> if alerting; otherwise, <c>false</c>.</value>
		private bool Alerting { get; set; }

		private DrawableTowerLifeBar LifeBar { get; set; }
		private bool Ally { get; set; }

		private Drawable Tower { get; set; }

        public DrawableTower(Tower tower, bool isAlly)
			: base(tower)
        {
			Ally = isAlly;

			Position = new Vector2(tower.Rectangle.X + tower.Rectangle.Width / 2f,
			                       tower.Rectangle.Bottom);

			Alerting = false;

			Tower = null;
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			if (Structure.Team == Teams.Left) {
				AddChild(Tower =  new DrawableImage("MapObjects/tower1") {
					RelativeOrigin = new Vector2(0.5f, 0.95f)
				}, 2);
			} else {
				AddChild(Tower = new DrawableImage("MapObjects/tower2") {
					RelativeOrigin = new Vector2(0.5f, 0.95f)
				}, 2);
				AddChild(new ParticleSystem(70,null,new TimeSpan(0,0,3)) {
				Tint = Color.White,
				ParticleInitialVelocity = new Vector2(5, 20),
				ParticleForce = new Vector2(-5, 2),
				Position = new Vector2(0,-135)},1);
			}

			AddChild(LifeBar = new DrawableTowerLifeBar(Ally) {
				Position = new Vector2(0f, -Structure.Rectangle.Height * 1.1f),
				Health = Structure.Health,
				MaxHealth = Structure.MaxHealth
			});
		}

		protected override void OnUpdate(GameTime dt)
		{
			base.OnUpdate(dt);

			LifeBar.Health = Structure.Health;
			LifeBar.MaxHealth = Structure.MaxHealth;
			LifeBar.Visible = Structure.Alive;

			if (!Structure.Alive) {
				AddChild(new DrawableBuildingExplosion() {Position = new Vector2(0,-75), RelativeOrigin = new Vector2(0.5f,0.5f), OverAction = () => {
						Parent.RemoveChild(this);
					}}, 3);
			}
		}

		/// <summary>
		/// Should be called before shooting to display the alert.
		/// </summary>
		public void WillShoot() {
			if (!Alerting && Tower != null) {
				Tower.PerformAction(new ActionTintBy(new TimeSpan(0,0,0,0,AlertDurationMs), new Vector3(0,-230,-230) ) { DoneAction = (thing) => 
						Tower.Tint = Color.White
				});
			}
		} 
    }
}

