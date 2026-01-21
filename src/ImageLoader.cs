using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace SuchOverlay
{
    public class ImageLoader : MelonMod
    {
        private CanvasOverlay _canvasOverlay;
        
        public void LoadLibs(CanvasOverlay canvasOverlay)
        {
            _canvasOverlay = canvasOverlay;
        }
        
        public ImageLoader()
        {
        }
        

        public float Alpha { get; set; } = 0.5f;
        public bool Unlit { get; set; } = false;
        
        private readonly List<string> _imagesPaths = new List<string>();
        private readonly string _modFolder = Path.Combine(MelonEnvironment.ModsDirectory, "SuchOverlay");
        private int _currentIndex = 0;
        
        public Material EmptyMaterial()
        {
            Material mat;
            
            if (Unlit)
                mat = new Material(Shader.Find("UI/Default"));
            else
                mat = new Material(Shader.Find("Standard"));
            
            mat.color = new Color(1f, 0f, 1f, Alpha);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            
            mat.renderQueue = 3000;

            return mat;
        }

        public void LoadImages()
        {
            if (!Directory.Exists(_modFolder))
            {
                Directory.CreateDirectory(_modFolder);
            }

            _imagesPaths.Clear();
            _currentIndex = 0;
            
            string[] allFiles = Directory.GetFiles(_modFolder, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string file in allFiles)
            {
                if (file.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase))
                {
                    _imagesPaths.Add(file);
                }
            }

            MelonLogger.Msg($"[ImageLoader] Found {_imagesPaths.Count} images");
        }

        public void NextImage()
        {
            if (_imagesPaths.Count == 0)
            {
                LoadImages();
                if (_imagesPaths.Count == 0)
                    return;
            }
            
            _currentIndex++;
            if (_currentIndex >= _imagesPaths.Count)
                _currentIndex = 0;

            _canvasOverlay.CurrentPlane.GetComponent<Renderer>().material = LoadTexture(_imagesPaths[_currentIndex]);
        }
        
        public void PreviousImage()
        {
            if (_imagesPaths.Count == 0)
            {
                LoadImages();
                if (_imagesPaths.Count == 0)
                    return;
            }
            
            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _imagesPaths.Count - 1;
            
            _canvasOverlay.CurrentPlane.GetComponent<Renderer>().material = LoadTexture(_imagesPaths[_currentIndex]);
            
        }

        private Material LoadTexture(string texturePath)
        {
            byte[] bytes = File.ReadAllBytes(texturePath);
            
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            SetPlaneAspectRatio(tex);
            
            Material mat = EmptyMaterial();
            mat.color = new Color(1f, 1f, 1f, Alpha);
            mat.mainTexture = tex;
            
            return mat;
        }
        
        private void SetPlaneAspectRatio(Texture2D tex)
        {
            if (_canvasOverlay.CurrentPlane == null || tex == null)
                return;
            
            Vector3 scale = _canvasOverlay.DetectCanvasSize();
            float planeWidth = scale.x;
            float planeHeight = scale.y;
            
            float textureRatio = (float)tex.width / tex.height;
            float planeRatio = planeWidth / planeHeight;

            if (textureRatio > planeRatio)
            {
                scale.x = planeWidth;
                scale.y = planeWidth / textureRatio;
            }
            else
            {
                scale.y = planeHeight;
                scale.x = planeHeight * textureRatio;
            }

            _canvasOverlay.CurrentPlane.transform.localScale = scale;
        }
        
    }
}