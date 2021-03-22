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
        add_characters();
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
