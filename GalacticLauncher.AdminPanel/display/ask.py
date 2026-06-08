from getpass import getpass


def ask_credentials() -> tuple[str, str]:
    print("\nAdmin credentials required...")
    username = input("Username: ")
    password = getpass("Password: ")
    return username, password


def ask_main_menu() -> str:
    print("\nMain Menu - Choose an option...")
    print("1 - Show all games")
    print("2 - Show game tree")
    print("3 - Show all tags")
    print("4 - Show history")
    print("5 - Add game")
    print("6 - Remove game")
    print("7 - Add tag")
    print("8 - Remove tag")
    print("9 - Modify game tree")
    print("10 - Exit")

    chdict = {
        "1": "display_games",
        "2": "display_game_data",
        "3": "display_tags",
        "4": "display_history",
        "5": "add_game",
        "6": "remove_game",
        "7": "add_tag",
        "8": "remove_tag",
        "9": "modify_game_tree",
        "10": "app_exit"
    }

    while True:
        choice = input("> ")

        if choice in chdict:
            return chdict[choice]


def ask_history_page() -> int:
    print("\nEnter history page number (starting from 0)...")

    while True:
        choice = input("> ")

        try:
            return int(choice)

        except ValueError:
            pass


def ask_select_game(games: list[dict], mode: str | None=None) -> int | None:
    if (len(games) == 0):
        return None

    if mode is not None:
        message = f"\nChoose game ID to {mode}..."
    else:
        message = f"\nChoose game ID..."

    print(f"\n{message} ('-1' or 'exit' to cancel)")

    while True:
        choice = input("> ")

        if choice == "-1" or choice == "exit":
            return None

        if any(choice == str(game['id']) for game in games):
            return int(choice)


def ask_new_game() -> dict:
    print("\nEnter new game information...")
    
    name = input("Name: ")
    author = input("Author: ")
    description = input("Description: ")

    return {
        "id": 0,
        "name": name,
        "author": author,
        "description": description
    }