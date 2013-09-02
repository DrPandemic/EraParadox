//
//  Menu.cs
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GREATClient.BaseClass.Input;
using System.Diagnostics;

namespace GREATClient.BaseClass.Menu
{
    public class Menu : Container
    {
		/// <summary>
		/// Gets or sets the menu items.
		/// </summary>
		/// <value>The menu items.</value>
		protected List<MenuItem> ItemList { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Menu.Menu"/> Enter key was registered.
		/// Used to not register enter twice.
		/// </summary>
		/// <value><c>true</c> if enter registered; otherwise, <c>false</c>.</value>
		bool EnterRegistered { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Menu.Menu"/> allow keyboard.
		/// </summary>
		/// <value><c>true</c> if allow keyboard; otherwise, <c>false</c>.</value>
		bool m_AllowKeyboard = false;
		public bool AllowKeyboard 
		{
			get {
				return m_AllowKeyboard;
			}
			set {
				m_AllowKeyboard = value;
				SetKeyboardListening();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Menu.MenuItem"/> is clickable.
		/// </summary>
		/// <value><c>true</c> if clickable; otherwise, <c>false</c>.</value>
		bool m_Clickable = true;
		public bool Clickable 
		{ 
			get {
				return m_Clickable;
			}
			set {
				m_Clickable = value;
				ItemList.ForEach((item) => item.Clickable = value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the items were aligned vertically.
		/// </summary>
		/// <value><c>true</c> if this instance is vertically aligned; otherwise, <c>false</c>.</value>
		public bool IsVertical { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the items were aligned horizontally.
		/// </summary>
		/// <value><c>true</c> if this instance is horinzotally aligned; otherwise, <c>false</c>.</value>
		public bool IsHorizontal { get; private set; }

		/// <summary>
		/// Gets or sets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		protected int SelectedItem { get; set; }

		/// <summary>
		/// Gets or sets the old selected item.
		/// </summary>
		/// <value>The old selected item.</value>
		protected int OldSelectedItem { get; set; }

		/// <summary>
		/// Called when an item is selected.
		/// Can be use to change menu focus.
		/// </summary>
		/// <value>An item selected.</value>
		public Action<Menu> AnItemSelected { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Menu.Menu"/> mouse left is press.
		/// Is use to stop selection when clicking.
		/// </summary>
		/// <value><c>true</c> if mouse left press; otherwise, <c>false</c>.</value>
		bool MouseLeftPressed { get; set; }

		// Event handlers for the arrows.
		EventHandler UpEvent { get; set; }
		EventHandler DownEvent { get; set; }
		EventHandler LeftEvent { get; set; }
		EventHandler RightEvent { get; set; }
		EventHandler EnterEvent { get; set; }
		EventHandler EnterPressedEvent { get; set; }
		EventHandler LeftReleasedEvent { get; set; }
		EventHandler LeftPressedEvent { get; set; }


		/*
		 * Quand tu hover ou change avec les fleche -> selectionne l'objet
		 * Enter le quel on a pressed et released
		 * Clic va a lui sur le quel on a pressed et released
		 */


		public Menu(params MenuItem[] items)
        {
			UpEvent = new EventHandler(PreviousSelection);
			DownEvent = new EventHandler(NextSelection);
			LeftEvent = new EventHandler(PreviousSelection);
			RightEvent = new EventHandler(NextSelection);
			EnterEvent = new EventHandler(ChooseSelectedItem);
			EnterPressedEvent = new EventHandler(PressSelectedItem);
			LeftPressedEvent = new EventHandler(LeftPressed);
			LeftReleasedEvent = new EventHandler(LeftReleased);

			AnItemSelected = null;

			MouseLeftPressed = false;
			EnterRegistered = false;
			SelectedItem = -1;
			OldSelectedItem = -1;
			IsVertical = false;
			IsHorizontal = false;

			ItemList = items.OfType<MenuItem>().ToList();
			ItemList.ForEach((MenuItem item) => AddItem(item));
        }

		void AddItem (MenuItem item)
		{
			AddChild(item);
		}

		/// <summary>
		/// Aligns the items vertically.
		/// </summary>
		/// <param name="padding">Padding.</param>
		public void AlignItemsVertically(float padding) 
		{
			IsVertical = true;
			IsHorizontal = false;

			float current = 0;

			ItemList.ForEach((MenuItem item) => {
				item.Position = new Vector2(0,current);
				current += padding;
			});

			SetKeyboardListening();
		}

		/// <summary>
		/// Aligns the items horizontally.
		/// </summary>
		/// <param name="padding">Padding.</param>
		public void AlignItemsHorizontally(float padding)
		{
			IsVertical = false;
			IsHorizontal = true;

			float current = 0;

			ItemList.ForEach((MenuItem item) => {
				item.Position = new Vector2(current,0);
				current += padding;
			});

			SetKeyboardListening();
		}

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			SetKeyboardListening();
		}

		/// <summary>
		/// Setup the listeners on the input manager.
		/// </summary>
		void SetKeyboardListening()
		{
			if (AllowKeyboard && inputManager != null && Clickable) {
				if (!EnterRegistered) {
					inputManager.RegisterEvent(InputActions.Enter, EnterEvent);
					inputManager.RegisterEvent(InputActions.EnterPressed, EnterPressedEvent);
					inputManager.RegisterEvent(InputActions.LeftClick, LeftReleasedEvent);
					inputManager.RegisterEvent(InputActions.LeftPressed, LeftPressedEvent);
				}
				inputManager.RemoveAction(InputActions.ArrowUp, UpEvent);
				inputManager.RemoveAction(InputActions.ArrowDown, DownEvent);
				inputManager.RemoveAction(InputActions.ArrowLeft, LeftEvent);
				inputManager.RemoveAction(InputActions.ArrowRight, RightEvent);

				if (IsVertical) {
					inputManager.RegisterEvent(InputActions.ArrowUp, UpEvent);
					inputManager.RegisterEvent(InputActions.ArrowDown, DownEvent);
				} else if (IsHorizontal) {
					inputManager.RegisterEvent(InputActions.ArrowLeft, LeftEvent);
					inputManager.RegisterEvent(InputActions.ArrowRight, RightEvent);
				}
			}
		}

		protected override void OnUnload()
		{
			inputManager.RemoveAction(InputActions.ArrowUp, UpEvent);
			inputManager.RemoveAction(InputActions.ArrowDown, DownEvent);
			inputManager.RemoveAction(InputActions.ArrowLeft, LeftEvent);
			inputManager.RemoveAction(InputActions.ArrowRight, RightEvent);
			inputManager.RemoveAction(InputActions.Enter, EnterEvent);
			inputManager.RemoveAction(InputActions.EnterPressed, EnterPressedEvent);
			inputManager.RemoveAction(InputActions.LeftClick, LeftReleasedEvent);
			inputManager.RemoveAction(InputActions.LeftPressed, LeftPressedEvent);
		}

		/// <summary>
		/// Select the next item.
		/// </summary>
		void NextSelection(object sender, EventArgs e)
		{
			if (Clickable) {
				SelectedItem++;
				SelecteGivenItem(null,null);
			}
		}

		/// <summary>
		/// Select the previous item.
		/// </summary>
		void PreviousSelection(object sender, EventArgs e)
		{
			if (Clickable) {
				SelectedItem--;
				SelecteGivenItem(null,null);
			}
		}

		/// <summary>
		/// Selecteds the given item.
		/// </summary>
		void SelecteGivenItem(object sender, EventArgs e)
		{
			// To remove the problem where Hover would always reset.
			if (!MouseLeftPressed) 
			{
				// Starts by the end if you you want to go before the no-item.
				if (SelectedItem < -1) {
					SelectedItem = ItemList.Count - 1;
				// Goes to the no-item if you bust the max.
				} else if (SelectedItem >= ItemList.Count) {
					SelectedItem = -1;
				}

				// Deselect/select all items.
				for (int i = 0 ; i < ItemList.Count; ++i) {
					if (i != SelectedItem) {
						ItemList[i].Normal();
					} else {
						ItemList[i].Select();
					}
				}

				OldSelectedItem = SelectedItem;
			}
		}

		/// <summary>
		/// Chooses the selected item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void ChooseSelectedItem(object sender, EventArgs e)
		{
			if (SelectedItem >= 0 && Clickable) {
				ItemList[SelectedItem].SelectionReleased();
				SelectedItem = -1;
				SelecteGivenItem(null,null);
			}
		}

		/// <summary>
		/// Presses the selected item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void PressSelectedItem(object sender, EventArgs e)
		{
			if (SelectedItem >= 0 && Clickable) {
				ItemList[SelectedItem].Clicking();
			}

		}

		/// <summary>
		/// Left mouse button pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void LeftPressed(object sender, EventArgs e)
		{
			MouseLeftPressed = true;
			InputEventArgs args = (InputEventArgs)e;
			if (!args.Handled && Clickable) {
				foreach (MenuItem item in ItemList) {
					if (item.IsBehind(args.MousePosition)) {
						item.Clicking();
					}
				}
			}
		}

		/// <summary>
		/// Left mouse button released.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void LeftReleased(object sender, EventArgs e)
		{
			MouseLeftPressed = false;
			InputEventArgs args = (InputEventArgs)e;
			if (!args.Handled && Clickable) {
				foreach (MenuItem item in ItemList) {
					if (item.IsBehind(args.MousePosition)) {
						item.SelectionReleased();
					}
				}
				UnselectItems();
			}
		}

		/// <summary>
		/// Unselects the items.
		/// </summary>
		public void UnselectItems()
		{
			SelectedItem = -1;			
			SelecteGivenItem(null,null);
		}

		/// <summary>
		/// Active the the menu.
		/// </summary>
		/// <param name="active">If set to <c>true</c> active.</param>
		public void Active(bool active)
		{
			MouseLeftPressed = !active;
			Clickable = active;
			Visible = active;
			SetKeyboardListening();
			if (active) {
				UnselectItems();
			}
		}

		protected override void OnUpdate(GameTime dt)
		{
			Debug.Assert(inputManager != null);
			if (Clickable && Visible) {
				for (int i = 0; i < ItemList.Count; ++i) {
					if (ItemList[i].IsBehind(inputManager.MousePosition)) {
						SelectedItem = i;
						SelecteGivenItem(null,null);
					}
				}
			}
		}
    }
}

