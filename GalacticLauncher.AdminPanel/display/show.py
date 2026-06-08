def show_success() -> None:
    print("Success!")


def show_games(games: list[dict]) -> None:
    print("\nAvailable Games:")

    for game in games:
        id = game['id']
        name = game['name']
        author = game['author']

        print(f"{id} - {name} (by {author})")


def show_game_data(game_data: dict) -> None:
    print(game_data)


def show_tags(tags: list[dict]) -> None:
    print("\nAvailable Tags:")
    for tag in tags:
        print(f"{tag['id']}. {tag['name']}")

def show_history(games: list[dict], history_list: list[dict]) -> None:
    print("\nHistory:")

    for history in history_list:
        id = history['id']
        info = history['info']
        timestamp = history['timestamp']
        id_game = history['idGame']

        game_name: str | None = next(
            (game['name'] for game in games if game['id'] == id_game), None)

        if game_name is not None:
            print(f"{id}. At {timestamp} (game: {game_name})")
        else:
            print(f"{id}. At {timestamp}")

        print(info + "\n")