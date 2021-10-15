/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.UI;

namespace FFStudio
{
	public class UIImage : UIEntity
	{
#region Fields
		public Image imageRenderer;
		
		private Vector2 imageSizeDelta;
#endregion

#region Unity API
		private void Awake()
		{
			imageSizeDelta = uiTransform.sizeDelta;
		}
#endregion

#region API
		public void SetSprite( Sprite sprite, SpriteSetMethod method )
		{
			if( method == SpriteSetMethod.Equlize )
				uiTransform.sizeDelta = new Vector2( sprite.textureRect.width, sprite.textureRect.height );
			else if( method == SpriteSetMethod.PreserveAspect )
			{
				if( imageSizeDelta.x > sprite.textureRect.width && imageSizeDelta.y > sprite.textureRect.height )
				{
					imageRenderer.preserveAspect = false;
					uiTransform.sizeDelta = new Vector2( sprite.textureRect.width, sprite.textureRect.height );
				}
				else
				{
					uiTransform.sizeDelta = imageSizeDelta;
					imageRenderer.preserveAspect = true;
				}
			}

			imageRenderer.sprite = sprite;
		}
#endregion
	}
}