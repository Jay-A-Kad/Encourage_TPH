using UnityEngine;
using System.Collections;

public class CrowdAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string[] animationStates = { "Cheer1", "Cheer2" };
    [SerializeField] private float minSpeed = 0.9f;
    [SerializeField] private float maxSpeed = 1.1f;

    private int lastIndex = -1;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.speed = Random.Range(minSpeed, maxSpeed);

        StartCoroutine(RandomAnim());
    }

    private IEnumerator RandomAnim()
    {
        while (true)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, animationStates.Length);
            }
            while (animationStates.Length > 1 && randomIndex == lastIndex);

            lastIndex = randomIndex;

            float randomStartTime = Random.Range(0f, 1f);
            animator.Play(animationStates[randomIndex], 0, randomStartTime);

            yield return null;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float waitTime = stateInfo.length / animator.speed;

            if (waitTime <= 0f)
                waitTime = 1f;

            yield return new WaitForSeconds(waitTime);
        }
    }


}
