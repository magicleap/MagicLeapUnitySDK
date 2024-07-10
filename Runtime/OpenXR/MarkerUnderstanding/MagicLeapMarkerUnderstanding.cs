using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.MarkerUnderstanding
{
    public partial class MagicLeapMarkerUnderstandingFeature
    {
        private readonly List<MarkerDetector> markerDetectors = new();
        
        /// <summary>
        /// The maximum number of marker detectors allowed at once.
        /// </summary>
        public const int MarkerDetectorsLimit = 64;

        /// <summary>
        /// The active marker detectors tracked by the marker understanding feature.
        /// </summary>
        /// <returns>A readonly list of the active marker detectors.</returns>
        public IReadOnlyList<MarkerDetector> MarkerDetectors => markerDetectors;

        private MarkerUnderstandingNativeFunctions nativeFunctions;
        
        /// <summary>
        /// Creates a marker detector with predefined settings.
        /// </summary>
        /// <param name="settings">The marker detector settings to be associated with the marker detector to be created.</param>
        /// <returns>The marker detector that has been created. Returns null if the number of active marker detectors is at the limit or an error occurred.</returns>
        public MarkerDetector CreateMarkerDetector(MarkerDetectorSettings settings)
        {
            if (markerDetectors.Count >= MarkerDetectorsLimit)
            {
                Debug.LogError($"The number of active marker detectors cannot exceed {MarkerDetectorsLimit}");
                return null;
            }

            var markerDetector = new MarkerDetector(settings, nativeFunctions, this);
            if(markerDetector.Status == MarkerDetectorStatus.Error)
            {
                Debug.LogError($"Failed to create Marker Detector");
                return null;
            }

            markerDetectors.Add(markerDetector);

            return markerDetector;
        }
        
        /// <summary>
        /// Provides the ability to modify a marker detector with new settings. 
        /// Note: this method actually destroys the old marker detector and replaces it with a newly created one.
        /// However, this approach maintains the index position of it in the marker detectors list.
        /// </summary>
        /// <param name="settings">The marker detector settings to be associated with the marker detector to be created.</param>
        /// <param name="markerDetector">The specified marker detector to modify.</param>
        /// <returns>The newly created marker detector that replaced the old one. This returns null if the specified marker detector is not tracked.</returns>
        public void ModifyMarkerDetector(MarkerDetectorSettings settings, ref MarkerDetector markerDetector)
        {
            var index = markerDetectors.IndexOf(markerDetector, 0);

            if (index == -1)
            {
                Debug.LogError("Marker detector cannot be modified because it is not tracked by the MarkerUnderstandingFeature. Either it was already destroyed or not created properly.");
                return;
            }

            DestroyMarkerDetector(markerDetector);          

            markerDetector = new MarkerDetector(settings, nativeFunctions, this);
            markerDetector.UpdateData();

            markerDetectors.Insert(index, markerDetector);
        }

        /// <summary>
        /// Updates the status and data for all actively tracked marker detectors.
        /// </summary>
        public void UpdateMarkerDetectors()
        {
            foreach (var markerDetector in markerDetectors)
            {
                markerDetector.UpdateData();
            }
        }
        
        /// <summary>
        /// Destroys the specified marker detector.
        /// </summary>
        /// <param name="markerDetector">The marker detector to be destroyed.</param>
        public void DestroyMarkerDetector(MarkerDetector markerDetector)
        {      
            markerDetectors.Remove(markerDetector);
            markerDetector.Destroy();
        }

        /// <summary>
        /// Destroys all actively tracked marker detectors.
        /// </summary>
        public void DestroyAllMarkerDetectors()
        {
            var markerTrackers = markerDetectors.ToList();
            markerDetectors.Clear();
            Task.Run(() =>
            {
                foreach (var markerDetector in markerTrackers)
                {
                    markerDetector.Destroy();
                }
            });
        }
        
        
    }
}
