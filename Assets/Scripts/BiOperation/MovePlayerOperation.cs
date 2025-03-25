using Phos;
using Phos.Navigate;
using Phos.Operation;
using Phos.Perform;
using UnityEngine;

namespace BiOperation {
    public class MovePlayerOperation : BaseBiOperation {
        public bool invert;
        public NavigateNode destination;

        private PlayerController _player;

        private void Start() {
            _player = SceneController.Instance.Player;
        }
        
        public override void Execute(bool trigger) {
            if (!destination || !_player || invert == trigger) return;
            Debug.Log("Move Player");

            PathManager.GetInstance().UpdateAccessable(_player.current);
            _player.MoveTo(destination);
        }
    }
}