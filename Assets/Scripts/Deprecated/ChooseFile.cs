//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEditor;
//using TMPro;

//public class ChooseFile : MonoBehaviour
//{
//#if UNITY_EDITOR
//#else
//    [DllImport("user32.dll")]
//    private static extern void OpenFileDialog();
//#endif

//    public RawImage slideImage;
//    public RawImage colorSelectorImage;
//    public RawImage combinedImage;
//    public GameObject filePanel;
//    public GameObject workspaceTab;
//    public UnityEngine.UI.Button lassoButton;

//    private string path;
//    private Texture2D imageTexture;

//    void Start()
//    {
//        if (imageTexture != null)
//        {
//            GameObject.Destroy(imageTexture);
//        }

//        imageTexture = new Texture2D(1, 1);
//        System.Windows.Forms.Application.EnableVisualStyles();
//    }

//    public void OpenBrowser()
//    {
//        StartCoroutine(SelectImage());
//    }

//    public void OpenSaveBrowser()
//    {
//        StartCoroutine(SaveImage());
//    }

//    private IEnumerator SelectImage()
//    {
//#if UNITY_EDITOR
//        path = EditorUtility.OpenFilePanel("Select slide", "", "png,jpg");
//#else
//        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

//        openFileDialog.InitialDirectory = "c:\\";
//        openFileDialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
//        openFileDialog.RestoreDirectory = false;
//        openFileDialog.AutoUpgradeEnabled = true;

//#endif

//        yield return new WaitForSeconds(.1f);

//#if UNITY_EDITOR
//        if (path.Length != 0)
//        {
//            workspaceTab.GetComponentInChildren<TextMeshProUGUI>().text = path.ToString();
//            var fileContent = File.ReadAllBytes(path);
//            imageTexture.LoadImage(fileContent);
//            slideImage.texture = imageTexture;
//            slideImage.SetNativeSize();
//            slideImage.gameObject.SetActive(true);
//            slideImage.GetComponentInChildren<ColorChecker>().RefreshImage();
//            filePanel.SetActive(false);
//            lassoButton.interactable = true;
//        }
//        else
//        {
//            filePanel.SetActive(false);

//        }


//#else
//        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//        {
//            path = openFileDialog.FileName;
//            workspaceTab.GetComponentInChildren<TextMeshProUGUI>().text = path.ToString();

//            var fileContent = File.ReadAllBytes(path);

//            imageTexture.LoadImage(fileContent);
//            slideImage.texture = imageTexture;
//            slideImage.SetNativeSize();
//            slideImage.gameObject.SetActive(true);
//            slideImage.GetComponentInChildren<ColorChecker>().RefreshImage();
//            filePanel.SetActive(false);
//            lassoButton.interactable = true;
//        }
//        else
//        {
//            slideImage.gameObject.SetActive(true);
//            filePanel.SetActive(false);

//        }
//#endif
//    }

//    private IEnumerator SaveImage()
//    {
//#if UNITY_EDITOR
//        path = EditorUtility.SaveFilePanel("Save Image", "", "Untitled.png", "png");
//#else
//        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

//        saveFileDialog.InitialDirectory = "c:\\";
//        saveFileDialog.Filter = "JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp";
//        saveFileDialog.FilterIndex = 2;
//        saveFileDialog.RestoreDirectory = true;
//        saveFileDialog.AutoUpgradeEnabled = true;

//#endif
//        yield return new WaitForEndOfFrame();
//        //Texture2D imageTexture = new Texture2D(UnityEngine.Screen.width, UnityEngine.Screen.height, TextureFormat.RGB24, false);
//        //imageTexture.ReadPixels(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), 0, 0);
//        //imageTexture.Apply();

//        ColorChecker colorChecker = slideImage.GetComponentInChildren<ColorChecker>();

//        Color32[] slidePixelArray = colorChecker.slideTexture.GetPixels32();
//        Color32[] selectorPixelArray = colorChecker.colorSelectorTexture.GetPixels32();
//        Color32[] combinedPixelArray = colorChecker.combinedTexture.GetPixels32();

//        int imageHeight = colorChecker.slideTexture.height;
//        int imageWidgth = colorChecker.slideTexture.width;

//        for (int i = 0; i < imageHeight; i++)
//        {
//            for (int j = 0; j < imageWidgth; j++)
//            {
//                int index = i * imageWidgth + j;
//                combinedPixelArray[index] = Color32.Lerp(slidePixelArray[index], Color.black, selectorPixelArray[index].a / 255f);
//            }
//        }

//        colorChecker.combinedTexture.SetPixels32(combinedPixelArray);
//        colorChecker.combinedTexture.Apply();

//        //colorChecker.combinedImage.texture = colorChecker.combinedTexture;

//        byte[] fileContent = colorChecker.combinedTexture.EncodeToPNG();

//        //slidePixelArray = null;
//        //selectorPixelArray = null;
//        //combinedPixelArray = null;

//        yield return new WaitForSeconds(.1f);
//#if UNITY_EDITOR
//        if (path.Length != 0)
//        {
//            File.WriteAllBytes(path, fileContent);
//        }

//        filePanel.SetActive(false);
//#else
//        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//        {
//            path = saveFileDialog.FileName;
            
//            if(path != null)
//            {
//                File.WriteAllBytes(path,fileContent);
//            }
//        }
//#endif

//    }

//}
