using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class ApplicationController : MonoBehaviour
{
    public static ApplicationController instance;

    //MENU FIELDS
    //side menu tolerance settings
    public int toleranceSetting = 0;
    public TMP_InputField toleranceInput;

    //SIDE MENU
    //side menu main swatch
    public Image colorDisplay;

    //side menu all buttons
    public List<UnityEngine.UI.Button> allSideMenuButtons = new List<UnityEngine.UI.Button>();

    //side menu cursor button
    public UnityEngine.UI.Button cursorButton;

    //side menu lasso button
    public UnityEngine.UI.Button lassoButton;

    //top menu file panel
    public GameObject filePanel;
    
    //workspace file tab
    public GameObject workspaceTab;
    public GameObject workspaceTabContent;
    //tab prefab
    public GameObject slideTab;


    [HideInInspector]
    public Color32 currentColor;

    //swatch channels
    public TMP_InputField redChannelText;
    public TMP_InputField greenChannelText;
    public TMP_InputField blueChannelText;
    public TMP_InputField alphaChannelText;

    //IMAGE CANVAS
    public Canvas imageCanvas;

    //active image
    public int activeSlideIndex;
    public GameObject activeSlideImage;
    public ImageOperations activeImageOperations;
    //all images
    public List<(GameObject,GameObject)> openTabs = new List<(GameObject,GameObject)>();


    //lasso selection bool
    public bool isAreaSelected;
    //color selection bool
    public bool isColorSelected;

    //image overlay
    //image info panel
    public GameObject infoPanel;

    //info panel parts
    public TextMeshProUGUI percentageLabelText;
    public TextMeshProUGUI percentageText;
    public TextMeshProUGUI areaLabelText;
    public TextMeshProUGUI areaText;

    //IMAGE PREFAB
    public GameObject slideImage;

    //POPUP PANEL
    public GameObject quitPanel;

    public BaseState currentState;
    public List<BaseState> stateQueue = new List<BaseState>();
    public StartupState startupState = new StartupState();
    public CursorState cursorState = new CursorState();
    public LassoState lassoState = new LassoState();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        allSideMenuButtons.Add(cursorButton);
        allSideMenuButtons.Add(lassoButton);
    }

    void Start()
    {
        TransitionToState(startupState);
    }

    void Update()
    {
        currentState.Update(this);

    }
    
    public void TransitionToState(BaseState newState)
    {
        currentState = newState;
        currentState.OnStateEnter(this);
    }

    public void SetCurrentColor(Color32 newColor)
    {
        currentColor = newColor;
        colorDisplay.color = newColor;

        if (slideImage.activeInHierarchy)
        {
            isColorSelected = true;
            activeSlideImage.GetComponentInChildren<ImageOperations>().HighlightToleranceArea();

        }

        redChannelText.text = currentColor.r.ToString();
        greenChannelText.text = currentColor.g.ToString();
        blueChannelText.text = currentColor.b.ToString();
        alphaChannelText.text = currentColor.a.ToString();

    }


    //FILE BROWSER CODE
#if UNITY_EDITOR
#else
    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();
#endif

    public void LoadImage()
    {
        Texture2D imageTexture = new Texture2D(1, 1);
        StartCoroutine(LoadingCoroutine(imageTexture));
    }

    public void SaveImage()
    {
        StartCoroutine(SavingCoroutine());

    }

    public void SaveData()
    {
        StartCoroutine(DataSavingCoroutine());
    }

    private IEnumerator LoadingCoroutine(Texture2D imageTexture)
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Select slide", "", "png,jpg");
#else
        System.Windows.Forms.Application.EnableVisualStyles();
        
        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

        openFileDialog.InitialDirectory = "c:\\";
        openFileDialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
        openFileDialog.RestoreDirectory = true;
        openFileDialog.AutoUpgradeEnabled = true;

#endif

        yield return new WaitForSeconds(.1f);

