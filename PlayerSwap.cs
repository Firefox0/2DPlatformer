using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerSwap : MonoBehaviour
{

    OrderedDictionary characters = new OrderedDictionary();
    string current_character = "Knight";

    void Start()
    {
        characters.Add("Knight", GameObject.Find("Knight"));
        characters.Add("Ninja", GameObject.Find("Ninja"));
        disable_non_default_characters();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && current_character != "Knight")
        {
            update_character("Knight");
        }
        else if (Input.GetKeyDown(KeyCode.F2) && current_character != "Ninja")
        {
            update_character("Ninja");
        }
    }

    void swap_character(string current_character, string next_character)
    {
        ((GameObject)characters[current_character]).SetActive(false);
        ((GameObject)characters[next_character]).SetActive(true);
    }

    void copy_position(string current_character, string next_character)
    {
        ((GameObject)characters[next_character]).transform.position = ((GameObject)characters[current_character]).transform.position;
    }

    void update_character(string next_character)
    {
        copy_position(current_character, next_character);
        swap_character(current_character, next_character);
        current_character = next_character;
    }

    void disable_non_default_characters()
    {
        for (int i = 1; i < characters.Count; i++)
        {
            ((GameObject)characters[i]).SetActive(false);
        }
    }
}
