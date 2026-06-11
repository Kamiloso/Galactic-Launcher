from model.create import Create
from model.data import Data
from display.ask import Ask
from display.show import Show
from model.edit import Edit

from errors import *

class Forms:

    @staticmethod
    def edit_header(game_tree: dict) -> None:
        Show.tree_header(game_tree)
        Edit.game_header(game_tree)
        Show.tree_header(game_tree)


    @staticmethod
    def add_new_version(game_tree: dict) -> None:
        version_list: list[dict] = game_tree['versions']
        version_list.append(Create.new_version())
        Data.replace_zero_ids(game_tree)


    @staticmethod
    def remove_versions(game_tree: dict) -> None:
        Show.tree_versions(game_tree)

        version_list: list[dict] = game_tree['versions']
        ids = Ask.select_objs(version_list, 'version', 'remove')

        if len(ids) == 0:
            raise SoftExitError
        
        for id in ids:
            Data.remove_version(game_tree, id)


    @staticmethod
    def modify_version(game_tree: dict) -> None:
        Show.tree_versions(game_tree)

        version_list: list[dict] = game_tree['versions']
        id = Ask.select_obj(version_list, 'version', 'modify')

        version = next(version for version in version_list if version['id'] == id)

        Show.version(version)
        Edit.version(version)
        Show.version(version)


    @staticmethod
    def add_new_image(game_tree: dict) -> None:
        image_list: list[dict] = game_tree['images']
        image_list.append(Create.new_image())
        Data.replace_zero_ids(game_tree)


    @staticmethod
    def remove_images(game_tree: dict) -> None:
        Show.tree_images(game_tree)

        image_list: list[dict] = game_tree['images']
        ids = Ask.select_objs(image_list, 'image', 'remove')

        if len(ids) == 0:
            raise SoftExitError
        
        for id in ids:
            Data.remove_image(game_tree, id)


    @staticmethod
    def modify_image(game_tree: dict) -> None:
        Show.tree_images(game_tree)

        image_list: list[dict] = game_tree['images']
        id = Ask.select_obj(image_list, 'image', 'modify')

        image = next(image for image in image_list if image['id'] == id)

        Show.image(image)
        Edit.image(image)
        Show.image(image)


    @staticmethod
    def toggle_tags(game_tree: dict, tags: list[dict]) -> None:
        Show.tree_tags(game_tree, tags, show_detached=True)

        ids = Ask.select_objs(tags, 'tag', 'toggle')

        if len(ids) == 0:
            raise SoftExitError

        for id in ids:
            Data.toggle_tag(game_tree, id)