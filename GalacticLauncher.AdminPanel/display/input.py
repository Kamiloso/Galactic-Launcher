from typing import Callable
from datetime import date
from getpass import getpass

import readchar

from errors import *


class Input:

    @staticmethod
    def string(prompt: str, is_pass: bool = False) -> str:
        print(f"\n{prompt}:")
        read = input if not is_pass else getpass
        choice = read("> ")
        return _handle_command(choice, read)


    @staticmethod
    def optional_string(prompt: str, is_pass: bool = False) -> str | None:
        print(f"\n{prompt} (leave empty to skip):")
        read = input if not is_pass else getpass
        choice = read("> ")
        return _handle_command(choice, read) if choice != "" else None


    @staticmethod
    def number(prompt: str, range: tuple[int, int]) -> int:
        r_min, r_max = range

        choice = Input.string(prompt)
        
        try:
            n = int(choice)
            if r_min > n or n > r_max:
                raise ValueError
            return n
        
        except ValueError:
            raise AbortError(f"Invalid input. Must be integer between {r_min} and {r_max}.")
    

    @staticmethod
    def number_list(prompt: str, range: tuple[int, int]) -> list[int]:
        r_min, r_max = range

        choice = Input.string(prompt)
        
        try:
            n_list = [int(arg) for arg in choice.split()]
            for n in n_list:
                if r_min > n or n > r_max:
                    raise ValueError
            return n_list
        
        except ValueError:
            raise AbortError(f"Invalid input. Must be space-separated integers between {r_min} and {r_max}.")
    

    @staticmethod
    def date_only(prompt: str) -> date:
        choice = Input.string(f"{prompt} (YYYY-MM-DD)")

        try:
            year, month, day = map(int, choice.split('-'))
            return date(year, month, day)
        
        except ValueError:
            raise AbortError("Invalid date format.")
    
    
    @staticmethod
    def enum(prompt: str, enum_values: list[str]) -> int:
        choice = Input.string(f"{prompt} ({', '.join(enum_values)})")

        enum_lower_list = [v.lower() for v in enum_values]
        choice_lower = choice.lower()
        
        try:
            return enum_lower_list.index(choice_lower)
        
        except ValueError:
            raise AbortError(f"Invalid enum value.")
    

    @staticmethod
    def boolean(prompt: str) -> bool:
        return Input.enum(prompt, ["Yes", "No"]) == 0


    @staticmethod
    def confirm(prompt: str) -> bool:
        print(f"\n{prompt} (y/n): ", end="", flush=True)

        while True:
            key = readchar.readkey().lower()
            
            if key in ['y', 'n']:
                print(key)
                break

        return key == 'y'


# --- Helpers ---

def _handle_command(arg: str, read: Callable[[str], str], rc = 0) -> str:
    if arg == "/abort":
        raise SoftExitError
    
    if arg == "/quit":
        print("\nApplication quit by the user.")
        exit(0)

    if arg == "/ml":
        result_arr = []
        while True:
            line = read(('--' * rc) + "--> ")
            ml_arg = _handle_command(line, read, rc + 1)

            if ml_arg == "/end":
                break

            result_arr.append(ml_arg)
        
        return "\n".join(result_arr)

    if arg.startswith("/:"):
        rawtext = arg[len("/:"):]

        try:
            encoded_bytes = rawtext.encode('raw_unicode_escape')
            return encoded_bytes.decode('unicode_escape')
        
        except UnicodeDecodeError:
            return rawtext

    return arg