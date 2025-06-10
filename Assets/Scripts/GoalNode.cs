using UnityEngine;

public class GoalNode : BaseNode
{
    [Header("Các thành phần con của mục tiêu")]
    public GameObject TargetHandBody;
    public GameObject SecondTargetHandler;
    
    public float targetRotationZ { get; private set; }
    public void Configure(NodePlacementInfo goalInfo)
    {
        this.targetRotationZ = goalInfo.goalMainHandRotation;

        transform.rotation = Quaternion.Euler(0, 0, this.targetRotationZ);

        if (SecondTargetHandler != null)
        {
            SecondTargetHandler.SetActive(goalInfo.hasTargetSecondHand);
            SecondTargetHandler.transform.localRotation = Quaternion.Euler(0, 0, goalInfo.targetSecondHandRotation);
        }
    }
    
    protected override void OnDrawGizmos()
    {
        gizmoColor = Color.green;
        base.OnDrawGizmos();
    }
}