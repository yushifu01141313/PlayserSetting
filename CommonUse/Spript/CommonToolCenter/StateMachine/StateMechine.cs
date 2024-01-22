public class StateMechine
{
    //只针对当前状态做管理
    #region Private Fields
    IState currentState;
    #endregion

    #region Publice Methods
    //切换状态
    public void ChangeState(IState nextState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = nextState;
        currentState.OnStateEnter();
    }

    //持续状态
    public void OnMechineUpdate()
    {
        currentState.OnStateUpdate();
    }
    #endregion
}