#if UNITY_EDITOR
        if (path.Length != 0)
        {
            (GameObject, GameObject) newSlideTab = CreateNewSlide(path);
            RawImage rawImage = newSlideTab.Item2.GetComponentInChildren<RawImage>();

            var fileContent = File.ReadAllBytes(path);
            imageTexture.LoadImage(fileContent);
            rawImage.texture = imageTexture;
            rawImage.SetNativeSize();

            foreach ((GameObject, GameObject) openSlide in openTabs)
            {
                openSlide.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = true;
                openSlide.Item2.SetActive(false);
            }

            newSlideTab.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = false;
            newSlideTab.Item2.SetActive(true);

            newSlideTab.Item2.GetComponentInChildren<ImageOperations>().RefreshImage();

            activeSlideIndex = openTabs.IndexOf(newSlideTab);
            activeSlideImage = newSlideTab.Item2;
            activeImageOperations = activeSlideImage.GetComponentInChildren<ImageOperations>();

            SlideFileData newSlideData = new SlideFileData();

            newSlideData.filePath = path;

            newSlideData.slideFileDataLines = new List<SlideFileDataLine>();
            newSlideData.imageData = fileContent;
            activeImageOperations.slideData = newSlideData;

            filePanel.SetActive(false);
        }
        else
        {
            filePanel.SetActive(false);
        }


#else
        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string path = openFileDialog.FileName;
            
            (GameObject, GameObject) newSlideTab = CreateNewSlide(path);
            RawImage rawImage = newSlideTab.Item2.GetComponentInChildren<RawImage>();

            var fileContent = File.ReadAllBytes(path);
            imageTexture.LoadImage(fileContent);
            rawImage.texture = imageTexture;
            rawImage.SetNativeSize();

            foreach ((GameObject, GameObject) openSlide in openTabs)
            {
                openSlide.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = true;
                openSlide.Item2.SetActive(false);
            }

            newSlideTab.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = false;
            newSlideTab.Item2.SetActive(true);

            newSlideTab.Item2.GetComponentInChildren<ImageOperations>().RefreshImage();

            activeSlideIndex = openTabs.IndexOf(newSlideTab);
            activeSlideImage = newSlideTab.Item2;
            activeImageOperations = activeSlideImage.GetComponentInChildren<ImageOperations>();

            SlideFileData newSlideData = new SlideFileData();

            newSlideData.filePath = path;

            newSlideData.slideFileDataLines = new List<SlideFileDataLine>();
            newSlideData.imageData = fileContent;
            activeImageOperations.slideData = newSlideData;

            filePanel.SetActive(false);
        }
        else
        {
            filePanel.SetActive(false);


        }
#endif
    }

    private IEnumerator SavingCoroutine()
    {
#if UNITY_EDITOR
        string path = EditorUtility.SaveFilePanel("Save Image", "", "Untitled.png", "png");
#else
        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

        saveFileDialog.InitialDirectory = "c:\\";
        saveFileDialog.Filter = "JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp";
        saveFileDialog.FilterIndex = 2;
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.AutoUpgradeEnabled = true;

#endif
        yield return new WaitForEndOfFrame();
        //Texture2D imageTexture = new Texture2D(UnityEngine.Screen.width, UnityEngine.Screen.height, TextureFormat.RGB24, false);
        //imageTexture.ReadPixels(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), 0, 0);
        //imageTexture.Apply();

        ImageOperations colorChecker = activeSlideImage.GetComponentInChildren<ImageOperations>();

        Color32[] slidePixelArray = colorChecker.slideTexture.GetPixels32();
        Color32[] selectorPixelArray = colorChecker.colorSelectorTexture.GetPixels32();
        Color32[] combinedPixelArray = colorChecker.combinedTexture.GetPixels32();

        int imageHeight = colorChecker.slideTexture.height;
        int imageWidgth = colorChecker.slideTexture.width;

        for (int i = 0; i < imageHeight; i++)
        {
            for (int j = 0; j < imageWidgth; j++)
            {
                int index = i * imageWidgth + j;
                combinedPixelArray[index] = Color32.Lerp(slidePixelArray[index], Color.black, selectorPixelArray[index].a / 255f);
            }
        }

        colorChecker.combinedTexture.SetPixels32(combinedPixelArray);
        colorChecker.combinedTexture.Apply();

        //colorChecker.combinedImage.texture = colorChecker.combinedTexture;

        byte[] fileContent = colorChecker.combinedTexture.EncodeToPNG();

        //slidePixelArray = null;
        //selectorPixelArray = null;
        //combinedPixelArray = null;

        yield return new WaitForSeconds(.1f);
#if UNITY_EDITOR
        if (path.Length != 0)
        {
            File.WriteAllBytes(path, fileContent);
        }

        filePanel.SetActive(false);
#else
        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string path = saveFileDialog.FileName;
            
            if(path != null)
            {
                File.WriteAllBytes(path,fileContent);
            }
        }
