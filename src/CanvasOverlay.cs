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

        public float PlaneDistance { get; set; } = 0.06f;
        public readonly float PlaneDistanceDefault = 0.06f;
        
        public void AddOverlay()
        {
            if (CurrentCanvas == null)
                return;
            
            MelonLogger.Msg($"+++ {CurrentCanvas.name}");
            
            CurrentPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(CurrentPlane.GetComponent<MeshCollider>());
            CurrentPlane.name = "SuchOverlay_Plane";
            CurrentPlane.transform.SetParent(CurrentCanvas.transform, worldPositionStays: false);
            
            CurrentPlane.transform.localPosition = Vector3.forward * 0.06f; // Distance from parent drawing canvas
            CurrentPlane.transform.localRotation = Quaternion.Euler(0, 180f, 0);

            CurrentPlane.transform.localScale = DetectCanvasSize(); // Wtf is wrong with the 0.6:0.6 tiny?
            
            CurrentPlane.GetComponent<Renderer>().material = _imageLoader.EmptyMaterial();
        }
        
        public void RemoveOverlay()
        {
            if (CurrentPlane == null)
                return;
            
            MelonLogger.Msg($"--- {CurrentPlane.name}");
            
            Object.DestroyImmediate(CurrentPlane);
        }
        
        public Vector3 DetectCanvasSize()
        {
            if (CurrentCanvas == null)
                return Vector3.one;
            
            return CurrentCanvas.transform.Find("CanvasFrameDimensions")?.localScale ?? Vector3.one;
        }
    }
}