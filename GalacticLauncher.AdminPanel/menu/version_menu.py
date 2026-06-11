from display.ask import Ask
from display.show import Show
from model.forms import Forms

from .menu_helpers import save_tracker
from .menu_helpers import handle_menu, prepare_edit_menu

from errors import *


def run_version_menu(game_tree: dict) -> bool:
    middle_lines = prepare_edit_menu(game_tree)

    return handle_menu(["EDIT VERSIONS"] + middle_lines, [
        ("Display versions", lambda: _display_versions(game_tree)),
        ("Add version", lambda: _add_version(game_tree)),
        ("Remove versions", lambda: _remove_versions(game_tree)),
        ("Modify version", lambda: _modify_version(game_tree))
    ])


# --- Version menu ---

def _display_versions(game_tree: dict):
    Show.tree_versions(game_tree)


def _add_version(game_tree: dict):
    Forms.add_new_version(game_tree)
    save_tracker.inform_modify()
    Show.done()


def _remove_versions(game_tree: dict):
    Forms.remove_versions(game_tree)
    save_tracker.inform_modify()
    Show.done()


def _modify_version(game_tree: dict):
    Forms.modify_version(game_tree)
    save_tracker.inform_modify()
    Show.done()