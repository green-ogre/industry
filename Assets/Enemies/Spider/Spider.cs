using UnityEngine;

public class Spider : MonoBehaviour
{
    public Animator animator;
    public SlideController slideController;

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = slideController.isGrounded;
        // animator.SetBool("isFalling", !isGrounded);
        float x = AxisNormalize.Movement(Input.GetAxisRaw("Horizontal"));
        animator.SetBool("isRunning", Mathf.Abs(x) > 1e-10 && isGrounded);
    }
}
