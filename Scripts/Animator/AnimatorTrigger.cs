using UnityEngine;

namespace Barliesque.Utils
{

    public class AnimatorTrigger
    {
        public readonly string Name;
        public readonly int id;
        public readonly Animator animator;
        public bool LogChanges;

        public AnimatorTrigger(Animator animator, string name, bool logChanges = false)
        {
            Name = name;
            this.animator = animator;
            id = Animator.StringToHash(name);
            LogChanges = logChanges;
        }

        public void Set()
        {
            if (LogChanges) Debug.Log($"{Name}.Set()");
            animator.SetTrigger(id);
        }

        public void Reset()
        {
            if (LogChanges) Debug.Log($"{Name}.Reset()");
            animator.ResetTrigger(id);
        }

        override public string ToString()
        {
            return $"[AnimatorTrigger] {Name}";
        }
        
        public bool CheckParameterExists() => CheckParameterExists(animator, Name);

        static public bool CheckParameterExists(Animator animator, string triggerParam)
        {
            if (!animator) return false;
            if (string.IsNullOrEmpty(triggerParam)) return false;
			
            animator.Update(0f); // Forces parameters list to be updated
            foreach (var param in animator.parameters)
            {
                if (!param.name.Equals(triggerParam)) continue;
                // It exists, but is it a trigger?
                return param.type == AnimatorControllerParameterType.Trigger;
            }
            return false;
        }

    }

}