using _Main.Scripts.BaseGame.Clases;
using _Main.Scripts.BaseGame.Interfaces;
using Clases;
using UnityEngine;

namespace _Main.Scripts.BaseGame.Commands
{
    public class CmdMove : ICommando
    {
        private MovementController _movementController;
        private Vector3 _direction;
        
        public CmdMove(MovementController movementController, Vector3 direction)
        {
            _movementController = movementController;
            _direction = direction;
        }

        public void Execute() => _movementController.Move(_direction);
        public void Undo() => _movementController.MoveBackwards(_direction);
    }
}