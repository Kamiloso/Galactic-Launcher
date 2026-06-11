from display.show import Show
from model.forms import Forms
from network.api_talker import ApiTalker

from .menu_helpers import save_tracker
from .menu_helpers import handle_menu, prepare_edit_menu

from errors import *


def run_tag_menu(game_tree: dict) -> bool:
    middle_lines = prepare_edit_menu(game_tree)

    return handle_menu(["TAG MENU"] + middle_lines, [
        ("Display tags", lambda: _display_tags(game_tree)),
        ("Toggle tags", lambda: _toggle_tags(game_tree))
    ])


# --- Tag menu ---

def _display_tags(game_tree: dict):
    tags = ApiTalker.download_all_tags()
    Show.tree_tags(game_tree, tags)


def _toggle_tags(game_tree: dict):
    tags = ApiTalker.download_all_tags()
    Forms.toggle_tags(game_tree, tags)
    save_tracker.inform_modify()
    Show.done()