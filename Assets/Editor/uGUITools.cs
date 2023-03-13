/// Credit Senshi
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/ (uGUITools link)

using UnityEditor;
namespace UnityEngine.UI.Extensions
{
    public static class uGUITools
    {
	    [MenuItem("uGUI/Anchors to Center")]
	    static void AnchorsToCenter()
	    {
		    foreach (Transform transform in Selection.transforms)
		    {
			    RectTransform t = transform as RectTransform;
			    RectTransform pt = Selection.activeTransform.parent as RectTransform;

			    if (t == null || pt == null) return;

			    Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + (t.offsetMin.x + 0.5f * t.rect.width) / pt.rect.width,
				    t.anchorMin.y + (t.offsetMin.y + 0.5f * t.rect.height) / pt.rect.height);
			    Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + (t.offsetMax.x - 0.5f * t.rect.width) / pt.rect.width,
				    t.anchorMax.y + (t.offsetMax.y - 0.5f * t.rect.height) / pt.rect.height);

			    var prevWidth = t.rect.width;
			    var prevHeight = t.rect.height;

			    t.anchorMin = newAnchorsMin;
			    t.anchorMax = newAnchorsMax;

			    t.offsetMin = new Vector2(-prevWidth * 0.5f, -prevHeight * 0.5f);
			    t.offsetMax = new Vector2(prevWidth * 0.5f, prevHeight * 0.5f);
		    }
	    }
	    
	    
	[MenuItem("uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners()
	{
	    foreach (Transform transform in Selection.transforms)
	    {
		RectTransform t = transform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if (t == null || pt == null) return;

		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
						    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
						    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	    }
	}
	    
	    [MenuItem("uGUI/Anchors to Corners Recursively")]
	    static void AnchorsToCornersRecursively()
	    {
		    AnchorsToCornersRecursively(Selection.activeTransform);
	    }

	    static void AnchorsToCornersRecursively(Transform transform)
	    {
		    for (int i = 0; i < transform.childCount; i++)
		    {
			    var childTransform = transform.GetChild(i);
			    
			    AnchorsToCornersRecursively(childTransform);
			    
			    RectTransform t = childTransform as RectTransform;
			    RectTransform pt = childTransform.parent as RectTransform;

			    if (t == null || pt == null) return;

			    Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
				    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			    Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
				    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			    t.anchorMin = newAnchorsMin;
			    t.anchorMax = newAnchorsMax;
			    t.offsetMin = t.offsetMax = new Vector2(0, 0);
		    }
	    }
	    
	    [MenuItem("uGUI/Anchors to Center Recursively")]
	    static void AnchorsToCenterRecursively()
	    {
		    AnchorsToCenterRecursively(Selection.activeTransform);
	    }

	    static void AnchorsToCenterRecursively(Transform transform)
	    {
		    for (int i = 0; i < transform.childCount; i++)
		    {
			    var childTransform = transform.GetChild(i);
			    
			    AnchorsToCenterRecursively(childTransform);
			    
			    RectTransform t = childTransform as RectTransform;
			    RectTransform pt = childTransform.parent as RectTransform;

			    if (t == null || pt == null) return;

			    Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + (t.offsetMin.x + 0.5f * t.rect.width) / pt.rect.width,
				    t.anchorMin.y + (t.offsetMin.y + 0.5f * t.rect.height) / pt.rect.height);
			    Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + (t.offsetMax.x - 0.5f * t.rect.width) / pt.rect.width,
				    t.anchorMax.y + (t.offsetMax.y - 0.5f * t.rect.height) / pt.rect.height);

			    var prevWidth = t.rect.width;
			    var prevHeight = t.rect.height;

			    t.anchorMin = newAnchorsMin;
			    t.anchorMax = newAnchorsMax;

			    t.offsetMin = new Vector2(-prevWidth * 0.5f, -prevHeight * 0.5f);
			    t.offsetMax = new Vector2(prevWidth * 0.5f, prevHeight * 0.5f);
		    }
	    }
	    
	    [MenuItem("uGUI/Image -> RawImage")]
	    static void ReplaceImageWithRawImage()
	    {
		    var gos = Selection.gameObjects;

		    foreach (var go in gos)
		    {
			    var image = go.GetComponent<Image>();
			    var sprite = image.sprite;
			    var material = image.material;
			    var color = image.color;
			    Object.DestroyImmediate(image);
			    var rawImage = go.AddComponent<RawImage>();
			    
			    if(sprite != null)
					rawImage.texture = sprite.texture;
			    
			    if(material != null)
					rawImage.material = material;
			    
			    rawImage.color = color;
			    rawImage.raycastTarget = false;
		    }
	    }
	    
	    [MenuItem("uGUI/Image -> SpriteRenderer")]
	    static void ReplaceImageWithSpriteRenderer()
	    {
		    var gos = Selection.gameObjects;

		    foreach (var go in gos)
		    {
			    var image = go.GetComponent<Image>();
			    var sprite = image.sprite;
			    var material = image.material;
			    var color = image.color;
			    Object.DestroyImmediate(image);
			    var spriteRenderer = go.AddComponent<SpriteRenderer>();
			    
			    if(sprite != null)
				    spriteRenderer.sprite = sprite;
			    
			    spriteRenderer.color = color;
		    }
	    }

	[MenuItem("uGUI/Corners to Anchors %]")]
	static void CornersToAnchors()
	{
	    foreach (Transform transform in Selection.transforms)
	    {
		RectTransform t = transform as RectTransform;

		if (t == null) return;

		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	    }
	}

	[MenuItem("uGUI/Mirror Horizontally Around Anchors %;")]
	static void MirrorHorizontallyAnchors()
	{
	    MirrorHorizontally(false);
	}

	[MenuItem("uGUI/Mirror Horizontally Around Parent Center %:")]
	static void MirrorHorizontallyParent()
	{
	    MirrorHorizontally(true);
	}

	static void MirrorHorizontally(bool mirrorAnchors)
	{
	    foreach (Transform transform in Selection.transforms)
	    {
		RectTransform t = transform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if (t == null || pt == null) return;

		if (mirrorAnchors)
		{
		    Vector2 oldAnchorMin = t.anchorMin;
		    t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
		    t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
		}

		Vector2 oldOffsetMin = t.offsetMin;
		t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
		t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

		t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
	    }
	}

	[MenuItem("uGUI/Mirror Vertically Around Anchors %'")]
	static void MirrorVerticallyAnchors()
	{
	    MirrorVertically(false);
	}

	[MenuItem("uGUI/Mirror Vertically Around Parent Center %\"")]
	static void MirrorVerticallyParent()
	{
	    MirrorVertically(true);
	}

	static void MirrorVertically(bool mirrorAnchors)
	{
	    foreach (Transform transform in Selection.transforms)
	    {
		RectTransform t = transform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if (t == null || pt == null) return;

		if (mirrorAnchors)
		{
		    Vector2 oldAnchorMin = t.anchorMin;
		    t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
		    t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
		}

		Vector2 oldOffsetMin = t.offsetMin;
		t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
		t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);

		t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
	    }
	}
    }
}
