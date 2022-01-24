using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ImageOperations : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector]
    public SlideFileData slideData;

    public RawImage slideImage;
    public RawImage selectorImage;
    public RawImage combinedImage;

    public GameObject quitPanel;

    public Texture2D slideTexture;
    public Texture2D colorSelectorTexture;
    public Texture2D combinedTexture;

    public LineRenderer lassoRenderer;
    private BoxCollider2D slideCollider;
    private PolygonCollider2D areaCollider;

    [HideInInspector]
    public List<Vector2> areaPoints = new List<Vector2>();
    [HideInInspector]
    public List<Vector3> lassoPoints = new List<Vector3>();
    [HideInInspector]
    public Vector3 dragPositionScreen;

    private int minX;
    private int maxX;
    private int minY;
    private int maxY;

    private int screenWidth;
    private int screenHeight;

    private float initialScaleRatio = 1f;
    private float scaleRatio = 1f;

    public bool isDragging = false;
    private Vector2 dragPosition;
    public bool isMouseOver = false;
    public bool isAreaSelected = false;
    public bool isColorSelected = false;
    public Color32 currentImageColor;

    public void RefreshImage()
    {
        if (slideTexture != null)
        {
            GameObject.Destroy(slideTexture);
        }
        if (colorSelectorTexture != null)
        {
            GameObject.Destroy(colorSelectorTexture);
        }
        if (combinedTexture != null)
        {
            GameObject.Destroy(combinedTexture);
        }

        screenWidth = Screen.width - 40;
        screenHeight = Screen.height - 60;

        scaleRatio = 1f;

        ApplicationController.instance.isColorSelected = false;
        ApplicationController.instance.isAreaSelected = false;
        ApplicationController.instance.infoPanel.gameObject.SetActive(false);

        selectorImage.gameObject.SetActive(false);
        slideImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        areaPoints.Clear();
        lassoPoints.Clear();
        
        slideTexture = GetReadableTexture((Texture2D)slideImage.texture);
        colorSelectorTexture = new Texture2D(slideTexture.width, slideTexture.height);
        combinedTexture = new Texture2D(slideTexture.width, slideTexture.height);

        float imageHeight = slideTexture.height;
        float imageWidth = slideTexture.width;

        maxX = 0;
        minX = (int)imageWidth;
        maxY = 0;
        minY = (int)imageHeight;

        float rectWidth = slideImage.rectTransform.rect.width;
        float rectHeight = slideImage.rectTransform.rect.height;

        if (rectHeight > screenHeight || rectWidth > screenWidth)
        {
            float heightRatio = screenHeight / rectHeight;
            float widthRatio = screenWidth / rectWidth;
            if (heightRatio > widthRatio)
            {
                scaleRatio = widthRatio;
                initialScaleRatio = scaleRatio;
                slideImage.rectTransform.localScale = new Vector3(widthRatio, widthRatio, widthRatio);
            }
            else
            {
                scaleRatio = heightRatio;
                initialScaleRatio = scaleRatio;
                slideImage.rectTransform.localScale = new Vector3(heightRatio, heightRatio, heightRatio);
            }
        }

        CenterImage();

        RectTransform selectorTransform = selectorImage.GetComponent<RectTransform>();
        selectorTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageWidth);
        selectorTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageHeight);

        RectTransform combinedTransform = combinedImage.GetComponent<RectTransform>();
        combinedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageWidth);
        combinedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageHeight);
        combinedTransform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);

        areaCollider = slideImage.GetComponent<PolygonCollider2D>();

        lassoRenderer = slideImage.GetComponentInChildren<LineRenderer>();
        lassoRenderer.startWidth = .02f;
        lassoRenderer.positionCount = 0;
        lassoRenderer.loop = false;

        areaCollider.pathCount = 0;

        slideCollider = slideImage.GetComponent<BoxCollider2D>();
        slideCollider.size = new Vector2(imageWidth, imageHeight);
        slideCollider.offset = new Vector2(imageWidth / 2, imageHeight / 2);
    }

    public Texture2D GetReadableTexture(Texture2D inputTexture)
    {
        byte[] rawData = inputTexture.GetRawTextureData();
        Texture2D newTexture = new Texture2D(inputTexture.width, inputTexture.height,inputTexture.format,false);
        newTexture.LoadRawTextureData(rawData);
        newTexture.Apply();
        return newTexture;
    }

    public void ShowSelectedColors()
    {
        int mouseX=0;
        int mouseY=0;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(slideImage.GetComponentInChildren<RectTransform>(),Input.mousePosition,Camera.main,out Vector2 localPoint))
        {
            mouseX = Mathf.RoundToInt(localPoint.x);
            mouseY = Mathf.RoundToInt(localPoint.y);

        }

        ApplicationController.instance.SetCurrentColor(GetAverageColor(mouseX, mouseY));
    }

    private Color GetAverageColor(int xPos, int yPos)
    {
        Color averageColor;
        float rAverage = 0;
        float gAverage = 0;
        float bAverage = 0;
        float aAverage = 0;

        for (int i = xPos-1; i<=xPos+1; i++)
        {
            for (int j = yPos-1; j <= yPos+1; j++)
            {
                rAverage += slideTexture.GetPixel(i, j).r;
                gAverage += slideTexture.GetPixel(i, j).g;
                bAverage += slideTexture.GetPixel(i, j).b;
                aAverage += slideTexture.GetPixel(i, j).a;

            }
        }
        rAverage = rAverage / 9;
        gAverage = gAverage / 9;
        bAverage = bAverage / 9;
        aAverage = aAverage / 9;

        averageColor = new Color(rAverage, gAverage, bAverage, aAverage);

        return averageColor;
    }

    public void HighlightToleranceArea()
    {
        selectorImage.gameObject.SetActive(true);
        slideCollider.enabled = false;

        int totalPixels = 0;
        int totalMatchingPixels = 0;
        int tolerance = ApplicationController.instance.toleranceSetting;
        float selectionAreaPercentage;

        Color32[] slidePixelArray = slideTexture.GetPixels32();
        Color32[] selectorPixelArray = colorSelectorTexture.GetPixels32();

        int imageHeight = slideTexture.height;
        int imageWidgth = slideTexture.width;

        float rectPosX = slideImage.rectTransform.anchoredPosition.x;
        float rectPosY = slideImage.rectTransform.anchoredPosition.y;

        byte currentColorRed = ApplicationController.instance.currentColor.r;
        byte currentColorGreen = ApplicationController.instance.currentColor.g;
        byte currentColorBlue = ApplicationController.instance.currentColor.b;
        byte currentColorAlpha = ApplicationController.instance.currentColor.a;

        Color32 matchingColor = new Color32(255, 255, 255, 0);
        Color32 wrongColor = new Color32(89, 89, 89, 200);
        
        RaycastHit2D hit;

        for (int i = 0; i < imageHeight; i++)
        {
            for (int j = 0; j < imageWidgth; j++)
            {
                int index = i * imageWidgth + j;
                byte rValue = slidePixelArray[index].r;
                byte gValue = slidePixelArray[index].g;
                byte bValue = slidePixelArray[index].b;
                byte aValue = slidePixelArray[index].a;

                if (!ApplicationController.instance.isAreaSelected)
                {
                    totalPixels++;

                    if (Mathf.Abs(currentColorRed - rValue) <= tolerance && Mathf.Abs(currentColorGreen - gValue) <= tolerance && Mathf.Abs(currentColorBlue - bValue) <= tolerance && Mathf.Abs(currentColorAlpha - aValue) <= tolerance)
                    {
                        totalMatchingPixels++;
                        selectorPixelArray[index] = matchingColor;
                    }
                    else
                    {
                        selectorPixelArray[index] = wrongColor;
                    }
                }
                else
                {
                    if (i < minY || i > maxY || j < minX || j > maxX)
                    {
                        selectorPixelArray[index] = wrongColor;
                    }
                    else
                    {
                        Vector2 pixelPosition = new Vector2(j*scaleRatio+rectPosX,i*scaleRatio+rectPosY);
                        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pixelPosition), Vector2.zero);

                        if(hit.collider == areaCollider)
                        {
                            totalPixels++;

                        }

                        if (hit.collider == areaCollider && Mathf.Abs(currentColorRed - rValue) <= tolerance && Mathf.Abs(currentColorGreen - gValue) <= tolerance && Mathf.Abs(currentColorBlue - bValue) <= tolerance && Mathf.Abs(currentColorAlpha - aValue) <= tolerance)
                        {
                            totalMatchingPixels++;
                            selectorPixelArray[index] = matchingColor;
                        }
                        else
                        {
                            selectorPixelArray[index] = wrongColor;
                        }
                    }
                }
               
            }
        }

        selectionAreaPercentage = 100*((float)totalMatchingPixels / (float)totalPixels);

        slideData.selectionTolerance = tolerance;
        slideData.selectedAreaPercentage = selectionAreaPercentage;
        slideData.currentColorPixels = totalMatchingPixels;
        slideData.totalPixels = totalPixels;
        slideData.currentImageColor = ApplicationController.instance.currentColor;
        slideData.AddDataLine();

        colorSelectorTexture.SetPixels32(selectorPixelArray);
        colorSelectorTexture.Apply();

        selectorImage.texture = colorSelectorTexture;

        ApplicationController.instance.DisplayInfoPanel(slideData);

        slideCollider.enabled = true;
    }

    public void DeselectPixels()
    {
        selectorImage.gameObject.SetActive(false);
        ApplicationController.instance.infoPanel.SetActive(false);
    }

    public void FinishSelectingArea()
    {
        ApplicationController.instance.isAreaSelected = true;
        Vector2[] colliderPoints = areaPoints.ToArray();
        //Camera.main.GetComponentInChildren<DrawSelectionLines>().completeSelection = true;

        areaCollider.SetPath(0, colliderPoints);
        lassoRenderer.loop = true;
        lassoRenderer.positionCount--;
    }

    public void DeselectArea()
    {
        lassoPoints.Clear();
        areaPoints.Clear();
        lassoRenderer.positionCount = 0;
        lassoRenderer.loop = false;
        areaCollider.pathCount = 0;
        //Camera.main.GetComponentInChildren<DrawSelectionLines>().completeSelection = false;
    }

    private void CenterImage()
    {
        float slideCenteredX = (screenWidth - slideImage.rectTransform.rect.width * scaleRatio) / 2;
        float slideCenteredY = (screenHeight - slideImage.rectTransform.rect.height * scaleRatio) / 2;
        slideImage.rectTransform.anchoredPosition = new Vector2(slideCenteredX, slideCenteredY);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            isDragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(slideImage.rectTransform, eventData.position, Camera.main, out Vector2 localPoint);
            dragPosition = localPoint*scaleRatio;
            dragPositionScreen = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Middle)
        {
            slideImage.rectTransform.anchoredPosition = eventData.position - dragPosition;

            Vector3 translation = Camera.main.ScreenToWorldPoint(eventData.position) - Camera.main.ScreenToWorldPoint(dragPositionScreen);

            for (int i = 0; i < lassoRenderer.positionCount; i++)
            {
                Vector3 newPosition = lassoPoints[i] + translation;
                lassoRenderer.SetPosition(i, new Vector3(newPosition.x,newPosition.y,-5f));
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        dragPosition = new Vector2(0f,0f);
        dragPositionScreen = new Vector3(0f, 0f, -5f);

        for(int i = 0; i < lassoPoints.Count; i++)
        {
            lassoPoints[i] = lassoRenderer.GetPosition(i);
        }
    }

    public void ZoomImageIn()
    {
        scaleRatio += .1f;
        scaleRatio = Mathf.Clamp(scaleRatio, .1f, 4f);
        slideImage.rectTransform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
        CenterImage();

        for (int i = 0; i < lassoPoints.Count; i++)
        {
            Vector3 pointPosition = Camera.main.ScreenToWorldPoint(areaCollider.points[i] * scaleRatio + slideImage.rectTransform.anchoredPosition);
            lassoPoints[i] = new Vector3(pointPosition.x, pointPosition.y, -5f);
            lassoRenderer.SetPosition(i, lassoPoints[i]);
        }
    }

    public void ZoomImageOut()
    {
        scaleRatio -= .1f;
        scaleRatio = Mathf.Clamp(scaleRatio, .1f, 4f);
        slideImage.rectTransform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
        CenterImage();

        for (int i = 0; i < lassoPoints.Count; i++)
        {
            Vector3 pointPosition = Camera.main.ScreenToWorldPoint(areaCollider.points[i] * scaleRatio + slideImage.rectTransform.anchoredPosition);
            lassoPoints[i] = new Vector3(pointPosition.x, pointPosition.y, -5f);
            lassoRenderer.SetPosition(i, lassoPoints[i]);
        }
    }

    public void ResetImage()
    {
        scaleRatio = initialScaleRatio;
        slideImage.rectTransform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
        CenterImage();

        for (int i = 0; i < lassoPoints.Count; i++)
        {
            Vector3 pointPosition = Camera.main.ScreenToWorldPoint(areaCollider.points[i] * scaleRatio + slideImage.rectTransform.anchoredPosition);
            lassoPoints[i] = new Vector3(pointPosition.x, pointPosition.y, -5f);
            lassoRenderer.SetPosition(i, lassoPoints[i]);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == this.gameObject)
        {
            isMouseOver = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(eventData.pointerEnter == this.gameObject)
        {
            isMouseOver = false;
        }
    }

    public void SaveLassoPoint()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != areaCollider && hit.collider != slideCollider)
                {
                    return;
                }
            }
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(slideImage.GetComponentInChildren<RectTransform>(), Input.mousePosition, Camera.main, out Vector2 localPoint);

        areaPoints.Add(localPoint);

        if (localPoint.x < minX)
        {
            minX = (int)localPoint.x;
        }
        if (localPoint.x > maxX)
        {
            maxX = (int)localPoint.x;
        }

        if (localPoint.y < minY)
        {
            minY = (int)localPoint.y;
        }
        if (localPoint.y > maxY)
        {
            maxY = (int)localPoint.y;
        }

        Vector3 drawPosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, -5f);
        lassoRenderer.SetPosition(lassoRenderer.positionCount - 1, drawPosition);
        lassoPoints.Add(drawPosition);

        lassoRenderer.positionCount++;
        Vector3 newPosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, -5f);
        lassoRenderer.SetPosition(lassoRenderer.positionCount - 1, newPosition);

    }
}
