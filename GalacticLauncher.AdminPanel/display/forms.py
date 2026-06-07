from display.menu import ChoiceMenu, InputMenu
from network.api_talker import *

def ask_if_dev_mode() -> bool:
    menu = ChoiceMenu(
        "Enable developer mode?",
        [
            "Yes",
            "No"
        ]
    )

    choice = menu.display()
    return choice == 1

def ask_for_credentials() -> tuple[str, str]:
    menu = InputMenu(
        "Enter your credentials",
        [
            "Username:",
            "Password:"
        ])
    
    answers = menu.display()
    return answers[0], answers[1]

def show_main_menu() -> int:
    menu = ChoiceMenu(
        "Main Menu",
        [
            "Test API Connection",
            "Display all games",
            "Exit"
        ])
    
    choice = menu.display()

    if choice == 1: # Test API Connection
        test_connection()

    if choice == 2: # Display all games
        games = all_games()
        for game in games:
            print(game["name"])

    if choice == 3: # Exit
        return False

    return True