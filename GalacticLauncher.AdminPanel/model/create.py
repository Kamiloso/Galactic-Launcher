from display.input import Input
from utils import Utils

from model.enums import VersionType, Platform, AlertLevel, ImageType
from errors import *


class Create:

    @staticmethod
    def new_game() -> dict:
        print("\nEnter new game information...")
        
        return {
            "id": 0,
            "name": Input.string("Enter name"),
            "author": Input.string("Enter author"),
            "description": Input.string("Enter description"),
        }


    @staticmethod
    def new_version() -> dict:
        print("\nEnter new version information...")

        return {
            "id": 0,
            "caption": Input.string("Enter caption"),
            "type": Input.enum(f"Enter type", VersionType.VALUES),
            "description": Input.string("Enter description"),
            "cliArgs": Input.string("Enter CLI arguments"),
            "isPrimary": Input.boolean("Is this the primary version?"),
            "releaseDate": Utils.date_to_str(Input.date_only("Enter release date")),
            "platform": Input.enum(f"Enter platform", Platform.VALUES),
            "downloadUrl": Input.string("Enter download URL"),
            "execLocation": Input.string("Enter executable location"),
            "sha256Hash": Input.optional_string("Enter SHA256 hash"),
            "alert": Input.enum(f"Enter alert", AlertLevel.VALUES),
        }
    

    @staticmethod
    def new_image() -> dict:
        print("\nEnter new image information...")

        return {
            "id": 0,
            "downloadUrl": Input.string("Enter download URL"),
            "type": Input.enum(f"Enter type", ImageType.VALUES),
            "sortIndex": Input.number("Enter sort index", (Utils.MIN_INT_32, Utils.MAX_INT_32)),
        }
    

    @staticmethod
    def new_tag() -> dict:
        print("\nEnter new tag information...")
        
        return {
            "id": 0,
            "name": Input.string("Enter name"),
            "description": Input.string("Enter description"),
        }