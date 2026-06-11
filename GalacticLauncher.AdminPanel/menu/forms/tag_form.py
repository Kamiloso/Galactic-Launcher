from model.data import Data
from display.ask import Ask
from display.show import Show
from network.api_talker import ApiTalker

from menu.menu_helpers import save_tracker

def form_edit_tags(game_tree: dict) -> bool:
    tags = ApiTalker.download_all_tags()
    Show.tree_tags(game_tree, tags, show_detached=True)

    ids = Ask.select_objs(tags, "tag", "toggle")
    for id in ids:
        Data.toggle_tag(game_tree, id)

    if len(ids) > 0:
        save_tracker.inform_modify()

    return False