using UnityEngine;

namespace SpaceShooter
{
    public class Route : MonoBehaviour
    {
        public int GetPointsCount => transform.childCount;
        public bool IsComplete => transform.childCount >= 2;

        public Vector3 GetPointPosition(int index)
        {
            if (IsComplete == false) return Vector3.zero;
            if (index >= GetPointsCount) return Vector3.zero;

            return transform.GetChild(index).position;
        }

        public void GetNextPointIndex(ref int index)
        {
            if (index + 1 >= GetPointsCount) index = 0;
            else index++;
        }

        /// <summary>
        /// Находит ближайшую точку маршрута к заданной позиции.
        /// </summary>
        /// <param name="position">Позиция, к которой ищется ближайшая точка маршрута</param>
        public Vector3 GetNearestPointPosition(Vector3 position)
        {
            if (IsComplete == false) return Vector3.zero;

            Vector3 potentialPoint = Vector3.zero;

            float minDist = float.MaxValue;

            for (int i = 0; i < transform.childCount; i++)
            {
                float dist = Vector3.Distance(position, transform.GetChild(i).position);

                if (dist < minDist)
                {
                    minDist = dist;
                    potentialPoint = transform.GetChild(i).position;
                }
            }

            return potentialPoint;
        }
    }
}