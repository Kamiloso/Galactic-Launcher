from errors.abort_error import AbortError
from utils import Utils

from display.input import *


def ask_choice() -> str | None:
    try:
        return input_string("Select an option:")
        
    except AbortError:
        return None


def ask_history_page() -> int:
    return input_number(
        "Enter history page number starting from 0:", (0, Utils.MAX_INT_32))


def ask_select_obj(objs: list[dict], type: str, mode: str) -> int:
    if (len(objs) == 0):
        raise AbortError(f"No {type}s available.")
    
    selected_id = input_number(
        f"Choose {type} ID to {mode}:", (1, Utils.MAX_INT_32))
    
    if not any(obj['id'] == selected_id for obj in objs):
        raise AbortError(f"Invalid {type} ID.")
    
    return selected_id


def ask_select_objs(objs: list[dict], type: str, mode: str) -> list[int]:
    if (len(objs) == 0):
        raise AbortError(f"No {type}s available.")
    
    choice = input_string(f"Enter {type} IDs to {mode}:")
    
    try:
        selected_ids = [int(id.strip()) for id in choice.split()]
        for id in selected_ids:
            if not any(obj['id'] == id for obj in objs):
                raise ValueError
        return selected_ids
    
    except ValueError:
        raise AbortError(f"Invalid {type} IDs.")


def ask_new_game() -> dict:
    print("\nEnter new game information:")
    
    return {
        "id": 0,
        "name": input_string("Enter game name..."),
        "author": input_string("Enter game author..."),
        "description": input_string("Enter game description...")
    }


def ask_new_tag() -> dict:
    print("\nEnter new tag information:")
    
    return {
        "id": 0,
        "name": input_string("Enter tag name..."),
        "description": input_string("Enter tag description...")
    }