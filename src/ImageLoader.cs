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
        public bool UnlitShader { get; set; } = false;
        public Texture2D CurrentTexture { get; set; }
        
        private readonly string _modFolder = Path.Combine(MelonEnvironment.ModsDirectory, "SuchOverlay");
        private readonly List<string> _imagesPaths = new List<string>();
        private int _currentIndex = 0;
        
        public Material EmptyMaterial()
        {
            Material mat;
            
            if (UnlitShader)
            {
                mat = new Material(Shader.Find("Sprites/Default"));
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
            else
            {
                mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(1f, 0f, 1f, Alpha);
                mat.SetFloat("_Mode", 3);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            
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
            
            ChangeTexture(_currentIndex);
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
            
            ChangeTexture(_currentIndex);
        }

        private Material LoadTexture(string texturePath, float brightness = 1.0f)
        {
            byte[] bytes = File.ReadAllBytes(texturePath);
            
            CurrentTexture = new Texture2D(2, 2);
            CurrentTexture.LoadImage(bytes);
            _canvasOverlay.SetPlaneAspectRatio(CurrentTexture);
            
            Material mat = EmptyMaterial();
            mat.color = new Color(brightness, brightness, brightness, Alpha);
            
            mat.mainTexture = CurrentTexture;
            
            return mat;
        }

        private void ChangeTexture(int index)
        {
            if (UnlitShader) 
                _canvasOverlay.CurrentPlane.GetComponent<Renderer>().material = LoadTexture(_imagesPaths[index], 0.5f);    
            else
                _canvasOverlay.CurrentPlane.GetComponent<Renderer>().material = LoadTexture(_imagesPaths[index]);
        }
    }
}