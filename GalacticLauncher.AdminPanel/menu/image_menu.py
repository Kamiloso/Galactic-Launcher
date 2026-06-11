from display.show import Show
from model.forms import Forms

from .menu_helpers import save_tracker
from .menu_helpers import handle_menu, prepare_edit_menu

from errors import *


def run_image_menu(game_tree: dict) -> bool:
    middle_lines = prepare_edit_menu(game_tree)

    return handle_menu(["IMAGE MENU"] + middle_lines, [
        ("Display images", lambda: _display_images(game_tree)),
        ("Add image", lambda: _add_image(game_tree)),
        ("Remove images", lambda: _remove_images(game_tree)),
        ("Modify image", lambda: _modify_image(game_tree))
    ])


# --- Image menu ---

def _display_images(game_tree: dict):
    Show.tree_images(game_tree)


def _add_image(game_tree: dict):
    Forms.add_new_image(game_tree)
    save_tracker.inform_modify()
    Show.done()


def _remove_images(game_tree: dict):
    Forms.remove_images(game_tree)
    save_tracker.inform_modify()
    Show.done()


def _modify_image(game_tree: dict):
    Forms.modify_image(game_tree)
    save_tracker.inform_modify()
    Show.done()