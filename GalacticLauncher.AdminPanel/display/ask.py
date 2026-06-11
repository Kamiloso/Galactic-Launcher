from typing import Callable

from display.input import Input
from utils import Utils

from model.enums import *
from errors import *


class Ask:

    @staticmethod
    def menu_choice() -> str | None:
        try:
            return Input.string("Select an option")
            
        except AbortError:
            return None


    @staticmethod
    def history_page() -> int:
        return Input.number(
            "Enter history page starting from 0", (0, Utils.MAX_INT_32))


    @staticmethod
    def select_obj(objs: list[dict], type: str, mode: str) -> int:
        if (len(objs) == 0):
            raise AbortError(f"No {type}s available.")
        
        selected_id = Input.number(
            f"Select {type} ID to {mode}", (1, Utils.MAX_INT_64))
        
        if not any(obj['id'] == selected_id for obj in objs):
            raise AbortError(f"Invalid {type} ID.")
        
        return selected_id


    @staticmethod
    def select_objs(objs: list[dict], type: str, mode: str) -> list[int]:
        if (len(objs) == 0):
            raise AbortError(f"No {type}s available.")
        
        selected_ids = Input.number_list(
            f"Select {type} IDs to {mode}", (1, Utils.MAX_INT_64))

        for id in selected_ids:
            if not any(obj['id'] == id for obj in objs):
                raise AbortError(f"Invalid {type} ID: {id}")
        
        return selected_ids


    @staticmethod
    def modify_optionally(
        prompt_confirm: str,
        prompt_ask: str,
        modify: Callable[[str], None]
    ) -> None:
        
        if Input.confirm(prompt_confirm):
            modify(prompt_ask)