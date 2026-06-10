from display.show import *
from display.ask import *
from network.api_talker import *
from model.actions import *

def form_edit_tags(game_tree: dict, tags: list[dict]) -> None:
    tags = download_all_tags()
    show_game_tags(game_tree, tags)

    ids = ask_select_objs(tags, "tag", "toggle")
    for id in ids:
        toggle_tag(game_tree, id)
