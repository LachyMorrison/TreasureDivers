using System.Collections.Generic;
using UnityEngine;

namespace TreasureDivers.World
{
    [DisallowMultipleComponent]
    public sealed class WaterSensor : MonoBehaviour
    {
        private readonly HashSet<WaterVolume> activeWaterVolumes = new HashSet<WaterVolume>();

        public bool IsInWater
        {
            get
            {
                activeWaterVolumes.RemoveWhere(volume => volume == null);
                return activeWaterVolumes.Count > 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            WaterVolume waterVolume = other.GetComponentInParent<WaterVolume>();

            if (waterVolume != null)
            {
                activeWaterVolumes.Add(waterVolume);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            WaterVolume waterVolume = other.GetComponentInParent<WaterVolume>();

            if (waterVolume != null)
            {
                activeWaterVolumes.Remove(waterVolume);
            }
        }
    }
}
