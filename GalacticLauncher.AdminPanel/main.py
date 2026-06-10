from display.ask import *
from display.show import *
from network.api_talker import *

from menu.main_menu import run_main_menu


def main():
    while run_main_menu(): pass
    print("\nExiting...")


if __name__ == "__main__":
    main()