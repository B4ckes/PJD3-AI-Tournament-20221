using ChallengeAI;

public class FSMInit : FSMInitializer //IFSMInitializer
{
  public override string Name => "AI_Backes";
  public override void Init()
  {
    RegisterState<GetEnergy>(AIBackesState.GET_ENERGY);
    RegisterState<CaptureFlag>(AIBackesState.CAPTURE_FLAG);
    RegisterState<GetAmmo>(AIBackesState.GET_AMMO);
    RegisterState<SecureFlag>(AIBackesState.SECURE_FLAG);
    RegisterState<WaitForEnergy>(AIBackesState.WAIT_FOR_ENERGY);
  }
}
