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
            return string.Format("[AnimatorTrigger] {0}", Name);
        }
    }

}