using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackKnight : PlayerAttack
{
    void Start()
    {
        player_cooldowns.add_cooldown("KnightAttack", 3f);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0) || !player_cooldowns.check_cooldown("KnightAttack")) {
            return;
        }
        attack_enemies();
    }
}
