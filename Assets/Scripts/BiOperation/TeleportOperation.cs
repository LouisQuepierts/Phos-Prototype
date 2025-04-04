﻿using Phos;
using Phos.Navigate;
using Phos.Operation;
using Phos.Perform;

namespace BiOperation {
    public class TeleportOperation : BaseBiOperation {
        public bool invert;
        public NavigateNode destination;

        private PlayerController _player;
        private PathManager _pathManager;
        
        private void Start() {
            _player = SceneController.Instance.Player;
            _pathManager = PathManager.GetInstance();
        }

        public override void Execute(bool trigger) {
            if (!destination || !_player || invert != trigger) return;

            _player.transform.up = destination.transform.up;
            _player.transform.position = destination.GetNodePosition();
            _pathManager.UpdateAccessable(destination);
        }
    }
}