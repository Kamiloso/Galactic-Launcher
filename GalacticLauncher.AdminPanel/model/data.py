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