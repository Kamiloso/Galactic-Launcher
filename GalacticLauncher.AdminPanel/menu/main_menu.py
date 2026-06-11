from model.create import Create
from network.api_talker import ApiTalker
from display.ask import Ask
from display.show import Show
from model.data import Data

from .menu_helpers import handle_menu, prepare_menu
from .edit_menu import run_edit_menu

from errors import *


def run_main_menu() -> bool:
    prepare_menu()

    return handle_menu("MAIN MENU", [
        ("Display games", _display_games),
        ("Add game", _add_game),
        ("Remove game", _remove_game),
        ("Edit game", _edit_game),
        ("Display tags", _display_tags),
        ("Add tag", _add_tag),
        ("Remove tag", _remove_tag),
        ("Display history", _display_history),
        ("Input instructions", _input_instructions),
    ], exit_mode="quit")


# --- Display ---

def _display_games():
    games = ApiTalker.download_all_games()
    Show.games(games)


def _display_tags():
    tags = ApiTalker.download_all_tags()
    Show.tags(tags)


def _display_history():
    page = Ask.history_page()
    history_list = ApiTalker.get_history_page(page)
    games = ApiTalker.download_all_games()
    Show.history(history_list, games)


def _input_instructions():
    Show.input_instructions()


# --- Modifications ---

def _add_game():
    game = Create.new_game()
    ApiTalker.create_game(game)
    Show.done()


def _remove_game():
    games = ApiTalker.download_all_games()
    Show.games(games)
    game_id = Ask.select_obj(games, "game", "remove")
    ApiTalker.delete_game(game_id)
    Show.done()


def _add_tag():
    tag = Create.new_tag()
    ApiTalker.create_tag(tag)
    Show.done()


def _remove_tag():
    tags = ApiTalker.download_all_tags()
    Show.tags(tags)
    tag_id = Ask.select_obj(tags, "tag", "remove")
    ApiTalker.delete_tag(tag_id)
    Show.done()


# --- Edit Menu ---

def _edit_game():
    games = ApiTalker.download_all_games()
    Show.games(games)
    game_id = Ask.select_obj(games, "game", "edit")

    game_data = ApiTalker.download_game_data(game_id)
    game_tree = Data.build_game_tree(game_data)

    while run_edit_menu(game_tree): pass
    raise SoftExitError