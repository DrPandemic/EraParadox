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

		public Menu(params MenuItem[] items)
        {
			EnterRegistered = false;
			SelectedItem = -1;
			OldSelectedItem = -1;
			IsVertical = false;
			IsHorizontal = false;

			ItemList = items.OfType<MenuItem>().ToList();
			ItemList.ForEach((MenuItem item) => AddChild(item));
        }

		/// <summary>
		/// Aligns the items vertically.
		/// </summary>
		/// <param name="padding">Padding.</param>
		public void AlignItemsVertically(float padding) 
		{
			IsVertical = true;
			IsHorizontal = false;

			float current = padding;

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

			float current = padding;

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
			if (AllowKeyboard && inputManager != null) {
				if (!EnterRegistered) {
					inputManager.RegisterEvent(InputActions.Enter, new EventHandler(ChooseSelectedItem));
				}
				inputManager.RemoveAction(InputActions.ArrowUp);
				inputManager.RemoveAction(InputActions.ArrowDown);
				inputManager.RemoveAction(InputActions.ArrowLeft);
				inputManager.RemoveAction(InputActions.ArrowRight);

				if (IsVertical) {
					inputManager.RegisterEvent(InputActions.ArrowUp, new EventHandler(PreviousSelection));
					inputManager.RegisterEvent(InputActions.ArrowDown, new EventHandler(NextSelection));
				} else if (IsHorizontal) {
					inputManager.RegisterEvent(InputActions.ArrowLeft, new EventHandler(PreviousSelection));
					inputManager.RegisterEvent(InputActions.ArrowRight, new EventHandler(NextSelection));
				}
			}
		}

		protected override void OnUnload()
		{
			inputManager.RemoveAction(InputActions.ArrowUp);
			inputManager.RemoveAction(InputActions.ArrowDown);
			inputManager.RemoveAction(InputActions.ArrowLeft);
			inputManager.RemoveAction(InputActions.ArrowRight);
			inputManager.RemoveAction(InputActions.Enter);
		}

		/// <summary>
		/// Select the next item.
		/// </summary>
		void NextSelection(object sender, EventArgs e)
		{
			SelectedItem++;
			SelecteGivenItem(null,null);
		}

		/// <summary>
		/// Select the previous item.
		/// </summary>
		void PreviousSelection(object sender, EventArgs e)
		{
			SelectedItem--;
			SelecteGivenItem(null,null);
		}

		/// <summary>
		/// Selecteds the given item.
		/// </summary>
		void SelecteGivenItem(object sender, EventArgs e)
		{
			// Start by the end if you you want to go before the no-item.
			if (SelectedItem < -1) {
				SelectedItem = ItemList.Count - 1;
			// Go to the no-item if you bust the max.
			} else if (SelectedItem >= ItemList.Count) {
				SelectedItem = -1;;
			}

			// Select the item.
			if (SelectedItem >= 0) {
				ItemList[SelectedItem].Select();
			}
			// Deselect the old item.
			if (OldSelectedItem != SelectedItem && OldSelectedItem >= 0) {
				ItemList[OldSelectedItem].Normal();
			}

			OldSelectedItem = SelectedItem;
		}

		/// <summary>
		/// Chooses the selected item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void ChooseSelectedItem(object sender, EventArgs e)
		{
			if (SelectedItem >= 0) {
				ItemList[SelectedItem].Click();
			}
		}
    }
}

