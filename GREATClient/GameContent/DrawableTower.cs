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

namespace GREATClient.GameContent
{
    public class DrawableTower : DrawableStructure
    {
		private DrawableTowerLifeBar LifeBar { get; set; }
		private bool Ally { get; set; }

        public DrawableTower(Tower tower, bool isAlly)
			: base(tower)
        {
			Ally = isAlly;

			Position = new Vector2(tower.Rectangle.X + tower.Rectangle.Width / 2f,
			                       tower.Rectangle.Bottom);
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			/*AddChild(new DrawableRectangle(new Rect(0f, 0f, Structure.Rectangle.Width, Structure.Rectangle.Height), 
			                               Ally ? Color.Green : Color.Red) {
				RelativeOrigin = new Vector2(0.5f, 1.0f)
			});*/

			if (Structure.Team == Teams.Left) {
				AddChild(new DrawableImage("MapObjects/tower1") {
					RelativeOrigin = new Vector2(0.5f, 0.95f)
				}, 2);
			} else {
				AddChild(new DrawableImage("MapObjects/tower2") {
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
    }
}

