using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public PlayerSwap player_info;
    string current_character = "";
    GameObject current_character_object;
    Vector3 camera_offset = new Vector3(0f, 1.5f, -10f);

    void Update() {
        if (!this.current_character.Equals(player_info.current_character)) {
            this.current_character = player_info.current_character;
            current_character_object = (GameObject)player_info.characters[this.current_character];
        }
        transform.position = current_character_object.transform.position + camera_offset;
    }
}
