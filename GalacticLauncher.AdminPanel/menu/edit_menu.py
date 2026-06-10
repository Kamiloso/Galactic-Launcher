from display.ask import *
from display.show import *
from network.api_talker import *
from utils import Utils

from .menu_utils import run_menu

from forms.edit_header import form_edit_header
# from forms.edit_versions import form_edit_versions
# from forms.edit_images import form_edit_images
from forms.edit_tags import form_edit_tags


def run_edit_menu(game_tree: dict) -> bool:
    Utils.sys_clear()

    if (Utils.DEV_MODE()):
        print(f"Running in DEV MODE...")

    middle_lines = [
        f"\nEditing game: '{game_tree['name']}'"
    ]

    return run_menu([f"GAME EDIT MENU"] + middle_lines, [
        ("Display all", lambda: _display_all(game_tree)),
        ("Edit header", lambda: _edit_header(game_tree)),
        ("Edit versions", lambda: _edit_versions(game_tree)),
        ("Edit images", lambda: _edit_images(game_tree)),
        ("Edit tags", lambda: _edit_tags(game_tree)),
        ("Save", lambda: _save(game_tree))
    ])


# --- Display ---

def _display_all(game_tree: dict):
    tags = download_all_tags()
    show_full_game_data(game_tree, tags)


# --- Edit ---

def _edit_header(game_tree: dict):
    form_edit_header(game_tree)
    show_done()


def _edit_versions(game_tree: dict):
    # show_game_versions(game_tree)
    show_done()


def _edit_images(game_tree: dict):
    # show_game_images(game_tree)
    show_done()


def _edit_tags(game_tree: dict):
    tags = download_all_tags()
    form_edit_tags(game_tree, tags)
    show_done()


# --- Save ---

def _save(game_tree: dict):
    admin_update_game_tree(game_tree)
    show_done()