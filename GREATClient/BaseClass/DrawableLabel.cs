//
//  DrawableLabel.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GREATClient.Display;

namespace GREATClient.BaseClass
{
    public class DrawableLabel : Drawable
    {
		/// <summary>
		/// Gets or sets the name of the font.
		/// </summary>
		/// <value>The name of the font.</value>
		string FontName { get; set; }

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font.</value>
		protected SpriteFont Font { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public String Text { get; set; }

		/// <summary>
		/// Gets or sets the update.
		/// </summary>
		/// <value>The update.</value>
		Action<DrawableLabel> UpdateAction { get; set; }
		public override float GetEffectiveAlpha()
		{
			return Alpha * (Parent != null ? Parent.GetEffectiveAlpha() : 1);
		}

		public DrawableLabel(string fontName = UIConstants.UI_FONT, Action<DrawableLabel> update = null) : base()
        {
			FontName = fontName;
			UpdateAction = update;
			Text = "";
        }

		/// <summary>
		/// Loads the image of the object.
		/// </summary>
		/// <param name="content">Content.</param>
		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			if (content != null && gd != null) {
				Font = content.Load<SpriteFont>(FontName);
				IsLoaded = true;
			}
		}

		protected override void OnDraw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

			Vector2 FontOrigin = Font.MeasureString( Text ) / 2;
			// Draw the string
			batch.DrawString( Font, Text, GetAbsolutePosition(), Tint * GetEffectiveAlpha(), 
			                 (float)Orientation, RelativeOrigin * Font.MeasureString(Text), Scale, Effects, 0 );

			batch.End();
		}

		protected override void OnUpdate(GameTime dt)
		{
			if (UpdateAction != null) {
				UpdateAction(this);
			}
		}

		public override bool IsBehind(Vector2 position)
		{
			if (Parent != null) {
				Vector2 labelPos = GetAbsolutePosition() - RelativeOrigin * Font.MeasureString(Text);
				return (position.X >= labelPos.X && position.X <= (labelPos + Font.MeasureString(Text)).X) &&
					(position.Y >= labelPos.Y && position.Y <= (labelPos + Font.MeasureString(Text)).Y);
			}
			return false;
		}
    }
}

