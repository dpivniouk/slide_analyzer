using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideFileData
{
    //the file path for the slide
    public string filePath;

    //the image for the slide
    public byte[] imageData;

    //the color selected
    public Color32 currentImageColor;

    public int selectionTolerance;

    //area stats
    public float selectedAreaPercentage;
    public int currentColorPixels;
    public int totalPixels;

    public List<SlideFileDataLine> slideFileDataLines;

    public void AddDataLine()
    {
        SlideFileDataLine newDataLine = new SlideFileDataLine(currentImageColor, selectionTolerance, selectedAreaPercentage, currentColorPixels, totalPixels);

        slideFileDataLines.Add(newDataLine);
    }
}

[System.Serializable]
public class SlideFileDataLine
{
    public Color32 selectedColor;
    public int selectionTolerance;
    public float selectedAreaPercentage;
    public int selectedColorPixels;
    public int totalPixels;

    public SlideFileDataLine(Color32 color, int tolerance, float percentage, int selectedPixels, int totalPixels)
    {
        this.selectedColor = color;
        this.selectionTolerance = tolerance;
        this.selectedAreaPercentage = percentage;
        this.selectedColorPixels = selectedPixels;
        this.totalPixels = totalPixels;
    }
}
