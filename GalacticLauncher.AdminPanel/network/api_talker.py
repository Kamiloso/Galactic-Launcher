import time

from requests.models import Response
from requests.exceptions import HTTPError, ConnectionError, Timeout, RequestException
from typing import Callable

from network.http_client import HttpClient
from display.input import Input

from errors import *

_http = HttpClient()
_current_token: str = ""


class ApiTalker:

    @staticmethod
    def download_all_games() -> list[dict]:
        response = _perform_connection(
            lambda: _http.get("/download/all-games"))
        return response.json()


    @staticmethod
    def download_game_data(game_id: int) -> dict:
        response = _perform_connection(
            lambda: _http.get(f"/download/game-data?id={game_id}"))
        return response.json()


    @staticmethod
    def download_all_tags() -> list[dict]:
        response = _perform_connection(
            lambda: _http.get("/download/all-tags"))
        return response.json()


    @staticmethod
    def req_admin(username: str, password: str) -> dict:
        response = _perform_connection(
            lambda: _http.post("/admin/req-admin", json={
                "username": username,
                "password": password
            }))
        return response.json()


    @staticmethod
    def get_history_page(page: int) -> list[dict]:
        response = _perform_admin_connection(
            lambda: _http.post(f"/admin/get-history-page?page={page}", json={
                "token": _current_token
            }))
        return response.json()


    @staticmethod
    def create_game(game: dict) -> None:
        _perform_admin_connection(
            lambda: _http.post("/admin/create-game", json={
                "token": _current_token,
                "body": game
            }))


    @staticmethod
    def delete_game(game_id: int) -> None:
        _perform_admin_connection(
            lambda: _http.post(f"/admin/delete-game?id={game_id}", json={
                "token": _current_token
            }))


    @staticmethod
    def create_tag(tag: dict) -> None:
        _perform_admin_connection(
            lambda: _http.post("/admin/create-tag", json={
                "token": _current_token,
                "body": tag
            }))


    @staticmethod
    def delete_tag(tag_id: int) -> None:
        _perform_admin_connection(
            lambda: _http.post(f"/admin/delete-tag?id={tag_id}", json={
                "token": _current_token
            }))


    @staticmethod
    def update_game_tree(game_tree: dict) -> None:
        _perform_admin_connection(
            lambda: _http.post("/admin/update-game-tree", json={
                "token": _current_token,
                "body": game_tree
            }))


# --- Internal connection handling  ---

def _perform_admin_connection(request: Callable[[], Response]) -> Response:
    while True:
        try:
            return _perform_connection(request)

        except ApiError as err:
            if err.status_code == 401: # Unauthorized - ask for credentials and retry
                global _current_token

                print("\nAuthentication required...")
                username = Input.string("Enter username")
                password = Input.string("Enter password", is_pass=True)

                token_obj = ApiTalker.req_admin(username, password)

                if not token_obj["authenticated"]:
                    raise AbortError("Invalid credentials.")
                
                _current_token = token_obj["token"]
            
            else:
                raise err


def _perform_connection(request: Callable[[], Response], retry=0) -> Response:
    try:
        response = request()
        response.raise_for_status()
        return response
    
    except HTTPError as http_err:
        status_code = http_err.response.status_code if http_err.response is not None else None

        if status_code == 429 and retry < 4:
            next_retry = retry + 1

            print(f"\nToo many requests! Retrying... ({next_retry}/4)")

            time.sleep(2 ** retry) # 1, 2, 4, 8 seconds
            return _perform_connection(request, next_retry)

        raise ApiError("HTTP error has occurred.", status_code=status_code)
        
    except ConnectionError as err:
        raise ApiError("Connection error has occurred.") from err

    except Timeout as err:
        raise ApiError("Timeout error has occurred.") from err
        
    except RequestException as err:
        raise ApiError("Unexpected error has occurred.") from err