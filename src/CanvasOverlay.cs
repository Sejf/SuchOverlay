using MelonLoader;
using UnityEngine;

namespace SuchOverlay
{
    public class CanvasOverlay : MelonMod
    {
        private ImageLoader _imageLoader;

        public CanvasOverlay()
        {
            _imageLoader = new ImageLoader();
        }

        public GameObject CurrentCanvas { get; set; }
        public GameObject CurrentPlane { get; set; }
        
        public float PlaneDistance { get; set; } = 0.09f;
        public readonly float PlaneDistanceDefault = 0.09f;
        public Vector3 CurrentScale { get; set; }
        public float ScaleFactor { get; set; } = 1.0f;
        
        public void AddOverlay()
        {
            if (CurrentCanvas == null)
                return;
            
            MelonLogger.Msg($"+++ {CurrentCanvas.name}");
            
            CurrentPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(CurrentPlane.GetComponent<MeshCollider>());
            CurrentPlane.name = "SuchOverlay_Plane";
            
            CurrentPlane.transform.SetParent(CurrentCanvas.transform, worldPositionStays: false);
            
            CurrentPlane.transform.localPosition = Vector3.forward * 0.09f; // Distance from parent drawing canvas
            CurrentPlane.transform.localRotation = Quaternion.Euler(0, 180f, 0);

            CurrentPlane.transform.localScale = DetectCanvasSize(); // Wtf is wrong with the 0.6:0.6 tiny?
            
            CurrentPlane.GetComponent<Renderer>().material = _imageLoader.EmptyMaterial();
        }
        
        public void RemoveOverlay()
        {
            ScaleFactor = 1f;
            PlaneDistance = PlaneDistanceDefault;
            
            if (CurrentPlane == null)
                return;
            
            MelonLogger.Msg($"--- {CurrentPlane.name}");
            
            Object.DestroyImmediate(CurrentPlane);
        }
        
        public void SetPlaneAspectRatio(Texture2D tex)
        {
            if (CurrentPlane == null)
                return;

            Vector3 scale = DetectCanvasSize();
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
            
            scale *= ScaleFactor;

            CurrentScale = scale;
            CurrentPlane.transform.localScale = CurrentScale;
        }
        
        
        public Vector3 DetectCanvasSize()
        {
            if (CurrentCanvas == null)
                return Vector3.one;
            
            return CurrentCanvas.transform.Find("CanvasFrameDimensions")?.localScale ?? Vector3.one;
        }
    }
}