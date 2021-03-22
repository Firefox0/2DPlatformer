using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Cooldown {
    public bool state;
    public float current_timer;
    public float original_timer;
};

public class PlayerCooldowns : MonoBehaviour
{
    Dictionary<string, Cooldown> cooldowns = new Dictionary<string, Cooldown>();

    void Start()
    {
        add_cooldown("Dash", 3f);
    }

    void Update()
    {
        handle_cooldowns();    
    }

    public void add_cooldown(string name, float cooldown)
    {
        if (cooldowns.ContainsKey(name)) {
            return;
        }
        cooldowns.Add(name, new Cooldown() { state = true, current_timer = cooldown, original_timer = cooldown });
    }

    public bool check_cooldown(string name)
    {
        if (!cooldowns.ContainsKey(name) || !cooldowns[name].state) {
            return false;
        }
        cooldowns[name].state = false;
        return true;
    }

    void handle_cooldowns()
    {
        foreach (KeyValuePair<string, Cooldown> e in cooldowns) {
            if (e.Value.state) {
                continue;
            }
            e.Value.current_timer -= Time.deltaTime;
            if (e.Value.current_timer <= 0) {
                e.Value.current_timer = e.Value.original_timer;
                e.Value.state = true;
            }
        }
    }
}
