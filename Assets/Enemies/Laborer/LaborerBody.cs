using UnityEngine;

public class LaborerBody : MonoBehaviour
{
    public Animator animator;
    public SlideController slideController;

    void Update()
    {
        // Debug.Log("hello");
        bool isGrounded = slideController.isGrounded;
        animator.SetBool("isFalling", !isGrounded);
        float x = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal"));
        animator.SetBool("isRunning", Mathf.Abs(x) > 1e-10 && isGrounded);
    }
}
