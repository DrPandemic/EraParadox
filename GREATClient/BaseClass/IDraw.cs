//
//  IDraw.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GREATClient.BaseClass.BaseAction;
using GREATClient.BaseClass.Input;
using System.Collections.Generic;
using GREATClient.BaseClass.ScreenInformation;

namespace GREATClient.BaseClass
{
	public abstract class IDraw : IUpdatable
    {
		/// <summary>
		/// Gets the input manager.
		/// </summary>
		/// <returns>The input manager.</returns>
		InputManager m_InputManager;
		public InputManager inputManager {
			get {
				if (m_InputManager != null) {
					return m_InputManager;
				} else {
					if (GetServices() != null) {
						m_InputManager = (InputManager)this.GetServices().GetService(typeof(InputManager));
						return m_InputManager;
					} else {
						return null;
					}
				}
			}
		}

		/// <summary>
		/// Gets the screen service.
		/// </summary>
		/// <returns>The screen service.</returns>
		ScreenService m_ScreenService;
		public ScreenService screenService {
			get {
				if (m_ScreenService != null) {
					return m_ScreenService;
				} else {
					if (Parent != null) {
						m_ScreenService = (ScreenService)this.GetServices().GetService(typeof(ScreenService));
						return m_ScreenService;
					} else {
						return null;
					}
				}
			}
		}

		SoundService m_SoundService;
		public SoundService soundService {
			get {
				if (m_SoundService != null) {
					return m_SoundService;
				} else {
					if (Parent != null) {
						m_SoundService = (SoundService)this.GetServices().GetService(typeof(SoundService));
						return m_SoundService;
					} else {
						return null;
					}
				}
			}
		}

		/// <summary>
		/// Gets the services.
		/// Is used to replace Game.Services.
		/// Only the screen hold the reference to the object.
		/// </summary>
		/// <value>The services.</value>
		public virtual GameServiceContainer GetServices() 
		{
			if (Parent != null) {
				return Parent.GetServices();
			}
			return null;
		}

		/// <summary>
		/// Gets the screen.
		/// </summary>
		public virtual Screen GetScreen()
		{
			if (Parent != null) {
				return Parent.GetScreen();
			}
			return null;
		}

		/// <summary>
		/// Gets the parent of the object.
		/// </summary>
		/// <value>The parent.</value>
		public Container Parent { get; protected set; }

		/// <summary>
		/// Gets or sets the z-index within the parent's container.
		/// The higher the z-index, the "closer" it is while displaying it.
		/// Don't change it manually
		/// </summary>
		/// <value>The z.</value>
		public int Z { get; set; }


		// Position.
		// Movement event.
		delegate void MovementEventHandler(IDraw bound);
		event MovementEventHandler Moved;

		/// <summary>
		/// Gets or sets the position.
		/// It is the absolute position.
		/// If the position mode is not at Normal, this value can't be changed from the outside.
		/// </summary>
		/// <value>The position.</value>
		Vector2 m_Position;
		public Vector2 Position 
		{ 
			get {
				return m_Position;
			}
			set {
				m_Position = value;
				if (Moved != null) {
					Moved(this);
				}
			}
		}

		/// <summary>
		/// Gets the absolute position.
		/// </summary>
		/// <returns>The absolute position.</returns>
		public virtual Vector2 GetAbsolutePosition()
		{
			return Position+Parent.GetAbsolutePosition();
		}

		/// <summary>
		/// Gets or sets the alpha.
		/// 0 to 1.
		/// </summary>
		/// <value>The alpha.</value>
		public float Alpha { get; set; }

		public abstract float GetEffectiveAlpha();

		/// <summary>
		/// Gets or sets the position mode.
		/// </summary>
		/// <value>The position mode.</value>
		public PositionType PositionMode { get; set; }

		// % of the screen.
		int XPercentOfTheScreen { get; set; }
		int YPercentOfTheScreen { get; set; }

		// Bound to screen.
		ScreenBound Bound { get; set; }
		Vector2 ScreenBoundOffset { get; set; }

