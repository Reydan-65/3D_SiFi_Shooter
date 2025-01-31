public class EntityContextAction : EntityAnimationAction
{
    public bool IsCanStart;

    public override void StartAction()
    {
        if (IsCanStart == false) return;

        base.StartAction();
    }
}
