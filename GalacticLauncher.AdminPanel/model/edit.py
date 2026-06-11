from typing import Callable

from display.input import Input
from display.ask import Ask

from model.enums import *
from utils import Utils


class Edit:

    @staticmethod
    def game_header(game_tree: dict) -> None:
        print("\nEdit header...")

        _edit_object(game_tree, [
            ('name', 'name', Input.string),
            ('author', 'author', Input.string),
            ('description', 'description', Input.string),
        ])


    @staticmethod
    def version(version: dict) -> None:
        print("\nEdit version...")

        def modify_version_type(prompt: str) -> int:
            return Input.enum(prompt, VersionType.VALUES)
        
        def modify_platform(prompt: str) -> int:
            return Input.enum(prompt, Platform.VALUES)
        
        def modify_alert(prompt: str) -> int:
            return Input.enum(prompt, AlertLevel.VALUES)
        
        def modify_release_date(prompt: str) -> str:
            return Utils.date_to_str(Input.date_only(prompt))

        _edit_object(version, [
            ('caption', 'caption', Input.string),
            ('type', 'type', modify_version_type),
            ('description', 'description', Input.string),
            ('cliArgs', 'CLI arguments', Input.string),
            ('isPrimary', 'is primary', Input.boolean),
            ('releaseDate', 'release date', modify_release_date),
            ('platform', 'platform', modify_platform),
            ('downloadUrl', 'download URL', Input.string),
            ('execLocation', 'executable location', Input.string),
            ('sha256Hash', 'SHA256 hash', Input.optional_string),
            ('alert', 'alert', modify_alert),
        ])


    @staticmethod
    def image(image: dict) -> None:
        print("\nEdit image...")

        def modify_image_type(prompt: str) -> int:
            return Input.enum(prompt, ImageType.VALUES)
        
        def modify_sort_index(prompt: str) -> int:
            return Input.number(prompt, (Utils.MIN_INT_32, Utils.MAX_INT_32))

        _edit_object(image, [
            ('downloadUrl', 'download URL',  Input.string),
            ('type', 'type', modify_image_type),
            ('sortIndex', 'sort index', modify_sort_index),
        ])



def _edit_object(
        obj: dict,
        fields: list[tuple[str, str, Callable[[str], object]]]
    ) -> None:

    temp_obj = {
        field: obj[field] for field, _, _ in fields
    }

    for field, field_visible, read in fields:
        Ask.modify_optionally(
            f"Modify {field_visible}?",
            f"Enter new {field_visible}",
            lambda prompt: temp_obj.update({
                field: read(prompt)
            })
        )
    
    # If any exception occurs during edition,
    # the original object remains unchanged.
    obj.update(temp_obj)