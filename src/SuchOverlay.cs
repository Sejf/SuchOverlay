using MelonLoader;

[assembly: MelonInfo(typeof(SuchOverlay.SuchOverlay), "SuchOverlay", "1.0.0", "Envy")]
[assembly: MelonGame("Voolgi", "SuchArt")]

namespace SuchOverlay
{
    public class SuchOverlay : MelonMod
    {
        private CanvasOverlay _canvasOverlay;
        private KBControls _kbControls;
        private ImageLoader _imageLoader;
        
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Mod Started!");
        }
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        { 
            if (sceneName.Equals("SCE_Studio")) // Game Scene
                InitializeMod();
        }

        private void InitializeMod()
        {
            _canvasOverlay = new CanvasOverlay();
            _kbControls = new KBControls();
            _imageLoader = new ImageLoader();
            
            _kbControls.LoadLibs(_canvasOverlay, _imageLoader);
            _imageLoader.LoadLibs(_canvasOverlay);
            
            MelonLogger.Msg("Mod loaded!");
        }
        
        public override void OnLateUpdate()
        {
            if (_kbControls != null)
                _kbControls.Update();
        }
    }
}