import time

from requests.models import Response
from requests.exceptions import HTTPError, ConnectionError, Timeout, RequestException
from typing import Callable

from display.ask import ask_credentials
from network.api_error import ApiError
from network.http_client import HttpClient

http = HttpClient()
current_token: str = ""


def download_all_games() -> list[dict]:
    response = _perform_connection(
        lambda: http.get("/download/all-games"))
    return response.json()


def download_game_data(game_id: int) -> dict:
    response = _perform_connection(
        lambda: http.get(f"/download/game-data?id={game_id}"))
    return response.json()


def download_all_tags() -> list[dict]:
    response = _perform_connection(
        lambda: http.get("/download/all-tags"))
    return response.json()


def admin_req_admin(username: str, password: str) -> dict:
    response = _perform_connection(
        lambda: http.post("/admin/req-admin", json={
            "username": username,
            "password": password
        }))
    return response.json()


def admin_get_history_page(page: int) -> list[dict] | None:
    response = _perform_admin_connection(
        lambda: http.post(f"/admin/get-history-page?page={page}", json={
            "token": current_token
        }))
    return response.json() if response is not None else None


def admin_create_game(game: dict) -> bool:
    response = _perform_admin_connection(
        lambda: http.post("/admin/create-game", json={
            "token": current_token,
            "body": game
        }))
    return response is not None


def admin_delete_game(game_id: int) -> bool:
    response = _perform_admin_connection(
        lambda: http.post(f"/admin/delete-game?id={game_id}", json={
            "token": current_token
        }))
    return response is not None


def _perform_admin_connection(request: Callable[[], Response]) -> Response | None:
    while True:
        try:
            return _perform_connection(request)

        except ApiError as err:
            if err.status_code == 401: # Unauthorized - ask for credentials and retry
                global current_token

                username, password = ask_credentials()

                token_obj = admin_req_admin(username, password)

                if not token_obj["authenticated"]:
                    print("Invalid credentials.")
                    return None
                
                current_token = token_obj["token"]
            
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

            print(f"Too many requests! Trying again... ({next_retry} / 4)")
            time.sleep(2 ** retry) # 1, 2, 4, 8 seconds
            return _perform_connection(request, next_retry)

        raise ApiError("HTTP error has occurred.", status_code=status_code)
        
    except ConnectionError as err:
        raise ApiError("Connection error has occurred.") from err

    except Timeout as err:
        raise ApiError("Timeout error has occurred.") from err
        
    except RequestException as err:
        raise ApiError("Unexpected error has occurred.") from err