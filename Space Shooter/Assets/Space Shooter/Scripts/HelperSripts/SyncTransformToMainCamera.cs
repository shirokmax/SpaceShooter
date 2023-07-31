using UnityEngine;

namespace SpaceShooter
{
    public class SyncTransformToMainCamera : MonoBehaviour
    {
        public enum UpdateMode
        {
            Update,
            FixedUpdate
        }

        [SerializeField] private UpdateMode updateMode;

        private void Update()
        {
            if (updateMode == UpdateMode.Update) SyncPosition();
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate) SyncPosition();
        }

        private void SyncPosition()
        {
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
        }
    }
}
