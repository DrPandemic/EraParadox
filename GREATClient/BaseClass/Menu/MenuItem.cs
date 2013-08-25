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
		/// Gets or sets the current state.
		/// </summary>
		/// <value>The state of the current.</value>
		protected IDraw CurrentState { get; set; }

		public bool Clickable { get; set; }

		// TODO : hover, clicked

        public MenuItem(IDraw stateNormal)
        {
			Clickable = true;
			StateNormal = stateNormal;
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
			}
		}

		/// <summary>
		/// Called when Click() is called and the instance is Clickable.
		/// </summary>
		protected virtual void OnClick()
		{
		}
    }
}

