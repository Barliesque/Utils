using UnityEngine;

namespace Barliesque.Utils
{

    static public class MaterialExtensions
    {

        static public bool GetBool(this Material material, int propID)
        {
            return material.GetFloat(propID) > 0f;
        }

        static public bool GetBool(this Material material, string property)
        {
            return material.GetFloat(property) > 0f;
        }

        static public void SetBool(this Material material, int propID, bool value)
        {
            material.SetFloat(propID, value ? 1f : 0f);
        }

        static public void SetBool(this Material material, string property, bool value)
        {
            material.SetFloat(property, value ? 1f : 0f);
        }
        
    }

}