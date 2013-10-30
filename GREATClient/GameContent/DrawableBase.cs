//
//  DrawableBase.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
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
using GREATClient.BaseClass;
using GREATLib;
using GREATLib.Entities.Structures;
using GREATLib.Entities;
using Microsoft.Xna.Framework;
using GREATClient.BaseClass.BaseAction;
using GREATClient.BaseClass.Particle;

namespace GREATClient.GameContent
{
    public class DrawableBase : Container
    {
		const string LEFT_IMAGE = "lbase";
		const string RIGHT_IMAGE = "rbase";

		private Base Base { get; set; }
		private Teams Team { get; set; }

		DrawableBaseLifeBar LifeBar { get; set; }

        public DrawableBase(Teams team, Base theBase)
        {
			Base = theBase;
			Team = team;

			Position = new Vector2(theBase.Rectangle.X + theBase.Rectangle.Width / 2f,
			                       theBase.Rectangle.Bottom);

			//Jesse vient mettre de quoi d'intélligent ici parce que je trouve pas :P thx
			LifeBar = new DrawableBaseLifeBar(true) { 
				Position = new Vector2(0f,-150f),
				MaxHealth = theBase.MaxHealth,
				Health = theBase.Health };
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			AddChild(new DrawableImage(Team == Teams.Left ? LEFT_IMAGE : RIGHT_IMAGE) {
				RelativeOrigin = new Vector2(0.5f, 1.0f)
			},2);
			// Add the gear and smoke.
			if (Team == Teams.Left) {
				DrawableImage gear = new DrawableImage("gearnexus") {
					RelativeOrigin = new Vector2(0.5f),
					Position = new Vector2(-70,-85)};
				AddChild(gear);
				gear.PerformAction(new ActionSequence(ActionSequence.INFINITE_SEQUENCE, new ActionRotateBy(new TimeSpan(0, 0, 1), 20, false)));

				AddChild(new SmokeSystem() {Position = new Vector2(-80,-180)});
				AddChild(new SmokeSystem() {Position = new Vector2(-13,-160)});
			}

			//Add the life bar
			AddChild(LifeBar,3);
		}

		protected override void OnUpdate(GameTime dt)
		{
			//Update life bar
			LifeBar.MaxHealth = Base.MaxHealth;
			LifeBar.Health = Base.Health;
			LifeBar.Visible = Base.Alive;

			base.OnUpdate(dt);
		}
    }
}

