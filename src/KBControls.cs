using System.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;

namespace SuchOverlay
{
    public class KBControls : MelonMod
    {
        private CanvasOverlay _canvasOverlay;
        private ImageLoader _imageLoader;
        
        public void LoadLibs(CanvasOverlay canvasOverlay, ImageLoader imageLoader)
        {
            _canvasOverlay = canvasOverlay;
            _imageLoader = imageLoader;
        }
        
        public KBControls()
        {
            
        }

        
        private readonly Regex _generalCanvasRegex = new Regex(@"AS_Canvas_.+", RegexOptions.None);

        public void Update()
        {
            // Main functions
            if (Input.GetKey(KeyCode.P)) // Add Overlay to the canvas (P) 
            {
                AddOverlay();
            }

            if (Input.GetKeyDown(KeyCode.I)) // Reload images from folder (I)
            {
                _imageLoader.LoadImages();
            }
            
            if (_canvasOverlay.CurrentPlane == null)
                return;
            
            // Change Opacity
            if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals)) // Adds Alpha (+ = numpad+)
            {
                float alpha = _imageLoader.Alpha;

                if (alpha > 1f)
                    ChangePlaneOpacity(1f);
                else
                    alpha += 0.01f;
                
                ChangePlaneOpacity(alpha);

            }

            if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus)) // Subtracts Alpha (- numpad-)
            {
                float alpha = _imageLoader.Alpha;

                if (alpha < 0f)
                    ChangePlaneOpacity(0f);
                else
                    alpha -= 0.01f;
                
                ChangePlaneOpacity(alpha);
            }

            if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.K)) // Change the opacity to 0 (Keypad0 K)
            {
                ChangePlaneOpacity(0f);
            }

            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.L)) // Change the opacity to 1 (Keypad1 L)
            {
                ChangePlaneOpacity(1f);
            }

            
            // Change images
            if (Input.GetKeyDown(KeyCode.Period)) // Go to the next image (. >)
            {
                _imageLoader.NextImage();
            }

            if (Input.GetKeyDown(KeyCode.Comma)) // Go to the previous image (, <)
            {
                _imageLoader.PreviousImage();
            }

            
            // Movement
            if (Input.GetKeyDown(KeyCode.UpArrow)) // Move Overplay plane up (Up Arrow)
            {
                MovePlane(new Vector3(0f, 0.1f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) // Move Overplay plane down (Down Arrow)
            {
                MovePlane(new Vector3(0f, -0.1f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) // Move Overplay plane right (Right Arrow)
            {
                MovePlane(new Vector3(-0.1f, 0f, 0f));
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) // Move Overplay plane left (Left Arrow)
            {
                MovePlane(new Vector3(0.1f, 0f, 0f));
            }

            
            // Distance from canvas
            if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.Keypad7)) // Move overlay forward (PageUp Keypad7)
            {
                _canvasOverlay.PlaneDistance += 0.01f;
                _canvasOverlay.PlaneDistance = Mathf.Min(_canvasOverlay.PlaneDistance, 1f);

                Vector3 pos = _canvasOverlay.CurrentPlane.transform.localPosition;
                _canvasOverlay.CurrentPlane.transform.localPosition = new Vector3(pos.x, pos.y, _canvasOverlay.PlaneDistance);
            }

            if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.Keypad4)) // Move overlay backwards (PageDown Keypad4)
            {
                _canvasOverlay.PlaneDistance -= 0.01f;
                _canvasOverlay.PlaneDistance = Mathf.Max(_canvasOverlay.PlaneDistance, 0.01f);

                Vector3 pos = _canvasOverlay.CurrentPlane.transform.localPosition;
                _canvasOverlay.CurrentPlane.transform.localPosition = new Vector3(pos.x, pos.y, _canvasOverlay.PlaneDistance);
            }
            
            // Rotation
            if (Input.GetKeyDown(KeyCode.O)) // Rotate the plane 90º (O)
            {
                _canvasOverlay.CurrentPlane.transform.Rotate(0f, 0f, 90f);
            }
            
            // Scale
            if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.LeftBracket)) // Increase the size of the Overlay (Keypad9 [)
            {
                _canvasOverlay.ScaleFactor += 0.01f;
                UpdatePlaneScale();
            }

            if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightBracket)) // Decrease the size of the Overlay (Keypad6 ])
            {
                _canvasOverlay.ScaleFactor = Mathf.Max(0.01f, _canvasOverlay.ScaleFactor - 0.01f);
                UpdatePlaneScale();
            }
            
            // Reset
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Keypad5)) // Reset plane distance, position and scale (Delete Keypad5)
            {
                // Positon Z
                _canvasOverlay.PlaneDistance = _canvasOverlay.PlaneDistanceDefault;
                // Position X Y
                _canvasOverlay.CurrentPlane.transform.localPosition = new Vector3(0f ,0f ,_canvasOverlay.PlaneDistance);
                // Rotation Y
                _canvasOverlay.CurrentPlane.transform.localRotation = Quaternion.Euler(0, 180f, 0);
                // Scale X Y
                _canvasOverlay.ScaleFactor = 1f;
                UpdatePlaneScale();
            }

            // Shaders
            if (Input.GetKeyDown(KeyCode.F1)) // Switch between the Lit and Unlit shaders
            {
                _imageLoader.UnlitShader = !_imageLoader.UnlitShader;
                _canvasOverlay.CurrentPlane.GetComponent<Renderer>().material = _imageLoader.EmptyMaterial();
                _imageLoader.PreviousImage();
                _imageLoader.NextImage();
            }
        }

        private void AddOverlay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject obj = hit.transform.gameObject;
                    
                if (_canvasOverlay.CurrentCanvas != obj)
                {
                    if (_generalCanvasRegex.IsMatch(obj.name))
                    {
                        _imageLoader.Alpha = 1f;
                        _canvasOverlay.RemoveOverlay();
                        _canvasOverlay.CurrentCanvas = obj;
                        _canvasOverlay.AddOverlay();
                    }
                }
            }
        }

        private void ChangePlaneOpacity(float alpha)
        {
            Renderer rend = _canvasOverlay.CurrentPlane.GetComponent<Renderer>();
            Color c = rend.material.color;

            c.a = alpha;
            _imageLoader.Alpha = c.a;
            rend.material.color = c;
        }
        
        private void MovePlane (Vector3 position)
        {
            _canvasOverlay.CurrentPlane.transform.localPosition += position;
        }
        
        private void UpdatePlaneScale()
        {
            if (_canvasOverlay.CurrentPlane == null || _imageLoader.CurrentTexture == null)
                return;
            
            Vector3 scale = _canvasOverlay.DetectCanvasSize();
            float planeWidth = scale.x;
            float planeHeight = scale.y;
            
            float textureRatio = (float)_imageLoader.CurrentTexture.width / _imageLoader.CurrentTexture.height;
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
            
            scale.x *= _canvasOverlay.ScaleFactor;
            scale.y *= _canvasOverlay.ScaleFactor;
            
            scale.z = 1f;

            _canvasOverlay.CurrentPlane.transform.localScale = scale;
        }
        
    }
    
}