namespace EnemyPack.States.RootStates
{
    public class GolemMain : StateBase, IRootState
    {
        private Chase _chaseState;
        
        public void Compose(EnemyLogic logic)
        {
            _chaseState = new Chase();
        }
        
        public override void Enter(EnemyLogic state)
        {
            state.SwitchState(_chaseState);
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}