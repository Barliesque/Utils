using UnityEditor;
using Barliesque.InspectorTools.Editor;

namespace Barliesque.Utils.Editor
{

    [CustomEditor(typeof(PoolablePrefab))]
    public class PoolablePrefabEditor : EditorBase<PoolablePrefab>
    {
    
        override protected void CustomInspector(PoolablePrefab inst)
        {
            PropertyField("_prewarm", "Pre-warm");
            PropertyField("_maxInstances");
            var scheme = (PoolablePrefab.Recycling)PropertyField("_recycling").intValue;

            if (scheme == PoolablePrefab.Recycling.Callback)
            {
                EditorTools.HelpBox("Call PoolableObject.Recycle() to return an instance to the pool.", MessageType.Info);
            }
            if (scheme == PoolablePrefab.Recycling.Timed)
            {
                PropertyField("_recycleAfter");
            }
        }
        
    }

}