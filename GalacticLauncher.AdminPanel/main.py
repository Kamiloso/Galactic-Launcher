from display.ask import *
from display.show import *
from network.api_talker import *

from utils import Utils

def main():
    if Utils.DEV_MODE():
        print("Running in development mode...")

    try:
        while True:
            choice = ask_main_menu()

            chdict = {
                "display_games": lambda: display_games(),
                "display_game_data": lambda: display_game_data(),
                "display_tags": lambda: display_tags(),
                "display_history": lambda: display_history(),
                "add_game": lambda: add_game(),
                "remove_game": lambda: remove_game(),
                "add_tag": lambda: add_tag(),
                "remove_tag": lambda: remove_tag(),
                "modify_game_tree": lambda: modify_game_tree(),
                "app_exit": lambda: app_exit()
            }

            if choice in chdict:
                chdict[choice]()

    except ApiError as err:
        print(err.error_str())
        exit(1)


def display_games():
    games = download_all_games()
    show_games(games)


def display_game_data():
    games = download_all_games()
    show_games(games)

    game_id = ask_select_game(games, "display")

    if game_id is not None:
        game_data = download_game_data(game_id)
        show_game_data(game_data)


def display_tags():
    tags = download_all_tags()
    show_tags(tags)


def display_history():
    page = ask_history_page()
    history_list = admin_get_history_page(page)

    if history_list is not None:
        games = download_all_games()
        show_history(games, history_list)


def add_game():
    game = ask_new_game()

    if admin_create_game(game):
        show_success()


def remove_game():
    games = download_all_games()
    show_games(games)

    game_id = ask_select_game(games, "remove")
    
    if game_id is not None:
        if admin_delete_game(game_id):
            show_success()


def add_tag():
    pass


def remove_tag():
    pass


def modify_game_tree():
    pass


def app_exit():
    print("Application closed.")
    exit(0)


if __name__ == "__main__":
    main()