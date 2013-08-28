//
//  MenuItem.cs
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
using GREATClient.BaseClass.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GREATClient.BaseClass.Menu
{
    public class MenuItem : Container
    {
		/// <summary>
		/// Gets or sets the normal state.
		/// When nothing else is active.
		/// </summary>
		/// <value>The state normal.</value>
		public IDraw StateNormal { get; set; }

		/// <summary>
		/// Gets or sets the selected state.
		/// </summary>
		/// <value>The state selected.</value>
		public IDraw StateSelected { get; set; }

		/// <summary>
		/// Gets or sets the clicking state.
		/// </summary>
		/// <value>The clicking state.</value>
		public IDraw StateClicking { get; set; }

		/// <summary>
		/// Gets or sets the hover state.
		/// </summary>
		/// <value>The state hover.</value>
		public IDraw StateHover { get; set; }

		/// <summary>
		/// Gets or sets the current state.
		/// </summary>
		/// <value>The state of the current.</value>
		protected IDraw CurrentState { get; set; }

		/// <summary>
		/// Gets or sets the left clicked event.
		/// </summary>
		/// <value>The left clicked event.</value>
		EventHandler LeftClickedEvent { get; set; }

		/// <summary>
		/// Gets or sets the left clicking event.
		/// </summary>
		/// <value>The left clicking event.</value>
		EventHandler LeftClickingEvent { get; set; }

		/// <summary>
		/// Gets or sets the click action.
		/// Will be call when the object is clicked.
		/// </summary>
		/// <value>The click action.</value>
		public Action ClickAction { get; set; }

		/// <summary>
		/// Gets or sets the click action for menu.
		/// Don't change it.
		/// </summary>
		/// <value>The click action for menu.</value>
		public Action<MenuItem> ClickActionForMenu { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Menu.MenuItem"/> is clickable.
		/// </summary>
		/// <value><c>true</c> if clickable; otherwise, <c>false</c>.</value>
		public bool Clickable { get; set; }

		// TODO : hover, clicking
        public MenuItem(IDraw stateNormal)
        {
			ClickAction = null;
			LeftClickedEvent = null;
			LeftClickingEvent = null;
			Clickable = true;
			StateNormal = stateNormal;
			StateHover = null;
			StateClicking = null;
			StateSelected = null;
			Normal();
        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			LeftClickedEvent = new EventHandler(LeftReleased);
			LeftClickingEvent = new EventHandler(LeftPressed);
			inputManager.RegisterEvent(InputActions.LeftClick, LeftClickedEvent);
			inputManager.RegisterEvent(InputActions.LeftClicking, LeftClickingEvent);
		}

		protected override void OnUnload()
		{
			if (LeftClickedEvent != null) {				
				inputManager.RemoveAction(InputActions.LeftClick,LeftClickedEvent);
			}
			if (LeftClickingEvent != null) {				
				inputManager.RemoveAction(InputActions.LeftClicking,LeftClickingEvent);
			}
		}

		/// <summary>
		/// Select this instance.
		/// Change the display to the StateSelected.
		/// </summary>
		public void Select()
		{
			if (CurrentState != StateSelected) {
				SetState(StateSelected);
			}
		}

		/// <summary>
		/// Change the display to the StateNormal.
		/// </summary>
		public void Normal()
		{
			// TODO : le normal ne dois pas casser le hover
			if (CurrentState != StateNormal) {
				SetState(StateNormal);
			}
		}

		/// <summary>
		/// Change the display to the StateClicking.
		/// </summary>
		protected void Clicking()
		{
			if (CurrentState != StateClicking) {
				if (StateClicking != null) {
					SetState(StateClicking);
				} else if (StateSelected != null) {
					SetState(StateSelected);
				}
			}
		}

		/// <summary>
		/// Sets the state.
		/// Change for the good IDraw.
		/// </summary>
		/// <param name="state">State.</param>
		protected void SetState(IDraw state)
		{
			if (CurrentState != state) {
				if (state != null) {
					RemoveChild(CurrentState);
					if (state.Parent == null) {
						AddChild(state);
					}
					CurrentState = state;
				} else if (CurrentState != StateNormal) {
					RemoveChild(CurrentState);
					if (StateNormal.Parent == null) {
						AddChild(StateNormal);
					}
					CurrentState = StateNormal;
				}
			}
		}

		/// <summary>
		/// When the item is clicked.
		/// </summary>
		public void Click() 
		{
			if (Clickable) {
				OnClick();
				if (ClickAction != null) {
					ClickAction();		
				}
				ClickActionForMenu(this);
			}
		}

		/// <summary>
		/// Called when Click() is called and the instance is Clickable.
		/// </summary>
		protected virtual void OnClick()
		{
		}

		/// <summary>
		/// Called by the InputManager.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void LeftReleased(object sender, EventArgs e)
		{
			InputEventArgs ev = (InputEventArgs)e;
			if (Clickable && !ev.Handled) {				
				Normal();
				if (CurrentState.IsBehind(ev.MousePosition)) {
					Click();
				}
			}
		}

		public void LeftPressed(object sender, EventArgs e)
		{
			InputEventArgs ev = (InputEventArgs)e;
			if (Clickable && !ev.Handled) {
				if (CurrentState.IsBehind(ev.MousePosition)) {
					Clicking();
				}
			}
		}
    }
}

