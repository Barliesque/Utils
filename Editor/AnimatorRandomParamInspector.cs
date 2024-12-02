using UnityEngine;
using UnityEditor;
using Barliesque.InspectorTools.Editor;

namespace Barliesque.Utils.Editor
{

    [CustomEditor(typeof(AnimatorRandomParam))]
    public class AnimatorRandomParamInspector : EditorBase<AnimatorRandomParam>
    {
    
        override protected bool ShowScriptField => false;

        override protected void CustomInspector(AnimatorRandomParam inst)
        {
            PropertyField("_parameter");
            var type = (AnimatorControllerParameterType)PropertyField("_type").intValue;
            if (type == AnimatorControllerParameterType.Float)
            {
                PropertyField("_floatRange");
            } else if (type == AnimatorControllerParameterType.Int)
            {
                PropertyField("_intRange");
            }
            PropertyField("_randomizeOnEnable");
        }
        
    }

}