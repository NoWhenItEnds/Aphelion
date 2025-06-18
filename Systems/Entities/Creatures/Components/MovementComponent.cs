using Aphelion.Types.States;
using Godot;
using System;

namespace Aphelion.Entities.Creatures.Components
{
    public partial class MovementComponent : Node
    {
        [ExportGroup("Nodes")]
        [Export] private CharacterBody3D _unit;

        [Export] private Node3D _headNode;

        [Export] private Camera3D _firstPersonCamera;


        [ExportGroup("Settings")]
        [Export] private Single _jumpVelocity = 6f;

        [Export] private Single _walkSpeed = 7f;

        [Export] private Single _sprintSpeed = 8.5f;

        [Export] private Single _groundAcceleration = 14f;

        [Export] private Single _groundDeceleration = 10f;

        [Export] private Single _groundFriction = 6f;

        /// <summary> The maximum speed the unit can move in the air. </summary>
        [Export] private Single _airSpeedCap = 0.85f;

        [Export] private Single _airAcceleration = 800f;

        [Export] private Single _airSpeed = 500f;

        private UnitState _unitState = UnitState.WALKING;


        /// <summary> The actual direction the character wants to move relative to itself. </summary>
        private Vector3 _actualDirection = Vector3.Zero;

        //  ------------- Option Referenced Variables -------------

        /// <summary> A local reference to the world's gravity. </summary>
        private Single _gravity = 9.81f;

        /// <summary> The range of the headbob effect. </summary>
        private Single _headbobMoveAmount = 0.06f;

        /// <summary> How often the headbob effect triggers. </summary>
        private Single _headbobFrequency = 2.4f;

        /// <summary> The progress along the wave function the headbob effect currently is. </summary>
        private Single _headbobProgress = 0f;


        public override void _Ready()
        {
            ReadGameOptions();
        }


        /// <summary> Reads the relavant game options into the class' local memory. A small optimisation. </summary>
        private void ReadGameOptions()
        {
            _gravity = (Single)ProjectSettings.GetSetting("physics/3d/default_gravity");
            //  TODO - Add headbob from settings.
        }


        private void DoHeadbob(Single delta)
        {
            _headbobProgress += delta * _unit.Velocity.Length();
            Transform3D modifiedTransform = _firstPersonCamera.Transform;
            modifiedTransform.Origin = new Vector3(
                Mathf.Cos(_headbobProgress * _headbobFrequency * 0.5f) * _headbobMoveAmount,
                Mathf.Sin(_headbobProgress * _headbobFrequency) * _headbobMoveAmount,
                0f);
            _firstPersonCamera.Transform = modifiedTransform;
        }


        public void TrySetUnitState(UnitState unitState)
        {
            _unitState = unitState;
        }

        public void PhysicsMove(Double delta, Vector3 direction)
        {
            Vector3 actualDirection = _unit.GlobalTransform.Basis * new Vector3(direction.X, direction.Y, direction.Z);

            if (_unit.IsOnFloor())
            {
                HandleGroundPhysics(delta, actualDirection);
            }
            else
            {
                HandleAirPhysics(delta, actualDirection);
            }

            _unit.MoveAndSlide();
        }


        public override void _PhysicsProcess(Double delta)
        {
            //GD.Print(Velocity);
        }

        private void HandleGroundPhysics(Double delta, Vector3 direction)
        {
            _unit.Velocity = new Vector3(
                direction.X * GetMovementSpeed(),
                direction.Y * _jumpVelocity,
                direction.Z * GetMovementSpeed());
            /*
            Single currentSpeed = Velocity.Dot(direction);
            Single difference = GetMovementSpeed() - currentSpeed;
            if (difference > 0) //  If the player is not moving at the cap...
            {
                Single acceleration = _groundAcceleration * GetMovementSpeed() * (Single)delta;
                acceleration = Mathf.Min(acceleration, difference);
                Velocity += acceleration * direction;
            }*/

            DoHeadbob((Single)delta);
        }

        private void HandleAirPhysics(Double delta, Vector3 direction)
        {
            //  First, apply gravity.
            _unit.Velocity -= new Vector3(0f, _gravity, 0f) * (Single)delta;

            Single currentSpeed = _unit.Velocity.Dot(direction);
            Single cappedSpeed = Mathf.Min((direction * _airSpeed).Length(), _airSpeedCap);
            Single difference = cappedSpeed - currentSpeed;
            if (difference > 0) //  If the player is not moving at the cap...
            {
                Single acceleration = _airAcceleration * _airSpeed * (Single)delta;
                acceleration = Mathf.Min(acceleration, cappedSpeed);
                _unit.Velocity += acceleration * new Vector3(direction.X, 0f, direction.Z);   //  Strip the 'Y' so we don't hover.
            }
        }

        public void RotateLook(Vector2 relativeRotation)
        {
            _unit.RotateY(relativeRotation.X);
            _headNode.RotateX(relativeRotation.Y);

            //  Stops from looking too far. Number is 90 deg in radians.
            _headNode.Rotation = new Vector3(
                Mathf.Clamp(_headNode.Rotation.X, -1.5707964f, 1.5707964f),
                _headNode.Rotation.Y,
                _headNode.Rotation.Z);
        }


        private Single GetMovementSpeed()
        {
            Single speed = 0f;

            switch (_unitState)
            {
                case UnitState.WALKING:
                    speed = _walkSpeed;
                    break;

                case UnitState.SPRINTING:
                    speed = _sprintSpeed;
                    break;

                case UnitState.CROUCHING:
                    speed = _walkSpeed * 0.5f;
                    break;
                default:
                    speed = 0f;
                    break;
            }

            return speed;
        }
    }
}
