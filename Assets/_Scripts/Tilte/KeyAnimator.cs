using UnityEngine;

[System.Serializable]
public class KeyAnimationSet
{
    public KeyCode Key;
    public string AnimatorBool;
    public Animator Animator;
}

public class KeyAnimator : MonoBehaviour
{
    [SerializeField] 
    private KeyAnimationSet[] _keyBindings;

    void Update()
    {
        foreach (var binding in _keyBindings)
        {
            if (Input.GetKeyDown(binding.Key))
            {
                binding.Animator.SetBool(binding.AnimatorBool, true);
            }
            if (Input.GetKeyUp(binding.Key))
            {
                binding.Animator.SetBool(binding.AnimatorBool, false);
            }
        }
    }
}
