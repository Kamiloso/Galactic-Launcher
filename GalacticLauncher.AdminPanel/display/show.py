import json

from typing import Callable

from utils import Utils, TextUtils
from display.table_builder import TableBuilder
from model.enums import VersionType, ImageType, Platform, AlertLevel


class Show:

    @staticmethod
    def done() -> None:
        print("\nDone!")


    @staticmethod
    def pause() -> None:
        print()
        Utils.sys_pause()


    @staticmethod
    def input_instructions() -> None:
        print("\nInput Instructions:")

        builder = TableBuilder(["Pattern", "Description"])
        for command, description in [
                ("[text]", "Type your text and press Enter."),
                ("/:[text]", "Evaluate escape characters, ignore commands. For example use '\\n' for a new line."),
                ("/ml", "Enter multiline mode. Type '/end' on a new line to finish and submit."),
                ("/abort", "Cancel the current action."),
                ("/quit", "Terminate the entire application immediately.")
            ]:
            builder.add_row([command, description])

        table = builder.build()
        print(table)


    @staticmethod
    def games(games: list[dict]) -> None:
        _print_table_with_rows(
            "Games",
            ["ID", "Name", "Author", "Description"],
            games,
            _game_row_add,
            multiline_mode=True
        )

    
    @staticmethod
    def tags(tags: list[dict]) -> None:
        _print_table_with_rows(
            "Tags",
            ["ID", "Name", "Description"],
            tags,
            _tag_row_add,
            multiline_mode=True
        )


    @staticmethod
    def history(history_list: list[dict], games: list[dict]) -> None:
        _print_table_with_rows(
            "History",
            ["ID", "Timestamp", "Game", "Action", "Object"],
            list(reversed(history_list)),
            lambda builder, history: _history_row_add(builder, history, games),
            multiline_mode=True
        )

    
    @staticmethod
    def full_game_data(game_tree: dict, tags: list[dict]) -> None:
        Show.tree_header(game_tree)
        Show.tree_versions(game_tree)
        Show.tree_images(game_tree)
        Show.tree_tags(game_tree, tags)


    @staticmethod
    def tree_header(game_tree: dict) -> None:
        Show.game(game_tree) # same layout anyway


    @staticmethod
    def tree_versions(game_tree: dict) -> None:
        _print_table_with_rows(
            "Versions",
            ["ID", "Caption", "Type", "Description", "CLI", "Primary",
             "Release Date", "Platform", "Download URL", "Exec Location",
             "SHA256", "Alert"],
            game_tree['versions'],
            _version_row_add,
            multiline_mode=True
        )


    @staticmethod
    def tree_images(game_tree: dict) -> None:
        _print_table_with_rows(
            "Images",
            ["ID", "Download URL", "Type", "Sort Index"],
            game_tree['images'],
            _image_row_add,
            multiline_mode=True
        )


    @staticmethod
    def tree_tags(
            game_tree: dict,
            tags: list[dict],
            show_attached: bool = True,
            show_detached: bool = False
        ) -> None:

        def print_tags_that(title: str, condition: Callable[[dict], bool]):
            _print_table_with_rows(
                title,
                ["ID", "Name", "Description"],
                [tag for tag in tags if condition(tag)],
                _tag_row_add,
                multiline_mode=True
            )

        if show_attached:
            print_tags_that(
                "Attached Tags", lambda tag: tag['id'] in game_tree['tagIds'])

        if show_detached:
            print_tags_that(
                "Detached Tags", lambda tag: tag['id'] not in game_tree['tagIds'])

    
    @staticmethod
    def game(game: dict) -> None:
        _print_table_with_rows(
            "Header",
            ["ID", "Name", "Author", "Description"],
            [game],
            _game_row_add,
            multiline_mode=True
        )


    @staticmethod
    def version(version: dict) -> None:
        _print_table_with_rows(
            "Version",
            ["ID", "Caption", "Type", "Description", "CLI",
             "Primary", "Release Date", "Platform", "Download URL",
             "Exec Location", "SHA256", "Alert"],
            [version],
            _version_row_add,
        )

    
    @staticmethod
    def image(image: dict) -> None:
        _print_table_with_rows(
            "Image",
            ["ID", "Download URL", "Type", "Sort Index"],
            [image],
            _image_row_add,
        )

    
    @staticmethod
    def tag(tag: dict) -> None:
        _print_table_with_rows(
            "Tag",
            ["ID", "Name", "Description"],
            [tag],
            _tag_row_add,
        )


# --- Row add functions for tables ---

def _game_row_add(builder: TableBuilder, game: dict) -> None:
    builder.add_row([
        game['id'],
        game['name'],
        game['author'],
        TextUtils.break_string(game['description'], 80)
    ])


def _version_row_add(builder: TableBuilder, version: dict) -> None:
    builder.add_row([
        version['id'],
        version['caption'],
        VersionType.to_string(version['type']),
        TextUtils.break_string(version['description'], 30),
        TextUtils.break_string(version['cliArgs'], 20),
        "Yes" if version['isPrimary'] else "No",
        version['releaseDate'],
        Platform.to_string(version['platform']),
        TextUtils.break_string(version['downloadUrl'], 20),
        TextUtils.break_string(version['execLocation'], 20),
        TextUtils.break_string(version['sha256Hash'], 16)
            if version['sha256Hash'] is not None else "N/A",
        AlertLevel.to_string(version['alert'])
    ])


def _image_row_add(builder: TableBuilder, image: dict) -> None:
    builder.add_row([
        image['id'],
        TextUtils.break_string(image['downloadUrl'], 80),
        ImageType.to_string(image['type']),
        image['sortIndex']
    ])


def _tag_row_add(builder: TableBuilder, tag: dict) -> None:
    builder.add_row([
        tag['id'],
        tag['name'],
        TextUtils.break_string(tag['description'], 80)
    ])


def _history_row_add(builder: TableBuilder, history: dict, games: list[dict]) -> None:
    id_game = history['idGame']
    if id_game is not None:
        game_display = next((g['name'] for g in games if g['id'] == id_game), str(id_game))
    else:
        game_display = "N/A"

    info: str = history['info']
    action, _, object_str = info.partition(' with ')

    try:
        formatted_object = json.dumps(json.loads(object_str), indent=2)
    except Exception:
        formatted_object = object_str

    if formatted_object == "":
        formatted_object = "N/A"

    builder.add_row([
        history['id'],
        history['timestamp'],
        game_display,
        TextUtils.break_string(action, 30),
        TextUtils.break_string(formatted_object, 80)
    ])


# --- Utility for printing tables ---

def _print_table_with_rows(
        title: str,
        headers: list[str],
        rows: list[dict],
        row_add_func: Callable[[TableBuilder, dict], None],
        multiline_mode: bool = False
    ) -> None:

    print(f"\n{title}:")

    builder = TableBuilder(headers, multiline_mode)
    for item in rows:
        row_add_func(builder, item)

    table = builder.build()
    print(table)