using ChallengeAI;
using UnityEngine;

public class AIBackesState {
  static public string GET_ENERGY = "getEnergy";
  static public string GET_AMMO = "getAmmo";
  static public string CAPTURE_FLAG = "captureFlag";
  static public string SECURE_FLAG = "secureFlag";
  static public string WAIT_FOR_ENERGY = "waitForEnergy";
}

public class GetEnergy : State {
  public GetEnergy(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate) {}

  Vector3 destination;

  public override void Enter()
  {
    destination = GameObject.FindObjectOfType<EnergyPickup>().transform.position;
    Agent.Move(destination);
  }

  public override void Exit() {}

  public override void Update(float deltaTime)
  {
    Debug.LogFormat("#!# {0}", (double) Agent.Data.RemainingDistance);
    string captureFlagState = AIBackesState.CAPTURE_FLAG;
    string secureFlagState = AIBackesState.SECURE_FLAG;
    string waitForEnergyState = AIBackesState.WAIT_FOR_ENERGY;

    bool hasGotEnergy = Agent.Data.RemainingDistance <= 0.05f;
    bool hasFlag = Agent.Data.HasFlag;
    bool hasEnemySight = Agent.Data.HasSightEnemy;
    bool isEnergyStillLow = Agent.Data.Energy < 40;
    bool hasToWaitForEnergy = Agent.Data.Energy < 1;

    if (hasEnemySight) {
      Agent.Fire();
      Agent.Move(destination);
    }

    if (hasToWaitForEnergy) {
      ChangeState(waitForEnergyState);
    }

    if(hasGotEnergy && hasFlag && !isEnergyStillLow) {
      ChangeState(secureFlagState);
    } else if(hasGotEnergy && !isEnergyStillLow) {
      ChangeState(captureFlagState);
    }
  }
}
public class GetAmmo : State {
  public GetAmmo(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate) {}

  Vector3 destination;

  public override void Enter()
  {
    destination = GameObject.FindObjectOfType<AmmoPickup>().transform.position;

    Agent.Move(destination);
  }

  public override void Exit() {}

  public override void Update(float deltaTime)
  {
    string captureFlagState = AIBackesState.CAPTURE_FLAG;
    string secureFlagState = AIBackesState.SECURE_FLAG;
    string getEnergyState = AIBackesState.GET_ENERGY;
    string waitForEnergyState = AIBackesState.WAIT_FOR_ENERGY;

    bool gotAmmo = Agent.Data.RemainingDistance <= 0.05f;
    bool hasFlag = Agent.Data.HasFlag;
    bool hasEnemySight = Agent.Data.HasSightEnemy;
    bool isEnergyLow = Agent.Data.Energy < 30;
    bool hasToWaitForEnergy = Agent.Data.Energy < 1;

    if (hasEnemySight) {
      Agent.Fire();
      Agent.Move(destination);
    }

    if (hasToWaitForEnergy) {
      ChangeState(waitForEnergyState);
    }

    if(gotAmmo && hasFlag && !isEnergyLow) {
      Agent.Rotate(360);
      ChangeState(captureFlagState);
    }


    if(gotAmmo && hasFlag && isEnergyLow) {
      ChangeState(getEnergyState);
    } else if(gotAmmo && hasFlag) {
      ChangeState(secureFlagState);
    }
  }
}

public class WaitForEnergy : State {
  public WaitForEnergy(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate) {}

  public override void Enter() {
    Agent.Stop();
  }
  
  public override void Exit() {}
  
  public override void Update(float deltaTime)
  {
    string getEnergyState = AIBackesState.GET_ENERGY;

    bool hasEnoughtEnergy = Agent.Data.Energy > 45f;
    bool hasFlag = Agent.Data.HasFlag;

    if (hasEnoughtEnergy) {
      ChangeState(getEnergyState);
    }
  }
}

public class CaptureFlag : State {
  public CaptureFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate) {}

  Vector3 destination;

  public override void Enter()
  {
    destination = (Vector3) Agent.EnemyData[0].FlagPosition;

    Agent.Move(destination);
  }

  public override void Exit() {}

  public override void Update(float deltaTime)
  {
    string getEnergyState = AIBackesState.GET_ENERGY;
    string waitForEnergyState = AIBackesState.WAIT_FOR_ENERGY;

    bool needEnergy = Agent.Data.Energy < 55f;
    bool needAmmo = Agent.Data.Ammo == 0 && Agent.EnemyData[0].HasFlag;
    bool hasEnemySight = Agent.Data.HasSightEnemy;
    bool gotFlag = Agent.Data.RemainingDistance <= 0.05f;
    bool hasToWaitForEnergy = Agent.Data.Energy < 1;

    if (hasEnemySight && Agent.EnemyData[0].HasFlag) {
      Agent.Fire();
      Agent.Move(destination);
    }

    if (hasToWaitForEnergy) {
      ChangeState(waitForEnergyState);
    }

    if (gotFlag && needEnergy) {
      ChangeState(getEnergyState);
    }
  }
}

public class SecureFlag : State {
  public SecureFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate) {}

  Vector3 destination;
  
  public override void Enter()
  {
    destination = (Vector3) (Vector3) Agent.Data.FlagPosition;

    Agent.Move(destination);
  }

  public override void Exit() {}

  public override void Update(float deltaTime)
  {
    string getEnergy = AIBackesState.GET_ENERGY;
    string captureFlag = AIBackesState.CAPTURE_FLAG;
    string waitForEnergyState = AIBackesState.WAIT_FOR_ENERGY;

    bool hasSecuredFlag = Agent.Data.RemainingDistance <= 0.05f;
    bool needEnergy = Agent.Data.Energy < 55f;
    bool hasEnemySight = Agent.Data.HasSightEnemy;
    bool hasLostFlag = !Agent.Data.HasFlag;
    bool hasToWaitForEnergy = Agent.Data.Energy < 1;

    if (hasEnemySight && Agent.EnemyData[0].HasFlag) {
      Agent.Fire();
      Agent.Move(destination);
    }

    if (Agent.EnemyData[0].HasFlag) {
      Agent.Stop();
      Agent.Rotate(180);
      Agent.Move(destination);
    }

    if (hasLostFlag) {
      ChangeState(captureFlag);
    }

    if (hasToWaitForEnergy) {
      ChangeState(waitForEnergyState);
    }

    if (hasSecuredFlag && needEnergy) {
      ChangeState(getEnergy);
    }
  }
}
