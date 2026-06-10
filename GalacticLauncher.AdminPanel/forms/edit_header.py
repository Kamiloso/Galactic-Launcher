from display.show import *
from display.input import *

def form_edit_header(game_tree: dict) -> None:
    show_game_header(game_tree)

    print("\nEdit header (leave blank to keep current value):")

    new_name = input_string("Provide new name...")
    new_author = input_string("Provide new author...")
    new_description = input_string("Provide new description...")

    game_tree["name"] = new_name if new_name != "" else game_tree['name']
    game_tree["author"] = new_author if new_author != "" else game_tree['author']
    game_tree["description"] = new_description if new_description != "" else game_tree['description']

    show_game_header(game_tree)