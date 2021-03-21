using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerSwap : MonoBehaviour
{

    public Dictionary<string, GameObject> characters = new Dictionary<string, GameObject>();
    public string default_character = "Knight";
    public string current_character;

    void Start()
    {
        current_character = default_character;

        characters.Add("Knight", GameObject.Find("Knight"));
        characters.Add("Ninja", GameObject.Find("Ninja"));
        disable_non_default_characters();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && current_character != "Knight") {
            update_character("Knight");
        } else if (Input.GetKeyDown(KeyCode.F2) && current_character != "Ninja") {
            update_character("Ninja");
        }
    }

    void swap_character(string current_character, string next_character)
    {
        characters[current_character].SetActive(false);
        characters[next_character].SetActive(true);
    }

    void copy_position(string current_character, string next_character)
    {
        characters[next_character].transform.position = characters[current_character].transform.position;
    }

    void update_character(string next_character)
    {
        copy_position(current_character, next_character);
        swap_character(current_character, next_character);
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
