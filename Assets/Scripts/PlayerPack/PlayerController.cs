using System.Collections;
using System.Collections.Generic;
using PlayerPack.SO;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SoCharacter currentCharacter;
    
    // todo: implement weapons

    private int currentHp;

    public void StartRun(SoCharacter character)
    {
        currentCharacter = character;
        
        currentHp = currentCharacter.MaxHp;
    }
    
    // todo: movement
    
    // todo: weapon activation
}
