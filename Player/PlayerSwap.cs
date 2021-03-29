using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerSwap : MonoBehaviour
{
    public Dictionary<string, GameObject> characters = new Dictionary<string, GameObject>();
    public string default_character = "Knight";
    public string current_character;
    Dictionary<KeyCode, string> character_map = new Dictionary<KeyCode, string> {
        {KeyCode.F1, "Knight"},
        {KeyCode.F2, "Ninja"}
    };

    void Awake()
    {
        current_character = default_character;
        add_characters();
        disable_non_default_characters();
    }

    void Update()
    {
        check_user_input();
    }

    void check_user_input()
    {
        foreach (KeyValuePair<KeyCode, string> e in character_map) {
            if (Input.GetKeyDown(e.Key)) {
                update_character(e.Value);
                break;
            }
        }
    }

    public GameObject get_current_object()
    {
        return characters[current_character];
    }

    void add_characters()
    {
        foreach (Transform child in transform) {
            characters.Add(child.name, GameObject.Find(child.name));
        }
    }

    void swap_character(string current_character, string next_character)
    {
        characters[current_character].SetActive(false);
        characters[next_character].SetActive(true);
    }

    void copy_attributes(string current_character, string next_character)
    {
        characters[next_character].transform.position = characters[current_character].transform.position;

        Vector3 new_scale = characters[next_character].transform.localScale;
        new_scale.x = characters[current_character].transform.localScale.x;
        characters[next_character].transform.localScale = new_scale;
    }

    void update_character(string next_character)
    {
        if (current_character.Equals(next_character)) {
            return;
        }

        copy_attributes(current_character, next_character);

        // Apply the current velocity to the new character.
        // To do so you have to save the velocity before swapping and apply it afterwards.
        Vector2 velocity = characters[current_character].GetComponent<Rigidbody2D>().velocity;
        swap_character(current_character, next_character);
        characters[next_character].GetComponent<Rigidbody2D>().velocity = velocity;

        current_character = next_character;
    }

    void disable_non_default_characters()
    {
        foreach (KeyValuePair<string, GameObject> e in characters) {
            characters[e.Key].SetActive(false);
        }
        characters[default_character].SetActive(true);
    }
}
