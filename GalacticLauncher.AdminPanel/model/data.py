from model.enums import ImageType


class Data:

    @staticmethod
    def build_game_tree(game_data: dict) -> dict:
        tree = {}
        for key in ['id', 'name', 'author', 'description', 'versions', 'images']:
            value = game_data[key]
            tree[key] = value if not isinstance(value, list) else value.copy()

        tag_ids_str: str | None = game_data['tagIdList']
        
        tree['tagIds'] = [
            int(tag_id)
                for tag_id in tag_ids_str.split(',')
                if all(c.isdigit() for c in tag_id)
        ] if tag_ids_str is not None else []

        return tree
    

    @staticmethod
    def replace_zero_ids(game_tree: dict) -> None:
        
        def inject_to_list(items: list[dict]):
            all_ids = {item['id'] for item in items}
            next_id = max(all_ids) + 1 if len(all_ids) > 0 else 1

            for item in items:
                if item['id'] == 0:
                    item['id'] = next_id
                    next_id += 1

        inject_to_list(game_tree['versions'])
        inject_to_list(game_tree['images'])


    @staticmethod
    def remove_version(game_tree: dict, version_id: int):
        game_tree['versions'] = [
            version for version in game_tree['versions']
            if version['id'] != version_id
        ]


    @staticmethod
    def remove_image(game_tree: dict, image_id: int):
        game_tree['images'] = [
            image for image in game_tree['images']
            if image['id'] != image_id
        ]


    @staticmethod
    def toggle_tag(game_tree: dict, tag_id: int):
        tag_ids: list[int] = game_tree['tagIds']
        if tag_id in tag_ids:
            tag_ids.remove(tag_id)
        else:
            tag_ids.append(tag_id)


    @staticmethod
    def has_multiple_primary(game_tree: dict) -> bool:
        versions: list[dict] = game_tree['versions']
        primary_count = sum(1 for version in versions if version['isPrimary'])
        return primary_count > 1


    @staticmethod
    def has_multiple_icons(game_tree: dict) -> bool:
        images: list[dict] = game_tree['images']
        icon_value = ImageType.to_int("Icon")
        icon_count = sum(1 for image in images if image['type'] == icon_value)
        return icon_count > 1