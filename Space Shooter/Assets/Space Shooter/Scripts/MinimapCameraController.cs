using UnityEngine;

namespace SpaceShooter
{
    public class MinimapCameraController : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (Player.Instance.ActiveShip != null)
            {
                Vector3 shipPosition = Player.Instance.ActiveShip.transform.position;
                Vector3 newPosition = new Vector3(shipPosition.x, shipPosition.y, transform.position.z);

                transform.position = newPosition;
            }
    }
    }
}
