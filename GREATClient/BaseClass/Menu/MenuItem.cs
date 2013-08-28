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
		/// Gets or sets the clicked state.
		/// </summary>
		/// <value>The state clicked.</value>
		public IDraw StateClicked { get; set; }

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
		/// Gets or sets the event handler for the click.
		/// </summary>
		/// <value>Something clicked.</value>
		EventHandler SomethingClickedEvent { get; set; }

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

		// TODO : hover, clicked
        public MenuItem(IDraw stateNormal)
        {
			ClickAction = null;
			SomethingClickedEvent = null;
			Clickable = true;
			StateNormal = stateNormal;
			Normal();
        }

		protected override void OnLoad(ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			SomethingClickedEvent = new EventHandler(SomethingWasClick);
			inputManager.RegisterEvent(InputActions.LeftClick, SomethingClickedEvent);
		}

		protected override void OnUnload()
		{
			if (SomethingClickedEvent != null) {				
				inputManager.RemoveAction(InputActions.LeftClick,SomethingClickedEvent);
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
		/// Sets the state.
		/// Change for the good IDraw.
		/// </summary>
		/// <param name="state">State.</param>
		protected void SetState(IDraw state)
		{
			if (state != null) {
				RemoveChild(CurrentState);
				AddChild(state);
			} else if (CurrentState != StateNormal) {
				RemoveChild(CurrentState);
				AddChild(StateNormal);
			}

			CurrentState = state;
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
		public void SomethingWasClick(object sender, EventArgs e)
		{
			InputEventArgs ev = (InputEventArgs)e;
			if (Clickable && !ev.Handled) {
				if (CurrentState.IsBehind(ev.MousePosition)) {
					Click();
				}
			}
		}
    }
}

