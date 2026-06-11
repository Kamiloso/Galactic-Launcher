from typing import Callable

from display.input import Input
from display.show import Show
from utils import Utils
from objects.save_tracker import SaveTracker

from errors import *

save_tracker = SaveTracker()


def prepare_menu():
    Utils.sys_clear()

    if (Utils.DEV_MODE()):
        print(f"Running in DEV MODE...")


def prepare_edit_menu(game_tree: dict) -> list[str]:
    prepare_menu()

    middle_lines = [f"\nEditing game: '{game_tree['name']}'"]
    if save_tracker.has_unsaved_changes():
        middle_lines.append("Unsaved changes!")

    return middle_lines


def handle_menu(
        header: str | list[str],
        options: list[tuple[str, Callable[[], None]]],
        exit_mode: str = "back"
    ) -> bool:

    title = header if isinstance(header, str) else header[0]
    middle_lines = None if isinstance(header, str) else header[1:]

    print(f"\n ----- {title} -----")

    if middle_lines is not None:
        for line in middle_lines:
            print(line)

    discarding = exit_mode == "exit" and save_tracker.has_unsaved_changes()
    exit_label = f"{_from_exit_mode(exit_mode)}{
        " (discard changes)" if discarding else ""
    }"

    options = options[:]
    options.append((exit_label, lambda: None))

    print()
    for i, (description, _) in enumerate(options):
        print(f" {i + 1} - {description}")

    try:
        choice = Input.string("Select an option")

        for i, (_, func) in enumerate(options[:-1]):
            if choice == str(i + 1):
                func()
                Show.pause()

        if choice == str(len(options)):
            if discarding and not Input.confirm(
                "You have unsaved changes. Exit anyway?"):
                raise SoftExitError

            return False # Exit

    except Exception as err:
        if _handle_error(err):
            print(f"\nError: {err}")
            Show.pause()

    return True # Continue


def _from_exit_mode(mode: str) -> str:
    if mode == "quit": return "Quit" # Quit the entire program
    if mode == "back": return "Back" # Going back to previous menu
    if mode == "exit": return "Exit" # Stop editing, discard changes
    raise ValueError(f"Invalid exit mode: {mode}")


def _handle_error(err: Exception) -> bool:
    if isinstance(err, ApiError):
        return True
    
    elif isinstance(err, AbortError):
        return True

    elif isinstance(err, SoftExitError):
        if err.throws > 0:
            raise err.with_decremented_ttl()
        else:
            return False

    raise err