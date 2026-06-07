from network.http_client import HttpClient

http = HttpClient()

def test_connection() -> bool:
    try:
        response = http.get("/download/all-games")
        print("Response:", response.text)
        return True
    
    except Exception as e:
        print("Error:", str(e))
        return False

def all_games() -> list[dict]:
    response = http.get("/download/all-games")
    return response.json()
