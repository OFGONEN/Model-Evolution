using UnityEngine;
using UnityEngine.UI;

namespace FFStudio
{
	public class UIImage : UIEntity
	{
		public Image imageRenderer;
		Vector2 imageSizeDelta;
		private void Awake()
		{
			imageSizeDelta = uiTransform.sizeDelta;
		}

		public void SetSprite( Sprite sprite, SpriteSetMethod method )
		{
			if( method == SpriteSetMethod.Equlize )
			{
				uiTransform.sizeDelta = new Vector2( sprite.textureRect.width, sprite.textureRect.height );
			}
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
	}
}