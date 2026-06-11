from model.data import Data
from display.show import Show
from model.forms import Forms
from network.api_talker import ApiTalker

from .menu_helpers import save_tracker
from .menu_helpers import handle_menu, prepare_edit_menu

from menu.version_menu import run_version_menu
from menu.image_menu import run_image_menu
from menu.tag_menu import run_tag_menu

from errors import *


def run_edit_menu(game_tree: dict) -> bool:
    save_tracker.inform_has(game_tree)
    middle_lines = prepare_edit_menu(game_tree)

    return handle_menu(["GAME EDIT MENU"] + middle_lines, [
        ("Display all", lambda: _display_all(game_tree)),
        ("Modify header", lambda: _modify_header(game_tree)),
        ("Edit versions", lambda: _edit_versions(game_tree)),
        ("Edit images", lambda: _edit_images(game_tree)),
        ("Edit tags", lambda: _edit_tags(game_tree)),
        ("Save & Exit", lambda: _save_and_exit(game_tree))
    ], exit_mode="exit")


# --- Display ---

def _display_all(game_tree: dict):
    tags = ApiTalker.download_all_tags()
    Show.full_game_data(game_tree, tags)


# --- Edit ---

def _modify_header(game_tree: dict):
    Forms.edit_header(game_tree)
    save_tracker.inform_modify()
    Show.done()


def _edit_versions(game_tree: dict):
    while run_version_menu(game_tree): pass
    raise SoftExitError


def _edit_images(game_tree: dict):
    while run_image_menu(game_tree): pass
    raise SoftExitError


def _edit_tags(game_tree: dict):
    while run_tag_menu(game_tree): pass
    raise SoftExitError


# --- Save ---

def _save_and_exit(game_tree: dict):
    if Data.has_multiple_primary(game_tree):
        raise AbortError("Multiple primary versions detected.")
    
    if Data.has_multiple_icons(game_tree):
        raise AbortError("Multiple icons detected.")
    
    ApiTalker.update_game_tree(game_tree)
    save_tracker.inform_save()
    Show.done()

    # Must exit (game tree will no longer be valid after saving)
    Show.pause()
    raise SoftExitError(ttl=1)