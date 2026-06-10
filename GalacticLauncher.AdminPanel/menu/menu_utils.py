from typing import Callable

from display.ask import *

from errors.api_error import ApiError
from errors.abort_error import AbortError
from errors.soft_exit_error import SoftExitError


def run_menu(header: str | list[str], options: list[tuple[str, Callable[[], None]]]) -> bool:
    title = header if isinstance(header, str) else header[0]
    middle_lines = None if isinstance(header, str) else header[1:]

    print(f"\n ----- {title} -----")

    if middle_lines is not None:
        for line in middle_lines:
            print(line)

    options = options[:]
    options.append(("Exit", lambda: None))

    print()
    for i, (description, _) in enumerate(options):
        print(f" {i + 1} - {description}")

    choice = ask_choice()
    if choice is None or choice == str(len(options)):
        return False # Exit
    
    pause = False

    for i, (_, func) in enumerate(options):
        if choice == str(i + 1):
            try:
                func()
                pause = True
                
            except Exception as err:
                if _handle_error(err):
                    pause = True
                    print(f"\nError: {err}")

    if pause:
        print()
        Utils.sys_pause()

    return True # Continue


# Returns whether the error should be displayed
# or throws if the error is unexpected.

def _handle_error(err: Exception) -> bool:
    if isinstance(err, SoftExitError):
        if err.throws > 0:
            raise err.with_decremented_ttl()
        else:
            return False
    
    elif isinstance(err, AbortError):
        return not err.is_user_abort

    elif isinstance(err, ApiError):
        return True

    else:
        raise err