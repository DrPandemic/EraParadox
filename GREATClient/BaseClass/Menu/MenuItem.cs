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
using Microsoft.Xna.Framework;

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
		/// Gets or sets the current state.
		/// </summary>
		/// <value>The state of the current.</value>
		public IDraw CurrentState { get; protected set; }

		/// <summary>
		/// Gets or sets the click action.
		/// Will be call when the object is clicked.
		/// </summary>
		/// <value>The click action.</value>
		public Action ClickAction { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Menu.MenuItem"/> is clickable.
		/// </summary>
		/// <value><c>true</c> if clickable; otherwise, <c>false</c>.</value>
		public bool Clickable { get; set; }

		public MenuItem(IDraw stateNormal, IDraw stateSelected, IDraw stateClicking)
        {
			ClickAction = null;
			Clickable = true;
			StateNormal = stateNormal;
			StateClicking = stateClicking;
			StateSelected = stateSelected;
			Normal();
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
			if (CurrentState != StateNormal) {
				SetState(StateNormal);
			}
		}

		/// <summary>
		/// Change the display to the StateClicking.
		/// </summary>
		public void Clicking()
		{
			if (CurrentState != StateClicking) {
				SetState(StateClicking);
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
				} else {
					RemoveChild(CurrentState);
					if (StateNormal.Parent == null) {
						AddChild(StateNormal);
					}
					CurrentState = StateNormal;
				}
			}
		}

		/// <summary>
		/// If was in clicking state, the Click() will be call.
		/// </summary>
		public void SelectionReleased()
		{
			if (CurrentState == StateClicking) {
				Click();
			}
		}

		/// <summary>
		/// When the item is clicked.
		/// </summary>
		protected void Click() 
		{
			if (Clickable) {
				OnClick();
				if (ClickAction != null) {
					ClickAction();		
				}
			}
		}

		/// <summary>
		/// Called when Click() is called and the instance is Clickable.
		/// </summary>
		protected virtual void OnClick()
		{
		}

		public override bool IsBehind(Vector2 position)
		{
			if (Visible && CurrentState != null) {				
				return CurrentState.IsBehind(position);
			}
			return false;
		}
    }
}

