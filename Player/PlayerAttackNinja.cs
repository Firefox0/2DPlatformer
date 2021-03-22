using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackNinja : PlayerAttack {
    void Start()
    {
        player_cooldowns.add_cooldown("NinjaAttack", 1.5f);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0) || !player_cooldowns.check_cooldown("NinjaAttack")) {
            return;
        }
        attack_enemies();
    }
}
