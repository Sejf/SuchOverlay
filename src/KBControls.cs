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
            if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals)) // Adds Alpha (+ = numpad+)
            {
                if (_canvasOverlay.CurrentPlane == null)
                    return;

                Renderer rend = _canvasOverlay.CurrentPlane.GetComponent<Renderer>();
                Color c = rend.material.color;

                if (c.a > 1f)
                    c.a = 1f;
                else
                    c.a += 0.01f;
                _imageLoader.Alpha = c.a;
                rend.material.color = c;
            }

            if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus)) // Subtracts Alpha (- numpad-)
            {
                if (_canvasOverlay.CurrentPlane == null)
                    return;

                Renderer rend = _canvasOverlay.CurrentPlane.GetComponent<Renderer>();
                Color c = rend.material.color;

                if (c.a < 0f)
                    c.a = 0f;
                else
                    c.a -= 0.01f;
                _imageLoader.Alpha = c.a;
                rend.material.color = c;
            }

            if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.K)) // Change the opacity to 0 (Keypad0 K)
            {
                if (_canvasOverlay.CurrentPlane == null)
                    return;
                Renderer rend = _canvasOverlay.CurrentPlane.GetComponent<Renderer>();
                Color c = rend.material.color;

                c.a = 0f;
                _imageLoader.Alpha = c.a;
                rend.material.color = c;
            }

            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.L)) // Change the opacity to 1 (Keypad1 L)
            {
                if (_canvasOverlay.CurrentPlane == null)
                    return;
                Renderer rend = _canvasOverlay.CurrentPlane.GetComponent<Renderer>();
                Color c = rend.material.color;

                c.a = 1f;
                _imageLoader.Alpha = c.a;
                rend.material.color = c;
            }

            if (Input.GetKeyDown(KeyCode.I)) // Reload images from folder (I)
            {
                _imageLoader.LoadImages();
            }

            if (Input.GetKeyDown(KeyCode.Period)) // Go to the next image (. >)
            {
                if (_canvasOverlay.CurrentPlane == null)
                    return;

                _imageLoader.NextImage();
            }

            if (Input.GetKeyDown(KeyCode.Comma)) // Go to the previous image (, <)
            {
                if (_canvasOverlay.CurrentPlane == null)
                    return;

                _imageLoader.PreviousImage();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) // Move Overplay plane up (Up Arrow)
            {
                _canvasOverlay.CurrentPlane.transform.localPosition += new Vector3(0f, 0.1f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) // Move Overplay plane down (Down Arrow)
            {
                _canvasOverlay.CurrentPlane.transform.localPosition += new Vector3(0f, -0.1f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) // Move Overplay plane right (Right Arrow)
            {
                _canvasOverlay.CurrentPlane.transform.localPosition += new Vector3(-0.1f, 0f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) // Move Overplay plane left (Left Arrow)
            {
                _canvasOverlay.CurrentPlane.transform.localPosition += new Vector3(0.1f, 0f, 0f);
            }

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
                _canvasOverlay.PlaneDistance = Mathf.Max(_canvasOverlay.PlaneDistance, _canvasOverlay.PlaneDistanceDefault);

                Vector3 pos = _canvasOverlay.CurrentPlane.transform.localPosition;
                _canvasOverlay.CurrentPlane.transform.localPosition = new Vector3(pos.x, pos.y, _canvasOverlay.PlaneDistance);
            }
            
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Keypad5)) // Reset plane distance and position (Delete)
            {
                _canvasOverlay.PlaneDistance = _canvasOverlay.PlaneDistanceDefault;
                _canvasOverlay.CurrentPlane.transform.localPosition = new Vector3(0f, 0f, _canvasOverlay.PlaneDistanceDefault);
            }

            if (Input.GetKeyDown(KeyCode.O)) // Rotate the plane 90º (O)
            {
                _canvasOverlay.CurrentPlane.transform.Rotate(0f, 0f, 90f);
            }
            
            if (Input.GetKey(KeyCode.P)) // Add Overlay to the canvas (P)
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
            
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _imageLoader.Unlit = !_imageLoader.Unlit;
                _canvasOverlay.CurrentPlane.GetComponent<Renderer>().material = _imageLoader.EmptyMaterial();
            }
        }
        
    }
}