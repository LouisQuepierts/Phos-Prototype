using Phos.Operation;
using Phos.UI;

namespace BiOperation {
    public class ToggleMenuOperation : BaseBiOperation {
        public string menu;
        public float speed = 1;
        public bool invert;
        
        public override void Execute(bool trigger) {
            bool toggle = trigger != invert;
            MenuManager.Instance.ToggleMenu(menu, toggle, speed);
        }
    }
}