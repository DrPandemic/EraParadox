//
//  Screen.cs
//
//  Author:
//       The Parasithe <bipbip500@hotmail.com>
//
//  Copyright (c) 2013 The Parasithe
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GREATClient.BaseClass.Input;
using GREATClient.BaseClass.ScreenInformation;

namespace GREATClient.BaseClass
{
    public class Screen : Container
    {
		/// <summary>
		/// Gets the services.
		/// Is used to replace Game.Services.
		/// </summary>
		/// <value>The services.</value>
		public GameServiceContainer Services { get; private set;}

		public override Screen GetScreen()
		{
			return this;
		}

		/// <summary>
		/// Gets or sets a value indicating whether it want to exit
		/// </summary>
		/// <value><c>true</c> if exit; otherwise, <c>false</c>.</value>
		public bool Exit { get; set; }

		/// <summary>
		/// Gets or sets the sprite batch.
		/// </summary>
		/// <value>The sprite batch.</value>
		protected SpriteBatch spriteBatch { get; set; }

		/// <summary>
		/// Gets or sets the graphics.
		/// </summary>
		/// <value>The graphics.</value>
		public GraphicsDevice Graphics { get; private set; }

		/// <summary>
		/// Gets or sets the cursor.
		/// </summary>
		/// <value>The cursor.</value>
		public DrawableImage Cursor { get; set; }

		/// <summary>
		/// Gets the absolute position.
		/// Is overriden in screen, because it's the base class
		/// </summary>
		/// <returns>The absolute position.</returns>
		public override Vector2 GetAbsolutePosition()
		{
			return Position;
		}

		/// <summary>
		/// Gets the services.
		/// Is used to replace Game.Services.
		/// Only the screen hold the reference to the object.
		/// </summary>
		/// <value>The services.</value>
		public override GameServiceContainer GetServices() 
		{
			return Services;
		}

		Game game;
		public override Game Game 
		{ 
			get {
				return game;
			}
			protected set {
				game = value;
			}
		}

		/// <summary>
		/// Gets the graphics.
		/// Only the screen hold the reference to the object
		/// </summary>
		/// <returns>The graphics.</returns>
		public override GraphicsDevice GetGraphics()
		{
			return Graphics;
		} 

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.Screen"/> class.
		/// </summary>
		/// <param name="content">Content.</param>
		public Screen(ContentManager content, Game game) : base()
        {
			Content = content;
			Services = new GameServiceContainer();
			this.Services.AddService(typeof(InputManager), new InputManager());
			this.Services.AddService(typeof(ScreenService), new ScreenService(
									(GraphicsDeviceManager)game.Services.GetService(typeof(IGraphicsDeviceService))));
			Exit = false;
			Game = game;
        }

		/// <summary>
		/// Loads the content.
		/// </summary>
		/// <param name="gd">The graphics device used for the screen.</param>
		/// <param name="batch">The spritebatch used to draw.</para>
		public void LoadContent(GraphicsDevice gd)
		{
			Graphics = gd;
			spriteBatch = new SpriteBatch(gd);
			OnLoadContent();

			Cursor = new DrawableImage("cursor");
			AddChild(Cursor,100);
			((InputManager)Services.GetService(typeof(InputManager))).Cursor = Cursor;
		}

		/// <summary>
		/// Call when the content is load
		/// </summary>
		protected virtual void OnLoadContent()
		{}

		/// <summary>
		/// Draw this instance.
		/// </summary>
		public virtual void Draw()
		{
			Children.ForEach(child => {
				if (child.Parent == null) {
					child.Load(this,GetGraphics());
				}
				child.Draw(spriteBatch);
			});
		}

		protected override void OnUpdate(GameTime dt)
		{
			((ScreenService)Services.GetService(typeof(ScreenService))).Update();
			((InputManager)Services.GetService(typeof(InputManager))).Update();
			base.OnUpdate(dt);
		}

		/// <summary>
		/// Called when the window from monogame is ready.
		/// </summary>
		public void WindowIsReady()
		{
			((InputManager)Services.GetService(typeof(InputManager))).IsWindowReady = true;
		}
    }
}

