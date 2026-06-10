import json

from typing import Callable

from utils import TextUtils
from display.table_builder import TableBuilder
from model.enums import VersionType, ImageType, Platform, AlertLevel


def show_done() -> None:
    print("\nDone!")


def show_games(games: list[dict]) -> None:
    print("\nAvailable Games:")

    builder = TableBuilder(["ID", "Name", "Author", "Description"], multiline_mode=True)
    for game in games:
        builder.add_row([
            game['id'],
            game['name'],
            game['author'],
            TextUtils.break_string(game['description'], 80)
        ])

    table = builder.build()
    print(table)


def show_tags(tags: list[dict]) -> None:
    print("\nAvailable Tags:")

    builder = TableBuilder(["ID", "Name", "Description"], multiline_mode=True)
    for tag in tags:
        builder.add_row([
            tag['id'],
            tag['name'],
            TextUtils.break_string(tag['description'], 80)
        ])

    table = builder.build()
    print(table)


def show_history(history_list: list[dict], games: list[dict]) -> None:
    print("\nHistory:")

    builder = TableBuilder(["ID", "Timestamp", "Game", "Action", "Object"], multiline_mode=True)

    for history in reversed(history_list): # Show most recent last (since this is terminal)
        id = history['id']
        info: str = history['info']
        timestamp = history['timestamp']
        id_game = history['idGame']

        # 1. Game name deduction (if possible)
        game_name: str | None = next(
            (game['name'] for game in games if game['id'] == id_game), None)
        
        game = game_name if game_name is not None else (
            f"{id_game}" if id_game is not None else "N/A")
        
        # 2. Action and object from info parsing (a bit heuristic)
        arr_split_with = info.split(' with ')
        action = arr_split_with[0]
        object_str = ' with '.join(
            arr_split_with[1:]) if len(arr_split_with) >= 1 else "N/A"

        # 3. Try to parse object as JSON (or display as string)
        try:
            object = json.dumps(
                json.loads(object_str), indent=2)
        except:
            object = object_str

        builder.add_row([id, timestamp, game, action, object])

    table = builder.build()
    print(table)


def show_game_header(game_tree: dict) -> None:
    print("\nHeader:")

    builder = TableBuilder(["ID", "Name", "Author", "Description"])
    builder.add_row([
        game_tree['id'],
        game_tree['name'],
        game_tree['author'],
        TextUtils.break_string(game_tree['description'], 60)
    ])

    table = builder.build()
    print(table)


def show_game_versions(game_tree: dict) -> None:
    print("\nVersions:")

    builder = TableBuilder([
        "ID", "Caption", "Type", "Description", "CLI Args", "Primary",
        "Release Date", "Platform", "Download URL", "Exec Location",
        "SHA256 Hash", "Alert"], multiline_mode=True)
    
    for version in game_tree['versions']:
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

    table = builder.build()
    print(table)


def show_game_images(game_tree: dict) -> None:
    print("\nImages:")

    builder = TableBuilder(["ID", "Download URL", "Type", "Sort Index"], multiline_mode=True)
    for image in game_tree['images']:
        builder.add_row([
            image['id'],
            TextUtils.break_string(image['downloadUrl'], 80),
            ImageType.to_string(image['type']),
            image['sortIndex']
        ])

    table = builder.build()
    print(table)


def show_game_tags(
        game_tree: dict,
        tags: list[dict],
        show_attached: bool = True,
        show_detached: bool = True
    ) -> None:

    def print_tags_that(condition: Callable[[dict], bool]):
        builder = TableBuilder(["ID", "Name"], multiline_mode=True)
        for tag in tags:
            if tag is not None and condition(tag):
                builder.add_row([
                    tag['id'],
                    tag['name']
                ])

        table = builder.build()
        print(table)

    if show_attached:
        print("\nAttached Tags:")
        print_tags_that(lambda tag: tag['id'] in game_tree['tagIds'])

    if show_detached:
        print("\nDetached Tags:")
        print_tags_that(lambda tag: tag['id'] not in game_tree['tagIds'])


def show_full_game_data(game_tree: dict, tags: list[dict]) -> None:
    show_game_header(game_tree)
    show_game_versions(game_tree)
    show_game_images(game_tree)
    show_game_tags(game_tree, tags, show_detached=False)