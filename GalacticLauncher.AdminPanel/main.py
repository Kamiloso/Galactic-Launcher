from display.forms import *
from utils.state import State

def main():
    result = ask_if_dev_mode()
    State.dev_mode = result

    results = ask_for_credentials()
    State.username = results[0]
    State.password = results[1]

    while show_main_menu():
        pass

if __name__ == "__main__":
    main()