		//Bound to object
		IDraw ObjectBound { get; set; }
		Vector2 ObjectBoundOffset { get; set; }
		MovementEventHandler MoveEvent { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.IDraw"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.IDraw"/> is updatable.
		/// </summary>
		/// <value><c>true</c> if updatable; otherwise, <c>false</c>.</value>
		public bool Updatable { get; set; }

		/// <summary>
		/// Gets or sets the actions over time.
		/// </summary>
		/// <value>The actions over time.</value>
		protected List<ActionOverTime> ActionsOverTime { get; set; }

		/// <summary>
		/// Gets or sets the actions over time to remove.
		/// </summary>
		/// <value>The actions over time to remove.</value>
		List<ActionOverTime> ActionsOverTimeToRemove { get; set; }

		/// <summary>
		/// Gets or sets the actions over time to activate.
		/// </summary>
		/// <value>The actions over time to activate.</value>
		List<ActionOverTime> ActionsOverTimeToActivate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.IDraw"/> is loaded.
		/// </summary>
		/// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
		public bool Loaded { get; set; }

		/// <summary>
		/// Gets or sets the game.
		/// </summary>
		/// <value>The game.</value>
		public virtual Game Game 
		{ 
			get {
				return Parent == null ? null : Parent.Game;
			}
			protected set { throw new NotImplementedException("Only the screen can set the Game"); }
		}

		public IDraw() 
		{
			PositionMode = PositionType.Normal;
			Bound = ScreenBound.TopLeft;
			ScreenBoundOffset = Vector2.Zero;
			XPercentOfTheScreen = 0;
			YPercentOfTheScreen = 0;
			MoveEvent = null;
			Alpha = 1;

			Parent = null;
			Loaded = false;
			Z = 0;
			Visible = true;
			Updatable = true;
			ActionsOverTime = new List<ActionOverTime>();
			ActionsOverTimeToRemove = new List<ActionOverTime>();
			ActionsOverTimeToActivate = new List<ActionOverTime>();
		}

		/// <summary>
		/// Load the drawable object.
		/// </summary>
		/// <param name="container">Its container.</param>
		/// <param name="content">The content manager, used to draw.</param>
		public virtual void Load(Container container, GraphicsDevice gd)
		{
			Parent = container;
			OnLoad(Parent.Content, gd);

			if (PositionMode == PositionType.ScreenRelativeInPercent) {
				SetPositionInScreenPercent(XPercentOfTheScreen,YPercentOfTheScreen);
			} else if (PositionMode == PositionType.ScreenRelative) {
				SetPositionRelativeToScreen(Bound,ScreenBoundOffset);
			}

			Loaded = true;
		}

		/// <summary>
		/// Raises the load event.
		/// Will be call after it is had to a container
		/// </summary>
		/// <param name="content">Content.</param>
		protected virtual void OnLoad(ContentManager content, GraphicsDevice gd) {}

		/// <summary>
		/// After load was call.
		/// </summary>
		public void UnLoad()
		{			
			OnUnload();
			Parent = null;
		}

		/// <summary>
		///  After Unload was call.
		/// </summary>
		protected virtual void OnUnload() { }

		/// <summary>
		/// Draw the specified batch.
		/// </summary>
		/// <param name="batch">Batch.</param>
		public virtual void Draw(SpriteBatch batch)
		{
			if(Visible) {
				OnDraw(batch);
			}
		}

		/// <summary>
		/// Called after Draw if visible
		/// </summary>
		/// <param name="batch">Batch.</param>
		protected abstract void OnDraw(SpriteBatch batch);

		/// <summary>
		/// Update
		/// Dt is disference of time since last call
		/// </summary>
		/// <param name="dt">Dt.</param>
		public virtual void Update(GameTime dt)
		{
			if (Updatable) {

				foreach(ActionOverTime action in ActionsOverTime) {
					action.Update(dt);
				}
				foreach (ActionOverTime action in ActionsOverTimeToRemove) {
					ActionsOverTime.Remove(action);
				}
				ActionsOverTimeToRemove.Clear();

				ActivateAction();

				OnUpdate(dt);
			}
		}

		/// <summary>
		/// Called after Update if Updatable
		/// </summary>
		/// <param name="dt">Dt.</param>
		protected virtual void OnUpdate(GameTime dt) { }

		/// <summary>
		/// Performs the action.
		/// </summary>
		/// <param name="action">Action.</param>
		public virtual void PerformAction(ActionOverTime action)
		{
			ActionsOverTimeToActivate.Add(action);
		}

		private void ActivateAction()
		{
			int size = ActionsOverTimeToActivate.Count;

			for (int i = size - 1; i>= 0 ; i--) {
				ActionsOverTime.Add(ActionsOverTimeToActivate[i]);
				// Set the target.
				ActionsOverTimeToActivate[i].Target = this;
				ActionsOverTimeToActivate[i].Ready();
				// Start the action.
				ActionsOverTimeToActivate[i].Start();
			}

			ActionsOverTimeToActivate.RemoveRange(0, size);
		}

		/// <summary>
		/// Called by an action when it's done.
		/// Will remove itself from the dictionary.
		/// Can be override but don't forget to call the base().
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="args">Argument.</param>
		public virtual void ActionDone(ActionOverTime action, Object args)
		{
			action.Stop();
			ActionsOverTimeToRemove.Add(action);
		}

		/// <summary>
		/// Stops all actions and clear all action lists.
		/// </summary>
		public void StopAllActions() 
		{
			ActionsOverTime.ForEach((ActionOverTime item) => item.Stop());
			ActionsOverTime.Clear();
			ActionsOverTimeToActivate.Clear();
			ActionsOverTimeToRemove.Clear();
		}

		/// <summary>
		/// Pauses all actions.
		/// </summary>
		public void PauseAllActions()
		{
			ActionsOverTime.ForEach((ActionOverTime item) => item.Pause());
		}

		/// <summary>
		/// Resumes all actions.
		/// </summary>
		public void ResumeAllActions()
		{
			ActionsOverTime.ForEach((ActionOverTime item) => item.Resume());
		}

		/// <summary>
		/// Determines whether this instance is beyond the specified position.
		/// </summary>
		/// <returns><c>true</c> if this instance is beyond the specified position; otherwise, <c>false</c>.</returns>
		/// <param name="position">Position.</param>
		public abstract bool IsBehind(Vector2 position);

		/// <summary>
		/// Sets the position in screen percent.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void SetPositionInScreenPercent(int x, int y)
		{
			PositionMode = PositionType.ScreenRelativeInPercent;
			// Limit x and y since they are %.
			XPercentOfTheScreen = x;
			YPercentOfTheScreen = y;

			if (screenService != null) {
				m_Position = new Vector2(XPercentOfTheScreen * screenService.GameWindowSize.X / 100, 
				                         YPercentOfTheScreen * screenService.GameWindowSize.Y / 100);
			}
		}

		/// <summary>
		/// Sets the position relative to screen bound.
		/// </summary>
		/// <param name="bound">Bound.</param>
		/// <param name="offset">Offset.</param>
		public void SetPositionRelativeToScreen(ScreenBound bound, Vector2 offset) 
		{
			PositionMode = PositionType.ScreenRelative;

			Bound = bound;
			ScreenBoundOffset = offset;

			if (screenService != null) {
				switch(bound) {
					case ScreenBound.TopLeft:
						m_Position = offset;
						break;
					case ScreenBound.TopRight:
						m_Position = Vector2.Add(offset,new Vector2(screenService.GameWindowSize.X,0));
						break;
					case ScreenBound.BottomRight:
						m_Position = Vector2.Add(offset,screenService.GameWindowSize);
						break;
					case ScreenBound.BottomLeft:
						m_Position = Vector2.Add(offset,new Vector2(0,screenService.GameWindowSize.Y));
						break;
				}
			}
		}

		/// <summary>
		/// Sets the position relative to object.
		/// </summary>
		/// <param name="objectBound">Object bound.</param>
		/// <param name="offset">Offset.</param>
		public void SetPositionRelativeToObject(IDraw objectBound, Vector2 offset, bool followBound = true)
		{
			PositionMode = PositionType.ObjectRelative;
			ObjectBoundOffset = offset;

			// I put it here to have to the movement code at the same place.
			if (MoveEvent == null) {
				MoveEvent = (Bound) => {
					// Replace the current IDraw.
					Position = Vector2.Add(offset,Bound.Position);
				};
			}

			// If there is currently no bound, it can simply add everyting.
			if (ObjectBound == null) {
				ObjectBound = objectBound;
				if (followBound) {
					objectBound.Moved += MoveEvent;
				}
			} 
			// If it is a new bound, remove old event handler.
			else if (ObjectBound != objectBound) {
				if (followBound) {
					ObjectBound.Moved -= MoveEvent;
				}
				ObjectBound = objectBound;
				if (followBound) {
					objectBound.Moved += MoveEvent;
				}
			}

			MoveEvent(objectBound);
		}		
	}
}

