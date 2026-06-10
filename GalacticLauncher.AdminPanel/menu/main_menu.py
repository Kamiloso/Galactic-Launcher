from display.ask import *
from display.show import *
from network.api_talker import *
from errors.soft_exit_error import SoftExitError
from utils import Utils

from .menu_utils import run_menu
from .edit_menu import run_edit_menu

from model.data import build_game_tree


def run_main_menu() -> bool:
    Utils.sys_clear()

    if (Utils.DEV_MODE()):
        print(f"Running in DEV MODE...")

    return run_menu("MAIN MENU", [
        ("Display games", _display_games),
        ("Add game", _add_game),
        ("Remove game", _remove_game),
        ("Edit game", _edit_game),
        ("Display tags", _display_tags),
        ("Add tag", _add_tag),
        ("Remove tag", _remove_tag),
        ("Display history", _display_history),
    ])

# --- Display ---

def _display_games():
    games = download_all_games()
    show_games(games)


def _display_tags():
    tags = download_all_tags()
    show_tags(tags)


def _display_history():
    page = ask_history_page()
    history_list = admin_get_history_page(page)
    games = download_all_games()
    show_history(history_list, games)


# --- Modifications ---

def _add_game():
    game = ask_new_game()
    admin_create_game(game)
    show_done()


def _remove_game():
    games = download_all_games()
    show_games(games)
    game_id = ask_select_obj(games, "game", "remove")
    admin_delete_game(game_id)
    show_done()


def _add_tag():
    tag = ask_new_tag()
    admin_create_tag(tag)
    show_done()


def _remove_tag():
    tags = download_all_tags()
    show_tags(tags)
    tag_id = ask_select_obj(tags, "tag", "remove")
    admin_delete_tag(tag_id)
    show_done()


# --- Advanced Modifications ---

def _edit_game():
    games = download_all_games()
    show_games(games)
    game_id = ask_select_obj(games, "game", "edit")

    game_data = download_game_data(game_id)
    game_tree = build_game_tree(game_data)

    while run_edit_menu(game_tree): pass
    raise SoftExitError