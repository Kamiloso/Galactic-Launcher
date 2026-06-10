def toggle_tag(game_tree: dict, tag_id: int):
    if tag_id in game_tree['tagIds']:
        game_tree['tagIds'].remove(tag_id)
    else:
        game_tree['tagIds'].append(tag_id)