#endif

    }

    private IEnumerator DataSavingCoroutine()
    {
#if UNITY_EDITOR
        string path = EditorUtility.SaveFilePanel("Save Data", "", "Untitled.txt", "txt");
#else
        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

        saveFileDialog.InitialDirectory = "c:\\";
        saveFileDialog.Filter = "TXT(*.txt)|*.txt";
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.AutoUpgradeEnabled = true;

#endif
        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        if (path.Length != 0)
        {
            ImageOperations colorChecker = activeSlideImage.GetComponentInChildren<ImageOperations>();

            List<SlideFileDataLine> dataLinesToWrite = colorChecker.slideData.slideFileDataLines;

            using(StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine(colorChecker.slideData.filePath);
                streamWriter.WriteLine("Color (RGBA)" + "\t" + "Selection Tolerance" + "\t" + "Area Percentage (%)" + "\t" + "Selected Pixels (#)" + "\t" + "Total Pixels (#)");

                foreach(SlideFileDataLine dataLine in dataLinesToWrite)
                {
                    string stringToWrite = dataLine.selectedColor.ToString() + "\t" + dataLine.selectionTolerance + "\t" + dataLine.selectedAreaPercentage.ToString() + "\t" + dataLine.selectedColorPixels.ToString() + "\t" + dataLine.totalPixels.ToString();

                    streamWriter.WriteLine(stringToWrite);
                }
            }
        }

        filePanel.SetActive(false);
#else
        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string path = saveFileDialog.FileName;
            
            if(path != null)
            {
                ImageOperations colorChecker = activeSlideImage.GetComponentInChildren<ImageOperations>();

                List<SlideFileDataLine> dataLinesToWrite = colorChecker.slideData.slideFileDataLines;

                using(StreamWriter streamWriter = new StreamWriter(path))
                {
                    streamWriter.WriteLine(colorChecker.slideData.filePath);
                    streamWriter.WriteLine("Color (RGBA)" + "\t" + "Selection Tolerance" + "\t" + "Area Percentage (%)" + "\t" + "Selected Pixels (#)" + "\t" + "Total Pixels (#)");

                    foreach(SlideFileDataLine dataLine in dataLinesToWrite)
                    {
                        string stringToWrite = dataLine.selectedColor.ToString() + "\t" + dataLine.selectionTolerance + "\t" + dataLine.selectedAreaPercentage.ToString() + "\t" + dataLine.selectedColorPixels.ToString() + "\t" + dataLine.totalPixels.ToString();

                        streamWriter.WriteLine(stringToWrite);
                    }
                }
            }
        }
#endif

    }


    private (GameObject, GameObject) CreateNewSlide(string filePath)
    {
        GameObject newSlide = Instantiate(slideImage);
        newSlide.transform.SetParent(imageCanvas.transform);

        GameObject newTab = Instantiate(slideTab);
        newTab.transform.SetParent(workspaceTabContent.transform,false);
        newTab.transform.Find("SlideFileName").GetComponentInChildren<TextMeshProUGUI>().text = filePath.Substring(filePath.LastIndexOf("\\")+1);

        (GameObject, GameObject) newSlideTab = (newTab, newSlide);
        openTabs.Add(newSlideTab);

        int newSlideIndex = openTabs.Count - 1;
        newTab.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => SetActiveSlide(newSlideTab));

        newTab.transform.Find("SlideCloseButton").GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => CloseSlide(newSlideTab));

        foreach ((GameObject,GameObject)openSlide in openTabs)
        {
            openSlide.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = true;
            openSlide.Item2.SetActive(false);
        }

        return newSlideTab;
    }

    public void SetActiveSlide((GameObject, GameObject) slideToSetActive)
    {
        activeImageOperations.isAreaSelected = isAreaSelected;
        activeImageOperations.currentImageColor = currentColor;
        activeImageOperations.isColorSelected = isColorSelected;

        foreach ((GameObject, GameObject) openSlide in openTabs)
        {
            openSlide.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = true;
            openSlide.Item2.SetActive(false);
        }

        slideToSetActive.Item1.GetComponentInChildren<UnityEngine.UI.Button>().interactable = false;
        slideToSetActive.Item2.SetActive(true);

        activeSlideIndex = openTabs.IndexOf(slideToSetActive);
        activeSlideImage = slideToSetActive.Item2;
        activeImageOperations = activeSlideImage.GetComponentInChildren<ImageOperations>();
        isAreaSelected = activeImageOperations.isAreaSelected;
        isColorSelected = activeImageOperations.isColorSelected;
        currentColor = activeImageOperations.currentImageColor;

        if (isColorSelected)
        {
            DisplayInfoPanel(activeImageOperations.slideData);

        }
        else
        {
            HideInfoPanel();
        }

        TransitionToState(currentState);
    }

    public void SetTolerance()
    {
        int.TryParse(toleranceInput.text, out toleranceSetting);
        if (toleranceSetting > 255)
        {
            toleranceSetting = 255;
        }
        toleranceInput.text = toleranceSetting.ToString();
    }

    public void ChangeChannelValue(string channelName)
    {
        if (channelName == "red")
        {
            byte channelValue = 0;
            byte.TryParse(redChannelText.text, out channelValue);
            if (channelValue > 255)
            {
                channelValue = 255;
            }
            else if (channelValue < 0)
            {
                channelValue = 0;
            }
            currentColor.r = channelValue;
            redChannelText.text = currentColor.r.ToString();
        }
        else if (channelName == "green")
        {
            byte channelValue = 0;
            byte.TryParse(greenChannelText.text, out channelValue);
            if (channelValue > 255)
            {
                channelValue = 255;
            }
            else if (channelValue < 0)
            {
                channelValue = 0;
            }
            currentColor.g = channelValue;
            greenChannelText.text = currentColor.g.ToString();
        }
        else if (channelName == "blue")
        {
            byte channelValue = 0;
            byte.TryParse(blueChannelText.text, out channelValue);
            if (channelValue > 255)
            {
                channelValue = 255;
            }
            else if (channelValue < 0)
            {
                channelValue = 0;
            }
            currentColor.b = channelValue;
            blueChannelText.text = currentColor.b.ToString();
        }
        else if (channelName == "alpha")
        {
            byte channelValue = 0;
            byte.TryParse(alphaChannelText.text, out channelValue);
            if (channelValue > 255)
            {
                channelValue = 255;
            }
            else if (channelValue < 0)
            {
                channelValue = 0;
            }
            currentColor.a = channelValue;
            alphaChannelText.text = currentColor.a.ToString();
        }

        colorDisplay.color = currentColor;
        if (activeImageOperations != null)
        {
            activeImageOperations.HighlightToleranceArea();
        }
    }

    public void CloseSlide((GameObject, GameObject) slideToRemove)
    {
        int slideIndex = openTabs.IndexOf(slideToRemove);
        if (slideIndex != activeSlideIndex)
        {

        }
        else if (slideIndex > 0)
        {
            SetActiveSlide(openTabs[slideIndex - 1]);
        }
        else if(slideIndex == 0 && openTabs.Count > 1)
        {
            SetActiveSlide(openTabs[1]);
        }
        else
        {
            HideInfoPanel();
        }
        openTabs.RemoveAt(slideIndex);

        ImageOperations slideToRemoveImageOperations = slideToRemove.Item2.GetComponentInChildren<ImageOperations>();
        Destroy(slideToRemoveImageOperations.slideTexture);
        Destroy(slideToRemoveImageOperations.colorSelectorTexture);
        Destroy(slideToRemoveImageOperations.combinedTexture);
        Destroy(slideToRemoveImageOperations.slideImage.texture);

        Destroy(slideToRemove.Item2);
        Destroy(slideToRemove.Item1);

    }

    public void CursorButtonClick()
    {
        TransitionToState(cursorState);
    }

    public void LassoButtonClick()
    {
        TransitionToState(lassoState);
    }

    public void DisplayInfoPanel(SlideFileData slideData)
    {
        infoPanel.SetActive(true);
        percentageText.text = slideData.selectedAreaPercentage.ToString("f") + "%";
        areaText.text = slideData.currentColorPixels.ToString() + "/" + slideData.totalPixels.ToString();
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
    }
}
